using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Kaleidoscope.Analysis
{
	public sealed class MemberMethodDeclare : MethodDeclare
	{
		public readonly bool IsNew;
		public readonly ReferenceToType ReturnType;
		public readonly ReferenceToType ExplicitInterface;
		public readonly ImmutableArray<GenericDeclare> GenericTypes;
		readonly string m_displayName;

		public MemberMethodDeclare(Builder builder, InstanceTypeDeclare owner)
			: base(builder, owner)
		{
			IsNew = builder.IsNew;
			ReturnType = builder.ReturnType;
			ExplicitInterface = builder.ExplicitInterface;
			GenericTypes = ImmutableArray.CreateRange(builder.GenericTypes.Select(item => new GenericDeclare(item)));

			var text = new StringBuilder();
			text.Append("[Method] ");
			PrintAccessModifier(text);
			if (IsNew) {
				text.Append("new ");
			}
			PrintInstanceKind(text);
			text.Append(ReturnType.Text);
			text.Append(' ');
			if (ExplicitInterface != null) {
				text.Append(ExplicitInterface.Text);
				text.Append('.');
			}
			text.Append(Name.Text);
			if (GenericTypes.Length > 0) {
				text.Append('<');
				for (int cnt = 0; cnt < GenericTypes.Length; ++cnt) {
					text.Append(GenericTypes[cnt].Text);
					if (cnt < GenericTypes.Length - 1) {
						text.Append(", ");
					}
				}
				text.Append('>');
			}
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
			public ReferenceToType ExplicitInterface;
			public IEnumerable<GenericDeclare.Builder> GenericTypes;
		}
	}
}
