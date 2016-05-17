using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public sealed class ConstructorDeclare : MethodDeclare
	{
		public readonly TokenBlock ChainCallContent;

		public ConstructorDeclare(Builder builder, InstanceTypeDeclare owner)
			: base(builder, owner)
		{
			ChainCallContent = builder.ChainCallContent;
		}

		public new sealed class Builder : MethodDeclare.Builder
		{
			public TokenBlock ChainCallContent;
		}
	}
}
