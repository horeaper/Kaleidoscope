using System.IO;
using Kaleidoscope;
using Kaleidoscope.Primitive;
using Kaleidoscope.Tokenizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestTokenizer
{
	[TestClass]
	public class TestCase
	{
		[TestMethod]
		public void TestCase1()
		{
			Process(@"..\..\..\TestCase\Int32.cs");
		}

		[TestMethod]
		public void TestCase2()
		{
			Process(@"..\..\..\TestCase\Double.cs");
		}

		[TestMethod]
		public void TestCase3()
		{
			Process(@"..\..\..\TestCase\Enumerable.cs");
		}

		[TestMethod]
		public void TestCase4()
		{
			Process(@"..\..\..\TestCase\FileStream.cs");
		}

		[TestMethod]
		public void TestCase5()
		{
			Process(@"..\..\..\TestCase\GC.cs");
		}

		[TestMethod]
		public void TestCase6()
		{
			Process(@"..\..\..\TestCase\IComparable.cs");
		}

		[TestMethod]
		public void TestCase7()
		{
			Process(@"..\..\..\TestCase\Int32.cs");
		}

		[TestMethod]
		public void TestCase8()
		{
			Process(@"..\..\..\TestCase\List.cs");
		}

		[TestMethod]
		public void TestCase9()
		{
			Process(@"..\..\..\TestCase\Object.cs");
		}

		void Process(string filePath)
		{
			filePath = Path.GetFullPath(filePath);
			string content = File.ReadAllText(filePath);
			var source = new SourceTextFile(filePath, content);
			try {
				var result = Tokenizer.Process(source, null);
			}
			catch (ParseException e) {
				e = e;
				throw;
			}
		}
	}
}
