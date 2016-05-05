using System.Collections.Generic;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	class CodeFileAnalysisCS : CodeFileAnalysis
	{
		CodeHub hub;
		AnalyzedFile codeFile;
		TokenBlock block;

		public CodeFileAnalysisCS(CodeHub hub, AnalyzedFile codeFile, TokenBlock block)
		{
			this.hub = hub;
			this.codeFile = codeFile;
			this.block = block;

			int index = 0;
			var usings = new UsingBlob.Builder();
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
						var content = block.ReadPastSpecificToken(ref index, TokenType.Semicolon, Error.Analysis.SemicolonExpected);
						
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

						}
					}
				}
			}
		}
	}
}
