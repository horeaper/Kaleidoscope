namespace Kaleidoscope
{
	public class IncludeHeaderFile
	{
		public readonly string FilePath;
		public readonly string FileContent;

		public IncludeHeaderFile(string filePath, string fileContent)
		{
			FilePath = filePath;
			FileContent = fileContent;
		}

		public override string ToString()
		{
			return "[IncludeHeaderFile] " + FilePath;
		}
	}
}
