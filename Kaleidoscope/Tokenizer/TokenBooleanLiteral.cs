using System;

namespace Kaleidoscope.Tokenizer
{
	public class TokenBooleanLiteral : Token
	{
		public readonly bool Value;

		public TokenBooleanLiteral(SourceTextFile sourceFile, int begin, int end, KeywordType keyword)
			: base(sourceFile, begin, end)
		{
			if (keyword == KeywordType.@true) {
				Value = true;
			}
			else if (keyword == KeywordType.@false) {
				Value = false;
			}
			else {
				throw new ArgumentException(nameof(keyword));
			}
		}
	}
}
