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

		public void OutputWarning(ParseWarning param)
		{
			lock (this) {
				OutputWarningWorker(param);
			}
		}

		protected abstract void OutputErrorWorker(ParseException param);
		protected abstract void OutputWarningWorker(ParseWarning param);
	}

	public abstract class OutputToText : InfoOutput
	{
		protected sealed override void OutputErrorWorker(ParseException e)
		{
			if (e.ColumnStart != e.ColumnEnd) {
				OnOutputError($"{e.SourceFile.FilePath}({e.Line},{e.ColumnStart},{e.Line},{e.ColumnEnd}): error: {e.Message}");
			}
			else {
				OnOutputError($"{e.SourceFile.FilePath}({e.Line},{e.ColumnStart}: error: {e.Message}");
			}
		}

		protected sealed override void OutputWarningWorker(ParseWarning e)
		{
			if (e.ColumnStart != e.ColumnEnd) {
				OnOutputWarning($"{e.SourceFile.FilePath}({e.Line},{e.ColumnStart},{e.Line},{e.ColumnEnd}): warning: {e.Message}");
			}
			else {
				OnOutputWarning($"{e.SourceFile.FilePath}({e.Line},{e.ColumnStart}: warning: {e.Message}");
			}
		}

		protected abstract void OnOutputError(string text);
		protected abstract void OnOutputWarning(string text);
	}
}
