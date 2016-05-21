using System.Collections.Generic;

namespace Kaleidoscope.Analysis
{
	public abstract class AnalysisCodeFile
	{
		public readonly List<RootClassTypeDeclare> DefinedClasses = new List<RootClassTypeDeclare>();
	}
}
