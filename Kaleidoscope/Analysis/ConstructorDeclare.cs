using System.Text;
using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public sealed class ConstructorDeclare : MethodDeclare
	{
		public readonly TokenBlock ChainCallContent;
		readonly string m_displayName;

		public ConstructorDeclare(Builder builder, InstanceTypeDeclare owner)
			: base(builder, owner)
		{
			ChainCallContent = builder.ChainCallContent;

			var text = new StringBuilder();
			text.Append("[Constructor] ");
			PrintAccessModifier(text);
			PrintInstanceKind(text);
			text.Append(Name.Text);
			PrintParameters(text);
			m_displayName = text.ToString();
		}

		public override string ToString()
		{
			return m_displayName;
		}

		public new sealed class Builder : MethodDeclare.Builder
		{
			public TokenBlock ChainCallContent;
		}
	}
}
