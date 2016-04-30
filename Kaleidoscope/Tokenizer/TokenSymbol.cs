using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaleidoscope.Primitive;

namespace Kaleidoscope.Tokenizer
{
	public class TokenSymbol : Token
	{
		public readonly SymbolType Type;

		public TokenSymbol(SourceTextFile sourceFile, int begin, int end, SymbolType type)
			: base(sourceFile, begin, end)
		{
			Type = type;
		}
	}
}
