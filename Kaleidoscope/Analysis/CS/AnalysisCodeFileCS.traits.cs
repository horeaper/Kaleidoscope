using System;
using System.Collections.Generic;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	partial class AnalysisCodeFileCS
	{
		//========================================================================
		// class/struct
		//========================================================================
		
		class ClassTraits
		{
			public AttributeObject.Builder[] CustomAttributes;
			public bool IsRoot;
			public TokenIdentifier Name;
			public TypeInstanceKind InstanceKind;
			public bool IsPartial;
			public List<GenericDeclare.Builder> GenericTypes;
			public List<ReferenceToManagedType> Inherits;
		}

		RootClassTypeDeclare ReadRootClassDeclare(AttributeObject.Builder[] customAttributes, bool isPublic, bool isPartial, TypeInstanceKind instanceKind, Func<ClassTraits, RootClassTypeDeclare.Builder> fnReadMembers)
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
				Name = (TokenIdentifier)nameToken,
				InstanceKind = instanceKind,
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

		NestedClassTypeDeclare.Builder ReadNestedClassDeclare(AttributeObject.Builder[] customAttributes, AccessModifier accessModifier, bool isNew, bool isPartial, TypeInstanceKind instanceKind, Func<ClassTraits, NestedClassTypeDeclare.Builder> fnReadMembers)
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
				Name = (TokenIdentifier)nameToken,
				InstanceKind = instanceKind,
				IsPartial = isPartial,
				GenericTypes = generics,
				Inherits = inherits
			});
			builder.AccessModifier = accessModifier;
			builder.IsNew = isNew;
			return builder;
		}

		//========================================================================
		// interface
		//========================================================================
		
		class InterfaceTraits
		{
			public AttributeObject.Builder[] CustomAttributes;
			public bool IsRoot;
			public TokenIdentifier Name;
			public bool IsPartial;
			public List<GenericDeclare.Builder> GenericTypes;
			public List<ReferenceToManagedType> Inherits;
		}

		RootInterfaceTypeDeclare ReadRootInterfaceDeclare(AttributeObject.Builder[] customAttributes, bool isPublic, bool isPartial)
		{
			var nameToken = block.GetToken(index++, Error.Analysis.IdentifierExpected);
			if (nameToken.Type != TokenType.Identifier) {
				--index;
				throw ParseException.AsToken(nameToken, Error.Analysis.IdentifierExpected);
			}

			var generics = GenericReader.ReadDeclare(block, ref index, Error.Analysis.LeftBraceExpected);
			var inherits = InheritanceReader.ReadParents(block, ref index, Error.Analysis.LeftBraceExpected);
			GenericReader.ReadConstraint(generics, block, ref index, Error.Analysis.LeftBraceExpected);

			var builder = ReadInterfaceMembers<RootInterfaceTypeDeclare.Builder>(new InterfaceTraits {
				CustomAttributes = customAttributes,
				IsRoot = true,
				Name = (TokenIdentifier)nameToken,
				IsPartial = isPartial,
				GenericTypes = generics,
				Inherits = inherits
			});
			builder.OwnerFile = ownerFile;
			builder.Usings = new UsingBlob(currentUsings.Peek());
			builder.Namespace = currentNamespace.Get();
			builder.IsPublic = isPublic;
			return new RootInterfaceTypeDeclare(builder);
		}

		NestedInterfaceTypeDeclare.Builder ReadNestedInterfaceDeclare(AttributeObject.Builder[] customAttributes, AccessModifier accessModifier, bool isNew, bool isPartial)
		{
			var nameToken = block.GetToken(index++, Error.Analysis.IdentifierExpected);
			if (nameToken.Type != TokenType.Identifier) {
				--index;
				throw ParseException.AsToken(nameToken, Error.Analysis.IdentifierExpected);
			}

			var generics = GenericReader.ReadDeclare(block, ref index, Error.Analysis.LeftBraceExpected);
			var inherits = InheritanceReader.ReadParents(block, ref index, Error.Analysis.LeftBraceExpected);
			GenericReader.ReadConstraint(generics, block, ref index, Error.Analysis.LeftBraceExpected);

			var builder = ReadInterfaceMembers<NestedInterfaceTypeDeclare.Builder>(new InterfaceTraits {
				CustomAttributes = customAttributes,
				IsRoot = false,
				Name = (TokenIdentifier)nameToken,
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
