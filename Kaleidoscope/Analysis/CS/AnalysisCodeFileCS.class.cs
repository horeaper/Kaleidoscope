using System;
using System.Collections.Generic;
using System.Linq;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	partial class AnalysisCodeFileCS
	{
		T ReadClassMembers<T>(ClassTraits classTraints, ClassTypeKind typeKind) where T : ClassTypeDeclare.Builder, new()
		{
			var builder = new T {
				TypeKind = typeKind,
				CustomAttributes = classTraints.CustomAttributes,
				Name = classTraints.Name,
				InstanceKind = classTraints.InstanceKind,
				IsUnsafe = classTraints.IsUnsafe,
				IsPartial = classTraints.IsPartial,
				GenericTypes = classTraints.GenericTypes,
				Inherits = classTraints.Inherits
			};
			var token = block.GetToken(index++, Error.Analysis.LeftBraceExpected);
			if (token.Type != TokenType.LeftBrace) {
				throw ParseException.AsToken(token, Error.Analysis.LeftBraceExpected);
			}

			TokenKeyword accessModifier = null;
			TokenKeyword newModifier = null;
			TokenKeyword sealedModifier = null;
			TokenKeyword instanceKindModifier = null;
			TokenKeyword readonlyModifier = null;
			TokenKeyword unsafeModifier = null;
			TokenIdentifier partialModifier = null;
			TokenIdentifier asyncModifier = null;
			var currentAttributes = new List<AttributeObject.Builder>();
			Action fnNextMember = () => {
				accessModifier = null;
				newModifier = null;
				sealedModifier = null;
				instanceKindModifier = null;
				readonlyModifier = null;
				unsafeModifier = null;
				partialModifier = null;
				asyncModifier = null;
				currentAttributes.Clear();
			};

			Func<AccessModifier> fnGetAccessModifier = () => {
				var access = AccessModifier.@private;
				if (accessModifier != null) {
					access = (AccessModifier)Enum.Parse(typeof(AccessModifier), accessModifier.Type.ToString());
				}
				return access;
			};

			while (true) {
				token = block.GetToken(index++, Error.Analysis.RightBraceExpected);

				if (token.Type == TokenType.RightBrace) {
					return builder;
				}
				else if (token.Type == TokenType.LeftBracket) {
					var modifierTokens = new Token[] {
						accessModifier, newModifier, sealedModifier, instanceKindModifier, readonlyModifier, unsafeModifier, partialModifier, asyncModifier
					};
					if (modifierTokens.Any(item => item != null)) {
						infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.InvalidAttributeUsage));
					}
					currentAttributes.Add(AttributeObjectReader.Read(block, ref index));
				}
				//========================================================================
				// Modifiers
				//========================================================================
				else if (ConstantTable.AccessModifiers.Contains(token.Type)) {
					CheckConflict(accessModifier, token);
					CheckInconsistent(newModifier, sealedModifier, instanceKindModifier, readonlyModifier, unsafeModifier, partialModifier, asyncModifier);
					accessModifier = (TokenKeyword)token;
				}
				else if (token.Type == TokenType.@new) {
					CheckDuplicate(newModifier, token);
					CheckInconsistent(sealedModifier, instanceKindModifier, readonlyModifier, unsafeModifier, partialModifier, asyncModifier);
					newModifier = (TokenKeyword)token;
				}
				else if (token.Type == TokenType.@sealed) {
					CheckDuplicate(sealedModifier, token);
					CheckInconsistent(instanceKindModifier, readonlyModifier, unsafeModifier, partialModifier, asyncModifier);
					sealedModifier = (TokenKeyword)token;
				}
				else if (ConstantTable.InstanceKindModifier.Contains(token.Type)) {
					CheckInconsistent(readonlyModifier, unsafeModifier, partialModifier, asyncModifier);
					if (newModifier != null && !ConstantTable.ValidNewInstanceKindModifier.Contains(token.Type)) {
						infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.ConflictModifier));
					}
					else {
						if ((instanceKindModifier?.Type == KeywordType.@static && token.Type == TokenType.@extern) ||
							(instanceKindModifier?.Type == KeywordType.@extern && token.Type == TokenType.@static)) {
							infoOutput.OutputWarning(ParseException.AsToken(token, Error.Analysis.ExternImpliesStatic));
							if (token.Type == TokenType.@extern) {
								instanceKindModifier = (TokenKeyword)token;
							}
						}
						else {
							CheckConflict(instanceKindModifier, token);
							instanceKindModifier = (TokenKeyword)token;
						}
					}
				}
				else if (token.Type == TokenType.@readonly) {
					CheckDuplicate(readonlyModifier, token);
					CheckInconsistent(unsafeModifier);
					CheckInvalid(sealedModifier, partialModifier, asyncModifier);
					if (instanceKindModifier != null && instanceKindModifier.Type != KeywordType.@static) {
						infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.ConflictModifier));
					}
					else {
						readonlyModifier = (TokenKeyword)token;
					}
				}
				else if (token.Type == TokenType.@unsafe) {
					CheckDuplicate(unsafeModifier, token);
					CheckInconsistent(partialModifier, asyncModifier);
					unsafeModifier = (TokenKeyword)token;
				}
				else if ((token as TokenIdentifier)?.ContextualKeyword == ContextualKeywordType.partial) {
					CheckDuplicate(partialModifier, token);
					CheckInvalid(asyncModifier);
					partialModifier = (TokenIdentifier)token;
				}
				else if ((token as TokenIdentifier)?.ContextualKeyword == ContextualKeywordType.async) {
					CheckDuplicate(asyncModifier, token);
					CheckInvalid(partialModifier);
					asyncModifier = (TokenIdentifier)token;
				}
				else if ((token as TokenIdentifier)?.ContextualKeyword == ContextualKeywordType.inline) {
					infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.InlineNotAllowed));
				}
				//========================================================================
				// Nested type
				//========================================================================
				else if (token.Type == TokenType.@class) {
					CheckInvalid(readonlyModifier, asyncModifier);

					var instanceKind = TypeInstanceKind.None;
					if (sealedModifier != null) {
						instanceKind = TypeInstanceKind.@sealed;
						if (instanceKindModifier != null) {
							infoOutput.OutputError(ParseException.AsToken(instanceKindModifier, Error.Analysis.InvalidModifier));
						}
					}
					else if (instanceKindModifier != null) {
						if (instanceKindModifier.Type == KeywordType.@abstract) {
							instanceKind = TypeInstanceKind.@abstract;
						}
						else if (instanceKindModifier.Type == KeywordType.@static) {
							instanceKind = TypeInstanceKind.@static;
						}
						else {
							infoOutput.OutputError(ParseException.AsToken(instanceKindModifier, Error.Analysis.InvalidModifier));
						}
					}
					
					builder.NestedClasses.Add(ReadNestedClassDeclare(currentAttributes.ToArray(), fnGetAccessModifier(), newModifier != null, unsafeModifier != null, partialModifier != null, instanceKind, nameToken => ReadClassMembers<NestedClassTypeDeclare.Builder>(nameToken, ClassTypeKind.@class)));
					fnNextMember();
				}
				else if (token.Type == TokenType.@struct) {
					CheckInvalid(sealedModifier, instanceKindModifier, readonlyModifier, asyncModifier);
					builder.NestedClasses.Add(ReadNestedClassDeclare(currentAttributes.ToArray(), fnGetAccessModifier(), newModifier != null, unsafeModifier != null, partialModifier != null, TypeInstanceKind.None, nameToken => ReadClassMembers<NestedClassTypeDeclare.Builder>(nameToken, ClassTypeKind.@struct)));
					fnNextMember();
				}
				else if (token.Type == TokenType.@interface) {
					CheckInvalid(sealedModifier, instanceKindModifier, readonlyModifier, asyncModifier);
					builder.NestedClasses.Add(ReadNestedClassDeclare(currentAttributes.ToArray(), fnGetAccessModifier(), newModifier != null, unsafeModifier != null, partialModifier != null, TypeInstanceKind.None, nameToken => ReadInterfaceMembers< NestedClassTypeDeclare.Builder>(nameToken)));
					fnNextMember();
				}
				else if (token.Type == TokenType.@enum) {

					fnNextMember();
				}
				else if (token.Type == TokenType.@delegate) {

					fnNextMember();
				}
				//========================================================================
				// Members
				//========================================================================
				else if (token.Text == classTraints.Name.Text) {
					//Constructor
					CheckInvalid(newModifier, sealedModifier, readonlyModifier, partialModifier);
					var constructor = new ConstructorDeclare.Builder {
						CustomAttributes = currentAttributes.ToArray(),
						AccessModifier = fnGetAccessModifier(),
						Name = (TokenIdentifier)token,
						IsUnsafe = unsafeModifier != null,
						IsAsync = asyncModifier != null,
					};

					//'static' modifier
					bool isStatic = false;
					if (instanceKindModifier != null) {
						if (instanceKindModifier.Type != KeywordType.@static) {
							if (builder.StaticConstructor != null) {
								infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.DuplicatedStaticConstructor));
							}
							if (accessModifier != null) {
								infoOutput.OutputError(ParseException.AsToken(accessModifier, Error.Analysis.StaticConstructorNoAccessModifier));
							}
							isStatic = true;
							constructor.AccessModifier = AccessModifier.@private;
						}
						else {
							infoOutput.OutputError(ParseException.AsToken(instanceKindModifier, Error.Analysis.InvalidModifier));
						}
					}
					constructor.InstanceKind = isStatic ? MethodInstanceKind.@static : MethodInstanceKind.None;
					if (classTraints.InstanceKind == TypeInstanceKind.@static && constructor.InstanceKind != MethodInstanceKind.@static) {
						infoOutput.OutputError(ParseException.AsToken(constructor.Name, Error.Analysis.StaticTypeOnly));
					}

					//Parameters
					block.NextToken(index, TokenType.LeftParenthesis, Error.Analysis.LeftParenthesisExpected);
					var parameters = ParameterReader.Read(infoOutput, block.ReadParenthesisBlock(ref index), false);
					if (isStatic && parameters.Count > 0) {
						infoOutput.OutputError(ParseException.AsToken(constructor.Name, Error.Analysis.StaticConstructorNoParams));
					}
					if (typeKind == ClassTypeKind.@struct && parameters.Count == 0) {
						infoOutput.OutputError(ParseException.AsToken(constructor.Name, Error.Analysis.StructConstructorInvalid));
					}
					constructor.Parameters = parameters;

					//base() call
					token = block.GetToken(index, Error.Analysis.LeftBraceExpected);
					if (token.Type == TokenType.Colon) {
						++index;
						token = block.GetToken(index, Error.Analysis.UnexpectedToken);
						if (token.Type != TokenType.@base && token.Type != TokenType.@this) {
							infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.UnexpectedToken));
							index = block.FindToken(index, TokenType.LeftBrace, Error.Analysis.LeftBraceExpected);
						}
						else {
							constructor.ChainCallContent = block.ReadPastSpecificToken(ref index, TokenType.LeftBrace, Error.Analysis.LeftBraceExpected);
							--index;
						}
					}

					//Body
					MethodBodyReader.ReadAsMethod(infoOutput, block, ref index, constructor);

					if (isStatic) {
						builder.StaticConstructor = constructor;
					}
					else {
						builder.Constructors.Add(constructor);
					}
					fnNextMember();
				}
				else if (token.Type == TokenType.BitwiseNot) {
					//Destructor
					CheckInvalid(accessModifier, newModifier, sealedModifier, instanceKindModifier, readonlyModifier, partialModifier);
					if (typeKind == ClassTypeKind.@struct) {
						infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.StructNoDestructor));
					}
					if (classTraints.InstanceKind == TypeInstanceKind.@static) {
						infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.StaticTypeOnly));
					}
					if (builder.Destructor != null) {
						infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.DuplicateDestructor));
					}
					var destructor = new DestructorDeclare.Builder {
						CustomAttributes = currentAttributes.ToArray(),
						AccessModifier = AccessModifier.@private,
						IsUnsafe = unsafeModifier != null,
						IsAsync = asyncModifier != null,
						InstanceKind = MethodInstanceKind.None,
						Parameters = new ParameterObject[0],
					};

					var nameToken = block.GetToken(index++, Error.Analysis.UnexpectedToken);
					if (nameToken.Text != classTraints.Name.Text) {
						infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.DestructorNameInvalid));
					}
					else {
						destructor.Name = (TokenIdentifier)nameToken;
					}

					//Empty parameter
					block.NextToken(index++, TokenType.LeftParenthesis, Error.Analysis.LeftParenthesisExpected);
					block.NextToken(index++, TokenType.RightParenthesis, Error.Analysis.RightParenthesisExpected);

					//Body
					MethodBodyReader.ReadAsMethod(infoOutput, block, ref index, destructor);

					builder.Destructor = destructor;
					fnNextMember();
				}
				else if (token.Type == TokenType.@explicit || token.Type == TokenType.@implicit) {
					//Conversion operator
					CheckInvalid(newModifier, sealedModifier, readonlyModifier, partialModifier);
					if (instanceKindModifier?.Type != KeywordType.@static) {
						infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.ConversionStaticOnly));
					}
					if (classTraints.InstanceKind == TypeInstanceKind.@static) {
						infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.StaticTypeNoOperator));
					}
					var conversion = new ConversionOperatorDeclare.Builder {
						CustomAttributes = currentAttributes.ToArray(),
						AccessModifier = fnGetAccessModifier(),
						IsUnsafe = unsafeModifier != null,
						IsAsync = asyncModifier != null,
						InstanceKind = MethodInstanceKind.@static,
						IsExplicit = token.Type == TokenType.@explicit,
					};

					//'operator'
					block.NextToken(index++, TokenType.@operator, Error.Analysis.UnexpectedToken);

					//ReturnType
					conversion.ReturnType = TypeReferenceReader.Read(block, ref index, TypeParsingRule.AllowVar | TypeParsingRule.AllowCppType);

					//Parameters
					block.NextToken(index, TokenType.LeftParenthesis, Error.Analysis.LeftParenthesisExpected);
					var parameters = ParameterReader.Read(infoOutput, block.ReadParenthesisBlock(ref index), false);
					if (parameters.Count != 1) {
						infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.OperatorParameterInvalid));
					}
					conversion.Parameters = parameters;

					//Body
					MethodBodyReader.ReadAsMethod(infoOutput, block, ref index, conversion);

					builder.ConversionOperators.Add(conversion);
					fnNextMember();
				}
				else {
					//Type
					--index;
					var type = TypeReferenceReader.Read(block, ref index, TypeParsingRule.AllowVoid | TypeParsingRule.AllowVar | TypeParsingRule.AllowCppType | TypeParsingRule.AllowArray);

					//Next
					token = block.GetToken(index, Error.Analysis.UnexpectedToken);
					if (token.Type == TokenType.@operator) {
						//Operator overloads
						CheckInvalid(newModifier, sealedModifier, readonlyModifier, partialModifier);
						if (classTraints.InstanceKind == TypeInstanceKind.@static) {
							infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.StaticTypeNoOperator));
						}
						var instanceKind = MethodInstanceKind.@static;
						if (accessModifier == null || instanceKindModifier == null) {
							infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.OperatorPublicStaticOnly));
						}
						else {
							if (accessModifier.Type != KeywordType.@public) {
								infoOutput.OutputError(ParseException.AsToken(accessModifier, Error.Analysis.OperatorPublicStaticOnly));
							}
							if (instanceKindModifier.Type != KeywordType.@static && instanceKindModifier.Type != KeywordType.@extern) {
								infoOutput.OutputError(ParseException.AsToken(instanceKindModifier, Error.Analysis.OperatorPublicStaticOnly));
							}
							else {
								instanceKind = (MethodInstanceKind)Enum.Parse(typeof(MethodInstanceKind), instanceKindModifier.Type.ToString());
							}
						}
						if (type is ReferenceVoid) {
							infoOutput.OutputError(ParseException.AsTokenBlock(type.Content, Error.Analysis.VoidNotAllowed));
						}
						var overloads = new OperatorOverloadDeclare.Builder {
							CustomAttributes = currentAttributes.ToArray(),
							AccessModifier = AccessModifier.@public,
							IsUnsafe = unsafeModifier != null,
							IsAsync = asyncModifier != null,
							InstanceKind = instanceKind,
							ReturnType = type,
						};

						//Operator symbol
						++index;
						var operatorToken = block.GetToken(index++, Error.Analysis.InvalidOperatorToken);
						if (ConstantTable.ValidArithmeticOperators.All(item => item.Item1 != operatorToken.Type)) {
							throw ParseException.AsToken(operatorToken, Error.Analysis.InvalidOperatorToken);
						}
						overloads.Operator = (TokenSymbol)operatorToken;

						//Parameters
						block.NextToken(index, TokenType.LeftParenthesis, Error.Analysis.LeftParenthesisExpected);
						var parameters = ParameterReader.Read(infoOutput, block.ReadParenthesisBlock(ref index), false);
						if (!ConstantTable.ValidArithmeticOperators.Any(item => item.Item1 == operatorToken.Type && parameters.Count == item.Item2)) {
							infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.OperatorParameterInvalid));
						}
						overloads.Parameters = parameters;

						//Body
						MethodBodyReader.ReadAsMethod(infoOutput, block, ref index, overloads);

						builder.OperatorOverloads.Add(overloads);
						fnNextMember();
					}
					else {
						//Name
						var nameContent = TypeReferenceReader.ReadTypeContent(block, ref index, TypeReferenceReader.ContentStyle.None);
						token = block.GetToken(index, Error.Analysis.SemicolonExpected);

						//Next
						switch (token.Type) {
							case TokenType.LeftParenthesis:
								{
									//Method
									CheckInvalid(readonlyModifier, partialModifier);
									var method = new MemberMethodDeclare.Builder {
										CustomAttributes = currentAttributes.ToArray(),
										AccessModifier = fnGetAccessModifier(),
										IsNew = newModifier != null,
										IsUnsafe = unsafeModifier != null,
										IsAsync = asyncModifier != null,
										ReturnType = type,
										IsSealed = sealedModifier != null,
									};

									//Instance kind
									if (instanceKindModifier != null) {
										if (sealedModifier != null && instanceKindModifier.Type != KeywordType.@override) {
											infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.SealedOnlyWithOverride));
											method.IsSealed = false;
										}
										if (!Enum.TryParse(instanceKindModifier.Type.ToString(), out method.InstanceKind)) {
											infoOutput.OutputError(ParseException.AsToken(instanceKindModifier, Error.Analysis.InvalidModifier));
										}
									}
									if (classTraints.InstanceKind == TypeInstanceKind.@static && method.InstanceKind != MethodInstanceKind.@static) {
										infoOutput.OutputError(ParseException.AsTokenBlock(nameContent, Error.Analysis.StaticTypeOnly));
									}
									if (classTraints.InstanceKind != TypeInstanceKind.@abstract && method.InstanceKind == MethodInstanceKind.@abstract) {
										infoOutput.OutputError(ParseException.AsTokenBlock(nameContent, Error.Analysis.AbstractTypeOnly));
									}

									//Generic
									int nameTokenIndex = nameContent.Count - 1;
									if (nameContent.Last.Type == TokenType.RightArrow) {
										int startIndex = -1;
										while (nameTokenIndex > 0) {
											if (nameContent[nameTokenIndex].Type == TokenType.LeftArrow) {
												break;
											}
											--nameTokenIndex;
										}
										if (nameTokenIndex == 0 || nameTokenIndex == nameContent.Count - 1) {
											throw ParseException.AsToken(nameContent.Last, Error.Analysis.UnexpectedToken);
										}
										else {
											method.GenericTypes = GenericReader.ReadDeclare(nameContent, ref startIndex, null);
										}
									}
									if (method.GenericTypes == null) {
										method.GenericTypes = new GenericDeclare.Builder[0];
									}

									//Name
									var name = nameContent[nameTokenIndex];
									if (name.Type != TokenType.Identifier) {
										throw ParseException.AsToken(name, Error.Analysis.IdentifierExpected);
									}
									method.Name = (TokenIdentifier)name;

									//Explicit interface
									if (nameTokenIndex > 0) {
										int dotIndex = nameTokenIndex - 1;
										if (dotIndex == 0 || nameContent[dotIndex].Type != TokenType.Dot) {
											throw ParseException.AsToken(nameContent[dotIndex], Error.Analysis.UnexpectedToken);
										}
										var typeBuilder = new ReferenceToManagedType.Builder {
											Content = nameContent.AsBeginEnd(0, dotIndex),
										};
										method.ExplicitInterface = new ReferenceToManagedType(typeBuilder);
										CheckInvalid(accessModifier, newModifier, sealedModifier, instanceKindModifier);
									}

									//Parameters
									bool isExtensionMethodAvailable = instanceKindModifier?.Type == KeywordType.@static && classTraints.InstanceKind == TypeInstanceKind.@static && classTraints.GenericTypes.Count == 0 && classTraints.IsRoot;
									method.Parameters = ParameterReader.Read(infoOutput, block.ReadParenthesisBlock(ref index), isExtensionMethodAvailable);

									//Generic constraint
									token = block.GetToken(index, Error.Analysis.LeftBraceExpected);
									if ((token as TokenIdentifier)?.ContextualKeyword == ContextualKeywordType.where) {
										var errorMessage = (method.InstanceKind != MethodInstanceKind.@abstract && method.InstanceKind != MethodInstanceKind.@extern) ? Error.Analysis.LeftBraceExpected : Error.Analysis.SemicolonExpected;
										GenericReader.ReadConstraint(method.GenericTypes, block, ref index, errorMessage);
									}

									//Body
									MethodBodyReader.ReadAsMethod(infoOutput, block, ref index, method);

									builder.Methods.Add(method);
									fnNextMember();
								}
								break;
							case TokenType.LeftBrace:
							case TokenType.LeftBracket:
							case TokenType.Lambda:
								{
									//Property
									bool isIndexer = token.Type == TokenType.LeftBracket;
									CheckInvalid(readonlyModifier, partialModifier, asyncModifier);
									if (type is ReferenceVoid) {
										infoOutput.OutputError(ParseException.AsTokenBlock(type.Content, Error.Analysis.VoidNotAllowed));
									}
									if (isIndexer && nameContent.Last.Type != TokenType.@this) {
										throw ParseException.AsToken(token, Error.Analysis.UnexpectedToken);
									}
									if (!isIndexer && nameContent.Last.Type != TokenType.Identifier) {
										throw ParseException.AsToken(token, Error.Analysis.IdentifierExpected);
									}

									var property = isIndexer ? (PropertyDeclare.Builder)new IndexerDeclare.Builder() : new MemberPropertyDeclare.Builder();
									property.CustomAttributes = currentAttributes.ToArray();
									property.AccessModifier = fnGetAccessModifier();
									property.IsNew = newModifier != null;
									property.IsSealed = sealedModifier != null;
									property.IsUnsafe = unsafeModifier != null;
									property.Type = type;

									//Instance kind
									if (instanceKindModifier != null) {
										if (sealedModifier != null && instanceKindModifier.Type != KeywordType.@override) {
											infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.SealedOnlyWithOverride));
											property.IsSealed = false;
										}
										if (!Enum.TryParse(instanceKindModifier.Type.ToString(), out property.InstanceKind)) {
											infoOutput.OutputError(ParseException.AsToken(instanceKindModifier, Error.Analysis.InvalidModifier));
										}
									}
									if (classTraints.InstanceKind == TypeInstanceKind.@static && property.InstanceKind != PropertyInstanceKind.@static) {
										infoOutput.OutputError(ParseException.AsTokenBlock(nameContent, Error.Analysis.StaticTypeOnly));
									}
									if (classTraints.InstanceKind != TypeInstanceKind.@abstract && property.InstanceKind == PropertyInstanceKind.@abstract) {
										infoOutput.OutputError(ParseException.AsTokenBlock(nameContent, Error.Analysis.AbstractTypeOnly));
									}

									//Name content
									if (!isIndexer) {
										property.Name = (TokenIdentifier)nameContent.Last;
									}
									if (nameContent.Count > 1) {
										int dotIndex = nameContent.Count - 2;
										if (dotIndex == 0 || nameContent[dotIndex].Type != TokenType.Dot) {
											throw ParseException.AsToken(nameContent[dotIndex], Error.Analysis.UnexpectedToken);
										}
										var typeBuilder = new ReferenceToManagedType.Builder {
											Content = block.AsBeginEnd(0, dotIndex),
										};
										property.ExplicitInterface = new ReferenceToManagedType(typeBuilder);
										CheckInvalid(accessModifier, newModifier, sealedModifier, instanceKindModifier);
									}

									//Parameter
									if (isIndexer) {
										var parameters = ParameterReader.Read(infoOutput, block.ReadBracketBlock(ref index), false);
										if (parameters.Count == 0) {
											infoOutput.OutputError(ParseException.AsTokenBlock(nameContent, Error.Analysis.IndexerParameterInvalid));
										}
										((IndexerDeclare.Builder)property).Parameters = parameters;
										token = block.GetToken(index, Error.Analysis.LeftBraceExpected);
									}

									//Expression body
									if (token.Type == TokenType.Lambda) {
										if (property.InstanceKind == PropertyInstanceKind.@abstract) {
											infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.MemberCannotHaveBody));
										}
										++index;
										token = block.GetToken(index, Error.Analysis.LeftBraceExpected);
										property.GetterMethod = new PropertyMethodDeclare.Builder {
											CustomAttributes = new AttributeObject.Builder[0],
											AccessModifier = AccessModifier.@public,
											Parameters = new ParameterObject[0],
										};
										if (token.Type != TokenType.LeftBrace) {
											property.GetterMethod.LambdaContentStyle = LambdaStyle.SingleLine;
											property.GetterMethod.BodyContent = block.ReadPastSpecificToken(ref index, TokenType.Semicolon, Error.Analysis.SemicolonExpected);
										}
										else {
											property.GetterMethod.LambdaContentStyle = LambdaStyle.MultiLine;
											property.GetterMethod.BodyContent = block.ReadBraceBlock(ref index);
										}
										goto tagAddProperty;
									}

									//Accessors
									accessModifier = null;
									unsafeModifier = null;
									asyncModifier = null;
									currentAttributes.Clear();
									block.NextToken(index++, TokenType.LeftBrace, Error.Analysis.LeftBraceExpected);
									while (true) {
										token = block.GetToken(index++, Error.Analysis.RightBraceExpected);
										if (token.Type == TokenType.RightBrace) {
											if (property.GetterMethod == null) {
												infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.PropertyGetterRequired));
											}
											break;
										}
										else if (token.Type == TokenType.LeftBracket) {
											var modifierTokens = new Token[] {
												accessModifier, unsafeModifier, asyncModifier
											};
											if (modifierTokens.Any(item => item != null)) {
												infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.InvalidAttributeUsage));
											}
											currentAttributes.Add(AttributeObjectReader.Read(block, ref index));
										}
										else if (token.Type == TokenType.@protected || token.Type == TokenType.@private || token.Type == TokenType.@internal) {
											CheckConflict(accessModifier, token);
											CheckInconsistent(unsafeModifier, asyncModifier);
											accessModifier = (TokenKeyword)token;
										}
										else if (token.Type == TokenType.@unsafe) {
											CheckDuplicate(unsafeModifier, token);
											CheckInconsistent(asyncModifier);
											unsafeModifier = (TokenKeyword)token;
										}
										else if ((token as TokenIdentifier)?.ContextualKeyword == ContextualKeywordType.async) {
											CheckDuplicate(asyncModifier, token);
											asyncModifier = (TokenIdentifier)token;
										}
										else if ((token as TokenIdentifier)?.ContextualKeyword == ContextualKeywordType.get ||
												 (token as TokenIdentifier)?.ContextualKeyword == ContextualKeywordType.set) {
											bool isGetter = (token as TokenIdentifier)?.ContextualKeyword == ContextualKeywordType.get;
											if ((isGetter && property.GetterMethod != null) ||
												(!isGetter && property.SetterMethod != null)) {
												infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.DuplicatePropertyAccessor));
											}
											var method = new PropertyMethodDeclare.Builder {
												CustomAttributes = currentAttributes.ToArray(),
												AccessModifier = AccessModifier.@public,
												Parameters = new ParameterObject[0],
												IsUnsafe = unsafeModifier != null,
												IsAsync = asyncModifier != null,
											};
											currentAttributes.Clear();

											//Access
											if (accessModifier != null) {
												var methodAccess = fnGetAccessModifier();
												if (!ConstantTable.ValidPropertyMethodAccessors[property.AccessModifier].Contains(methodAccess)) {
													infoOutput.OutputError(ParseException.AsToken(accessModifier, Error.Analysis.InvalidModifier));
												}
												else {
													method.AccessModifier = methodAccess;
												}
											}

											//Body
											MethodBodyReader.ReadAsProperty(infoOutput, block, ref index, property, method);

											if (isGetter) {
												property.GetterMethod = method;
											}
											else {
												property.SetterMethod = method;
											}
										}
										else {
											throw ParseException.AsToken(token, Error.Analysis.UnexpectedToken);
										}
									}

									//Auto property
									bool isAutoProperty = false;
									if (property.GetterMethod.BodyContent == null && property.InstanceKind != PropertyInstanceKind.@abstract) {
										isAutoProperty = true;
										if (property.SetterMethod?.BodyContent != null) {
											infoOutput.OutputError(ParseException.AsTokenBlock(property.SetterMethod.BodyContent, Error.Analysis.PropertyInvalidAuto));
										}
										((MemberPropertyDeclare.Builder)property).IsAuto = true;
									}
									if (property.SetterMethod != null && property.SetterMethod.BodyContent == null && property.GetterMethod.BodyContent != null) {
										infoOutput.OutputError(ParseException.AsTokenBlock(property.GetterMethod.BodyContent, Error.Analysis.PropertyInvalidAuto));
									}
								
									//Default value
									token = block.GetToken(index);
									if (token?.Type == TokenType.Assign) {
										if (isIndexer) {
											throw ParseException.AsToken(token, Error.Analysis.IndexerNoDefaultValue);
										}
										if (!isAutoProperty) {
											throw ParseException.AsToken(token, Error.Analysis.PropertyDefaultValueOnlyForAuto);
										}
										if (property.InstanceKind == PropertyInstanceKind.@abstract) {
											throw ParseException.AsToken(token, Error.Analysis.AbstractPropertyCannotHaveDefaultValue);
										}

										++index;
										((MemberPropertyDeclare.Builder)property).DefaultValueContent = block.ReadPastSpecificToken(ref index, TokenType.Semicolon, Error.Analysis.SemicolonExpected);
									}

									tagAddProperty:
									if (isIndexer) {
										builder.Indexers.Add((IndexerDeclare.Builder)property);
									}
									else {
										builder.Properties.Add((MemberPropertyDeclare.Builder)property);
									}
									fnNextMember();
								}
								break;
							case TokenType.Semicolon:
							case TokenType.Assign:
								{
									CheckInvalid(sealedModifier, partialModifier, asyncModifier);
									if (type is ReferenceVoid) {
										infoOutput.OutputError(ParseException.AsTokenBlock(type.Content, Error.Analysis.VoidNotAllowed));
									}
									if (!(nameContent[0] is TokenIdentifier) || nameContent.Count > 1) {
										throw ParseException.AsTokenBlock(nameContent, Error.Analysis.UnexpectedToken);
									}
									var field = new FieldDeclare.Builder {
										CustomAttributes = currentAttributes.ToArray(),
										AccessModifier = fnGetAccessModifier(),
										Name = (TokenIdentifier)nameContent.First,
										IsNew = newModifier != null,
										IsReadonly = readonlyModifier != null,
										IsUnsafe = unsafeModifier != null,
										Type = type,
									};

									//Instance kind
									if (instanceKindModifier != null && !Enum.TryParse(instanceKindModifier.Type.ToString(), out field.InstanceKind)) {
										infoOutput.OutputError(ParseException.AsToken(instanceKindModifier, Error.Analysis.InvalidModifier));
									}
									if (classTraints.InstanceKind == TypeInstanceKind.@static && field.InstanceKind != FieldInstanceKind.@static) {
										infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.StaticTypeOnly));
									}

									//Default value
									++index;
									if (token.Type == TokenType.Assign) {
										field.DefaultValueContent = block.ReadPastSpecificToken(ref index, TokenType.Semicolon, Error.Analysis.SemicolonExpected);
									}

									builder.Fields.Add(field);
									fnNextMember();
								}
								break;
						}

					}
				}
			}
		}
	}
}

/*
 * public/protected/private/internal
 * 
 * new (with virtual/abstract/static/extern)
 * sealed (with override)
 * 
 * virtual/override/abstract/sealed/static/extern/const
 * readonly (and static)
 * 
 * unsafe
 * async
 * 
 */
