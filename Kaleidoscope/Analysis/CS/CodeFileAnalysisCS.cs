using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaleidoscope.SyntaxObject;
using Kaleidoscope.SyntaxObject.Primitive;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	class CodeFileAnalysisCS : CodeFileAnalysis
	{
		CodeHub hub;
		CodeFile codeFile;
		TokenBlock block;

		public CodeFileAnalysisCS(CodeHub hub, CodeFile codeFile, TokenBlock block)
		{
			this.hub = hub;
			this.codeFile = codeFile;
			this.block = block;

			int index = 0;
			while (true) {
				var token = block.GetToken(index);
				if (token == null) {
					return;
				}

				if (token.TokenType == TokenType.@using) {
					++index;
					token = block.GetToken(index);

					if (token.TokenType == TokenType.@static) {

					}
				}
			}
		}
	}
}
