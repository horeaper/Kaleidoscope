using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public sealed class AttributeObjectOnMember : AttributeObject
	{
		public AttributeObjectOnMember(ReferenceToManagedType type, TokenBlock constructContent)
			: base(type, constructContent)
		{
		}
	}
}
