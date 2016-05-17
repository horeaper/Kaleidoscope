using System.Collections.Generic;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	public static class InheritanceReader
	{
		public static IEnumerable<ReferenceToManagedType> ReadParents(TokenBlock block, ref int index, string eofErrorMessage)
		{
			var result = new List<ReferenceToManagedType>();

			var token = block.GetToken(index, eofErrorMessage);
			if (token.Type == TokenType.Colon) {
				++index;
				while (true) {
					result.Add((ReferenceToManagedType)TypeReferenceReader.Read(block, ref index, TypeParsingRule.None));

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
