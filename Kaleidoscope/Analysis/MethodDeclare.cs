﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public abstract class MethodDeclare : MemberDeclare
	{
		public readonly bool IsSealed;
		public readonly MethodInstanceKind InstanceKind;
		public readonly bool IsAsync;
		public readonly ImmutableArray<ParameterObject> Parameters;
		public readonly LambdaStyle LambdaContentStyle;
		public readonly TokenBlock BodyContent;

		protected MethodDeclare(Builder builder, InstanceTypeDeclare owner)
			: base(builder, owner)
		{
			IsSealed = builder.IsSealed;
			InstanceKind = builder.InstanceKind;
			IsAsync = builder.IsAsync;
			Parameters = ImmutableArray.CreateRange(builder.Parameters);
			LambdaContentStyle = builder.LambdaContentStyle;
			BodyContent = builder.BodyContent;
		}

		protected void PrintInstanceKind(StringBuilder builder)
		{
			if (IsSealed) {
				builder.Append("sealed ");
			}
			if (InstanceKind != MethodInstanceKind.None) {
				builder.Append(InstanceKind);
				builder.Append(' ');
			}
			if (IsAsync) {
				builder.Append("async ");
			}
		}

		protected void PrintParameters(StringBuilder builder)
		{
			builder.Append('(');
			for (int cnt = 0; cnt < Parameters.Length; ++cnt) {
				builder.Append(Parameters[cnt].Text);
				if (cnt < Parameters.Length - 1) {
					builder.Append(", ");
				}
			}
			builder.Append(')');
		}

		public new abstract class Builder : MemberDeclare.Builder
		{
			public bool IsSealed;
			public MethodInstanceKind InstanceKind;
			public bool IsAsync;
			public IEnumerable<ParameterObject> Parameters;
			public LambdaStyle LambdaContentStyle;
			public TokenBlock BodyContent;
		}
	}
}
