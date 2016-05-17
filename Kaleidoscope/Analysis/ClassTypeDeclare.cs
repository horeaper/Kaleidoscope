using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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

		protected ClassTypeDeclare(TokenIdentifier name)
			: base(name)
		{
		}

		protected void ApplyMembers(Builder builder)
		{
			CustomAttributes = ImmutableArray.CreateRange(builder.CustomAttributes.Select(item => new AttributeObject(item, this)));
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
			public IEnumerable<AttributeObject.Builder> CustomAttributes;
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
