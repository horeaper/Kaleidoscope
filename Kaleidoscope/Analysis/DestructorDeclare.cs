namespace Kaleidoscope.Analysis
{
	public sealed class DestructorDeclare : MethodDeclare
	{
		public DestructorDeclare(Builder builder)
			: base(builder)
		{
		}

		public new sealed class Builder : MethodDeclare.Builder
		{
		}
	}
}
