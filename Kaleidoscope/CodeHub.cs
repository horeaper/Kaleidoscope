﻿using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using Kaleidoscope.Analysis;
using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope
{
	public sealed class CodeHub
	{
		public Configuration Configuration { get; }
		public InfoOutput InfoOutput { get; }

		public readonly ImmutableArray<CodeFile> AnalyzedFiles;

		public CodeHub(Configuration config, InfoOutput infoOutput)
		{
			if (infoOutput == null) {
				throw new ArgumentNullException(nameof(infoOutput));
			}
			Configuration = config;
			InfoOutput = infoOutput;

			//Analysis - Get code file structure
			var errorList = new ConcurrentBag<ParseException>();
			var codeFiles = new ConcurrentBag<CodeFile>();
			Parallel.ForEach(Configuration.InputFiles, file => {
				try {
					string extension = Path.GetExtension(file.FileName);
					if (string.Compare(extension, ".cs", StringComparison.CurrentCultureIgnoreCase) == 0) {
						var tokens = Tokenizer.Tokenizer.Process(InfoOutput, file, Configuration.DefinedSymbols, false, false);
						codeFiles.Add(new CodeFile(InfoOutput, new TokenBlock(tokens), LanguageType.CS));
					}
					else if (string.Compare(extension, ".cfs", StringComparison.CurrentCultureIgnoreCase) == 0) {
						var tokens = Tokenizer.Tokenizer.Process(InfoOutput, file, Configuration.DefinedSymbols, true, false);
						codeFiles.Add(new CodeFile(InfoOutput, new TokenBlock(tokens), LanguageType.CFS));
					}
				}
				catch (ParseException e) {
					errorList.Add(e);
				}
			});
			CheckErrorList(ref errorList);
			AnalyzedFiles = ImmutableArray.CreateRange(codeFiles);

			//Extract - Get all declared types

			//Bind - Resolve all ReferenceToType

			//Arrange - Combine partials

			//Generate - Output transpiled result
		}

#region Utility

		void CheckErrorList(ref ConcurrentBag<ParseException> errorList)
		{
			if (errorList == null) {
				throw new ArgumentNullException(nameof(errorList));
			}
			foreach (var error in errorList) {
				InfoOutput.OutputError(error);
			}
			errorList = new ConcurrentBag<ParseException>();
		}

#endregion
	}
}
