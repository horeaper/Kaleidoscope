namespace Kaleidoscope.Tokenizer
{
	public class TokenSymbol : Token
	{
		public readonly SymbolType Type;

		public TokenSymbol(SourceTextFile sourceFile, int begin, int end, SymbolType type)
			: base(sourceFile, begin, end)
		{
			Type = type;
		}
	}
}
