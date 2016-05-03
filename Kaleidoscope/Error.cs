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
			public const string InvalidRealConstant = "invalid real constant";
			public const string InvalidDecimalConstant = "invalid decimal constant";
			public const string UnknownUnicodeCharacter = "unrecognized unicode character escape sequence";
			public const string UnknownEscapeSequence = "unrecognized escape sequence";
			public const string NewLineNotAllowedOnChar = "new line is not allowed when declaring char constants";
			public const string EmptyCharLiteral = "empty character literal";
			public const string BadCharConstant = "bad char constant";
			public const string NewLineNotAllowedOnString = "new line is not allowed when declaring non-verbatim string constants";
			public const string MultiLineCommentNotClosed = "'*/' expected";
			public const string InvalidPreprocessor = "Invalid preprocessor directive";
		}
	}
}
