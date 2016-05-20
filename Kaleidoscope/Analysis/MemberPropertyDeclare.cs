using System.Text;
using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public sealed class MemberPropertyDeclare : PropertyDeclare
	{
		public readonly TokenBlock DefaultValueContent;
		public readonly bool IsAuto;
		readonly string m_displayName;

		public MemberPropertyDeclare(Builder builder, InstanceTypeDeclare owner)
			: base(builder, owner)
		{
			DefaultValueContent = builder.DefaultValueContent;
			IsAuto = builder.IsAuto;

			var text = new StringBuilder();
			text.Append("[Property] ");
			PrintAccessModifier(text);
			PrintInstanceKind(text);
			text.Append(Type.Text);
			text.Append(' ');
			if (ExplicitInterface != null) {
				text.Append(ExplicitInterface.Text);
				text.Append('.');
			}
			text.Append(Name.Text);
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
			public bool IsAuto;
		}
	}
}
