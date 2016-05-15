using System;
using System.Collections.Generic;
using System.Linq;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	partial class AnalysisCodeFileCS
	{
		void ReadRootClassTypeDeclare(AttributeObject[] customAttributes, bool isPublic, bool isUnsafe, bool isPartial, TypeInstanceKind instanceKind, Func<TokenIdentifier, RootClassTypeDeclare.Builder> fnReadMembers)
		{
			var nameToken = block.GetToken(index++, Error.Analysis.IdentifierExpected);
			if (nameToken.Type != TokenType.Identifier) {
				throw ParseException.AsToken(nameToken, Error.Analysis.IdentifierExpected);
			}

			var generics = GenericReader.ReadDeclare(block, ref index, Error.Analysis.LeftBraceExpected);
			var inherits = InheritanceReader.ReadParents(block, ref index, Error.Analysis.LeftBraceExpected);
			GenericReader.ReadConstraint(generics, block, ref index, Error.Analysis.LeftBraceExpected);

			var builder = fnReadMembers((TokenIdentifier)nameToken);
			builder.IsPublic = isPublic;
			builder.IsUnsafe = isUnsafe;
			builder.IsPartial = isPartial;
			builder.InstanceKind = instanceKind;
			builder.GenericTypes = generics.Select(item => new GenericDeclare(item)).ToArray();
			builder.Inherits = inherits;
			builder.Name = (TokenIdentifier)nameToken;
			builder.OwnerFile = ownerFile;
			builder.Usings = new UsingBlob(currentUsings.Peek());
			builder.Namespace = currentNamespace.ToArray();
			builder.CustomAttributes = customAttributes;
		}

		T ReadClassMembers<T>(TokenIdentifier nameToken, ClassTypeKind typeKind) where T : ClassTypeDeclare.Builder, new()
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
			TokenIdentifier asyncModifier = null;
			Action fnResetModifiers = () => {
				accessModifier = null;
				newModifier = null;
				instanceKindModifier = null;
				readonlyModifier = null;
				unsafeModifier = null;
				asyncModifier = null;
			};

			var currentAttributes = new List<AttributeObjectOnMember>();
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
					CheckInconsistent(newModifier, sealedModifier, instanceKindModifier, readonlyModifier, unsafeModifier, asyncModifier);
					accessModifier = (TokenKeyword)token;
				}
				else if (token.Type == TokenType.@new) {
					CheckDuplicate(newModifier, token);
					CheckInconsistent(sealedModifier, instanceKindModifier, readonlyModifier, unsafeModifier, asyncModifier);
					newModifier = (TokenKeyword)token;
				}
				else if (token.Type == TokenType.@sealed) {
					CheckDuplicate(sealedModifier, token);
					CheckInconsistent(instanceKindModifier, readonlyModifier, unsafeModifier, asyncModifier);
					sealedModifier = (TokenKeyword)token;
				}
				else if (ConstantTable.InstanceKindModifier.Contains(token.Type)) {
					if ((instanceKindModifier?.Type == KeywordType.@static && token.Type == TokenType.@extern) ||
						(instanceKindModifier?.Type == KeywordType.@extern && token.Type == TokenType.@static)) {
						infoOutput.OutputWarning(ParseException.AsToken(token, Error.Analysis.ExternImpliesStatic));
					}
					CheckConflict(instanceKindModifier, token);
					CheckInconsistent(readonlyModifier, unsafeModifier, asyncModifier);
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
					CheckInconsistent(unsafeModifier, asyncModifier);
					if (sealedModifier != null || (instanceKindModifier != null && instanceKindModifier.Type != KeywordType.@static)) {
						infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.ConflictModifier));
					}
					else {
						readonlyModifier = (TokenKeyword)token;
					}
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
				else if ((token as TokenIdentifier)?.ContextualKeyword == ContextualKeywordType.partial) {
					infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.PartialWithClassOnly));
				}
				else if ((token as TokenIdentifier)?.ContextualKeyword == ContextualKeywordType.inline) {
					infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.InlineNotAllowed));
				}
				//========================================================================
				// Nested type
				//========================================================================
				else if (token.Type == TokenType.@class) {

					//Next member
					fnResetModifiers();
				}
				else if (token.Type == TokenType.@struct) {

					//Next member
					fnResetModifiers();
				}
				else if (token.Type == TokenType.@interface) {

					//Next member
					fnResetModifiers();
				}
				else if (token.Type == TokenType.@enum) {

					//Next member
					fnResetModifiers();
				}
				else if (token.Type == TokenType.@delegate) {

					//Next member
					fnResetModifiers();
				}
				//========================================================================
				// Members
				//========================================================================
				else {
					//Constructor
					if (token.Text == nameToken.Text) {
						var nextToken = block.GetToken(index, Error.Analysis.LeftParenthesisExpected);
						if (nextToken.Type == TokenType.LeftParenthesis) {
							
						}
					}

					//Destructor
					if (token.Type == TokenType.BitwiseNot) {
						if (typeKind == ClassTypeKind.@struct) {
							throw ParseException.AsToken(token, Error.Analysis.StructNoDestructor);
						}

					}

					//Conversion operator
					if (token.Type == TokenType.@explicit || token.Type == TokenType.@implicit) {
						
					}

					//Next member
					fnResetModifiers();
				}
			}
		}

		T ReadInterfaceMembers<T>(TokenIdentifier nameToken) where T : ClassTypeDeclare.Builder, new()
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
