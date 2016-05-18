using System.Collections.Immutable;
using System.Linq;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	public abstract class MemberDeclare : ManagedDeclare
	{
		public readonly InstanceTypeDeclare OwnerType;
		public readonly ImmutableArray<AttributeObject> CustomAttributes;

		protected MemberDeclare(Builder builder, InstanceTypeDeclare owner)
			: base(builder.Name)
		{
			OwnerType = owner;
			CustomAttributes = ImmutableArray.CreateRange(builder.CustomAttributes.Select(item => new AttributeObject(item, this)));
		}

		public abstract class Builder
		{
			public TokenIdentifier Name;
			public AttributeObject.Builder[] CustomAttributes;
		}
	}
}
