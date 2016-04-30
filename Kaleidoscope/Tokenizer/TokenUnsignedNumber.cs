using Kaleidoscope.Primitive;

namespace Kaleidoscope.Tokenizer
{
	public class TokenUnsignedNumber : Token
	{
		public readonly ulong Value;
		public readonly IntegerNumberType Type;

		public TokenUnsignedNumber(SourceTextFile sourceFile, int begin, int end, ulong value, IntegerNumberType type)
			: base(sourceFile, begin, end)
		{
			Value = value;
			Type = type;
		}
	}
}
