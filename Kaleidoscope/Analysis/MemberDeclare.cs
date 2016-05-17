using System.Collections.Immutable;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	public abstract class MemberDeclare : ManagedDeclare
	{
		public InstanceTypeDeclare OwnerType { get; protected set; }
		public ImmutableArray<AttributeObject> CustomAttributes { get; protected set; }

		protected MemberDeclare(TokenIdentifier name)
			: base(name)
		{
		}
	}
}
