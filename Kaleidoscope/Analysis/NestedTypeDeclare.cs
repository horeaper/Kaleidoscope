using System.Text;
using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public sealed class NestedTypeDeclare<T> : NestedInstanceTypeDeclare where T : InstanceTypeDeclare
	{
		public readonly T Type;
		public readonly ClassTypeDeclare ContainerType;
		readonly string m_displayName;

		public override InstanceTypeDeclare InstanceType => Type;

		public NestedTypeDeclare(Builder builder, ClassTypeDeclare containerType)
			: base(builder)
		{
			Type = builder.Type;
			ContainerType = containerType;

			var text = new StringBuilder();
			text.Append("[NestedClassTypeDeclare] ");
			if (AccessModifier != AccessModifier.@private) {
				text.Append(AccessModifier);
				text.Append(' ');
			}
			if (IsNew) {
				text.Append("new ");
			}
			text.Append(Type.DisplayName);
			m_displayName = text.ToString();
		}

		public override string ToString()
		{
			return m_displayName;
		}

		public new sealed class Builder : NestedInstanceTypeDeclare.Builder
		{
			public T Type;
		}
	}
}
