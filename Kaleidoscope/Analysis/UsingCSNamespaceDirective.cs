using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
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

		public DeclaredNamespaceOrTypeName Target { get; private set; }

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

		internal void BindNamespace(InfoOutput infoOutput, DeclaredNamespaceOrTypeName rootNamespace)
		{
			Func<DeclaredNamespaceOrTypeName, IEnumerable<TokenIdentifier>, DeclaredNamespaceOrTypeName> fnGetNamespace = (currentNs, tokens) => {
				foreach (var token in tokens) {
					var target = currentNs.NamespaceOrTypeName.FirstOrDefault(item => item.Name.Name == token.Text && item.Name.Generics.Length == 0);
					if (target == null) {
						infoOutput.OutputError(ParseException.AsToken(token, Error.Bind.UsingNamespaceError));
						return null;
					}
					currentNs = target;
				}
				return currentNs;
			};

			if (OwnerNamespace.Length == 0) {
				Target = rootNamespace.GetNamespaceOrTypeName(Namespace);
			}
			else {
				for (int length = OwnerNamespace.Length; length >= 0; --length) {
					var enclosing = new TokenIdentifier[length];
					OwnerNamespace.CopyTo(0, enclosing, 0, length);

					var findTarget = rootNamespace.GetNamespaceOrTypeName(enclosing);
					Debug.Assert(findTarget != null);
					Target = findTarget.GetNamespaceOrTypeName(Namespace);
					if (Target != null) {
						return;
					}
				}
			}
		}
	}
}
