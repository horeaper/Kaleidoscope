using System.Collections.Generic;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis.CS
{
	partial class AnalysisCodeFileCS
	{
		void CheckEmpty(List<AttributeObject.Builder> currentAttributes)
		{
			if (currentAttributes.Count > 0) {
				infoOutput.OutputError(ParseException.AsTokenBlock(currentAttributes[currentAttributes.Count - 1].Type.Content, Error.Analysis.InvalidAttributeUsage));
				currentAttributes.Clear();
			}
		}

		void CheckDuplicate(Token existingToken, Token token)
		{
			if (existingToken != null) {
				infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.DuplicatedModifier));
			}
		}

		void CheckConflict(Token lastToken, Token token)
		{
			if (lastToken != null) {
				infoOutput.OutputError(ParseException.AsToken(token, Error.Analysis.ConflictModifier));
			}
		}

		void CheckInconsistent(params Token[] tokens)
		{
			foreach (var item in tokens) {
				if (item != null) {
					infoOutput.OutputError(ParseException.AsToken(item, Error.Analysis.InconsistentModifierOrder));
				}
			}
		}

		void CheckInvalid(params Token[] tokens)
		{
			foreach (var item in tokens) {
				if (item != null) {
					infoOutput.OutputError(ParseException.AsToken(item, Error.Analysis.InvalidModifier));
				}
			}
		}
	}
}
