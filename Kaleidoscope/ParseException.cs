using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaleidoscope.Primitive;

namespace Kaleidoscope
{
	public class ParseException : Exception
	{
		public readonly SourceTextFile SourceFile;
		public readonly int ErrorLine;
		public readonly int ErrorColumnStart;
		public readonly int ErrorColumnEnd;

		internal ParseException(SourceTextFile sourceFile, int errorLine, int columnStart, int columnEnd, string errorMessage)
			: base(errorMessage)
		{
			SourceFile = sourceFile;
			ErrorLine = errorLine;
			ErrorColumnStart = columnStart;
			ErrorColumnEnd = columnEnd;
		}

		internal static ParseException AsToken(Tokenizer.Token token, string errorMessage)
		{
			return new ParseException(token.SourceFile, token.Line, token.Column, token.Column + token.Text.Length, errorMessage);
		}
	}
}
