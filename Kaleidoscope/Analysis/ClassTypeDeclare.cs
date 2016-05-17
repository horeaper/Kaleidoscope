using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

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

		public readonly ImmutableArray<NestedClassTypeDeclare> NestedClasses;

		protected ClassTypeDeclare(Builder builder)
			: base(builder)
		{
			TypeKind = builder.TypeKind;
			InstanceKind = builder.InstanceKind;
			IsUnsafe = builder.IsUnsafe;
			IsPartial = builder.IsPartial;
			GenericTypes = ImmutableArray.CreateRange(builder.GenericTypes);
			Inherits = ImmutableArray.CreateRange(builder.Inherits);

			NestedClasses = ImmutableArray.CreateRange(builder.NestedClasses.Select(item => new NestedClassTypeDeclare(item, this)));
		}

		public new abstract class Builder : InstanceTypeDeclare.Builder
		{
			public ClassTypeKind TypeKind;
			public TypeInstanceKind InstanceKind;
			public bool IsUnsafe;
			public bool IsPartial;
			public IEnumerable<GenericDeclare> GenericTypes;
			public IEnumerable<ReferenceToManagedType> Inherits;

			public readonly List<NestedClassTypeDeclare.Builder> NestedClasses = new List<NestedClassTypeDeclare.Builder>();
		}
	}
}
