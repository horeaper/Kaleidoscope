using System;
using Kaleidoscope.Primitive;

namespace Kaleidoscope.Tokenizer
{
	public class TokenIdentifier : Token
	{
		public ContextualKeywordType? ContextualKeyword;
		public bool IsContextualKeyword => ContextualKeyword.HasValue;

		public TokenIdentifier(SourceTextFile sourceFile, int begin, int end)
			: base(sourceFile, begin, end)
		{
			ContextualKeywordType type;
			if (Enum.TryParse(Text, out type)) {
				ContextualKeyword = type;
			}
		}
	}
}
