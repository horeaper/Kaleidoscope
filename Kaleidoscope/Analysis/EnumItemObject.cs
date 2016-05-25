using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	public sealed class EnumItemObject : ManagedDeclare
	{
		public readonly EnumTypeDeclare OwnerType;
		public readonly ImmutableArray<AttributeObject> CustomAttributes;
		public readonly TokenBlock DefaultValueContent;
		readonly string m_displayName;

		public EnumItemObject(Builder builder, EnumTypeDeclare owner)
			: base(builder.Name)
		{
			OwnerType = owner;
			CustomAttributes = ImmutableArray.CreateRange(builder.CustomAttributes.Select(item => new AttributeObject(item, this)));
			DefaultValueContent = builder.DefaultValueContent;

			var text = new StringBuilder();
			text.Append("[EnumItem] ");
			text.Append(Name.Text);
			if (DefaultValueContent != null) {
				text.Append(" = ");
				text.Append(DefaultValueContent.Text);
			}
			m_displayName = text.ToString();
		}

		public override string ToString()
		{
			return m_displayName;
		}

		public sealed class Builder
		{
			public AttributeObject.Builder[] CustomAttributes;
			public TokenIdentifier Name;
			public TokenBlock DefaultValueContent;
		}
	}
}
