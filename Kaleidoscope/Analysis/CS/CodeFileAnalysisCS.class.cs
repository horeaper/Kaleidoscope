using System;
using System.Linq;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	partial class CodeFileAnalysisCS
	{
		void ReadClass(AttributeObject[] customAttributes, bool isPublic, TokenKeyword instanceKindModifier, bool isUnsafe, bool isPartial)
		{
			var nameToken = block.GetToken(index++, Error.Analysis.IdentifierExpected);
			if (nameToken.Type != TokenType.Identifier) {
				throw ParseException.AsToken(nameToken, Error.Analysis.UnexpectedToken);
			}

			var generics = GenericReader.ReadDeclare(block, ref index, Error.Analysis.LeftBraceExpected);
			var inherits = InheritanceReader.ReadParents(block, ref index, Error.Analysis.LeftBraceExpected);
			GenericReader.ReadConstraint(generics, block, ref index, Error.Analysis.LeftBraceExpected);

			var builder = (RootClassTypeDeclare.Builder)ReadClassMembers(false);
			builder.TypeKind = ClassTypeKind.@class;
			builder.InstanceKind = (TypeInstanceKind)Enum.Parse(typeof(TypeInstanceKind), instanceKindModifier.Type.ToString());
			builder.IsPublic = isPublic;
			builder.IsUnsafe = isUnsafe;
			builder.IsPartial = isPartial;
			builder.GenericTypes = generics.Select(item => new GenericDeclare(item)).ToArray();
			builder.Inherits = inherits;

			builder.Name = (TokenIdentifier)nameToken;
			builder.Usings = new UsingBlob(currentUsings.Peek());
			builder.Namespace = currentNamespace.ToArray();
			builder.CustomAttributes = customAttributes;
		}

		void ReadStruct(AttributeObject[] customAttributes, bool isPublic, bool isUnsafe, bool isPartial)
		{
			var nameToken = block.GetToken(index++, Error.Analysis.IdentifierExpected);
			if (nameToken.Type != TokenType.Identifier) {
				throw ParseException.AsToken(nameToken, Error.Analysis.UnexpectedToken);
			}

			var generics = GenericReader.ReadDeclare(block, ref index, Error.Analysis.LeftBraceExpected);
			var inherits = InheritanceReader.ReadParents(block, ref index, Error.Analysis.LeftBraceExpected);
			GenericReader.ReadConstraint(generics, block, ref index, Error.Analysis.LeftBraceExpected);

			var builder = (RootClassTypeDeclare.Builder)ReadClassMembers(false);
			builder.TypeKind = ClassTypeKind.@struct;
			builder.IsPublic = isPublic;
			builder.IsUnsafe = isUnsafe;
			builder.IsPartial = isPartial;
			builder.GenericTypes = generics.Select(item => new GenericDeclare(item)).ToArray();
			builder.Inherits = inherits;

			builder.Name = (TokenIdentifier)nameToken;
			builder.Usings = new UsingBlob(currentUsings.Peek());
			builder.Namespace = currentNamespace.ToArray();
			builder.CustomAttributes = customAttributes;
		}

		void ReadInterface(AttributeObject[] customAttributes, bool isPublic, bool isPartial)
		{
			var nameToken = block.GetToken(index++, Error.Analysis.IdentifierExpected);
			if (nameToken.Type != TokenType.Identifier) {
				throw ParseException.AsToken(nameToken, Error.Analysis.UnexpectedToken);
			}

			var generics = GenericReader.ReadDeclare(block, ref index, Error.Analysis.LeftBraceExpected);
			var inherits = InheritanceReader.ReadParents(block, ref index, Error.Analysis.LeftBraceExpected);
			GenericReader.ReadConstraint(generics, block, ref index, Error.Analysis.LeftBraceExpected);

			var builder = (RootClassTypeDeclare.Builder)ReadInterfaceMembers(false, Error.Analysis.LeftBraceExpected);
			builder.TypeKind = ClassTypeKind.@interface;
			builder.IsPublic = isPublic;
			builder.IsPartial = isPartial;
			builder.GenericTypes = generics.Select(item => new GenericDeclare(item)).ToArray();
			builder.Inherits = inherits;

			builder.Name = (TokenIdentifier)nameToken;
			builder.Usings = new UsingBlob(currentUsings.Peek());
			builder.Namespace = currentNamespace.ToArray();
			builder.CustomAttributes = customAttributes;
		}

		ClassTypeDeclare.Builder ReadClassMembers(bool isNestedClass)
		{
			var builder = isNestedClass ? (ClassTypeDeclare.Builder)new NestedClassTypeDeclare.Builder() : new RootClassTypeDeclare.Builder();
		}
	}
}
