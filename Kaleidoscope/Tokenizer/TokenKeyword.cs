using System;
using Kaleidoscope.Primitive;

namespace Kaleidoscope.Tokenizer
{
	public class TokenKeyword : Token
	{
		public readonly KeywordType Type;

		public TokenKeyword(SourceTextFile sourceFile, int begin, int end)
			: base(sourceFile, begin, end)
		{
			if (!Enum.TryParse(Text, out Type)) {
				throw new InvalidOperationException("Token is not a keyword type");
			}
		}

		public static bool IsKeyword(string text)
		{
			KeywordType tmp;
			return Enum.TryParse(text, out tmp);
		}
	}
}
