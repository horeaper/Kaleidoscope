namespace Kaleidoscope.Tokenizer
{
	public class TokenFloatNumber : Token
	{
		public readonly double Value;
		public readonly FloatNumberType Type;

		public TokenFloatNumber(SourceTextFile sourceFile, int begin, int end, double value, FloatNumberType type)
			: base(sourceFile, begin, end)
		{
			Value = value;
			Type = type;
		}
	}
}
