using System.Collections.Generic;
using System.Collections.Immutable;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	public sealed class ReferenceToCppType : ReferenceToType
	{
		public readonly bool IsGlobalNamespace;
		public readonly int PointerRank;
		public readonly ImmutableArray<ReferenceToInstance> ArrayItemNumber;    //[12][33][SomeClass.SomeConstant] -> { 12, 33, SomeClass.SomeConstant }
		public int ArrayRank => ArrayItemNumber.Length;

		public ReferenceToCppType(Builder builder)
			: base(builder.Content)
		{
			IsGlobalNamespace = builder.IsGlobalNamespace;
			ArrayItemNumber = ImmutableArray.Create(builder.ArrayItemNumber.ToArray());
		}

		public sealed class Builder
		{
			public TokenBlock Content;
			public bool IsGlobalNamespace;
			public int PointerRank;
			public List<ReferenceToInstance> ArrayItemNumber = new List<ReferenceToInstance>();
		}
	}
}
