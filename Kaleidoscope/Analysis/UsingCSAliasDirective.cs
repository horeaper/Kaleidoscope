using System.Collections.Immutable;
using Kaleidoscope.SyntaxObject;
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

		public UsingCSAliasDirective(TokenIdentifier[] ownerNamespace, Token name, ReferenceToManagedType type)
		{
			OwnerNamespace = ImmutableArray.Create(ownerNamespace);
			Name = name;
			Type = type;
		}

		public override string ToString()
		{
			return $"[UsingCSAliasDirective] using {Name.Text} = {Type.Content.Text};";
		}
	}
}
