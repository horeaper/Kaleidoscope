namespace Kaleidoscope.Analysis
{
	public sealed class ConversionOperatorDeclare : MethodDeclare
	{
		public readonly ReferenceToType ReturnType;
		public readonly bool IsExplicit;

		public ConversionOperatorDeclare(Builder builder, InstanceTypeDeclare owner)
			: base(builder, owner)
		{
			ReturnType = builder.ReturnType;
			IsExplicit = builder.IsExplicit;
		}

		public new sealed class Builder : MethodDeclare.Builder
		{
			public ReferenceToType ReturnType;
			public bool IsExplicit;
		}
	}
}
