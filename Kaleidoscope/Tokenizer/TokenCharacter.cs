namespace Kaleidoscope.Tokenizer
{
	public class TokenCharacter : Token
	{
		public readonly char Value;

		public TokenCharacter(SourceTextFile sourceFile, int begin, int end, char value)
			: base(sourceFile, begin, end, TokenType.Character)
		{
			Value = value;
		}
	}
}
