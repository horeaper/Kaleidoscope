using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaleidoscope.Tokenizer
{
	public enum PreprocessorType
	{
		@if,
		@else,
		@elif,
		@endif,
		@define,
		@undef,
		@warning,
		@error,
		@line,
		@region,
		@endregion,
		@pragma,
	}
}
