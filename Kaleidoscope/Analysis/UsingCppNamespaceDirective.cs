using System.Collections.Generic;
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
		public readonly ImmutableArray<Token> Namespace;
		readonly string m_displayName;

		internal UsingCppNamespaceDirective(IEnumerable<Token> @namespace)
		{
			Namespace = ImmutableArray.CreateRange(@namespace);

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
