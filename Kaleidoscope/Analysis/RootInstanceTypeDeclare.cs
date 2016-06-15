using System.Collections.Immutable;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	public abstract class RootInstanceTypeDeclare : UserTypeDeclare
	{
		public readonly CodeFile OwnerFile;
		public readonly UsingBlob Usings;
		public readonly ImmutableArray<TokenIdentifier> Namespace;
		public readonly bool IsPublic;

		protected RootInstanceTypeDeclare(Builder builder)
		{
			OwnerFile = builder.OwnerFile;
			Usings = builder.Usings;
			Namespace = builder.Namespace.MoveToImmutable();
			IsPublic = builder.IsPublic;
		}

		public abstract class Builder
		{
			public CodeFile OwnerFile;
			public UsingBlob Usings;
			public ImmutableArray<TokenIdentifier>.Builder Namespace;
			public bool IsPublic;
		}
	}
}
