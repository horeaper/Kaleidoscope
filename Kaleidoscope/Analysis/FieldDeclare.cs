using System.Text;
using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public sealed class FieldDeclare : MemberDeclare
	{
		public readonly bool IsNew;
		public readonly FieldInstanceKind InstanceKind;
		public readonly bool IsReadonly;
		public readonly bool IsUnsafe;
		public readonly ReferenceToType Type;
		public readonly TokenBlock DefaultValueContent;
		readonly string m_displayName;

		public FieldDeclare(Builder builder, InstanceTypeDeclare owner)
			: base(builder, owner)
		{
			IsNew = builder.IsNew;
			InstanceKind = builder.InstanceKind;
			IsReadonly = builder.IsReadonly;
			IsUnsafe = builder.IsUnsafe;
			Type = builder.Type;
			DefaultValueContent = builder.DefaultValueContent;

			var text = new StringBuilder();
			text.Append("[Field] ");
			PrintAccessModifier(text);
			if (IsNew) {
				text.Append("new ");
			}
			if (InstanceKind != FieldInstanceKind.None) {
				text.Append(InstanceKind);
				text.Append(' ');
			}
			if (IsReadonly) {
				text.Append("readonly ");
			}
			if (IsUnsafe) {
				text.Append("unsafe ");
			}
			text.Append(Type.Text);
			text.Append(' ');
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

		public new sealed class Builder : MemberDeclare.Builder
		{
			public bool IsNew;
			public FieldInstanceKind InstanceKind;
			public bool IsReadonly;
			public bool IsUnsafe;
			public ReferenceToType Type;
			public TokenBlock DefaultValueContent;
		}
	}
}
