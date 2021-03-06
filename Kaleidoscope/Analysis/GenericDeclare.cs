﻿using System.Collections.Generic;
using System.Collections.Immutable;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	/// <summary>
	/// Declare of a generic type member, eg. T in "class List&lt;T&gt; { ... }"
	/// </summary>
	public sealed class GenericDeclare : ManagedDeclare
	{
		public readonly bool IsContravariance;
		public readonly GenericKeywordConstraintType KeywordConstraint;
		public readonly TokenKeyword NewConstraint;
		public readonly EnumValueType? EnumTypeConstraint;
		public readonly ImmutableArray<ReferenceToManagedType> TypeConstraints;
		public string Text => Name.Text;

		public GenericDeclare(Builder builder)
			: base(builder.Name)
		{
			IsContravariance = builder.IsContravariance;
			KeywordConstraint = builder.KeywordConstraint;
			NewConstraint = builder.NewConstraint;
			EnumTypeConstraint = builder.EnumTypeConstraint;
			TypeConstraints = ImmutableArray.CreateRange(builder.TypeConstraints);
		}

		public override string ToString()
		{
			return "[GenericDeclare] " + Name.Text;
		}

		public sealed class Builder
		{
			public TokenIdentifier Name;
			public bool IsContravariance;
			public GenericKeywordConstraintType KeywordConstraint = GenericKeywordConstraintType.None;
			public TokenKeyword NewConstraint;
			public EnumValueType? EnumTypeConstraint;
			public readonly List<ReferenceToManagedType> TypeConstraints = new List<ReferenceToManagedType>();
		}
	}
}
