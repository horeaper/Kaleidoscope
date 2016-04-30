﻿using Kaleidoscope.Primitive;

namespace Kaleidoscope.Tokenizer
{
	public class TokenDecimalNumber : Token
	{
		public readonly decimal Value;

		public TokenDecimalNumber(SourceTextFile sourceFile, int begin, int end, decimal value)
			: base(sourceFile, begin, end)
		{
			Value = value;
		}
	}
}
