using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaleidoscope.Primitive;

namespace Kaleidoscope.Tokenizer
{
	public class TokenPreprocessor : Token
	{
		public readonly PreprocessorType Type;
		public readonly Token[] ContentTokens;

		public TokenPreprocessor(SourceTextFile sourceFile, int begin, int end, PreprocessorType type, Token[] contentTokens)
			: base(sourceFile, begin, end)
		{
			Type = type;
			ContentTokens = contentTokens;
		}

		public static bool IsPreprocessor(string text)
		{
			PreprocessorType tmp;
			return Enum.TryParse(text, out tmp);
		}
	}
}
