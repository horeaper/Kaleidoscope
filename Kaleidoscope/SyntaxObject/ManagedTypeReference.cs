using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Kaleidoscope.SyntaxObject
{
	public sealed class ManagedTypeReference : TypeReference
	{
		public readonly NamespaceOrTypeName Name;
		public readonly ImmutableArray<TypeReference> GenericMembers;
		public override string DisplayName { get; }

		public ManagedTypeReference(Builder builder)
		{
			Name = builder.Name;
			GenericMembers = ImmutableArray.CreateRange(builder.GenericTypes);

			var text = new StringBuilder();
			text.Append(Name != null ? Name.Name : "(null)");
			if (GenericMembers.Length > 0) {
				text.Append('<');
				for (int cnt = 0; cnt < GenericMembers.Length; ++cnt) {
					text.Append(GenericMembers[cnt].DisplayName);
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
			public NamespaceOrTypeName Name;
			public List<TypeReference> GenericTypes = new List<TypeReference>();
		}
	}
}
