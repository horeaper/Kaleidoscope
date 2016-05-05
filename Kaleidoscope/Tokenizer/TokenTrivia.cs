namespace Kaleidoscope.Tokenizer
{
	public class TokenTrivia : Token
	{
		public new readonly TriviaType Type;
		const int TriviaFirst = (int)TokenType.NewLine;

		public TokenTrivia(SourceTextFile sourceFile, int begin, int end, TriviaType type)
			: base(sourceFile, begin, end, (TokenType)(TriviaFirst + type))
		{
			Type = type;
		}

		public override string ToString()
		{
			return "[TokenTrivia] " + Type;
		}
	}
}
