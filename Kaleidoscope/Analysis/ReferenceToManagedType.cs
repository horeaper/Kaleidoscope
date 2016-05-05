using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public sealed class ReferenceToManagedType : ReferenceToType
	{
		public readonly bool IsGlobalNamespace;
		public readonly bool IsNullable;
		public readonly ImmutableArray<int> ArrayDimensions;	//[,][][,,] -> { 2, 1, 3 }
		public int ArrayRank => ArrayDimensions.Length;

		public ReferenceToManagedType(Builder builder)
			: base(builder.Content)
		{
			IsGlobalNamespace = builder.IsGlobalNamespace;
			IsNullable = builder.IsNullable;
			ArrayDimensions = ImmutableArray.Create(builder.ArrayDimensions.ToArray());
		}

		public string Text
		{
			get
			{
				var builder = new StringBuilder();
				builder.Append(Content.Text);
				if (IsNullable) {
					builder.Append('?');
				}
				else if (ArrayDimensions != null) {
					foreach (int dimension in ArrayDimensions) {
						builder.Append('[');
						for (int iCnt = 1; iCnt < dimension; ++iCnt) {
							builder.Append(',');
						}
						builder.Append(']');
					}
				}
				return builder.ToString();
			}
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
