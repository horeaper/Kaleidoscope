using System;
using System.Collections.Generic;
using System.Linq;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	partial class AnalysisCodeFileCS
	{
		delegate RootClassTypeDeclare.Builder FuncReadRootMembers(TokenIdentifier nameToken);

		RootClassTypeDeclare ReadRootClassDeclare(AttributeObject.Builder[] customAttributes, bool isPublic, bool isUnsafe, bool isPartial, TypeInstanceKind instanceKind, FuncReadRootMembers fnReadMembers)
		{
			var nameToken = block.GetToken(index++, Error.Analysis.IdentifierExpected);
			if (nameToken.Type != TokenType.Identifier) {
				--index;
				infoOutput.OutputError(ParseException.AsToken(nameToken, Error.Analysis.IdentifierExpected));
			}

			var generics = GenericReader.ReadDeclare(block, ref index, Error.Analysis.LeftBraceExpected);
			var inherits = InheritanceReader.ReadParents(block, ref index, Error.Analysis.LeftBraceExpected);
			GenericReader.ReadConstraint(generics, block, ref index, Error.Analysis.LeftBraceExpected);

			var builder = fnReadMembers((TokenIdentifier)nameToken);
			builder.CustomAttributes = customAttributes;
			builder.OwnerFile = ownerFile;
			builder.Usings = new UsingBlob(currentUsings.Peek());
			builder.Namespace = currentNamespace.Get();
			builder.IsPublic = isPublic;
			builder.InstanceKind = instanceKind;
			builder.IsUnsafe = isUnsafe;
			builder.IsPartial = isPartial;
			builder.GenericTypes = generics.Select(item => new GenericDeclare(item));
			builder.Inherits = inherits;
			return new RootClassTypeDeclare(builder);
		}

		delegate NestedClassTypeDeclare.Builder FuncReadNestedMembers(TokenIdentifier nameToken);

		NestedClassTypeDeclare.Builder ReadNestedClassDeclare(AttributeObject.Builder[] customAttributes, AccessModifier accessModifier, bool isNew, bool isUnsafe, bool isPartial, TypeInstanceKind instanceKind, FuncReadNestedMembers fnReadMembers)
		{
			var nameToken = block.GetToken(index++, Error.Analysis.IdentifierExpected);
			if (nameToken.Type != TokenType.Identifier) {
				--index;
				infoOutput.OutputError(ParseException.AsToken(nameToken, Error.Analysis.IdentifierExpected));
			}

			var generics = GenericReader.ReadDeclare(block, ref index, Error.Analysis.LeftBraceExpected);
			var inherits = InheritanceReader.ReadParents(block, ref index, Error.Analysis.LeftBraceExpected);
			GenericReader.ReadConstraint(generics, block, ref index, Error.Analysis.LeftBraceExpected);

			var builder = fnReadMembers((TokenIdentifier)nameToken);
			builder.CustomAttributes = customAttributes;
			builder.AccessModifier = accessModifier;
			builder.IsNew = isNew;
			builder.InstanceKind = instanceKind;
			builder.IsUnsafe = isUnsafe;
			builder.IsPartial = isPartial;
			builder.GenericTypes = generics.Select(item => new GenericDeclare(item));
			builder.Inherits = inherits;
			return builder;
		}

		T ReadClassMembers<T>(TokenIdentifier typeName, ClassTypeKind typeKind) where T : ClassTypeDeclare.Builder, new()
		{
			var builder = new T();
			builder.TypeKind = typeKind;
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
					currentAttributes.Add(AttributeObjectReader.Read(block, ref index));
				}
				//========================================================================
				// Modifiers
				//========================================================================
				else if (ConstantTable.AccessModifier.Contains(token.Type)) {
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
					else if (sealedModifier != null && token.Type != TokenType.@override) {
						infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.SealedOnlyWithOverride));
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
					CheckInconsistent(unsafeModifier, partialModifier, asyncModifier);
					CheckInvalid(partialModifier, asyncModifier);
					if (sealedModifier != null || (instanceKindModifier != null && instanceKindModifier.Type != KeywordType.@static)) {
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
				else if (token.Text == typeName.Text) { //Constructor
					CheckInvalid(newModifier, sealedModifier, readonlyModifier, partialModifier);
					var constructor = new ConstructorDeclare.Builder {
						Name = (TokenIdentifier)token,
						CustomAttributes = currentAttributes.ToArray(),
						AccessModifier = fnGetAccessModifier(),
					};

					//'static' modifier
					bool isStatic = false;
					if (instanceKindModifier != null) {
						if (instanceKindModifier.Type != KeywordType.@static) {
							isStatic = true;
						}
						else {
							infoOutput.OutputError(ParseException.AsToken(instanceKindModifier, Error.Analysis.InvalidModifier));
						}
					}
					if (isStatic && builder.StaticConstructor != null) {
						infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.DuplicatedStaticConstructor));
					}
					constructor.InstanceKind = isStatic ? MethodInstanceKind.@static : MethodInstanceKind.None;

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
							token = block.GetToken(index, Error.Analysis.UnexpectedToken);
						}
					}

					//Body
					if (token.Type == TokenType.Semicolon) {
						infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.MethodBodyExpected));
						++index;
					}
					else {
						MethodBodyReader.Read(block, ref index, constructor);
					}

					if (isStatic) {
						builder.StaticConstructor = constructor;
					}
					else {
						builder.Constructors.Add(constructor);
					}
					fnNextMember();
				}
				else if (token.Type == TokenType.BitwiseNot) {  //Destructor
					CheckInvalid(accessModifier, newModifier, sealedModifier, instanceKindModifier, readonlyModifier, partialModifier);
					if (typeKind == ClassTypeKind.@struct) {
						infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.StructNoDestructor));
					}
					if (builder.Destructor != null) {
						infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.DuplicateDestructor));
					}
					var destructor = new DestructorDeclare.Builder {
						CustomAttributes = currentAttributes.ToArray(),
						AccessModifier = AccessModifier.@private,
						InstanceKind = MethodInstanceKind.None,
						Parameters = new ParameterObject[0],
					};

					var nameToken = block.GetToken(index++, Error.Analysis.UnexpectedToken);
					if (nameToken.Text != typeName.Text) {
						infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.DestructorNameInvalid));
					}
					else {
						destructor.Name = (TokenIdentifier)nameToken;
					}

					//Empty parameter
					block.NextToken(index++, TokenType.LeftParenthesis, Error.Analysis.LeftParenthesisExpected);
					block.NextToken(index++, TokenType.RightParenthesis, Error.Analysis.RightParenthesisExpected);

					//Body
					if (token.Type == TokenType.Semicolon) {
						infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.MethodBodyExpected));
						++index;
					}
					else {
						MethodBodyReader.Read(block, ref index, destructor);
					}

					builder.Destructor = destructor;
					fnNextMember();
				}
				else if (token.Type == TokenType.@explicit || token.Type == TokenType.@implicit) {  //Conversion operator
					//TODO:
					fnNextMember();
				}
				else {
					throw ParseException.AsToken(token, Error.Analysis.UnexpectedToken);
				}
			}
		}

		T ReadInterfaceMembers<T>(TokenIdentifier typeName) where T : ClassTypeDeclare.Builder, new()
		{
			var builder = new T();
			builder.TypeKind = ClassTypeKind.@interface;
			var token = block.GetToken(index++, Error.Analysis.LeftBraceExpected);
			if (token.Type != TokenType.LeftBrace) {
				throw ParseException.AsToken(token, Error.Analysis.LeftBraceExpected);
			}

			while (true) {
				token = block.GetToken(index++, Error.Analysis.RightBraceExpected);

				if (token.Type == TokenType.RightBrace) {
					return builder;
				}
				else if (ConstantTable.AccessModifier.Contains(token.Type) ||
						 ConstantTable.InstanceKindModifier.Contains(token.Type) ||
						 token.Type == TokenType.@readonly ||
						 token.Type == TokenType.@unsafe)
				{
					throw ParseException.AsToken(token, Error.Analysis.InvalidModifier);
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
