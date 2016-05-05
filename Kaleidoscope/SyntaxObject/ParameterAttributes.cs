using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaleidoscope.SyntaxObject
{
	[Flags]
	public enum ParameterAttributes
	{
		None,
		This,		// The parameter is a extension method this type
		Ref,		// The parameter is a pass-by-ref parameter
		Out,        // The parameter is an output parameter
		Const,		// The parameter is marked as const
		Variable,	// The parameter is used as variable (params keyword) input
	}
}
