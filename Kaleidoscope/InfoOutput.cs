namespace Kaleidoscope
{
	public abstract class InfoOutput
	{
		public bool IsError { get; private set; }

		public void OutputError(ParseException param)
		{
			lock (this) {
				IsError = true;
				OutputErrorWorker(param);
			}
		}

		public void OutputWarning(ParseException param)
		{
			lock (this) {
				OutputWarningWorker(param);
			}
		}

		public void OutputMessage(ParseException param)
		{
			lock (this) {
				OutputMessageWorker(param);
			}
		}

		protected abstract void OutputErrorWorker(ParseException param);
		protected abstract void OutputWarningWorker(ParseException param);
		protected abstract void OutputMessageWorker(ParseException param);
	}

	public abstract class OutputToText : InfoOutput
	{
		protected sealed override void OutputErrorWorker(ParseException e)
		{
			if (e.ErrorColumnStart != e.ErrorColumnEnd) {
				OnOutputError($"{e.SourceFile.FilePath}({e.ErrorLine},{e.ErrorColumnStart},{e.ErrorLine},{e.ErrorColumnEnd}): error: {e.Message}");
			}
			else {
				OnOutputError($"{e.SourceFile.FilePath}({e.ErrorLine},{e.ErrorColumnStart}: error: {e.Message}");
			}
		}

		protected sealed override void OutputWarningWorker(ParseException e)
		{
			if (e.ErrorColumnStart != e.ErrorColumnEnd) {
				OnOutputWarning($"{e.SourceFile.FilePath}({e.ErrorLine},{e.ErrorColumnStart},{e.ErrorLine},{e.ErrorColumnEnd}): warning: {e.Message}");
			}
			else {
				OnOutputWarning($"{e.SourceFile.FilePath}({e.ErrorLine},{e.ErrorColumnStart}: warning: {e.Message}");
			}
		}

		protected sealed override void OutputMessageWorker(ParseException e)
		{
			if (e.ErrorColumnStart != e.ErrorColumnEnd) {
				OnOutputMessage($"{e.SourceFile.FilePath}({e.ErrorLine},{e.ErrorColumnStart},{e.ErrorLine},{e.ErrorColumnEnd}): message: {e.Message}");
			}
			else {
				OnOutputMessage($"{e.SourceFile.FilePath}({e.ErrorLine},{e.ErrorColumnStart}: message: {e.Message}");
			}
		}

		protected abstract void OnOutputError(string text);
		protected abstract void OnOutputWarning(string text);
		protected abstract void OnOutputMessage(string text);
	}
}
