using System.Collections.Immutable;
using System.Text;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	public class AttributeObject
	{
		public readonly UsingBlob Usings;
		public readonly ImmutableArray<TokenIdentifier> OwnerNamespace;

		public readonly ReferenceToManagedType Type;
		public readonly TokenBlock ConstructContent;
		readonly string m_displayName;

		public AttributeObject(UsingBlob usings, TokenIdentifier[] ownerNamespace, ReferenceToManagedType type, TokenBlock constructContent)
		{
			Usings = usings;
			OwnerNamespace = ImmutableArray.Create(ownerNamespace);
			Type = type;
			ConstructContent = constructContent;

			var builder = new StringBuilder();
			builder.Append(Type.Text);
			if (ConstructContent != null) {
				builder.Append('(');
				builder.Append(ConstructContent.Text);
				builder.Append(')');
			}
			m_displayName = builder.ToString();
		}

		public override string ToString()
		{
			return $"[AttributeObject] {m_displayName}({ConstructContent?.Text})" ;
		}
	}
}
