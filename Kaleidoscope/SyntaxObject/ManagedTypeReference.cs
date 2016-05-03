using System.Collections.Generic;
using System.Collections.Immutable;
using Kaleidoscope.SyntaxObject.Primitive;

namespace Kaleidoscope.SyntaxObject
{
	public sealed class ManagedTypeReference : TypeReference
	{
		public readonly bool IsGlobalNamespace;
		public readonly ImmutableArray<NamespaceOrTypeName> Identifiers;
		public readonly bool IsNullable;
		public readonly ImmutableArray<int> ArrayDimensions;
		public int ArrayRankCount => ArrayDimensions.Length;

		public override string ToString()
		{
			return "[ManagedTypeReference] " + Content.Text;
		}

		ManagedTypeReference(Builder builder)
			: base(builder.Content)
		{
			IsGlobalNamespace = builder.IsGlobalNamespace;
			Identifiers = ImmutableArray.Create(builder.Identifiers.ToArray());
			IsNullable = builder.IsNullable;
			ArrayDimensions = ImmutableArray.Create(builder.ArrayDimensions.ToArray());
		}

		public sealed class Builder
		{
			public TokenBlock Content;
			public bool IsGlobalNamespace;
			public List<NamespaceOrTypeName> Identifiers = new List<NamespaceOrTypeName>();
			public bool IsNullable;
			public List<int> ArrayDimensions = new List<int>();
		}
	}
}
