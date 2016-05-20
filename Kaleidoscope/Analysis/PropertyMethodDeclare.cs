namespace Kaleidoscope.Analysis
{
	public sealed class PropertyMethodDeclare : MethodDeclare
	{
		public PropertyMethodDeclare(Builder builder, InstanceTypeDeclare owner)
			: base(builder, owner)
		{
		}

		public new sealed class Builder : MethodDeclare.Builder
		{
		}
	}
}
