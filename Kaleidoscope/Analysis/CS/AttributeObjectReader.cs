using System.Linq;
using System.Collections.Generic;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	public static class AttributeObjectReader
	{
		public static AttributeObject.Builder Read(TokenBlock block, ref int index)
		{
			var type = (ReferenceToManagedType)TypeReferenceReader.Read(block, ref index, TypeParsingRule.None);

			TokenBlock content = null;
			var token = block.GetToken(index, Error.Analysis.RightBracketExpected);
			if (token.Type == TokenType.LeftParenthesis) {
				content = block.ReadParenthesisBlock(ref index);
				token = block.GetToken(index++, Error.Analysis.RightBracketExpected);
			}
			if (token.Type != TokenType.RightBracket) {
				throw ParseException.AsToken(token, Error.Analysis.RightBracketExpected);
			}

			return new AttributeObject.Builder {
				Type = type,
				ConstructContent = content,
			};
		}

		public static IEnumerable<AttributeObject> Get(IEnumerable<AttributeObject.Builder> attributes, ManagedDeclare target)
		{
			return attributes.Select(builder => new AttributeObject(builder, target));
		}
	}
}
