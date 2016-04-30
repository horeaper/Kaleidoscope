using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaleidoscope.Primitive;

namespace Kaleidoscope.Tokenizer
{
	public class TokenTrivia : Token
	{
		public readonly TriviaType Type;

		public TokenTrivia(SourceTextFile sourceFile, int begin, int end, TriviaType type)
			: base(sourceFile, begin, end)
		{
			Type = type;
		}

		public override string ToString()
		{
			return "[TokenTrivia] " + Type;
		}
	}
}
