namespace Kaleidoscope.Tokenizer
{
	public class TokenUnsignedInteger : Token
	{
		public readonly ulong Value;
		public new readonly IntegerNumberType Type;

		public TokenUnsignedInteger(SourceTextFile sourceFile, int begin, int end, ulong value, IntegerNumberType type)
			: base(sourceFile, begin, end, TokenType.NumberLiteral)
		{
			Value = value;
			Type = type;
		}
	}
}
