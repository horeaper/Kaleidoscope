using System.Collections.Immutable;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	public abstract class InstanceTypeDeclare : ManagedDeclare
	{
		public readonly ImmutableArray<AttributeObject> CustomAttributes;

		protected InstanceTypeDeclare(TokenIdentifier name, AttributeObject[] customAttributes)
			: base(name)
		{
			CustomAttributes = ImmutableArray.Create(customAttributes);
		}
	}
}
