namespace Kaleidoscope
{
	public class SourceTextFile
	{
		public readonly string FilePath;
		public readonly string FileContent;

		public SourceTextFile(string filePath, string fileContent)
		{
			FilePath = filePath;
			FileContent = fileContent;
		}

		public override string ToString()
		{
			return "[SourceTextFile] " + FilePath;
		}
	}
}
