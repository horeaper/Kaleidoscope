using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	public static class AttributeObjectReader
	{
		public static AttributeObjectOnType ReadOnType(TokenBlock block, ref int index, UsingBlob.Builder currentUsings, TokenIdentifier[] currentNamespace)
		{
			var type = (ReferenceToManagedType)TypeReferenceReader.Read(block, ref index, TypeParsingRule.None);

			TokenBlock content = null;
			var token = block.GetToken(index, Error.Analysis.RightBracketExpected);
			if (token.Type == TokenType.LeftParenthesis) {
				int startIndex = index + 1;
				int endIndex = block.FindNextParenthesisBlockEnd(index);
				content = block.AsBeginEnd(startIndex, endIndex - 1);

				index = endIndex;
				token = block.GetToken(index++, Error.Analysis.RightBracketExpected);
			}
			if (token.Type != TokenType.RightBracket) {
				throw ParseException.AsToken(token, Error.Analysis.RightBracketExpected);
			}

			return new AttributeObjectOnType(type, content, new UsingBlob(currentUsings), currentNamespace);
		}

		public static AttributeObjectOnMember ReadOnMember(TokenBlock block, ref int index)
		{
			var type = (ReferenceToManagedType)TypeReferenceReader.Read(block, ref index, TypeParsingRule.None);

			TokenBlock content = null;
			var token = block.GetToken(index, Error.Analysis.RightBracketExpected);
			if (token.Type == TokenType.LeftParenthesis) {
				int startIndex = index + 1;
				int endIndex = block.FindNextParenthesisBlockEnd(index);
				content = block.AsBeginEnd(startIndex, endIndex - 1);

				index = endIndex;
				token = block.GetToken(index++, Error.Analysis.RightBracketExpected);
			}
			if (token.Type != TokenType.RightBracket) {
				throw ParseException.AsToken(token, Error.Analysis.RightBracketExpected);
			}

			return new AttributeObjectOnMember(type, content);
		}
	}
}
