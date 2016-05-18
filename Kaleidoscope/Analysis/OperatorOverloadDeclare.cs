using System.Text;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	public sealed class OperatorOverloadDeclare : MethodDeclare
	{
		public readonly ReferenceToType ReturnType;
		public readonly TokenSymbol Operator;
		readonly string m_displayName;

		public OperatorOverloadDeclare(Builder builder, InstanceTypeDeclare owner)
			: base(builder, owner)
		{
			ReturnType = builder.ReturnType;
			Operator = builder.Operator;

			var text = new StringBuilder();
			text.Append("[OperatorOverload] ");
			text.Append("public static ");
			text.Append(ReturnType.Text);
			text.Append(" operator ");
			text.Append(Operator.Text);
			PrintParameters(text);
			m_displayName = text.ToString();
		}

		public override string ToString()
		{
			return m_displayName;
		}

		public new sealed class Builder : MethodDeclare.Builder
		{
			public ReferenceToType ReturnType;
			public TokenSymbol Operator;
		}
	}
}
