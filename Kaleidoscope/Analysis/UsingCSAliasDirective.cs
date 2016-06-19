using System.Collections.Immutable;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	/// <summary>
	/// using LookupTable = System.Collections.Generic.Dictionary&lt;int, string&gt;;
	/// Col = System.Collections.Generic;
	/// </summary>
	public sealed class UsingCSAliasDirective
	{
		public readonly ImmutableArray<TokenIdentifier> OwnerNamespace;
		public readonly Token Name;
		public readonly ReferenceToManagedType Type;

		public DeclaredNamespaceOrTypeName Target { get; private set; }

		public UsingCSAliasDirective(ImmutableArray<TokenIdentifier>.Builder ownerNamespace, Token name, ReferenceToManagedType type)
		{
			OwnerNamespace = ownerNamespace.MoveToImmutable();
			Name = name;
			Type = type;
		}

		public override string ToString()
		{
			return $"[UsingCSAliasDirective] using {Name.Text} = {Type.Content.Text};";
		}
	}
}
