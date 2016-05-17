namespace Kaleidoscope.Analysis
{
	public sealed class DestructorDeclare : MethodDeclare
	{
		public DestructorDeclare(Builder builder, InstanceTypeDeclare owner)
			: base(builder, owner)
		{
		}

		public new sealed class Builder : MethodDeclare.Builder
		{
		}
	}
}
