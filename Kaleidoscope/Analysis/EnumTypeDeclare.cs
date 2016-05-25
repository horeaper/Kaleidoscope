using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public abstract class EnumTypeDeclare : InstanceTypeDeclare
	{
		public readonly EnumValueType ValueType;
		public readonly ImmutableArray<EnumItemObject> Items;

		protected EnumTypeDeclare(Builder builder)
			: base(builder)
		{
			ValueType = builder.ValueType;
			Items = ImmutableArray.CreateRange(builder.Items.Select(item => new EnumItemObject(item, this)));
		}

		public new abstract class Builder : InstanceTypeDeclare.Builder
		{
			public EnumValueType ValueType;
			public readonly List<EnumItemObject.Builder> Items = new List<EnumItemObject.Builder>();
		}
	}
}
