using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	public sealed class ReferenceToCppType : ReferenceToType
	{
		public readonly bool IsGlobalNamespace;
		public readonly int PointerRank;
		public readonly ImmutableArray<ReferenceToInstance> ArrayItemNumber;    //[12][33][SomeClass.SomeConstant] -> { 12, 33, SomeClass.SomeConstant }
		public int ArrayRank => ArrayItemNumber.Length;

		public ImmutableArray<CppTypeReference> Target { get; private set; }

		public ReferenceToCppType(Builder builder)
			: base(builder.Content)
		{
			IsGlobalNamespace = builder.IsGlobalNamespace;
			PointerRank = builder.PointerRank;
			ArrayItemNumber = ImmutableArray.Create(builder.ArrayItemNumber.ToArray());
		}

		public sealed class Builder
		{
			public TokenBlock Content;
			public bool IsGlobalNamespace;
			public int PointerRank;
			public List<ReferenceToInstance> ArrayItemNumber = new List<ReferenceToInstance>();
		}

		internal override void Bind(InfoOutput infoOutput, DeclaredNamespaceOrTypeName rootNamespace, UsingBlob usings, IEnumerable<TokenIdentifier> namespaces, IEnumerable<ClassTypeDeclare> containers, IEnumerable<GenericDeclare> enclosingGenerics, Stack<ReferenceToType> resolveChain)
		{
			var result = new List<CppTypeReference.Builder>();
			int index = 0;
			var token = Content.GetToken(index++);
			while (true) {
				var builder = new CppTypeReference.Builder();

				//Name
				if (token.Type != TokenType.Identifier) {
					infoOutput.OutputError(ParseException.AsTokenBlock(Content, Error.Analysis.IdentifierExpected));
				}
				builder.Name = token.Text;

				//Template
				token = Content.GetToken(index);
				if (token == null) {
					result.Add(builder);
					Target = ImmutableArray.CreateRange(result.Select(item => new CppTypeReference(item)));
					return;
				}
				else if (token.Type == TokenType.Dot) {
					result.Add(builder);
					++index
					token = Content.GetToken(index)
				}
				else if (token.Type == TokenType.LeftArrow) {
					
				}
			}
		}
	}
}
