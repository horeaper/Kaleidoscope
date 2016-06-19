﻿using System.Collections.Generic;
using System.Collections.Immutable;
using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public sealed class ReferenceToManagedType : ReferenceToType
	{
		public readonly bool IsGlobalNamespace;
		public readonly bool IsNullable;
		public readonly ImmutableArray<int> ArrayDimensions;	//[,][][,,] -> { 2, 1, 3 }
		public int ArrayRank => ArrayDimensions.Length;

		public DeclaredManagedType Target { get; private set; }

		public ReferenceToManagedType(Builder builder)
			: base(builder.Content)
		{
			IsGlobalNamespace = builder.IsGlobalNamespace;
			IsNullable = builder.IsNullable;
			ArrayDimensions = ImmutableArray.Create(builder.ArrayDimensions.ToArray());
		}

		public override string ToString()
		{
			return "[ReferenceToManagedType] " + Text;
		}

		public sealed class Builder
		{
			public TokenBlock Content;
			public bool IsGlobalNamespace;
			public bool IsNullable;
			public List<int> ArrayDimensions = new List<int>();
		}
	}
}
