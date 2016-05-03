using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaleidoscope.SyntaxObject.Primitive;

namespace Kaleidoscope.SyntaxObject
{
	/// <summary>
	/// using static System.Math;
	/// </summary>
	public sealed class UsingStaticDirective
	{
		public readonly CodeFile Source;
		public readonly TokenBlock TypeContent;
	}
}
