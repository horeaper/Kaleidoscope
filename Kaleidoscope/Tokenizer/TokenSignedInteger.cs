using Kaleidoscope.Primitive;

namespace Kaleidoscope.Tokenizer
{
	public class TokenSignedInteger : Token
	{
		public readonly long Value;
		public readonly IntegerNumberType Type;

		public TokenSignedInteger(SourceTextFile sourceFile, int begin, int end, long value, IntegerNumberType type)
			: base(sourceFile, begin, end)
		{
			Value = value;
			Type = type;
		}
	}
}
