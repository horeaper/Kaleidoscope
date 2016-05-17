namespace Kaleidoscope.Analysis
{
	public sealed class ConversionOperatorDeclare : MethodDeclare
	{
		public readonly ReferenceToType ReturnType;
		public readonly bool IsExplicit;

		public ConversionOperatorDeclare(Builder builder)
			: base(builder)
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
