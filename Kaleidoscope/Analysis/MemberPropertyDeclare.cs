using System.Text;
using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public sealed class MemberPropertyDeclare : PropertyDeclare
	{
		public readonly TokenBlock DefaultValueContent;
		readonly string m_displayName;

		public MemberPropertyDeclare(Builder builder, InstanceTypeDeclare owner)
			: base(builder, owner)
		{
			DefaultValueContent = builder.DefaultValueContent;

			var text = new StringBuilder();
			text.Append("[Property] ");
			PrintAccessModifier(text);
			if (IsNew) {
				text.Append("new ");
			}
			PrintInstanceKind(text);
			text.Append(Type.Text);
			text.Append(' ');
			text.Append(NameContent.Text);
			PrintMethods(text);
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

		public new sealed class Builder : PropertyDeclare.Builder
		{
			public TokenBlock DefaultValueContent;
		}
	}
}
