namespace Kaleidoscope.Tokenizer
{
	public class TokenSignedInteger : Token
	{
		public readonly long Value;
		public new readonly IntegerNumberType Type;

		public TokenSignedInteger(SourceTextFile sourceFile, int begin, int end, long value, IntegerNumberType type)
			: base(sourceFile, begin, end, TokenType.NumberLiteral)
		{
			Value = value;
			Type = type;
		}
	}
}
