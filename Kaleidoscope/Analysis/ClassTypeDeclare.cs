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

		public readonly ConstructorDeclare StaticConstructor;
		public readonly ImmutableArray<ConstructorDeclare> Constructors;
		public readonly DestructorDeclare Destructor;

		public readonly ImmutableArray<MemberMethodDeclare> Methods;
		public readonly ImmutableArray<OperatorOverloadDeclare> OperatorOverloads;
		public readonly ImmutableArray<ConversionOperatorDeclare> ConversionOperators;

		public readonly ImmutableArray<IndexerDeclare> Indexers;
		public readonly ImmutableArray<MemberPropertyDeclare> Properties;
		public readonly ImmutableArray<FieldDeclare> Fields;

		public readonly ImmutableArray<NestedClassTypeDeclare> NestedClasses;

		protected ClassTypeDeclare(Builder builder)
			: base(builder)
		{
			TypeKind = builder.TypeKind;
			InstanceKind = builder.InstanceKind;
			IsUnsafe = builder.IsUnsafe;
			IsPartial = builder.IsPartial;
			GenericTypes = ImmutableArray.CreateRange(builder.GenericTypes.Select(item => new GenericDeclare(item)));
			Inherits = ImmutableArray.CreateRange(builder.Inherits);

			StaticConstructor = new ConstructorDeclare(builder.StaticConstructor, this);
			Constructors = ImmutableArray.CreateRange(builder.Constructors.Select(item => new ConstructorDeclare(item, this)));
			Destructor = new DestructorDeclare(builder.Destructor, this);

			Methods = ImmutableArray.CreateRange(builder.Methods.Select(item => new MemberMethodDeclare(item, this)));
			OperatorOverloads = ImmutableArray.CreateRange(builder.OperatorOverloads.Select(item => new OperatorOverloadDeclare(item, this)));
			ConversionOperators = ImmutableArray.CreateRange(builder.ConversionOperators.Select(item => new ConversionOperatorDeclare(item, this)));

			Indexers = ImmutableArray.CreateRange(builder.Indexers.Select(item => new IndexerDeclare(item, this)));
			Properties = ImmutableArray.CreateRange(builder.Properties.Select(item => new MemberPropertyDeclare(item, this)));
			Fields = ImmutableArray.CreateRange(builder.Fields.Select(item => new FieldDeclare(item, this)));

			NestedClasses = ImmutableArray.CreateRange(builder.NestedClasses.Select(item => new NestedClassTypeDeclare(item, this)));
		}

		public new abstract class Builder : InstanceTypeDeclare.Builder
		{
			public ClassTypeKind TypeKind;
			public TypeInstanceKind InstanceKind;
			public bool IsUnsafe;
			public bool IsPartial;
			public IEnumerable<GenericDeclare.Builder> GenericTypes;
			public IEnumerable<ReferenceToManagedType> Inherits;

			public ConstructorDeclare.Builder StaticConstructor;
			public readonly List<ConstructorDeclare.Builder> Constructors = new List<ConstructorDeclare.Builder>();
			public DestructorDeclare.Builder Destructor;

			public readonly List<MemberMethodDeclare.Builder> Methods = new List<MemberMethodDeclare.Builder>();
			public readonly List<OperatorOverloadDeclare.Builder> OperatorOverloads = new List<OperatorOverloadDeclare.Builder>();
			public readonly List<ConversionOperatorDeclare.Builder> ConversionOperators = new List<ConversionOperatorDeclare.Builder>();

			public readonly List<IndexerDeclare.Builder> Indexers = new List<IndexerDeclare.Builder>();
			public readonly List<MemberPropertyDeclare.Builder> Properties = new List<MemberPropertyDeclare.Builder>();
			public readonly List<FieldDeclare.Builder> Fields = new List<FieldDeclare.Builder>();

			public readonly List<NestedClassTypeDeclare.Builder> NestedClasses = new List<NestedClassTypeDeclare.Builder>();
		}
	}
}
