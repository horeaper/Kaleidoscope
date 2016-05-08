using System.Collections.Immutable;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	public abstract class InstanceTypeDeclare : ManagedDeclare
	{
		public readonly UsingBlob Usings;
		public readonly ImmutableArray<TokenIdentifier> Namespace;
		public readonly ImmutableArray<AttributeObject> CustomAttributes;

		protected InstanceTypeDeclare(Builder builder)
			: base(builder.Name)
		{
			Usings = builder.Usings;
			Namespace = ImmutableArray.Create(builder.Namespace);
			CustomAttributes = ImmutableArray.Create(builder.CustomAttributes);
		}

		public abstract class Builder
		{
			public TokenIdentifier Name;
			public UsingBlob Usings;
			public TokenIdentifier[] Namespace;
			public AttributeObject[] CustomAttributes;
		}
	}
}
