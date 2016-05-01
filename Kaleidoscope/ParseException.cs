using System;
using Kaleidoscope.Primitive;

namespace Kaleidoscope
{
	public class ParseException : Exception
	{
		public readonly SourceTextFile SourceFile;
		public readonly int ErrorLine;
		public readonly int ErrorColumnStart;
		public readonly int ErrorColumnEnd;

		ParseException(SourceTextFile sourceFile, int errorLine, int columnStart, int columnEnd, string errorMessage)
			: base(errorMessage)
		{
			SourceFile = sourceFile;
			ErrorLine = errorLine;
			ErrorColumnStart = columnStart;
			ErrorColumnEnd = columnEnd;
		}

		internal static ParseException AsEOF(SourceTextFile sourceFile, string errorMessage)
		{
			return AsIndex(sourceFile, sourceFile.Length, errorMessage);
		}

		internal static ParseException AsIndex(SourceTextFile sourceFile, int index, string errorMessage)
		{
			int line, column;
			sourceFile.GetLineColumn(index, out line, out column);
			return new ParseException(sourceFile, line, column, column, errorMessage);
		}

		internal static ParseException AsRange(SourceTextFile sourceFile, int startIndex, int endIndex, string errorMessage)
		{
			int line, startColumn;
			sourceFile.GetLineColumn(startIndex, out line, out startColumn);
			int endLine, endColumn;
			sourceFile.GetLineColumn(endIndex, out endLine, out endColumn);
			return new ParseException(sourceFile, line, startColumn, endColumn, errorMessage);
		}

		internal static ParseException AsToken(Tokenizer.Token token, string errorMessage)
		{
			return new ParseException(token.SourceFile, token.Line, token.Column, token.Column + token.Text.Length, errorMessage);
		}
	}
}
