using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaleidoscope.SyntaxObject.Primitive;

namespace Kaleidoscope.SyntaxObject
{
	/// <summary>
	/// [SomeAttribute("Parameter")]
	/// </summary>
	public sealed class AttributeObject
	{
		public readonly ManagedTypeReference Type;
		public readonly TokenBlock ConstructContent;

		public string Text => $"{Type.Content.Text}({ConstructContent.Text})";

		AttributeObject(ManagedTypeReference type, TokenBlock content)
		{
			Type = type;
			ConstructContent = content;
		}

		public override string ToString()
		{
			return "[AttributeUsage] " + Text;
		}
	}
}
