using System.Text;
using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public class AttributeObject
	{
		public readonly ManagedDeclare Target;
		public readonly ReferenceToManagedType Type;
		public readonly TokenBlock ConstructContent;
		readonly string m_displayName;

		public AttributeObject(Builder builder, ManagedDeclare target)
		{
			Target = target;
			Type = builder.Type;
			ConstructContent = builder.ConstructContent;

			var displayName = new StringBuilder();
			displayName.Append(Type.Text);
			if (ConstructContent != null) {
				displayName.Append('(');
				displayName.Append(ConstructContent.Text);
				displayName.Append(')');
			}
			m_displayName = displayName.ToString();
		}

		public override string ToString()
		{
			return $"[AttributeObject] {m_displayName}({ConstructContent?.Text})" ;
		}

		public sealed class Builder
		{
			public ReferenceToManagedType Type;
			public TokenBlock ConstructContent;
		}
	}
}
