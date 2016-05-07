using System.Collections.Generic;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	class CodeFileAnalysisCS : CodeFileAnalysis
	{
		IInfoOutput infoOutput;
		AnalyzedFile codeFile;
		TokenBlock block;

		public CodeFileAnalysisCS(IInfoOutput infoOutput, AnalyzedFile codeFile, TokenBlock block)
		{
			this.infoOutput = infoOutput;
			this.codeFile = codeFile;
			this.block = block;

			int index = 0;
			var currentUsings = new UsingBlob.Builder();
			var currentNamespace = new List<Token>();
			while (true) {
				var token = block.GetToken(index);
				if (token == null) {
					return;
				}

				if (token.Type == TokenType.@using) {
					++index;
					token = block.GetToken(index);

					if (token.Type == TokenType.@static) {
						++index;
						var typeContent = block.ReadPastSpecificToken(ref index, TokenType.Semicolon, Error.Analysis.SemicolonExpected);
						currentUsings.UsingStaticDirectives.Add(UsingReader.ReadStatic(currentNamespace.ToArray(), typeContent));
						continue;
					}

					bool isCppType = false;
					if (token.Type == TokenType.Identifier && token.Text == "cpp") {
						token = block.GetToken(index + 1, Error.Analysis.SemicolonExpected);
						if (token.Type == TokenType.DoubleColon) {
							isCppType = true;
							index += 2;
						}
					}

					var content = block.ReadPastSpecificToken(ref index, TokenType.Semicolon, Error.Analysis.SemicolonExpected);
					if (content.FindToken(0, TokenType.Assign) == -1) {
						if (!isCppType) {
							currentUsings.UsingCSNamespaceDirectives.Add(UsingReader.ReadCSNamespace(currentNamespace.ToArray(), content));
						}
						else {
							currentUsings.UsingCppNamespaceDirectives.Add(UsingReader.ReadCppNamespace(currentNamespace.ToArray(), content));
						}
					}
					else {
						if (!isCppType) {
							currentUsings.UsingCSAliasDirectives.Add(UsingReader.ReadCSAlias(currentNamespace.ToArray(), content));
						}
						else {
							currentUsings.UsingCppAliasDirectives.Add(UsingReader.ReadCppAlias(currentNamespace.ToArray(), content));
						}
					}
				}
			}
		}
	}
}
