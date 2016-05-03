namespace Kaleidoscope.Tokenizer
{
	public class TokenSymbol : Token
	{
		public readonly SymbolType Type;
		const int SymbolTypeFirst = (int)TokenType.Dot;

		public TokenSymbol(SourceTextFile sourceFile, int begin, int end, SymbolType type)
			: base(sourceFile, begin, end, (TokenType)(SymbolTypeFirst + type))
		{
			Type = type;
		}
	}
}
