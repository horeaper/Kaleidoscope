using System.Collections.Generic;
using System.Collections.Immutable;
using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public abstract class MethodDeclare : MemberDeclare
	{
		public readonly AccessModifier AccessModifier;
		public readonly bool IsNew;
		public readonly MethodInstanceKind InstanceKind;
		public readonly ImmutableArray<ParameterObject> Parameters;
		public readonly LambdaStyle LambdaContentStyle;
		public readonly TokenBlock BodyContent;

		protected MethodDeclare(Builder builder)
			: base(builder)
		{
			AccessModifier = builder.AccessModifier;
			IsNew = builder.IsNew;
			InstanceKind = builder.InstanceKind;
			Parameters = ImmutableArray.CreateRange(builder.Parameters);
			LambdaContentStyle = builder.LambdaContentStyle;
			BodyContent = builder.BodyContent;
		}

		public new abstract class Builder : MemberDeclare.Builder
		{
			public AccessModifier AccessModifier;
			public bool IsNew;
			public MethodInstanceKind InstanceKind;
			public readonly List<ParameterObject> Parameters = new List<ParameterObject>();
			public LambdaStyle LambdaContentStyle;
			public TokenBlock BodyContent;
		}
	}
}
