using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
				token.Type == TokenType.String) {
				return new ReferenceToInstance(block.AsBeginEnd(index, 1), isConstantOnly);
			}

			var startIndex = index;
			while (true) {
				token = block.GetToken(index++);
				if (token == null) {
					throw ParseException.AsToken(block.Last, Error.Analysis.IdentifierExpected);
				}

				token = block.GetToken(index);
				if (token?.Type == TokenType.Dot) {
					++index;
				}
				else {
					break;
				}
			}

			return new ReferenceToInstance(block.AsBeginEnd(startIndex, index), isConstantOnly);
		}
	}
}
