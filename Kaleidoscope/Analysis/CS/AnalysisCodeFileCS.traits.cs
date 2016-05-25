using System.Collections.Generic;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	partial class AnalysisCodeFileCS
	{
		class ClassTraits
		{
			public AttributeObject.Builder[] CustomAttributes;
			public bool IsRoot;
			public AccessModifier AccessModifier;
			public TokenIdentifier Name;
			public TypeInstanceKind InstanceKind;
			public bool IsUnsafe;
			public bool IsPartial;
			public List<GenericDeclare.Builder> GenericTypes;
			public List<ReferenceToManagedType> Inherits;
		}

		delegate RootClassTypeDeclare.Builder FuncReadRootMembers(ClassTraits classTraints);

		RootClassTypeDeclare ReadRootClassDeclare(AttributeObject.Builder[] customAttributes, bool isPublic, bool isUnsafe, bool isPartial, TypeInstanceKind instanceKind, FuncReadRootMembers fnReadMembers)
		{
			var nameToken = block.GetToken(index++, Error.Analysis.IdentifierExpected);
			if (nameToken.Type != TokenType.Identifier) {
				--index;
				throw ParseException.AsToken(nameToken, Error.Analysis.IdentifierExpected);
			}

			var generics = GenericReader.ReadDeclare(block, ref index, Error.Analysis.LeftBraceExpected);
			var inherits = InheritanceReader.ReadParents(block, ref index, Error.Analysis.LeftBraceExpected);
			GenericReader.ReadConstraint(generics, block, ref index, Error.Analysis.LeftBraceExpected);

			var builder = fnReadMembers(new ClassTraits {
				CustomAttributes = customAttributes,
				IsRoot = true,
				AccessModifier = isPublic ? AccessModifier.@public : AccessModifier.@internal,
				Name = (TokenIdentifier)nameToken,
				InstanceKind = instanceKind,
				IsUnsafe = isUnsafe,
				IsPartial = isPartial,
				GenericTypes = generics,
				Inherits = inherits
			});
			builder.OwnerFile = ownerFile;
			builder.Usings = new UsingBlob(currentUsings.Peek());
			builder.Namespace = currentNamespace.Get();
			builder.IsPublic = isPublic;
			return new RootClassTypeDeclare(builder);
		}

		delegate NestedClassTypeDeclare.Builder FuncReadNestedMembers(ClassTraits classTraints);

		NestedClassTypeDeclare.Builder ReadNestedClassDeclare(AttributeObject.Builder[] customAttributes, AccessModifier accessModifier, bool isNew, bool isUnsafe, bool isPartial, TypeInstanceKind instanceKind, FuncReadNestedMembers fnReadMembers)
		{
			var nameToken = block.GetToken(index++, Error.Analysis.IdentifierExpected);
			if (nameToken.Type != TokenType.Identifier) {
				--index;
				throw ParseException.AsToken(nameToken, Error.Analysis.IdentifierExpected);
			}

			var generics = GenericReader.ReadDeclare(block, ref index, Error.Analysis.LeftBraceExpected);
			var inherits = InheritanceReader.ReadParents(block, ref index, Error.Analysis.LeftBraceExpected);
			GenericReader.ReadConstraint(generics, block, ref index, Error.Analysis.LeftBraceExpected);

			var builder = fnReadMembers(new ClassTraits {
				CustomAttributes = customAttributes,
				IsRoot = false,
				AccessModifier = accessModifier,
				Name = (TokenIdentifier)nameToken,
				InstanceKind = instanceKind,
				IsUnsafe = isUnsafe,
				IsPartial = isPartial,
				GenericTypes = generics,
				Inherits = inherits
			});
			builder.AccessModifier = accessModifier;
			builder.IsNew = isNew;
			return builder;
		}
	}
}
