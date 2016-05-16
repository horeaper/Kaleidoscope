using System;
using System.Collections.Generic;
using System.Linq;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	partial class AnalysisCodeFileCS
	{
		delegate RootClassTypeDeclare.Builder FuncReadRootMembers(RootClassTypeDeclare self, TokenIdentifier nameToken);

		RootClassTypeDeclare ReadRootClassDeclare(AttributeObject[] customAttributes, bool isPublic, bool isUnsafe, bool isPartial, TypeInstanceKind instanceKind, FuncReadRootMembers fnReadMembers)
		{
			var nameToken = block.GetToken(index++, Error.Analysis.IdentifierExpected);
			if (nameToken.Type != TokenType.Identifier) {
				throw ParseException.AsToken(nameToken, Error.Analysis.IdentifierExpected);
			}

			return new RootClassTypeDeclare((TokenIdentifier)nameToken, customAttributes, self => ReadRootClassMembers(self, (TokenIdentifier)nameToken, isPublic, isUnsafe, isPartial, instanceKind, fnReadMembers));
		}

		RootClassTypeDeclare.Builder ReadRootClassMembers(RootClassTypeDeclare self, TokenIdentifier nameToken, bool isPublic, bool isUnsafe, bool isPartial, TypeInstanceKind instanceKind, FuncReadRootMembers fnReadMembers)
		{
			var generics = GenericReader.ReadDeclare(block, ref index, Error.Analysis.LeftBraceExpected);
			var inherits = InheritanceReader.ReadParents(block, ref index, Error.Analysis.LeftBraceExpected);
			GenericReader.ReadConstraint(generics, block, ref index, Error.Analysis.LeftBraceExpected);

			var builder = fnReadMembers(self, nameToken);
			builder.OwnerFile = ownerFile;
			builder.Usings = new UsingBlob(currentUsings.Peek());
			builder.Namespace = currentNamespace.ToArray();
			builder.IsPublic = isPublic;
			builder.InstanceKind = instanceKind;
			builder.IsUnsafe = isUnsafe;
			builder.IsPartial = isPartial;
			builder.GenericTypes = generics.Select(item => new GenericDeclare(item)).ToArray();
			builder.Inherits = inherits;
			return builder;
		}

		delegate NestedClassTypeDeclare.Builder FuncReadNestedMembers(NestedClassTypeDeclare self, TokenIdentifier nameToken);

		NestedClassTypeDeclare ReadNestedClassDeclare(ClassTypeDeclare owner, AttributeObjectOnMember[] customAttributes, AccessModifier accessModifier, bool isNew, bool isUnsafe, bool isPartial, TypeInstanceKind instanceKind, FuncReadNestedMembers fnReadMembers)
		{
			var nameToken = block.GetToken(index++, Error.Analysis.IdentifierExpected);
			if (nameToken.Type != TokenType.Identifier) {
				throw ParseException.AsToken(nameToken, Error.Analysis.IdentifierExpected);
			}

			return new NestedClassTypeDeclare((TokenIdentifier)nameToken, customAttributes.Cast<AttributeObject>().ToArray(), self => ReadNestedClassMembers(owner, self, (TokenIdentifier)nameToken, accessModifier, isNew, isUnsafe, isPartial, instanceKind, fnReadMembers));
		}

		NestedClassTypeDeclare.Builder ReadNestedClassMembers(ClassTypeDeclare owner, NestedClassTypeDeclare self, TokenIdentifier nameToken, AccessModifier accessModifier, bool isNew, bool isUnsafe, bool isPartial, TypeInstanceKind instanceKind, FuncReadNestedMembers fnReadMembers)
		{
			var generics = GenericReader.ReadDeclare(block, ref index, Error.Analysis.LeftBraceExpected);
			var inherits = InheritanceReader.ReadParents(block, ref index, Error.Analysis.LeftBraceExpected);
			GenericReader.ReadConstraint(generics, block, ref index, Error.Analysis.LeftBraceExpected);

			var builder = fnReadMembers(self, nameToken);
			builder.ContainerType = owner;
			builder.AccessModifier = accessModifier;
			builder.IsNew = isNew;
			builder.InstanceKind = instanceKind;
			builder.IsUnsafe = isUnsafe;
			builder.IsPartial = isPartial;
			builder.GenericTypes = generics.Select(item => new GenericDeclare(item)).ToArray();
			builder.Inherits = inherits;
			return builder;
		}

		T ReadClassMembers<T>(ClassTypeDeclare self, TokenIdentifier typeName, ClassTypeKind typeKind) where T : ClassTypeDeclare.Builder, new()
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
			var currentAttributes = new List<AttributeObjectOnMember>();
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

			while (true) {
				token = block.GetToken(index++, Error.Analysis.RightBraceExpected);

				if (token.Type == TokenType.RightBrace) {
					return builder;
				}
				else if (token.Type == TokenType.LeftBracket) {
					currentAttributes.Add(AttributeObjectReader.ReadOnMember(block, ref index));
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
					if ((instanceKindModifier?.Type == KeywordType.@static && token.Type == TokenType.@extern) ||
						(instanceKindModifier?.Type == KeywordType.@extern && token.Type == TokenType.@static)) {
						infoOutput.OutputWarning(ParseException.AsToken(token, Error.Analysis.ExternImpliesStatic));
					}
					CheckConflict(instanceKindModifier, token);
					CheckInconsistent(readonlyModifier, unsafeModifier, partialModifier, asyncModifier);
					if (newModifier != null && !ConstantTable.ValidNewInstanceKindModifier.Contains(token.Type)) {
						infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.ConflictModifier));
					}
					else if (sealedModifier != null && token.Type != TokenType.@override) {
						infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.SealedOnlyWithOverride));
					}
					else {
						instanceKindModifier = (TokenKeyword)token;
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

					var access = AccessModifier.@private;
					if (accessModifier != null) {
						access = (AccessModifier)Enum.Parse(typeof(AccessModifier), accessModifier.Type.ToString());
					}

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

					builder.NestedClasses.Add(ReadNestedClassDeclare(self, currentAttributes.ToArray(), access, newModifier != null, unsafeModifier != null, partialModifier != null, instanceKind, (nestedSelf, nameToken) => ReadClassMembers<NestedClassTypeDeclare.Builder>(nestedSelf, nameToken, ClassTypeKind.@class)));
					fnNextMember();
				}
				else if (token.Type == TokenType.@struct) {
					CheckInvalid(sealedModifier, instanceKindModifier, readonlyModifier, asyncModifier);

					var access = AccessModifier.@private;
					if (accessModifier != null) {
						access = (AccessModifier)Enum.Parse(typeof(AccessModifier), accessModifier.Type.ToString());
					}

					builder.NestedClasses.Add(ReadNestedClassDeclare(self, currentAttributes.ToArray(), access, newModifier != null, unsafeModifier != null, partialModifier != null, TypeInstanceKind.None, (nestedSelf, nameToken) => ReadClassMembers<NestedClassTypeDeclare.Builder>(nestedSelf, nameToken, ClassTypeKind.@struct)));
					fnNextMember();
				}
				else if (token.Type == TokenType.@interface) {
					CheckInvalid(sealedModifier, instanceKindModifier, readonlyModifier, asyncModifier);

					var access = AccessModifier.@private;
					if (accessModifier != null) {
						access = (AccessModifier)Enum.Parse(typeof(AccessModifier), accessModifier.Type.ToString());
					}

					builder.NestedClasses.Add(ReadNestedClassDeclare(self, currentAttributes.ToArray(), access, newModifier != null, unsafeModifier != null, partialModifier != null, TypeInstanceKind.None, (nestedSelf, nameToken) => ReadInterfaceMembers< NestedClassTypeDeclare.Builder>(nestedSelf, nameToken)));
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
				else {
					//Constructor
					if (token.Text == typeName.Text) {
						var nextToken = block.GetToken(index, Error.Analysis.LeftParenthesisExpected);
						if (nextToken.Type == TokenType.LeftParenthesis) {
							
						}
					}

					//Destructor
					if (token.Type == TokenType.BitwiseNot) {
						if (typeKind == ClassTypeKind.@struct) {
							infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.StructNoDestructor));
						}

					}

					//Conversion operator
					if (token.Type == TokenType.@explicit || token.Type == TokenType.@implicit) {
						
					}

					//Next member
					fnNextMember();
				}
			}
		}

		T ReadInterfaceMembers<T>(ClassTypeDeclare self, TokenIdentifier typeName) where T : ClassTypeDeclare.Builder, new()
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
