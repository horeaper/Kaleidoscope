using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public sealed class ReferenceToCppType : ReferenceToType
	{
		public readonly bool IsGlobalNamespace;
		public readonly int PointerRank;
		public readonly ImmutableArray<ReferenceToConstant> ArrayItemNumber;    //[12][33][SomeClass.SomeConstant] -> { 12, 33, SomeClass.SomeConstant }
		public int ArrayRank => ArrayItemNumber.Length;

		public string Target { get; private set; }

		public ReferenceToCppType(Builder builder)
			: base(builder.Content)
		{
			IsGlobalNamespace = builder.IsGlobalNamespace;
			PointerRank = builder.PointerRank;
			ArrayItemNumber = ImmutableArray.Create(builder.ArrayItemNumber.ToArray());
		}

		public sealed class Builder
		{
			public TokenBlock Content;
			public bool IsGlobalNamespace;
			public int PointerRank;
			public List<ReferenceToConstant> ArrayItemNumber = new List<ReferenceToConstant>();
		}

		internal override void Bind(BindContext context)
		{
			var builder = new StringBuilder();
			foreach (var item in Content) {
				builder.Append(item.Text);
			}
			Target = builder.ToString();
		}
	}
}
