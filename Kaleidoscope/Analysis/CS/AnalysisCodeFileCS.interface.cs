using System;
using System.Collections.Generic;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	partial class AnalysisCodeFileCS
	{
		InterfaceTypeDeclare ReadInterfaceMembers(TypeTraits traits)
		{
			var builder = new InterfaceTypeDeclare.Builder {
				Name = traits.Name,
				CustomAttributes = traits.CustomAttributes,
				IsPartial = traits.IsPartial,
				GenericTypes = traits.GenericTypes,
			};

			//Managed type inheritance only
			var inherits = new List<ReferenceToManagedType>();
			foreach (var item in traits.Inherits) {
				var managedType = item as ReferenceToManagedType;
				if (managedType == null) {
					infoOutput.OutputError(ParseException.AsTokenBlock(item.Content, Error.Analysis.CppTypeNotAllowed));
				}
				else {
					inherits.Add(managedType);
				}
			}
			builder.Inherits = inherits;

			var token = block.GetToken(index++, Error.Analysis.LeftBraceExpected);
			if (token.Type != TokenType.LeftBrace) {
				throw ParseException.AsToken(token, Error.Analysis.LeftBraceExpected);
			}

			var currentAttributes = new List<AttributeObject.Builder>();
			Action fnNextMember = () => {
				currentAttributes.Clear();
			};
			while (true) {
				token = block.GetToken(index++, Error.Analysis.RightBraceExpected);

				if (token.Type == TokenType.RightBrace) {
					return new InterfaceTypeDeclare(builder);
				}
				else if (token.Type == TokenType.LeftBracket) {
					currentAttributes.Add(AttributeObjectReader.Read(block, ref index));
				}
				//========================================================================
				// Modifiers (not allowed)
				//========================================================================
				else if (ConstantTable.AccessModifiers.Contains(token.Type) ||
						 ConstantTable.InstanceKindModifier.Contains(token.Type) ||
						 token.Type == TokenType.@new ||
						 token.Type == TokenType.@sealed ||
						 token.Type == TokenType.@readonly ||
						 (token as TokenIdentifier)?.ContextualKeyword == ContextualKeywordType.partial ||
						 (token as TokenIdentifier)?.ContextualKeyword == ContextualKeywordType.async)
				{
					infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.InterfaceModifierNotAllowed));
				}
				else if (token.Type == TokenType.@unsafe) {
					infoOutput.OutputWarning(ParseWarning.AsToken(token, Error.Analysis.UnsafeNotAllowed));
				}
				else if ((token as TokenIdentifier)?.ContextualKeyword == ContextualKeywordType.inline) {
					infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.InlineNotAllowed));
				}
				//========================================================================
				// Nested type (not allowed)
				//========================================================================
				else if (token.Type == TokenType.@class ||
						 token.Type == TokenType.@struct ||
						 token.Type == TokenType.@interface ||
						 token.Type == TokenType.@enum ||
						 token.Type == TokenType.@delegate)
				{
					throw ParseException.AsToken(token, Error.Analysis.InterfaceNoNestedType);
				}
				//========================================================================
				// Members
				//========================================================================
				else if (token.Text == traits.Name.Text ||
						 token.Type == TokenType.BitwiseNot ||
						 token.Type == TokenType.@explicit ||
						 token.Type == TokenType.@implicit)
				{
					throw ParseException.AsToken(token, Error.Analysis.IdentifierExpected);
				}
				else {
					//Type
					--index;
					var type = ReferenceReader.Read(block, ref index, TypeParsingRule.AllowVoid | TypeParsingRule.AllowCppType | TypeParsingRule.AllowArray);

					//Name
					var nameToken = block.GetToken(index++, Error.Analysis.UnexpectedToken);
					token = block.GetToken(index, Error.Analysis.LeftParenthesisExpected);

					//Next
					switch (token.Type) {
						case TokenType.LeftArrow:
						case TokenType.LeftParenthesis:
							{
								//Method
								var method = new MemberMethodDeclare.Builder {
									CustomAttributes = currentAttributes.ToArray(),
									AccessModifier = AccessModifier.@public,
									ReturnType = type,
								};

								//Name
								if (nameToken.Type != TokenType.Identifier) {
									throw ParseException.AsToken(token, Error.Analysis.IdentifierExpected);
								}
								method.Name = (TokenIdentifier)nameToken;

								//Generic
								method.GenericTypes = GenericReader.ReadDeclare(block, ref index, Error.Analysis.UnexpectedToken);

								//Parameters
								block.NextToken(index, TokenType.LeftParenthesis, Error.Analysis.LeftParenthesisExpected);
								method.Parameters = ParameterReader.Read(infoOutput, block.ReadParenthesisBlock(ref index), false);

								//Generic constraint
								GenericReader.ReadConstraint(method.GenericTypes, block, ref index, Error.Analysis.SemicolonExpected);

								token = block.GetToken(index++, Error.Analysis.SemicolonExpected);
								if (token.Type != TokenType.Semicolon) {
									throw ParseException.AsToken(token, Error.Analysis.InterfaceMethodNoBody);
								}
								builder.Methods.Add(method);
								fnNextMember();
							}
							break;
						case TokenType.LeftBrace:
						case TokenType.LeftBracket:
							{
								//Property
								bool isIndexer = token.Type == TokenType.LeftBracket;
								if (type is ReferenceVoid) {
									infoOutput.OutputError(ParseException.AsTokenBlock(type.Content, Error.Analysis.VoidNotAllowed));
								}
								var property = isIndexer ? (PropertyDeclare.Builder)new IndexerDeclare.Builder() : new MemberPropertyDeclare.Builder();
								property.CustomAttributes = currentAttributes.ToArray();
								property.AccessModifier = AccessModifier.@public;
								property.Type = type;

								//Name
								if (!isIndexer) {
									if (nameToken.Type != TokenType.Identifier) {
										throw ParseException.AsToken(token, Error.Analysis.IdentifierExpected);
									}
									property.Name = (TokenIdentifier)nameToken;
								}
								else if (nameToken.Type != TokenType.@this) {
									throw ParseException.AsToken(token, Error.Analysis.UnexpectedToken);
								}

								//Parameter
								if (isIndexer) {
									var parameters = ParameterReader.Read(infoOutput, block.ReadBracketBlock(ref index), false);
									if (parameters.Count == 0) {
										infoOutput.OutputError(ParseException.AsToken(nameToken, Error.Analysis.IndexerParameterInvalid));
									}
									((IndexerDeclare.Builder)property).Parameters = parameters;
								}

								//Accessors
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
										currentAttributes.Add(AttributeObjectReader.Read(block, ref index));
									}
									else if (token.Type == TokenType.@protected ||
											 token.Type == TokenType.@private ||
											 token.Type == TokenType.@internal ||
											 (token as TokenIdentifier)?.ContextualKeyword == ContextualKeywordType.async)
									{
										infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.InterfaceModifierNotAllowed));
									}
									else if (token.Type == TokenType.@unsafe) {
										infoOutput.OutputWarning(ParseWarning.AsToken(token, Error.Analysis.UnsafeNotAllowed));
									}
									else if ((token as TokenIdentifier)?.ContextualKeyword == ContextualKeywordType.get ||
											 (token as TokenIdentifier)?.ContextualKeyword == ContextualKeywordType.set)
									{
										bool isGetter = (token as TokenIdentifier)?.ContextualKeyword == ContextualKeywordType.get;
										if ((isGetter && property.GetterMethod != null) ||
											(!isGetter && property.SetterMethod != null)) {
											infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.DuplicatePropertyAccessor));
										}
										var method = new PropertyMethodDeclare.Builder {
											CustomAttributes = currentAttributes.ToArray(),
											AccessModifier = AccessModifier.@public,
											Parameters = new ParameterObject[0],
										};
										currentAttributes.Clear();

										//Body
										++index;
										token = block.GetToken(index, Error.Analysis.SemicolonExpected);
										if (token.Type != TokenType.Semicolon) {
											throw ParseException.AsToken(token, Error.Analysis.InterfaceMethodNoBody);
										}

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

								if (isIndexer) {
									builder.Indexers.Add((IndexerDeclare.Builder)property);
								}
								else {
									builder.Properties.Add((MemberPropertyDeclare.Builder)property);
								}
								fnNextMember();
							}
							break;
						default:
							throw ParseException.AsToken(token, Error.Analysis.UnexpectedToken);
					}
				}
			}
		}
	}
}
