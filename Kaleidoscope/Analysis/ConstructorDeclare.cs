using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public sealed class ConstructorDeclare : MethodDeclare
	{
		public readonly TokenBlock ParentCallContent;

		public ConstructorDeclare(Builder builder)
			: base(builder)
		{
			ParentCallContent = builder.ParentCallContent;
		}

		public new sealed class Builder : MethodDeclare.Builder
		{
			public TokenBlock ParentCallContent;
		}
	}
}
