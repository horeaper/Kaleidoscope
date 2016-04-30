using Kaleidoscope.Primitive;

namespace Kaleidoscope.Tokenizer
{
	public class TokenSignedNumber : Token
	{
		public readonly long Value;
		public readonly IntegerNumberType Type;

		public TokenSignedNumber(SourceTextFile sourceFile, int begin, int end, long value, IntegerNumberType type)
			: base(sourceFile, begin, end)
		{
			Value = value;
			Type = type;
		}
	}
}
