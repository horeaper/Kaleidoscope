using System.Collections.Generic;
using System.Collections.Immutable;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	public abstract class ClassTypeDeclare : InstanceTypeDeclare
	{
		public ClassTypeKind TypeKind { get; protected set; }
		public TypeInstanceKind InstanceKind { get; protected set; }
		public bool IsUnsafe { get; protected set; }
		public bool IsPartial { get; protected set; }
		public ImmutableArray<GenericDeclare> GenericTypes { get; protected set; }
		public ImmutableArray<ReferenceToManagedType> Inherits { get; protected set; }

		public ImmutableArray<NestedClassTypeDeclare> NestedClasses { get; protected set; }

		protected ClassTypeDeclare(TokenIdentifier name, AttributeObject[] customAttributes)
			: base(name, customAttributes)
		{
		}

		protected void ApplyMembers(Builder builder)
		{
			TypeKind = builder.TypeKind;
			InstanceKind = builder.InstanceKind;
			IsUnsafe = builder.IsUnsafe;
			IsPartial = builder.IsPartial;
			GenericTypes = ImmutableArray.Create(builder.GenericTypes);
			Inherits = ImmutableArray.Create(builder.Inherits);

			NestedClasses = ImmutableArray.CreateRange(builder.NestedClasses);
		}

		public abstract class Builder
		{
			public ClassTypeKind TypeKind;
			public TypeInstanceKind InstanceKind;
			public bool IsUnsafe;
			public bool IsPartial;
			public GenericDeclare[] GenericTypes;
			public ReferenceToManagedType[] Inherits;

			public readonly List<NestedClassTypeDeclare> NestedClasses = new List<NestedClassTypeDeclare>();
		}
	}
}
