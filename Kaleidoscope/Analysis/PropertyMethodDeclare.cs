namespace Kaleidoscope.Analysis
{
	public sealed class PropertyMethodDeclare : MethodDeclare
	{
		public readonly bool IsAuto;

		public PropertyMethodDeclare(Builder builder)
			: base(builder)
		{
			IsAuto = builder.IsAuto;
		}

		public new sealed class Builder : MethodDeclare.Builder
		{
			public bool IsAuto;
		}
	}
}
