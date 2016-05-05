using System;

namespace Kaleidoscope.Tokenizer
{
	public class TokenIdentifier : Token
	{
		public readonly ContextualKeywordType? ContextualKeyword;
		public readonly bool IsFromKeyword;

		public TokenIdentifier(SourceTextFile sourceFile, int begin, int end, bool isFromKeyword)
			: base(sourceFile, begin, end, TokenType.Identifier)
		{
			ContextualKeywordType type;
			if (Enum.TryParse(Text, out type)) {
				ContextualKeyword = type;
			}
			IsFromKeyword = isFromKeyword;
		}

		public bool IsContextualKeyword(ContextualKeywordType keyword)
		{
			return ContextualKeyword.HasValue && ContextualKeyword.Value == keyword;
		}
	}
}
