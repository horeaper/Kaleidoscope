using System.Collections.Immutable;
using System.Text;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	public sealed class RootClassTypeDeclare : ClassTypeDeclare
	{
		public CodeFile OwnerFile { get; }
		public UsingBlob Usings { get; }
		public ImmutableArray<TokenIdentifier> Namespace { get; }
		public bool IsPublic { get; }
		public override string Fullname { get; }

		public RootClassTypeDeclare(Builder builder)
			: base(builder)
		{
			OwnerFile = builder.OwnerFile;
			Usings = builder.Usings;
			Namespace = builder.Namespace.MoveToImmutable();
			IsPublic = builder.IsPublic;

			var fullname = new StringBuilder();
			foreach (var item in Namespace) {
				fullname.Append(item.Text);
				fullname.Append(".");
			}
			fullname.Append(Name.Text);
			Fullname = fullname.ToString();
		}

		public override string ToString()
		{
			return $"[RootClassTypeDeclare] {TypeKind} {Fullname}";
		}

		public new sealed class Builder : ClassTypeDeclare.Builder
		{
			public CodeFile OwnerFile;
			public UsingBlob Usings;
			public ImmutableArray<TokenIdentifier>.Builder Namespace;
			public bool IsPublic;
		}
	}
}
