using System;
using System.Collections.Immutable;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.SyntaxObject.Primitive
{
	/// <summary>
	/// A collection of consecutive tokens
	/// </summary>
	public class TokenBlock
	{
		public readonly ImmutableArray<Token> Items;

		public Token First => Items[0];
		public Token Last => Items[Items.Length - 1];
		public int Count => Items.Length;
		public SourceTextFile SourceFile => First.SourceFile;
		public string Text => SourceFile.Substring(First.Begin, Last.End);

		public Token this[int index] => Items[index];

		public override string ToString()
		{
			return "[TokenBlock] Count=" + Count;
		}

#region Create/Recreate

		public TokenBlock(ImmutableArray<Token> tokens)
		{
			if (tokens == null) {
				throw new ArgumentNullException(nameof(tokens));
			}
			if (tokens.Length == 0) {
				throw new ArgumentOutOfRangeException(nameof(tokens));
			}
			Items = tokens;
		}

		public TokenBlock(Token[] tokens)
		{
			Items = ImmutableArray.Create(tokens);
		}

		public TokenBlock(Token token)
		{
			Items = ImmutableArray.Create(token);
		}

		public TokenBlock AsStartLength(int startIndex, int length)
		{
			return new TokenBlock(ImmutableArray.Create(Items, 0, length));
		}

		public TokenBlock AsBeginEnd(int begin, int end)
		{
			return AsStartLength(begin, end - begin - 1);
		}

		public TokenBlock RemoveHeadAndTail()
		{
			return AsStartLength(1, Count - 2);
		}

#endregion

#region Token read

		/// <summary>
		/// Get the token on current index
		/// </summary>
		public Token GetToken(int index, string errorMessage = null)
		{
			if (index < Items.Length) {
				return Items[index];
			}

			if (errorMessage != null) {
				throw ParseException.AsToken(Last, errorMessage);
			}
			return null;
		}

		/// <summary>
		/// Find a symbol token, starting from index
		/// </summary>
		public int FindToken(int index, SymbolType symbol, string errorMessage = null)
		{
			while (index < Items.Length) {
				var token = Items[index] as TokenSymbol;
				if (token?.Type == symbol) {
					return index;
				}
				else {
					++index;
				}
			}

			if (errorMessage != null) {
				throw ParseException.AsToken(Last, errorMessage);
			}
			return -1;
		}

		/// <summary>
		/// Find the end of next {...} block
		/// </summary>
		public int FindNextBraceBlockEnd(int index, SymbolType left, SymbolType right)
		{
			index = FindToken(index, SymbolType.LeftBrace, Error.Analysis.LeftBraceExpected);

			int braceCount = 0;
			while (true) {
				var token = GetToken(index, Error.Analysis.RightBraceExpected);
				switch (token.TokenType) {
					case TokenType.LeftBrace:
						++braceCount;
						break;
					case TokenType.RightBrace:
						--braceCount;
						if (braceCount == 0) {
							return index + 1;
						}
						break;
				}

				++index;
			}
		}

		/// <summary>
		/// Find the end of next (...) block
		/// </summary>
		public int FindNextParenthesisBlockEnd(int index, SymbolType left, SymbolType right)
		{
			index = FindToken(index, SymbolType.LeftParenthesis, Error.Analysis.LeftParenthesisExpected);

			int braceCount = 0;
			while (true) {
				var token = GetToken(index, Error.Analysis.RightParenthesisExpected);
				switch (token.TokenType) {
					case TokenType.LeftParenthesis:
						++braceCount;
						break;
					case TokenType.RightParenthesis:
						--braceCount;
						if (braceCount == 0) {
							return index + 1;
						}
						break;
				}

				++index;
			}
		}

		/// <summary>
		/// Find the end of next [...] block
		/// </summary>
		public int FindNextBracketBlockEnd(int index, SymbolType left, SymbolType right)
		{
			index = FindToken(index, SymbolType.LeftBracket, Error.Analysis.LeftBracketExpected);

			int braceCount = 0;
			while (true) {
				var token = GetToken(index, Error.Analysis.RightBracketExpected);
				switch (token.TokenType) {
					case TokenType.LeftBracket:
						++braceCount;
						break;
					case TokenType.RightBracket:
						--braceCount;
						if (braceCount == 0) {
							return index + 1;
						}
						break;
				}

				++index;
			}
		}

#endregion

#region Block read

#endregion
	}
}
