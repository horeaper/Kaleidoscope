using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	partial class CodeFileAnalysisCS
	{
		void CheckConflict(Token lastToken, Token token)
		{
			if (lastToken != null) {
				throw ParseException.AsToken(token, Error.Analysis.ConflictModifier);
			}
		}

		void CheckInconsistent(params Token[] tokens)
		{
			foreach (var item in tokens) {
				if (item != null) {
					throw ParseException.AsToken(item, Error.Analysis.InconsistentModifierOrder);
				}
			}
		}

	}
}
