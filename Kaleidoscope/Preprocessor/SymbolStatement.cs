using System.Collections.Generic;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Preprocessor
{
	public class SymbolStatement : IBooleanExpression
	{
		public readonly string Symbol;
		public readonly SortedSet<string> DefinedSymbol;

		public SymbolStatement(TokenIdentifier token, SortedSet<string> definedSymbol)
		{
			Symbol = token.Text;
			DefinedSymbol = definedSymbol;
		}

		public bool Evaluate => DefinedSymbol.Contains(Symbol);
	}
}
