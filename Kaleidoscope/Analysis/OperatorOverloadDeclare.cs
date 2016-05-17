using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	public sealed class OperatorOverloadDeclare : MethodDeclare
	{
		public readonly ReferenceToType ReturnType;
		public readonly TokenSymbol Operator;

		public OperatorOverloadDeclare(Builder builder)
			: base(builder)
		{
			ReturnType = builder.ReturnType;
			Operator = builder.Operator;
		}

		public new sealed class Builder : MethodDeclare.Builder
		{
			public ReferenceToType ReturnType;
			public TokenSymbol Operator;
		}
	}
}
