using System.Text;
using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public sealed class NestedDelegateTypeDeclare : DelegateTypeDeclare
	{
		public readonly ClassTypeDeclare ContainerType;
		public readonly AccessModifier AccessModifier;
		public readonly bool IsNew;
		public override string Fullname { get; }
		readonly string m_displayName;

		public NestedDelegateTypeDeclare(Builder builder, ClassTypeDeclare containerType)
			: base(builder)
		{
			ContainerType = containerType;
			AccessModifier = builder.AccessModifier;
			IsNew = builder.IsNew;

			Fullname = ContainerType.Fullname + "." + Name.Text;

			var text = new StringBuilder();
			text.Append("[NestedDelegateTypeDeclare] ");
			if (AccessModifier != AccessModifier.@private) {
				text.Append(AccessModifier);
				text.Append(' ');
			}
			if (IsNew) {
				text.Append("new ");
			}
			text.Append("enum ");
			text.Append(Fullname);
			m_displayName = text.ToString();
		}

		public override string ToString()
		{
			return m_displayName;
		}

		public new sealed class Builder : DelegateTypeDeclare.Builder
		{
			public AccessModifier AccessModifier;
			public bool IsNew;
		}
	}
}
