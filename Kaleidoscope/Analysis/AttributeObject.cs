using System.Text;
using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public abstract class AttributeObject
	{
		public readonly ReferenceToManagedType Type;
		public readonly TokenBlock ConstructContent;
		readonly string m_displayName;

		protected AttributeObject(ReferenceToManagedType type, TokenBlock constructContent)
		{
			Type = type;
			ConstructContent = constructContent;

			var builder = new StringBuilder();
			builder.Append(Type.Text);
			if (ConstructContent != null) {
				builder.Append('(');
				builder.Append(ConstructContent.Text);
				builder.Append(')');
			}
			m_displayName = builder.ToString();
		}

		public override string ToString()
		{
			return $"[AttributeObject] {m_displayName}({ConstructContent?.Text})" ;
		}
	}
}
