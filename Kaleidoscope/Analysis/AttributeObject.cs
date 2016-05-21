using System.Text;
using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public sealed class AttributeObject
	{
		public readonly ManagedDeclare Target;
		public readonly ReferenceToManagedType Type;
		public readonly TokenBlock ConstructContent;

		public AttributeObject(Builder builder, ManagedDeclare target)
		{
			Target = target;
			Type = builder.Type;
			ConstructContent = builder.ConstructContent;
		}

		public override string ToString()
		{
			return $"[AttributeObject] {Type.Text}({ConstructContent?.Text})" ;
		}

		public sealed class Builder
		{
			public ReferenceToManagedType Type;
			public TokenBlock ConstructContent;
		}
	}
}
