using System.Collections.Immutable;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.SyntaxObject
{
	/// <summary>
	/// using System.Collections.Generic;
	/// </summary>
	public sealed class UsingCSNamespaceDirective
	{
		public readonly CodeFile Source;
		public readonly ImmutableArray<Token> Namespace;
		public readonly string DisplayName;

		internal UsingCSNamespaceDirective(CodeFile source, Token[] nsTokens, string displayName)
		{
			Source = source;
			Namespace = ImmutableArray.Create(nsTokens);
			DisplayName = displayName;
		}

		public override string ToString()
		{
			return "[UsingCSNamespaceDirective] using " + DisplayName;
		}
	}
}
