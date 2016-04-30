using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaleidoscope.Primitive;

namespace Kaleidoscope.Tokenizer
{
	public class TokenString : Token
	{
		public readonly string ConvertedText;

		public TokenString(SourceTextFile sourceFile, int begin, int end, string convertedText)
			: base(sourceFile, begin, end)
		{
			ConvertedText = convertedText;
		}

		public override string ToString()
		{
			return "[TokenString] " + ConvertedText;
		}
	}
}
