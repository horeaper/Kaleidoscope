using System.Collections.Immutable;

namespace Kaleidoscope.SyntaxObject
{
	public abstract class MemberDeclare
	{
		public readonly ClassTypeDeclare Owner;
		public readonly ImmutableArray<AttributeObject> CustomAttributes;

		protected MemberDeclare(ClassTypeDeclare owner, AttributeObject[] attributes)
		{
			Owner = owner;
			CustomAttributes = ImmutableArray.Create(attributes);
		}
	}
}
