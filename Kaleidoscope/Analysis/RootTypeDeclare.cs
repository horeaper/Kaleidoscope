using System.Text;

namespace Kaleidoscope.Analysis
{
	public sealed class RootTypeDeclare<T> : RootInstanceTypeDeclare where T : InstanceTypeDeclare
	{
		public readonly T Type;
		readonly string m_displayName;

		public override InstanceTypeDeclare InstanceType => Type;

		public RootTypeDeclare(Builder builder)
			: base(builder)
		{
			Type = builder.Type;

			var text = new StringBuilder();
			text.Append("[RootClassTypeDeclare] ");
			if (IsPublic) {
				text.Append("public ");
			}
			text.Append(Type.DisplayName);
			m_displayName = text.ToString();
		}

		public override string ToString()
		{
			return m_displayName;
		}

		public new sealed class Builder : RootInstanceTypeDeclare.Builder
		{
			public T Type;
		}
	}
}
