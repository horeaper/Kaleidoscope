using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	public static class InstanceReferenceReader
	{
		public static ReferenceToInstance Read(TokenBlock block, ref int index, bool isConstantOnly)
		{
			var token = block.GetToken(index, Error.Analysis.UnexpectedToken);
			if (token.Type == TokenType.NumberLiteral ||
				token.Type == TokenType.BooleanLiteral ||
				token.Type == TokenType.Character ||
				token.Type == TokenType.StringLiteral) {
				++index;
				return new ReferenceToInstance(block.AsBeginEnd(index, 1), isConstantOnly);
			}

			return new ReferenceToInstance(ReferenceReader.ReadTypeContent(block, ref index, ReferenceReader.ContentStyle.None), isConstantOnly);
		}
	}
}
