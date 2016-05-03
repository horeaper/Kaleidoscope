using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	/// <summary>
	/// A collection of consecutive tokens
	/// </summary>
	public class TokenBlock
	{
		public readonly ImmutableArray<Token> Content;

		public Token First => Content[0];
		public Token Last => Content[Content.Length - 1];
		public int Count => Content.Length;
		public SourceTextFile SourceFile => First.SourceFile;
		public string Text => SourceFile.Substring(First.Begin, Last.End);

		public override string ToString()
		{
			return "[TokenBlob] Count=" + Count;
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
			Content = tokens;
		}

		public TokenBlock(IEnumerable<Token> tokens)
		{
			Content = ImmutableArray.CreateRange(tokens);
		}

		public TokenBlock(Token token)
		{
			Content = ImmutableArray.Create(token);
		}

		public TokenBlock AsStartLength(int startIndex, int length)
		{
			var tokens = new Token[length];
			Content.CopyTo(startIndex, tokens, 0, length);
			return new TokenBlock(tokens);
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
			if (index < Content.Length) {
				return Content[index];
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
			while (index < Content.Length) {
				var token = Content[index] as TokenSymbol;
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
		/// Find the end of next {...} block
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
	}
}
