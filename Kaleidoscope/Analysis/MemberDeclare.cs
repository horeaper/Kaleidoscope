using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	public abstract class MemberDeclare : ManagedDeclare
	{
		public readonly AccessModifier AccessModifier;
		public readonly InstanceTypeDeclare OwnerType;
		public readonly ImmutableArray<AttributeObject> CustomAttributes;

		protected MemberDeclare(Builder builder, InstanceTypeDeclare owner)
			: base(builder.Name)
		{
			AccessModifier = builder.AccessModifier;
			OwnerType = owner;
			CustomAttributes = ImmutableArray.CreateRange(builder.CustomAttributes.Select(item => new AttributeObject(item, this)));
		}

		protected void PrintAccessModifier(StringBuilder builder)
		{
			if (AccessModifier != AccessModifier.@private) {
				builder.Append(AccessModifier);
				builder.Append(' ');
			}
		}

		public abstract class Builder
		{
			public AttributeObject.Builder[] CustomAttributes;
			public AccessModifier AccessModifier;
			public TokenIdentifier Name;
		}
	}
}
