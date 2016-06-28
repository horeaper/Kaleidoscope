using System.Collections.Generic;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	public static class InheritanceReader
	{
		public static List<ReferenceToType> ReadParents(TokenBlock block, ref int index, string eofErrorMessage)
		{
			var result = new List<ReferenceToType>();

			var token = block.GetToken(index, eofErrorMessage);
			if (token.Type == TokenType.Colon) {
				++index;
				while (true) {
					result.Add(ReferenceReader.Read(block, ref index, TypeParsingRule.AllowCppType));

					token = block.GetToken(index, eofErrorMessage);
					if (token.Type == TokenType.Comma) {
						++index;
					}
					else {
						break;
					}
				}
			}

			return result;
		}
	}
}
