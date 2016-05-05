using System.Collections.Immutable;
using System.Text;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	/// <summary>
	/// using cpp::std.tr2;
	/// </summary>
	public sealed class UsingCppNamespaceDirective
	{
		public readonly AnalyzedFile Source;
		public readonly ImmutableArray<Token> Namespace;
		readonly string m_displayName;

		internal UsingCppNamespaceDirective(AnalyzedFile source, Token[] nsTokens)
		{
			Source = source;
			Namespace = ImmutableArray.Create(nsTokens);

			var builder = new StringBuilder();
			for (int cnt = 0; cnt < Namespace.Length; ++cnt) {
				builder.Append(Namespace[cnt].Text);
				if (cnt < Namespace.Length - 1) {
					builder.Append('.');
				}
			}
			m_displayName = builder.ToString();
		}

		public override string ToString()
		{
			return $"[UsingCppNamespaceDirective] using cpp::{m_displayName};";
		}
	}
}
