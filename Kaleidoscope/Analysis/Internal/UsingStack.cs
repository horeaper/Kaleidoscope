using System.Collections.Generic;
using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis.Internal
{
	class UsingStack
	{
		readonly Stack<UsingBlob.Builder> m_stack = new Stack<UsingBlob.Builder>();

		public UsingStack()
		{
			m_stack.Push(new UsingBlob.Builder());
		}

		public void Push()
		{
			m_stack.Push(new UsingBlob.Builder());
		}

		public UsingBlob.Builder Peek()
		{
			return m_stack.Peek();
		}

		public void Pop()
		{
			m_stack.Pop();
		}
	}
}
