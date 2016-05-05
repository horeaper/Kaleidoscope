namespace Kaleidoscope.Tokenizer
{
	public abstract class Token
	{
		public readonly SourceTextFile SourceFile;
		public readonly int Begin;
		public readonly int End;
		public readonly TokenType Type;

		public readonly int Line;
		public readonly int Column;
		public readonly string Text;
		public int Length => Text.Length;

		protected Token(SourceTextFile sourceFile, int begin, int end, TokenType type)
		{
			SourceFile = sourceFile;
			Begin = begin;
			End = end;
			Type = type;

			Text = SourceFile.Substring(Begin, End);
			SourceFile.GetLineColumn(begin, out Line, out Column);
		}

		public override string ToString()
		{
			return $"[{GetType().Name}] {Text}";
		}
	}
}
