using System.Text;

namespace Kaleidoscope.Analysis
{
	public sealed class ConversionOperatorDeclare : MethodDeclare
	{
		public readonly ReferenceToType ReturnType;
		public readonly bool IsExplicit;
		readonly string m_displayName;

		public ConversionOperatorDeclare(Builder builder, InstanceTypeDeclare owner)
			: base(builder, owner)
		{
			ReturnType = builder.ReturnType;
			IsExplicit = builder.IsExplicit;

			var text = new StringBuilder();
			text.Append("[ConversionOperator] ");
			PrintAccessModifier(text);
			text.Append("static ");
			text.Append(IsExplicit ? "explicit " : "implicit ");
			text.Append("operator ");
			text.Append(ReturnType.Text);
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
			public bool IsExplicit;
		}
	}
}
