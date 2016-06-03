using System.Collections.Immutable;
using System.Text;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	public sealed class RootTypeDeclare<T> where T : InstanceTypeDeclare
	{
		public readonly T Type;
		public readonly CodeFile OwnerFile;
		public readonly UsingBlob Usings;
		public readonly ImmutableArray<TokenIdentifier> Namespace;
		public readonly bool IsPublic;
		readonly string m_displayName;

		public RootTypeDeclare(Builder builder)
		{
			Type = builder.Type;
			OwnerFile = builder.OwnerFile;
			Usings = builder.Usings;
			Namespace = builder.Namespace.MoveToImmutable();
			IsPublic = builder.IsPublic;

			var text = new StringBuilder();
			text.Append("[RootClassTypeDeclare] ");
			if (IsPublic) {
				text.Append("public ");
			}
			text.Append(Type.DisplayName);
			m_displayName = text.ToString();
		}

		public override string ToString()
		{
			return m_displayName;
		}

		public sealed class Builder
		{
			public T Type;
			public CodeFile OwnerFile;
			public UsingBlob Usings;
			public ImmutableArray<TokenIdentifier>.Builder Namespace;
			public bool IsPublic;
		}
	}
}
