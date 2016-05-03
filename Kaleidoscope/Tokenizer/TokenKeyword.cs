using System;

namespace Kaleidoscope.Tokenizer
{
	public class TokenKeyword : Token
	{
		public readonly KeywordType Type;

		public TokenKeyword(SourceTextFile sourceFile, int begin, int end, KeywordType type)
			: base(sourceFile, begin, end)
		{
			Type = type;
		}

		public static bool IsKeyword(string text)
		{
			KeywordType tmp;
			return Enum.TryParse(text, out tmp);
		}
	}
}
