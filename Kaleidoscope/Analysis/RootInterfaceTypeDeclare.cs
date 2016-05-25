﻿using System.Collections.Immutable;
using System.Text;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	public sealed class RootInterfaceTypeDeclare : InterfaceTypeDeclare
	{
		public readonly CodeFile OwnerFile;
		public readonly UsingBlob Usings;
		public readonly ImmutableArray<TokenIdentifier> Namespace;
		public readonly bool IsPublic;
		public override string Fullname { get; }
		readonly string m_displayName;

		public RootInterfaceTypeDeclare(Builder builder)
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

			var text = new StringBuilder();
			text.Append("[RootInterfaceTypeDeclare] ");
			if (IsPublic) {
				text.Append("public ");
			}
			text.Append("interface ");
			text.Append(Fullname);
			m_displayName = text.ToString();
		}

		public override string ToString()
		{
			return m_displayName;
		}

		public new sealed class Builder : InterfaceTypeDeclare.Builder
		{
			public CodeFile OwnerFile;
			public UsingBlob Usings;
			public ImmutableArray<TokenIdentifier>.Builder Namespace;
			public bool IsPublic;
		}
	}
}
