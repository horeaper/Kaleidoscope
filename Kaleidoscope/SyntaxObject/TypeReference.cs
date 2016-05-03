using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaleidoscope.SyntaxObject.Primitive;

namespace Kaleidoscope.SyntaxObject
{
	public abstract class TypeReference
	{
		public static readonly ConcurrentBag<TypeReference> All = new ConcurrentBag<TypeReference>();

		protected TypeReference(TokenBlock content)
		{
			All.Add(this);

			Content = content;
		}

		public readonly TokenBlock Content;
	}
}
