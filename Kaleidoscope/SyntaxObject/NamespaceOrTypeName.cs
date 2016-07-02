using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Kaleidoscope.Analysis;

namespace Kaleidoscope.SyntaxObject
{
	public sealed class NamespaceOrTypeName
	{
		public readonly NameWithGeneric Name;
		public readonly ImmutableArray<ReferenceToType> GenericMembers;
		public string DisplayName { get; }

		public NamespaceOrTypeName(Builder builder)
		{
			Name = builder.Name;
			GenericMembers = ImmutableArray.CreateRange(builder.GenericMembers);

			var text = new StringBuilder();
			text.Append(Name != null ? Name.Name : "(null)");
			if (GenericMembers.Length > 0) {
				text.Append('<');
				for (int cnt = 0; cnt < GenericMembers.Length; ++cnt) {
					text.Append(GenericMembers[cnt].Text);
					if (cnt < GenericMembers.Length - 1) {
						text.Append(", ");
					}
				}
				text.Append('>');
			}
			DisplayName = text.ToString();
		}

		public override string ToString()
		{
			return "[ManagedTypeReference] " + DisplayName;
		}

		public sealed class Builder
		{
			public NameWithGeneric Name;
			public List<ReferenceToType> GenericMembers = new List<ReferenceToType>();
		}
	}
}
