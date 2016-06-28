using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Kaleidoscope.SyntaxObject
{
	public sealed class CppTypeReference : TypeReference
	{
		public readonly string Name;
		public readonly ImmutableArray<CppTemplateItem> TemplateItems;
		public override string DisplayName { get; }

		public CppTypeReference(Builder builder)
		{
			Name = builder.Name;
			TemplateItems = ImmutableArray.CreateRange(builder.TemplateItems);

			var text = new StringBuilder();
			text.Append(Name ?? "(null)");
			if (TemplateItems.Length > 0) {
				text.Append('<');
				for (int cnt = 0; cnt < TemplateItems.Length; ++cnt) {
					text.Append(TemplateItems[cnt].DisplayName);
					if (cnt < TemplateItems.Length - 1) {
						text.Append(", ");
					}
				}
				text.Append('>');
			}
			DisplayName = text.ToString();
		}

		public override string ToString()
		{
			return "[CppTypeReference] " + DisplayName;
		}

		public sealed class Builder
		{
			public string Name;
			public List<CppTemplateItem> TemplateItems = new List<CppTemplateItem>();
		}
	}
}
