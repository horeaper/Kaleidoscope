using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public sealed class DeclaredManagedType
	{
		public readonly DeclaredNamespaceOrTypeName Owner;
		public readonly NamespaceOrTypeName Name;
		public readonly ImmutableArray<UserTypeDeclare> Items;
		public string Fullname { get; }

		public DeclaredManagedType(Builder builder, DeclaredNamespaceOrTypeName owner)
		{
			Owner = owner;
			Name = new NamespaceOrTypeName(builder.Name);
			Items = ImmutableArray.CreateRange(builder.Items);

			var text = new StringBuilder();
			text.Append(Name.DisplayName);
			var currentNsOrTN = Owner;
			while (currentNsOrTN?.Name.Name != null) {
				text.Insert(0, '.');
				text.Insert(0, currentNsOrTN.Name.DisplayName);
				currentNsOrTN = currentNsOrTN.Owner;
			}
			Fullname = text.ToString();
		}

		public override string ToString()
		{
			return "[DeclaredManagedType] " + Fullname;
		}

		public sealed class Builder
		{
			public NamespaceOrTypeName.Builder Name = new NamespaceOrTypeName.Builder();
			public readonly List<UserTypeDeclare> Items = new List<UserTypeDeclare>();
		}
	}
}
