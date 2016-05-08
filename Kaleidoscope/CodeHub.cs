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
	public class CodeHub
	{
		public Configuration Configuration { get; }
		public IInfoOutput InfoOutput { get; }

		public readonly ImmutableArray<AnalyzedFile> AnalyzedFiles;

		public CodeHub(Configuration config, IInfoOutput infoOutput)
		{
			Configuration = config;
			InfoOutput = infoOutput;

			//Analysis - Get code file structure
			if (config.IsVerboseMode) {
				infoOutput?.OutputVerbose("Stage - Analysis");
			}
			var errorList = new ConcurrentBag<ParseException>();
			var codeFiles = new ConcurrentBag<AnalyzedFile>();
			Parallel.ForEach(Configuration.InputFiles, file => {
				try {
					string extension = Path.GetExtension(file.FileName);
					if (string.Compare(extension, ".cs", StringComparison.CurrentCultureIgnoreCase) == 0) {
						var tokens = Tokenizer.Tokenizer.Process(InfoOutput, file, Configuration.DefinedSymbols, false, false);
						codeFiles.Add(new AnalyzedFile(new TokenBlock(tokens), LanguageType.CS));
					}
					else if (string.Compare(extension, ".cfs", StringComparison.CurrentCultureIgnoreCase) == 0) {
						var tokens = Tokenizer.Tokenizer.Process(InfoOutput, file, Configuration.DefinedSymbols, true, false);
						codeFiles.Add(new AnalyzedFile(new TokenBlock(tokens), LanguageType.CFS));
					}
				}
				catch (ParseException e) {
					errorList.Add(e);
				}
			});
			CheckErrorList(errorList);
			AnalyzedFiles = ImmutableArray.CreateRange(codeFiles);

			//Extract - Get all declared types

			//Bind - Resolve all ReferenceToType
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
