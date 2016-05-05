using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using Kaleidoscope.Analysis;
using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope
{
	public partial class CodeHub
	{
		public Configuration Configuration { get; }
		public IInfoOutput InfoOutput { get; }

		public readonly ImmutableArray<AnalyzedFile> AnalyzedFiles;

		public CodeHub(Configuration config, IInfoOutput infoOutput)
		{
			Configuration = config;
			InfoOutput = infoOutput;

			//Analysis
			var errorList = new ConcurrentBag<ParseException>();
			var codeFiles = new ConcurrentBag<AnalyzedFile>();
			Parallel.ForEach(Configuration.InputFiles, file => {
				try {
					if (string.Compare(Path.GetExtension(file.FileName), ".cs", StringComparison.CurrentCultureIgnoreCase) == 0) {
						var tokens = Tokenizer.Tokenizer.Process(InfoOutput, file, Configuration.DefinedSymbols, true);
						codeFiles.Add(new AnalyzedFile(this, new TokenBlock(tokens), LanguageType.CS));
					}
				}
				catch (ParseException e) {
					errorList.Add(e);
				}
			});
			CheckErrorList(errorList);
			AnalyzedFiles = ImmutableArray.CreateRange(codeFiles);

			//Merging

			//Binding
		}

#region Utility

		void CheckErrorList(IEnumerable<ParseException> errorList)
		{
			if (errorList == null) {
				throw new ArgumentNullException(nameof(errorList));
			}

			bool isErrorExist = false;
			foreach (var error in errorList) {
				InfoOutput?.OutputError(error);
				isErrorExist = true;
			}
			if (isErrorExist) {
				throw new KaleidoscopeSystemException();
			}
		}

#endregion
	}
}
