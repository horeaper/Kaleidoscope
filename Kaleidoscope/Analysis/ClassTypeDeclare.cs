using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	public sealed class ClassTypeDeclare : InstanceTypeDeclare
	{
		public readonly ClassTypeKind TypeKind;
		public readonly TypeInstanceKind InstanceKind;
		public readonly bool IsPartial;
		public readonly ImmutableArray<GenericDeclare> GenericTypes;
		public readonly ImmutableArray<ReferenceToType> Inherits;

		public readonly ConstructorDeclare StaticConstructor;
		public readonly ImmutableArray<ConstructorDeclare> Constructors;
		public readonly DestructorDeclare Destructor;

		public readonly ImmutableArray<MemberMethodDeclare> Methods;
		public readonly ImmutableArray<OperatorOverloadDeclare> OperatorOverloads;
		public readonly ImmutableArray<ConversionOperatorDeclare> ConversionOperators;

		public readonly ImmutableArray<MemberPropertyDeclare> Properties;
		public readonly ImmutableArray<IndexerDeclare> Indexers;
		public readonly ImmutableArray<FieldDeclare> Fields;

		public readonly ImmutableArray<NestedTypeDeclare<ClassTypeDeclare>> NestedClasses;
		public readonly ImmutableArray<NestedTypeDeclare<InterfaceTypeDeclare>> NestedInterfaces;
		public readonly ImmutableArray<NestedTypeDeclare<EnumTypeDeclare>> NestedEnums;
		public readonly ImmutableArray<NestedTypeDeclare<DelegateTypeDeclare>> NestedDelegates;

		public IEnumerable<NestedInstanceTypeDeclare> DefinedTypes => NestedClasses.Cast<NestedInstanceTypeDeclare>()
																				   .Concat(NestedInterfaces)
																				   .Concat(NestedEnums)
																				   .Concat(NestedDelegates);

		public override string DisplayName => TypeKind + " " + Name.Text;
		public override IEnumerable<GenericDeclare> DeclaredGenerics => GenericTypes;

		public ClassTypeDeclare(Builder builder)
			: base(builder)
		{
			TypeKind = builder.TypeKind;
			InstanceKind = builder.InstanceKind;
			IsPartial = builder.IsPartial;
			GenericTypes = ImmutableArray.CreateRange(builder.GenericTypes.Select(item => new GenericDeclare(item)));
			Inherits = ImmutableArray.CreateRange(builder.Inherits);

			StaticConstructor = builder.StaticConstructor != null ? new ConstructorDeclare(builder.StaticConstructor, this) : null;
			Constructors = ImmutableArray.CreateRange(builder.Constructors.Select(item => new ConstructorDeclare(item, this)));
			Destructor = builder.Destructor != null ? new DestructorDeclare(builder.Destructor, this) : null;

			Methods = ImmutableArray.CreateRange(builder.Methods.Select(item => new MemberMethodDeclare(item, this)));
			OperatorOverloads = ImmutableArray.CreateRange(builder.OperatorOverloads.Select(item => new OperatorOverloadDeclare(item, this)));
			ConversionOperators = ImmutableArray.CreateRange(builder.ConversionOperators.Select(item => new ConversionOperatorDeclare(item, this)));

			Properties = ImmutableArray.CreateRange(builder.Properties.Select(item => new MemberPropertyDeclare(item, this)));
			Indexers = ImmutableArray.CreateRange(builder.Indexers.Select(item => new IndexerDeclare(item, this)));
			Fields = ImmutableArray.CreateRange(builder.Fields.Select(item => new FieldDeclare(item, this)));

			NestedClasses = ImmutableArray.CreateRange(builder.NestedClasses.Select(item => new NestedTypeDeclare<ClassTypeDeclare>(item, this)));
			NestedInterfaces = ImmutableArray.CreateRange(builder.NestedInterfaces.Select(item => new NestedTypeDeclare<InterfaceTypeDeclare>(item, this)));
			NestedEnums = ImmutableArray.CreateRange(builder.NestedEnums.Select(item => new NestedTypeDeclare<EnumTypeDeclare>(item, this)));
			NestedDelegates = ImmutableArray.CreateRange(builder.NestedDelegates.Select(item => new NestedTypeDeclare<DelegateTypeDeclare>(item, this)));
		}

		public new sealed class Builder : InstanceTypeDeclare.Builder
		{
			public ClassTypeKind TypeKind;
			public TypeInstanceKind InstanceKind;
			public bool IsPartial;
			public IEnumerable<GenericDeclare.Builder> GenericTypes;
			public IEnumerable<ReferenceToType> Inherits;

			public ConstructorDeclare.Builder StaticConstructor;
			public readonly List<ConstructorDeclare.Builder> Constructors = new List<ConstructorDeclare.Builder>();
			public DestructorDeclare.Builder Destructor;

			public readonly List<MemberMethodDeclare.Builder> Methods = new List<MemberMethodDeclare.Builder>();
			public readonly List<OperatorOverloadDeclare.Builder> OperatorOverloads = new List<OperatorOverloadDeclare.Builder>();
			public readonly List<ConversionOperatorDeclare.Builder> ConversionOperators = new List<ConversionOperatorDeclare.Builder>();

			public readonly List<MemberPropertyDeclare.Builder> Properties = new List<MemberPropertyDeclare.Builder>();
			public readonly List<IndexerDeclare.Builder> Indexers = new List<IndexerDeclare.Builder>();
			public readonly List<FieldDeclare.Builder> Fields = new List<FieldDeclare.Builder>();

			public readonly List<NestedTypeDeclare<ClassTypeDeclare>.Builder> NestedClasses = new List<NestedTypeDeclare<ClassTypeDeclare>.Builder>();
			public readonly List<NestedTypeDeclare<InterfaceTypeDeclare>.Builder> NestedInterfaces = new List<NestedTypeDeclare<InterfaceTypeDeclare>.Builder>();
			public readonly List<NestedTypeDeclare<EnumTypeDeclare>.Builder> NestedEnums = new List<NestedTypeDeclare<EnumTypeDeclare>.Builder>();
			public readonly List<NestedTypeDeclare<DelegateTypeDeclare>.Builder> NestedDelegates = new List<NestedTypeDeclare<DelegateTypeDeclare>.Builder>();
		}

		public void BindParents(InfoOutput infoOutput, DeclaredNamespaceOrTypeName rootNamespace, UsingBlob usings, IEnumerable<TokenIdentifier> namespaces, Stack<ClassTypeDeclare> containers)
		{
			foreach (var item in Inherits) {
				var genericTarget = GenericTypes.FirstOrDefault(generic => generic.Text == item.Text);
				if (genericTarget != null) {
					if (genericTarget.KeywordConstraint != GenericKeywordConstraintType.@interface) {
						infoOutput.OutputError(ParseException.AsToken(genericTarget.Name, Error.Bind.GenericMustBeInterface));
					}
					else {
						((ReferenceToManagedType)item).GenericTarget = genericTarget;
					}
				}
				else {
					item.Bind(new BindContext(infoOutput, rootNamespace, usings, namespaces, containers, new GenericDeclare[0]));
				}
			}

			containers.Push(this);
			foreach (var item in NestedClasses) {
				item.Type.BindParents(infoOutput, rootNamespace, usings, namespaces, containers);
			}
			foreach (var item in NestedInterfaces) {
				item.Type.BindParents(infoOutput, rootNamespace, usings, namespaces, containers);
			}
			containers.Pop();
		}
	}
}
