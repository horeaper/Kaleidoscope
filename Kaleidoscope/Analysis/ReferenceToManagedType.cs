using System.Collections.Generic;
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

		public NamespaceOrTypeName[] Identifiers { get; private set; }


		public GenericDeclare GenericTarget { get; internal set; }

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

		internal override void Bind(BindContext context)
		{
			try {
				BindWorker(context);
			}
			catch (ParseException e) {
				context.InfoOutput.OutputError(e);
			}
		}

		void BindWorker(BindContext context)
		{
			if (context.ParentResolveChain.Contains(this)) {
				throw ParseException.AsTokenBlock(Content, Error.Bind.CircularParent);
			}
			context.ParentResolveChain.Push(this);

			//TODO: Resolve

			context.ParentResolveChain.Pop();
			throw new System.NotImplementedException();
		}
	}
}
