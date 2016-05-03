using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaleidoscope.Analysis;
using Kaleidoscope.Structure;

namespace Kaleidoscope
{
	public partial class CodeHub
	{
		public Configuration Configuration { get; }
		public IInfoOutput InfoOutput { get; }

		public readonly ImmutableArray<CodeFile> CodeFiles;

		public CodeHub(Configuration config, IInfoOutput infoOutput)
		{
			Configuration = config;
			InfoOutput = infoOutput;

			var errorList = new ConcurrentBag<ParseException>();
			var codeFiles = new ConcurrentBag<CodeFile>();
			Parallel.ForEach(Configuration.InputFiles, file => {
				try {
					var tokens = Tokenizer.Tokenizer.Process(InfoOutput, file, Configuration.DefinedSymbols);
					codeFiles.Add(new CodeFile(new TokenBlob(tokens)));
				}
				catch (ParseException e) {
					errorList.Add(e);
				}
			});
			CheckErrorList(errorList);

			CodeFiles = ImmutableArray.CreateRange(codeFiles);
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
