using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Kaleidoscope
{
	class Program
	{
		const string CommandLineHelpString = @"Kaleidoscope Transpiler (alpha)
usage: kalc [options] <input_files...>

Options:
  -h                Display this help info
  -D <SYMBOL>       Define conditional compilation symbol
  -o <folder>       Set output directory. If not specified, the file will be generated in current directory
  -debug            Set transpiler's output to debug mode
  -inc <folder>     Specify C++ header files search path
  -header <file>    Include additional C++ header file
  -clang <params>   Set clang's parse parameters

Internal options:
  -mini             Enable mini mode (don't include standard libraries)
  -i <folder>       Include all *.cs files in this folder to process
";

		class OpenedFile
		{
			public string FilePath;
			public string FileContent;
		}

		static void WriteParamError(string text)
		{
			Console.WriteLine("kalc: " + text);
		}

		static Configuration ReadArgumentsFromRspFile(string filePath)
		{
			try {
				return ProcessCommandArguments(File.ReadAllLines(filePath));
			}
			catch (IOException) {
				WriteParamError("Cannot open input file: " + filePath);
				return null;
			}
		}

		static Configuration ProcessCommandArguments(string[] args)
		{
			var config = new Configuration();
			var inputFiles = new List<string>();
			var definedSymbols = new SortedSet<string>();
			var includeSearchPaths = new List<string>();
			var additionalIncludeFiles = new List<string>();

			//Parse parameters
			try {
				int currentIndex = 0;
				while (true) {
					switch (args[currentIndex]) {
						case "-h":
							Console.WriteLine(CommandLineHelpString);
							return null;
						case "-D":
							++currentIndex;
							definedSymbols.Add(args[currentIndex]);
							break;
						case "-o":
							++currentIndex;
							config.OutputFolder = args[currentIndex];
							break;
						case "-debug":
							config.IsDebugMode = true;
							definedSymbols.Add("DEBUG");
							break;
						case "-inc":
							++currentIndex;
							if (Directory.Exists(args[currentIndex])) {
								includeSearchPaths.Add(args[currentIndex]);
							}
							else {
								WriteParamError("cannot find include directory: " + args[currentIndex]);
								return null;
							}
							break;
						case "-header":
							++currentIndex;
							additionalIncludeFiles.Add(args[currentIndex]);
							break;
						case "-clang":
							++currentIndex;
							config.ClangParseParameters = args[currentIndex]; 
							break;
						case "-mini":
							config.IsMiniMode = true;
							break;
						case "-i":
							++currentIndex;
							if (Directory.Exists(args[currentIndex])) {
								inputFiles.AddRange(Directory.GetFiles(args[currentIndex], "*.cs", SearchOption.AllDirectories));
							}
							else {
								WriteParamError("cannot find include directory: " + args[currentIndex]);
								return null;
							}
							break;
						default:
							inputFiles.Add(args[currentIndex]);
							break;
					}

					++currentIndex;
					if (currentIndex >= args.Length) {
						break;
					}
				}
			}
			catch (IndexOutOfRangeException) {
				WriteParamError("invalid arguments, type -h to see usage information");
				return null;
			}

			//Read file
			Func<string, string> ReadFileContent = file => {
				try {
					using (var fileStream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
					using (var reader = new StreamReader(fileStream)) {
						return reader.ReadToEnd();
					}
				}
				catch (IOException) {
					return null;
				}
			};
			Func<List<string>, List<OpenedFile>> ProcessFileList = files => {
				var openedFiles = new List<OpenedFile>();
				foreach (var file in files) {
					if (!File.Exists(file)) {
						WriteParamError("cannot find input file: " + file);
						return null;
					}

					string content = ReadFileContent(file);
					if (content == null) {
						WriteParamError("cannot read input file: " + file);
						return null;
					}
					openedFiles.Add(new OpenedFile {
						FilePath = file,
						FileContent = content,
					});
				}
				return openedFiles;
			};
			var openedInputFiles = ProcessFileList(inputFiles);
			var openedIncludeFiles = ProcessFileList(additionalIncludeFiles);
			if (openedInputFiles == null || openedIncludeFiles == null) {
				return null;
			}

			//Set Configuration info
			config.DefinedSymbols = definedSymbols.ToImmutableSortedSet();
			config.IncludeSearchPaths = includeSearchPaths.ToImmutableArray();
			config.AdditionalIncludeFiles = openedIncludeFiles.Select(item => new IncludeHeaderFile(item.FilePath, item.FileContent)).ToImmutableArray();
			config.InputFiles = openedInputFiles.Select(item => new SourceTextFile(Path.GetFullPath(item.FilePath), item.FileContent)).ToImmutableArray();
			return config;
		}

		static int Main(string[] args)
		{
			if (args.Length == 0) {
				Console.WriteLine(CommandLineHelpString);
				return -1;
			}

			var config = args.Length == 1 && args[0][0] == '@'
				? ReadArgumentsFromRspFile(args[0])
				: ProcessCommandArguments(args);
			if (config == null) {
				return -1;
			}

			try {
				var codeHub = new CodeHub(config, new OutputToConsole());
			}
			catch (KaleidoscopeSystemException) {
				return -1;
			}

			return 0;
		}
	}
}
