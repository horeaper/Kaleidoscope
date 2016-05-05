namespace Kaleidoscope.Tokenizer
{
	public class TokenFloatNumber : Token
	{
		public readonly double Value;
		public new readonly FloatNumberType Type;

		public TokenFloatNumber(SourceTextFile sourceFile, int begin, int end, double value, FloatNumberType type)
			: base(sourceFile, begin, end, TokenType.NumberLiteral)
		{
			Value = value;
			Type = type;
		}
	}
}
