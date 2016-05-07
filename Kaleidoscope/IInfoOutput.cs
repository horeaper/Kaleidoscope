using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaleidoscope
{
	public interface IInfoOutput
	{
		void OutputError(ParseException param);
		void OutputWarning(ParseException param);
		void OutputMessage(ParseException param);
		void OutputVerbose(string content);
	}

	public abstract class DefaultInfoOutput : IInfoOutput
	{
		public void OutputError(ParseException e)
		{
			if (e.ErrorColumnStart != e.ErrorColumnEnd) {
				OnOutputError($"{e.SourceFile.FilePath}({e.ErrorLine},{e.ErrorColumnStart},{e.ErrorLine},{e.ErrorColumnEnd}): error: {e.Message}");
			}
			else {
				OnOutputError($"{e.SourceFile.FilePath}({e.ErrorLine},{e.ErrorColumnStart}: error: {e.Message}");
			}
		}

		public void OutputWarning(ParseException e)
		{
			if (e.ErrorColumnStart != e.ErrorColumnEnd) {
				OnOutputWarning($"{e.SourceFile.FilePath}({e.ErrorLine},{e.ErrorColumnStart},{e.ErrorLine},{e.ErrorColumnEnd}): warning: {e.Message}");
			}
			else {
				OnOutputWarning($"{e.SourceFile.FilePath}({e.ErrorLine},{e.ErrorColumnStart}: warning: {e.Message}");
			}
		}

		public void OutputMessage(ParseException e)
		{
			if (e.ErrorColumnStart != e.ErrorColumnEnd) {
				OnOutputMessage($"{e.SourceFile.FilePath}({e.ErrorLine},{e.ErrorColumnStart},{e.ErrorLine},{e.ErrorColumnEnd}): message: {e.Message}");
			}
			else {
				OnOutputMessage($"{e.SourceFile.FilePath}({e.ErrorLine},{e.ErrorColumnStart}: message: {e.Message}");
			}
		}

		public void OutputVerbose(string content)
		{
			OnOutputVerbose(content);
		}

		public abstract void OnOutputError(string text);
		public abstract void OnOutputWarning(string text);
		public abstract void OnOutputMessage(string text);
		public abstract void OnOutputVerbose(string text);
	}
}
