using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.SyntaxObject
{
	/// <summary>
	/// A collection of consecutive tokens
	/// </summary>
	public class TokenBlock : IEnumerable<Token>
	{
		public readonly ImmutableArray<Token> Items;

		public Token First => Items.Length > 0 ? Items[0] : null;
		public Token Last => Items.Length > 0 ? Items[Items.Length - 1] : null;
		public int Count => Items.Length;
		public SourceTextFile SourceFile => Items.Length > 0 ? First.SourceFile : null;
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
			Items = tokens;
		}

		public TokenBlock(Token[] tokens)
		{
			if (tokens == null) {
				throw new ArgumentNullException(nameof(tokens));
			}
			Items = ImmutableArray.Create(tokens);
		}

		public TokenBlock(Token token)
		{
			if (token == null) {
				throw new ArgumentNullException(nameof(token));
			}
			Items = ImmutableArray.Create(token);
		}

		public TokenBlock AsStartLength(int startIndex, int length)
		{
			return new TokenBlock(ImmutableArray.Create(Items, startIndex, length));
		}

		public TokenBlock AsBeginEnd(int begin, int end)
		{
			return AsStartLength(begin, end - begin);
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
		/// Get the next token and ensure it's of certain token type
		/// </summary>
		public Token NextToken(int index, TokenType type, string errorMessage)
		{
			var token = GetToken(index, errorMessage);
			if (token.Type != type) {
				throw ParseException.AsToken(token, errorMessage);
			}
			return token;
		}

		/// <summary>
		/// Find a symbol token, starting from index
		/// </summary>
		public int FindToken(int index, TokenType target, string errorMessage = null)
		{
			while (index < Items.Length) {
				var token = Items[index];
				if (token.Type == target) {
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
		public int FindBraceBlockEnd(int index)
		{
			Debug.Assert(this[index].Type == TokenType.LeftBrace);

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
		public int FindParenthesisBlockEnd(int index)
		{
			Debug.Assert(this[index].Type == TokenType.LeftParenthesis);

			int parenthesisCount = 0;
			while (true) {
				var token = GetToken(index, Error.Analysis.RightParenthesisExpected);
				switch (token.Type) {
					case TokenType.LeftParenthesis:
						++parenthesisCount;
						break;
					case TokenType.RightParenthesis:
						--parenthesisCount;
						if (parenthesisCount == 0) {
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
		public int FindBracketBlockEnd(int index)
		{
			Debug.Assert(this[index].Type == TokenType.LeftBracket);

			int bracketCount = 0;
			while (true) {
				var token = GetToken(index, Error.Analysis.RightBracketExpected);
				switch (token.Type) {
					case TokenType.LeftBracket:
						++bracketCount;
						break;
					case TokenType.RightBracket:
						--bracketCount;
						if (bracketCount == 0) {
							return index + 1;
						}
						break;
				}

				++index;
			}
		}

		/// <summary>
		/// Read the next {...} block, and move index past the end. return value does not contain '{' and '}'
		/// </summary>
		public TokenBlock ReadBraceBlock(ref int index)
		{
			Debug.Assert(this[index].Type == TokenType.LeftBrace);
			int startIndex = index;
			int endIndex = FindBraceBlockEnd(startIndex);
			index = endIndex;
			return AsBeginEnd(startIndex + 1, endIndex - 1);
		}

		/// <summary>
		/// Read the next (...) block, and move index past the end. return value does not contain '(' and ')'
		/// </summary>
		public TokenBlock ReadParenthesisBlock(ref int index)
		{
			Debug.Assert(this[index].Type == TokenType.LeftParenthesis);
			int startIndex = index;
			int endIndex = FindParenthesisBlockEnd(startIndex);
			index = endIndex;
			return AsBeginEnd(startIndex + 1, endIndex - 1);
		}

		/// <summary>
		/// Read the next [...] block, and move index past the end. return value does not contain '(' and ')'
		/// </summary>
		public TokenBlock ReadBracketBlock(ref int index)
		{
			Debug.Assert(this[index].Type == TokenType.LeftBracket);
			int startIndex = index;
			int endIndex = FindBracketBlockEnd(startIndex);
			index = endIndex;
			return AsBeginEnd(startIndex + 1, endIndex - 1);
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
		/// 读取到指定的Token，并将index移动到此Token后，返回范围内不包括target
		/// </summary>
		public TokenBlock ReadPastSpecificTokens(ref int index, params TokenType[] targets)
		{
			var startIndex = index;
			while (true) {
				var token = GetToken(index, Error.Analysis.UnexpectedToken);
				foreach (var item in targets) {
					if (token.Type == item) {
						return AsBeginEnd(startIndex, index++);
					}
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

#region IEnumerator

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<Token> GetEnumerator()
		{
			return ((IEnumerable<Token>)Items).GetEnumerator();
		}

#endregion
	}
}
