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

		internal abstract void Bind(BindContext context);
	}

	//DO NOT REUSE!
	class BindContext
	{
		public BindContext(InfoOutput infoOutput, DeclaredNamespaceOrTypeName rootNamespace, UsingBlob usings, IEnumerable<TokenIdentifier> namespaces, IEnumerable<ClassTypeDeclare> containers, IEnumerable<GenericDeclare> enclosingGenerics)
		{
			InfoOutput = infoOutput;
			RootNamespace = rootNamespace;
			Usings = usings;
			Namespaces = namespaces;
			Containers = containers;
			EnclosingGenerics = enclosingGenerics;
		}

		public readonly InfoOutput InfoOutput;
		public readonly DeclaredNamespaceOrTypeName RootNamespace;
		public readonly UsingBlob Usings;
		public readonly IEnumerable<TokenIdentifier> Namespaces;
		public readonly IEnumerable<ClassTypeDeclare> Containers;
		public readonly IEnumerable<GenericDeclare> EnclosingGenerics;

		public readonly Stack<ReferenceToType> ParentResolveChain = new Stack<ReferenceToType>();
	}
}
