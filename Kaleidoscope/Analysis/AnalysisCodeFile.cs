using System.Linq;
using System.Collections.Generic;

namespace Kaleidoscope.Analysis
{
	public abstract class AnalysisCodeFile
	{
		public readonly List<RootTypeDeclare<ClassTypeDeclare>.Builder> DefinedClasses = new List<RootTypeDeclare<ClassTypeDeclare>.Builder>();
		public readonly List<RootTypeDeclare<InterfaceTypeDeclare>.Builder> DefinedInterfaces = new List<RootTypeDeclare<InterfaceTypeDeclare>.Builder>();
		public readonly List<RootTypeDeclare<EnumTypeDeclare>.Builder> DefinedEnums = new List<RootTypeDeclare<EnumTypeDeclare>.Builder>();
		public readonly List<RootTypeDeclare<DelegateTypeDeclare>.Builder> DefinedDelegates = new List<RootTypeDeclare<DelegateTypeDeclare>.Builder>();
	}
}
