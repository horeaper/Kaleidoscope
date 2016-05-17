using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	public abstract class InstanceTypeDeclare : ManagedDeclare
	{
		public readonly ImmutableArray<AttributeObject> CustomAttributes;
		public abstract string Fullname { get; }

		protected InstanceTypeDeclare(Builder builder)
			: base(builder.Name)
		{
			CustomAttributes = ImmutableArray.CreateRange(builder.CustomAttributes.Select(item => new AttributeObject(item, this)));
		}

		public abstract class Builder
		{
			public TokenIdentifier Name;
			public IEnumerable<AttributeObject.Builder> CustomAttributes;
		}
	}
}
