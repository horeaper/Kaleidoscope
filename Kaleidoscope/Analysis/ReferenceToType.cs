using System.Collections.Generic;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	public abstract class ReferenceToType
	{
		public readonly TokenBlock Content;
		public string Text => Content.Text;

		protected ReferenceToType(TokenBlock content)
		{
			Content = content;
		}

		internal abstract void Bind(InfoOutput infoOutput, DeclaredNamespaceOrTypeName rootNamespace, UsingBlob usings, IEnumerable<TokenIdentifier> namespaces, IEnumerable<ClassTypeDeclare> containers, IEnumerable<GenericDeclare> enclosingGenerics, Stack<ReferenceToType> resolveChain);
	}
}
