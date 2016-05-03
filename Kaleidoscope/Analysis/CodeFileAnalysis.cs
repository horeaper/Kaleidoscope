using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.SyntaxObject.Primitive;

namespace Kaleidoscope.Analysis
{
	public abstract class CodeFileAnalysis
	{
		public readonly List<UsingCSNamespaceDirective> UsingCSNamespaceDirectives = new List<UsingCSNamespaceDirective>();
	}
}
