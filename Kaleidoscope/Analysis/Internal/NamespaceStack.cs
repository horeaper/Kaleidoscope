using System.Collections.Generic;
using System.Collections.Immutable;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.Internal
{
	class NamespaceStack
	{
		readonly List<TokenIdentifier> m_tokens = new List<TokenIdentifier>();
		readonly Stack<int> m_next = new Stack<int>();

		public void Push(IEnumerable<TokenIdentifier> @namespace)
		{
			m_next.Push(m_tokens.Count);
			m_tokens.AddRange(@namespace);
		}

		public void Pop()
		{
			int newIndex = m_next.Pop();
			m_tokens.RemoveRange(newIndex, m_tokens.Count - newIndex);
		}

		public ImmutableArray<TokenIdentifier>.Builder Get()
		{
			var builder = ImmutableArray.CreateBuilder<TokenIdentifier>(m_tokens.Count);
			builder.AddRange(m_tokens);
			return builder;
		}
	}
}
