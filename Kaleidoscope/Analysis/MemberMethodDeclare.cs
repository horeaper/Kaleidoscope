using System.Text;
using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public sealed class MemberMethodDeclare : MethodDeclare
	{
		public readonly bool IsNew;
		public readonly ReferenceToType ReturnType;
		public readonly TokenBlock NameContent;
		public readonly TokenBlock GenericConstraintContent;
		readonly string m_displayName;

		public MemberMethodDeclare(Builder builder, InstanceTypeDeclare owner)
			: base(builder, owner)
		{
			IsNew = builder.IsNew;
			ReturnType = builder.ReturnType;
			NameContent = builder.NameContent;
			GenericConstraintContent = builder.GenericConstraintContent;

			var text = new StringBuilder();
			text.Append("[Method] ");
			PrintAccessModifier(text);
			if (IsNew) {
				text.Append("new ");
			}
			PrintInstanceKind(text);
			text.Append(ReturnType.Text);
			text.Append(' ');
			text.Append(NameContent.Text);
			PrintParameters(text);
			m_displayName = text.ToString();
		}

		public override string ToString()
		{
			return m_displayName;
		}

		public new sealed class Builder : MethodDeclare.Builder
		{
			public bool IsNew;
			public ReferenceToType ReturnType;
			public TokenBlock NameContent;
			public TokenBlock GenericConstraintContent;
		}
	}
}
