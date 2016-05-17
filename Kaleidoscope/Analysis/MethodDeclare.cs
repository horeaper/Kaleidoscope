using System.Collections.Generic;
using System.Collections.Immutable;
using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public abstract class MethodDeclare : MemberDeclare
	{
		public readonly AccessModifier AccessModifier;
		public readonly MethodInstanceKind InstanceKind;
		public readonly ImmutableArray<ParameterObject> Parameters;
		public readonly LambdaStyle LambdaContentStyle;
		public readonly TokenBlock BodyContent;

		protected MethodDeclare(Builder builder, InstanceTypeDeclare owner)
			: base(builder, owner)
		{
			AccessModifier = builder.AccessModifier;
			InstanceKind = builder.InstanceKind;
			Parameters = ImmutableArray.CreateRange(builder.Parameters);
			LambdaContentStyle = builder.LambdaContentStyle;
			BodyContent = builder.BodyContent;
		}

		public new abstract class Builder : MemberDeclare.Builder
		{
			public AccessModifier AccessModifier;
			public MethodInstanceKind InstanceKind;
			public IEnumerable<ParameterObject> Parameters;
			public LambdaStyle LambdaContentStyle;
			public TokenBlock BodyContent;
		}
	}
}
