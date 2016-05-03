using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.SyntaxObject.Primitive
{
	public sealed class NamespaceOrTypeName
	{
		public readonly Token Name;
		public readonly ImmutableArray<TypeReference> GenericReferences;

		public NamespaceOrTypeName(Token name, TypeReference[] generics)
		{
			Name = name;
			GenericReferences = ImmutableArray.Create(generics);
		}
	}
}
