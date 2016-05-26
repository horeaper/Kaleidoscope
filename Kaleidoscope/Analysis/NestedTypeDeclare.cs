using System.Text;
using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public sealed class NestedTypeDeclare<T> where T : InstanceTypeDeclare
	{
		public readonly T Type;
		public readonly ClassTypeDeclare ContainerType;
		public readonly AccessModifier AccessModifier;
		public readonly bool IsNew;
		readonly string m_displayName;

		public NestedTypeDeclare(Builder builder, ClassTypeDeclare containerType)
		{
			Type = builder.Type;
			ContainerType = containerType;
			AccessModifier = builder.AccessModifier;
			IsNew = builder.IsNew;

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

		public sealed class Builder
		{
			public T Type;
			public AccessModifier AccessModifier;
			public bool IsNew;
		}
	}
}
