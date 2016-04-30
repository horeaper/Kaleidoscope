using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaleidoscope
{
	public static class Error
	{
		public static class Tokenizer
		{
			public const string UnexpectedEOF = "unexpected end of file";
			public const string UnknownToken = "unrecognized token";
			public const string InvalidIntegralConstant = "invalid integral constant";
		}
	}
}
