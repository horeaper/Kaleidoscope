using System;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope
{
	public class ParseException : Exception
	{
		public readonly SourceTextFile SourceFile;
		public readonly int Line;
		public readonly int ColumnStart;
		public readonly int ColumnEnd;

		ParseException(SourceTextFile sourceFile, int line, int columnStart, int columnEnd, string message)
			: base(message)
		{
			SourceFile = sourceFile;
			Line = line;
			ColumnStart = columnStart;
			ColumnEnd = columnEnd;
		}

		internal static ParseException AsEOF(SourceTextFile sourceFile, string message)
		{
			return AsIndex(sourceFile, sourceFile.Length, message);
		}

		internal static ParseException AsIndex(SourceTextFile sourceFile, int index, string message)
		{
			int line, column;
			sourceFile.GetLineColumn(index, out line, out column);
			return new ParseException(sourceFile, line, column, column, message);
		}

		internal static ParseException AsRange(SourceTextFile sourceFile, int startIndex, int endIndex, string message)
		{
			int line, startColumn;
			sourceFile.GetLineColumn(startIndex, out line, out startColumn);
			int endLine, endColumn;
			sourceFile.GetLineColumn(endIndex, out endLine, out endColumn);
			return new ParseException(sourceFile, line, startColumn, endColumn, message);
		}

		internal static ParseException AsToken(Token token, string message)
		{
			return new ParseException(token.SourceFile, token.Line, token.Column, token.Column + token.Text.Length, message);
		}

		internal static ParseException AsTokenBlock(TokenBlock block, string message)
		{
			return AsRange(block.SourceFile, block.First.Begin, block.Last.End, message);
		}
	}

	public class ParseWarning
	{
		public readonly SourceTextFile SourceFile;
		public readonly int Line;
		public readonly int ColumnStart;
		public readonly int ColumnEnd;
		public readonly string Message;

		ParseWarning(SourceTextFile sourceFile, int line, int columnStart, int columnEnd, string message)
		{
			SourceFile = sourceFile;
			Line = line;
			ColumnStart = columnStart;
			ColumnEnd = columnEnd;
			Message = message;
		}

		internal static ParseWarning AsToken(Token token, string message)
		{
			return new ParseWarning(token.SourceFile, token.Line, token.Column, token.Column + token.Text.Length, message);
		}
	}
}
