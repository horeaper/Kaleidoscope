using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	/// <summary>
	/// using System.Collections.Generic;
	/// </summary>
	public sealed class UsingCSNamespaceDirective
	{
		public readonly ImmutableArray<TokenIdentifier> OwnerNamespace;
		public readonly ImmutableArray<TokenIdentifier> Namespace;
		readonly string m_displayName;

		internal UsingCSNamespaceDirective(ImmutableArray<TokenIdentifier>.Builder ownerNamespace, IEnumerable<TokenIdentifier> @namespace)
		{
			OwnerNamespace = ownerNamespace.MoveToImmutable();
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
			return $"[UsingCSNamespaceDirective] using {m_displayName};";
		}
	}
}
