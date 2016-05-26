using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Kaleidoscope.Analysis
{
	public sealed class InterfaceTypeDeclare : InstanceTypeDeclare
	{
		public readonly bool IsPartial;
		public readonly ImmutableArray<GenericDeclare> GenericTypes;
		public readonly ImmutableArray<ReferenceToManagedType> Inherits;

		public readonly ImmutableArray<MemberMethodDeclare> Methods;
		public readonly ImmutableArray<MemberPropertyDeclare> Properties;
		public readonly ImmutableArray<IndexerDeclare> Indexers;

		public override string DisplayName => "interface " + Name.Text;

		public InterfaceTypeDeclare(Builder builder)
			: base(builder)
		{
			IsPartial = builder.IsPartial;
			GenericTypes = ImmutableArray.CreateRange(builder.GenericTypes.Select(item => new GenericDeclare(item)));
			Inherits = ImmutableArray.CreateRange(builder.Inherits);

			Methods = ImmutableArray.CreateRange(builder.Methods.Select(item => new MemberMethodDeclare(item, this)));
			Properties = ImmutableArray.CreateRange(builder.Properties.Select(item => new MemberPropertyDeclare(item, this)));
			Indexers = ImmutableArray.CreateRange(builder.Indexers.Select(item => new IndexerDeclare(item, this)));
		}

		public new sealed class Builder : InstanceTypeDeclare.Builder
		{
			public bool IsPartial;
			public IEnumerable<GenericDeclare.Builder> GenericTypes;
			public IEnumerable<ReferenceToManagedType> Inherits;

			public readonly List<MemberMethodDeclare.Builder> Methods = new List<MemberMethodDeclare.Builder>();
			public readonly List<MemberPropertyDeclare.Builder> Properties = new List<MemberPropertyDeclare.Builder>();
			public readonly List<IndexerDeclare.Builder> Indexers = new List<IndexerDeclare.Builder>();
		}
	}
}
