using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaleidoscope.Analysis
{
	[Flags]
	public enum TypeParsingRule
	{
		None         = 0,
		AllowVoid    = 1 << 0,
		AllowVar     = 1 << 1,
		AllowCppType = 1 << 2,
		AllowArray   = 1 << 3,
	}
}
