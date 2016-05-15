using System.Collections.Immutable;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	public abstract class InstanceTypeDeclare : ManagedDeclare
	{
		public readonly CodeFile OwnerFile;
		public readonly UsingBlob Usings;
		public readonly ImmutableArray<TokenIdentifier> Namespace;
		public readonly ImmutableArray<AttributeObject> CustomAttributes;

		protected InstanceTypeDeclare(Builder builder)
			: base(builder.Name)
		{
			OwnerFile = builder.OwnerFile;
			Usings = builder.Usings;
			Namespace = ImmutableArray.Create(builder.Namespace);
			CustomAttributes = ImmutableArray.Create(builder.CustomAttributes);
		}

		public abstract class Builder
		{
			public TokenIdentifier Name;
			public CodeFile OwnerFile;
			public UsingBlob Usings;
			public TokenIdentifier[] Namespace;
			public AttributeObject[] CustomAttributes;
		}
	}
}
