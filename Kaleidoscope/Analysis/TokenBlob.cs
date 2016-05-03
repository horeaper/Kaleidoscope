using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	public class TokenBlob
	{
		public readonly ImmutableArray<Token> Content;
		public Token First => Content[0];
		public Token Last => Content[Content.Length - 1];
		public int Count => Content.Length;
		public SourceTextFile SourceFile => First.SourceFile;

		public TokenBlob(ImmutableArray<Token> tokens)
		{
			if (tokens == null) {
				throw new ArgumentNullException(nameof(tokens));
			}
			if (tokens.Length == 0) {
				throw new ArgumentOutOfRangeException(nameof(tokens));
			}
			Content = tokens;
		}

		public TokenBlob(IEnumerable<Token> tokens)
		{
			Content = ImmutableArray.CreateRange(tokens);
		}

		public TokenBlob(Token token)
		{
			Content = ImmutableArray.Create(token);
		}

		public override string ToString()
		{
			return "[TokenBlob] Count=" + Count;
		}
	}
}
