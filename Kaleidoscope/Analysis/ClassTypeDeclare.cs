using System.Collections.Immutable;
using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public abstract class ClassTypeDeclare : InstanceTypeDeclare
	{
		public readonly ClassTypeKind TypeKind;
		public readonly TypeInstanceKind InstanceKind;
		public readonly bool IsUnsafe;
		public readonly bool IsPartial;
		public readonly ImmutableArray<GenericDeclare> GenericTypes;
		public readonly ImmutableArray<ReferenceToManagedType> Inherits;

		protected ClassTypeDeclare(Builder builder)
			: base(builder)
		{
			TypeKind = builder.TypeKind;
			InstanceKind = builder.InstanceKind;
			IsUnsafe = builder.IsUnsafe;
			IsPartial = builder.IsPartial;
			GenericTypes = ImmutableArray.Create(builder.GenericTypes);
			Inherits = ImmutableArray.Create(builder.Inherits);
		}

		public new abstract class Builder : InstanceTypeDeclare.Builder
		{
			public ClassTypeKind TypeKind;
			public TypeInstanceKind InstanceKind = TypeInstanceKind.None;
			public bool IsUnsafe;
			public bool IsPartial;
			public GenericDeclare[] GenericTypes;
			public ReferenceToManagedType[] Inherits;
		}
	}
}
