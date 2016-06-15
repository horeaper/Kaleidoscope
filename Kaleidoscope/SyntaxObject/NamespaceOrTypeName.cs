using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Kaleidoscope.SyntaxObject
{
	public sealed class NamespaceOrTypeName
	{
		public readonly string Name;
		public readonly ImmutableArray<string> Generics;
		public string DisplayName { get; }

		public NamespaceOrTypeName(Builder builder)
		{
			Name = builder.Name;
			Generics = ImmutableArray.CreateRange(builder.Generics);

			var text = new StringBuilder();
			text.Append(Name ?? "(null)");
			if (Generics.Length > 0) {
				text.Append('<');
				for (int cnt = 0; cnt < Generics.Length; ++cnt) {
					text.Append(Generics[cnt]);
					if (cnt < Generics.Length - 1) {
						text.Append(", ");
					}
				}
				text.Append('>');
			}
			DisplayName = text.ToString();
		}

		public override string ToString()
		{
			return "[NsOrTN] " + DisplayName;
		}

		public sealed class Builder
		{
			public string Name;
			public readonly List<string> Generics = new List<string>();
		}
	}
}
