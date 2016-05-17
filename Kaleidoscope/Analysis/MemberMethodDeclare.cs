using System.Collections.Generic;
using System.Collections.Immutable;

namespace Kaleidoscope.Analysis
{
	public sealed class MemberMethodDeclare : MethodDeclare
	{
		public readonly ReferenceToType ReturnType;
		public readonly ReferenceToManagedType ExplicitInterface;
		public readonly ImmutableArray<GenericDeclare> GenericTypes;

		public MemberMethodDeclare(Builder builder)
			: base(builder)
		{
			ReturnType = builder.ReturnType;
			ExplicitInterface = builder.ExplicitInterface;
			GenericTypes = ImmutableArray.CreateRange(builder.GenericTypes);
		}

		public new sealed class Builder : MethodDeclare.Builder
		{
			public ReferenceToType ReturnType;
			public ReferenceToManagedType ExplicitInterface;
			public IEnumerable<GenericDeclare> GenericTypes;
		}
	}
}
