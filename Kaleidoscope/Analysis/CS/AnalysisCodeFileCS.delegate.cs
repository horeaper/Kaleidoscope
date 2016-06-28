using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	partial class AnalysisCodeFileCS
	{
		RootTypeDeclare<DelegateTypeDeclare>.Builder ReadRootDelegateTypeDeclare(AttributeObject.Builder[] customAttributes, bool isPublic)
		{
			return new RootTypeDeclare<DelegateTypeDeclare>.Builder {
				Type = ReadDelegate(customAttributes),
				OwnerFile = ownerFile,
				Usings = new UsingBlob(currentUsings.Peek()),
				Namespace = currentNamespace.Get(),
				IsPublic = isPublic,
			};
		}

		NestedTypeDeclare<DelegateTypeDeclare>.Builder ReadNestedDelegateTypeDeclare(AttributeObject.Builder[] customAttributes, AccessModifier accessModifier, bool isNew)
		{
			return new NestedTypeDeclare<DelegateTypeDeclare>.Builder {
				Type = ReadDelegate(customAttributes),
				AccessModifier = accessModifier,
				IsNew = isNew,
			};
		}

		DelegateTypeDeclare ReadDelegate(AttributeObject.Builder[] customAttributes)
		{
			var builder = new DelegateTypeDeclare.Builder {
				CustomAttributes = customAttributes,
			};

			//Return type
			builder.ReturnType = ReferenceReader.Read(block, ref index, TypeParsingRule.AllowVoid | TypeParsingRule.AllowVar | TypeParsingRule.AllowCppType | TypeParsingRule.AllowArray);

			//Name
			var token = block.GetToken(index++, Error.Analysis.UnexpectedToken);
			if (token.Type != TokenType.Identifier) {
				throw ParseException.AsToken(token, Error.Analysis.IdentifierExpected);
			}
			builder.Name = (TokenIdentifier)token;

			//Generic
			builder.GenericTypes = GenericReader.ReadDeclare(block, ref index, Error.Analysis.LeftParenthesisExpected);

			//Parameter
			builder.Parameters = ParameterReader.Read(infoOutput, block.ReadParenthesisBlock(ref index), false);

			//Generic constraints
			GenericReader.ReadConstraint(builder.GenericTypes, block, ref index, Error.Analysis.SemicolonExpected);

			//End
			block.NextToken(index++, TokenType.Semicolon, Error.Analysis.SemicolonExpected);

			return new DelegateTypeDeclare(builder);
		}
	}
}
