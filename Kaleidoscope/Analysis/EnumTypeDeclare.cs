using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public sealed class EnumTypeDeclare : InstanceTypeDeclare
	{
		public readonly EnumValueType ValueType;
		public readonly ImmutableArray<EnumItemObject> Items;

		public override string DisplayName => "enum " + Name.Text;
		public override IEnumerable<GenericDeclare> DeclaredGenerics => new GenericDeclare[0];

		public EnumTypeDeclare(Builder builder)
			: base(builder)
		{
			ValueType = builder.ValueType;
			Items = ImmutableArray.CreateRange(builder.Items.Select(item => new EnumItemObject(item, this)));
		}

		public new sealed class Builder : InstanceTypeDeclare.Builder
		{
			public EnumValueType ValueType = EnumValueType.@int;
			public readonly List<EnumItemObject.Builder> Items = new List<EnumItemObject.Builder>();
		}
	}
}
