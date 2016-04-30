using System.Collections.Immutable;
using Kaleidoscope.Primitive;

namespace Kaleidoscope
{
	public class Configuration
	{
		public bool IsMiniMode = false;
		public bool IsDebugMode = false;
		public string OutputFolder = null;

		public ImmutableSortedSet<string> DefinedSymbols;
		public ImmutableArray<string> IncludeSearchPaths;
		public ImmutableArray<IncludeHeaderFile> AdditionalIncludeFiles;
		public string ClangParseParameters = null;

		public ImmutableArray<SourceTextFile> InputFiles;
	}
}
