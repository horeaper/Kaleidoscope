using System;

namespace Kaleidoscope.Tokenizer
{
	public class TokenKeyword : Token
	{
		public new readonly KeywordType Type;
		const int KeywordTypeFirst = (int)TokenType.@abstract;

		public TokenKeyword(SourceTextFile sourceFile, int begin, int end, KeywordType type)
			: base(sourceFile, begin, end, (TokenType)(KeywordTypeFirst + type))
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
