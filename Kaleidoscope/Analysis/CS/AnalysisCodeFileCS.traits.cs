using System;
using System.Collections.Generic;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	partial class AnalysisCodeFileCS
	{
		class TypeTraits
		{
			public bool IsRoot;
			public TokenIdentifier Name;
			public AttributeObject.Builder[] CustomAttributes;
			public TypeInstanceKind InstanceKind;
			public bool IsPartial;
			public List<GenericDeclare.Builder> GenericTypes;
			public List<ReferenceToType> Inherits;
		}

		RootTypeDeclare<T>.Builder ReadRootTypeDeclare<T>(AttributeObject.Builder[] customAttributes, bool isPublic, bool isPartial, TypeInstanceKind instanceKind, Func<TypeTraits, InstanceTypeDeclare> fnReadType) where T : InstanceTypeDeclare
		{
			var nameToken = block.GetToken(index++, Error.Analysis.IdentifierExpected);
			if (nameToken.Type != TokenType.Identifier) {
				--index;
				throw ParseException.AsToken(nameToken, Error.Analysis.IdentifierExpected);
			}

			var generics = GenericReader.ReadDeclare(block, ref index, Error.Analysis.LeftBraceExpected);
			var inherits = InheritanceReader.ReadParents(block, ref index, Error.Analysis.LeftBraceExpected);
			GenericReader.ReadConstraint(generics, block, ref index, Error.Analysis.LeftBraceExpected);

			var type = fnReadType(new TypeTraits {
				IsRoot = true,
				Name = (TokenIdentifier)nameToken,
				CustomAttributes = customAttributes,
				InstanceKind = instanceKind,
				IsPartial = isPartial,
				GenericTypes = generics,
				Inherits = inherits
			});

			return new RootTypeDeclare<T>.Builder {
				Type = (T)type,
				OwnerFile = ownerFile,
				Usings = new UsingBlob(currentUsings.Peek()),
				Namespace = currentNamespace.Get(),
				IsPublic = isPublic,
			};
		}

		NestedTypeDeclare<T>.Builder ReadNestedTypeDeclare<T>(AttributeObject.Builder[] customAttributes, AccessModifier accessModifier, bool isNew, bool isPartial, TypeInstanceKind instanceKind, Func<TypeTraits, InstanceTypeDeclare> fnReadType) where T : InstanceTypeDeclare
		{
			var nameToken = block.GetToken(index++, Error.Analysis.IdentifierExpected);
			if (nameToken.Type != TokenType.Identifier) {
				--index;
				throw ParseException.AsToken(nameToken, Error.Analysis.IdentifierExpected);
			}

			var generics = GenericReader.ReadDeclare(block, ref index, Error.Analysis.LeftBraceExpected);
			var inherits = InheritanceReader.ReadParents(block, ref index, Error.Analysis.LeftBraceExpected);
			GenericReader.ReadConstraint(generics, block, ref index, Error.Analysis.LeftBraceExpected);

			var type = fnReadType(new TypeTraits {
				IsRoot = false,
				Name = (TokenIdentifier)nameToken,
				CustomAttributes = customAttributes,
				InstanceKind = instanceKind,
				IsPartial = isPartial,
				GenericTypes = generics,
				Inherits = inherits
			});

			return new NestedTypeDeclare<T>.Builder {
				Type = (T)type,
				AccessModifier = accessModifier,
				IsNew = isNew,
			};
		}
	}
}
