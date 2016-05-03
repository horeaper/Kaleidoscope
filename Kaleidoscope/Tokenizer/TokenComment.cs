namespace Kaleidoscope.Tokenizer
{
	public class TokenComment : Token
	{
		public readonly bool IsMultiline;

		public TokenComment(SourceTextFile sourceFile, int begin, int end, bool isMultiline)
			: base(sourceFile, begin, end, TokenType.Comment)
		{
			IsMultiline = isMultiline;
		}
	}
}
