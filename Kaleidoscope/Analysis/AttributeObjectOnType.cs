using System.Collections.Immutable;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	public sealed class AttributeObjectOnType : AttributeObject
	{
		public readonly UsingBlob Usings;
		public readonly ImmutableArray<TokenIdentifier> OwnerNamespace;

		public AttributeObjectOnType(ReferenceToManagedType type, TokenBlock constructContent, UsingBlob usings, TokenIdentifier[] ownerNamespace)
			: base(type, constructContent)
		{
			Usings = usings;
			OwnerNamespace = ImmutableArray.Create(ownerNamespace);
		}
	}
}
