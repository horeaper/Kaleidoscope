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
