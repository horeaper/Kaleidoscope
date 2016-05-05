using System;
using System.Collections.Immutable;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.SyntaxObject
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
		/// Skip all consecutive trivias
		/// </summary>
		public void SkipTrivia(ref int index)
		{
			while (index < Items.Length && Items[index] is TokenTrivia) {
				++index;
			}
		}

		/// <summary>
		/// Find a symbol token, starting from index
		/// </summary>
		public int FindToken(int index, TokenType target, string errorMessage = null)
		{
			while (index < Items.Length) {
				var token = Items[index] as TokenSymbol;
				if (((Token)token).Type == target) {
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
			index = FindToken(index, TokenType.LeftBrace, Error.Analysis.LeftBraceExpected);

			int braceCount = 0;
			while (true) {
				var token = GetToken(index, Error.Analysis.RightBraceExpected);
				switch (token.Type) {
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
			index = FindToken(index, TokenType.LeftParenthesis, Error.Analysis.LeftParenthesisExpected);

			int braceCount = 0;
			while (true) {
				var token = GetToken(index, Error.Analysis.RightParenthesisExpected);
				switch (token.Type) {
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
			index = FindToken(index, TokenType.LeftBracket, Error.Analysis.LeftBracketExpected);

			int braceCount = 0;
			while (true) {
				var token = GetToken(index, Error.Analysis.RightBracketExpected);
				switch (token.Type) {
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

		/// <summary>
		/// 读取到指定的Token，并将index移动到此Token后，返回范围内不包括target
		/// </summary>
		public TokenBlock ReadPastSpecificToken(ref int index, TokenType target, string errorMessage = null)
		{
			var startIndex = index;
			while (true) {
				var token = GetToken(index, errorMessage);
				if (token == null) {
					return AsBeginEnd(startIndex, index);
				}
				else if (token.Type == target) {
					return AsBeginEnd(startIndex, index++);
				}
				++index;
			}
		}

		/// <summary>
		/// 读取到一行结尾，并将index移动到换行Token后，返回范围内不包括换行Token
		/// </summary>
		public TokenBlock ReadToLineEnd(ref int index, string errorMessage = null)
		{
			return ReadPastSpecificToken(ref index, TokenType.NewLine, errorMessage);
		}

#endregion
	}
}
