using System.Collections.Immutable;

namespace Kaleidoscope.Primitive
{
	public class SourceTextFile
	{
		ImmutableArray<ushort> m_lineNumbers;
		ImmutableArray<int> m_lineStartIndex; 

		public readonly string FilePath;
		public readonly string FileContent;

		public SourceTextFile(string filePath, string fileContent)
		{
			FilePath = filePath;
			FileContent = fileContent;

			//Calculate line number for each character, and each line's start index
			var lineNumbers = ImmutableArray.CreateBuilder<ushort>(FileContent.Length);
			var lineStartIndex = ImmutableArray.CreateBuilder<int>();
			lineStartIndex.Add(0);
			ushort currentLine = 1;
			for (int cnt = 0; cnt < FileContent.Length; ++cnt) {
				lineNumbers.Add(currentLine);
				if (FileContent[cnt] == '\n') {
					++currentLine;
					lineStartIndex.Add(cnt + 1);
				}
			}
			m_lineNumbers = lineNumbers.MoveToImmutable();
			m_lineStartIndex = lineStartIndex.MoveToImmutable();
		}

		public char this[int index] => FileContent[index];

		public override string ToString()
		{
			return "[SourceTextFile] " + FilePath;
		}

		public int GetLine(int index)
		{
			return m_lineNumbers[index];
		}

		public void GetLineColumn(int index, out int line, out int column)
		{
			line = m_lineNumbers[index];
			column = index - m_lineStartIndex[line];
		}
	}
}
