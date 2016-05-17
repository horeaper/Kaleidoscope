using System.Collections.Generic;
using System.Collections.Immutable;

namespace Kaleidoscope.Analysis
{
	public sealed class MemberMethodDeclare : MethodDeclare
	{
		public readonly bool IsNew;
		public readonly ReferenceToType ReturnType;
		public readonly ReferenceToManagedType ExplicitInterface;
		public readonly ImmutableArray<GenericDeclare> GenericTypes;

		public MemberMethodDeclare(Builder builder, InstanceTypeDeclare owner)
			: base(builder, owner)
		{
			IsNew = builder.IsNew;
			ReturnType = builder.ReturnType;
			ExplicitInterface = builder.ExplicitInterface;
			GenericTypes = ImmutableArray.CreateRange(builder.GenericTypes);
		}

		public new sealed class Builder : MethodDeclare.Builder
		{
			public bool IsNew;
			public ReferenceToType ReturnType;
			public ReferenceToManagedType ExplicitInterface;
			public IEnumerable<GenericDeclare> GenericTypes;
		}
	}
}
