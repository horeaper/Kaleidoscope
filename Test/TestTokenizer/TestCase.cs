using System.IO;
using Kaleidoscope;
using Kaleidoscope.Tokenizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestTokenizer
{
	[TestClass]
	public class TestCase
	{
		[TestMethod]
		public void TestCaseAction()
		{
			Process(@"..\..\..\TestCase\Action.cs");
		}

		[TestMethod]
		public void TestCaseDouble()
		{
			Process(@"..\..\..\TestCase\Double.cs");
		}

		[TestMethod]
		public void TestCaseEnumerable()
		{
			Process(@"..\..\..\TestCase\Enumerable.cs");
		}

		[TestMethod]
		public void TestCaseFileStream()
		{
			Process(@"..\..\..\TestCase\FileStream.cs");
		}

		[TestMethod]
		public void TestCaseGC()
		{
			Process(@"..\..\..\TestCase\GC.cs");
		}

		[TestMethod]
		public void TestCaseIComparable()
		{
			Process(@"..\..\..\TestCase\IComparable.cs");
		}

		[TestMethod]
		public void TestCaseInt32()
		{
			Process(@"..\..\..\TestCase\Int32.cs");
		}

		[TestMethod]
		public void TestCaseList()
		{
			Process(@"..\..\..\TestCase\List.cs");
		}

		[TestMethod]
		public void TestCaseObject()
		{
			Process(@"..\..\..\TestCase\Object.cs");
		}

		[TestMethod]
		public void TestCaseString()
		{
			Process(@"..\..\..\TestCase\String.cs");
		}

		[TestMethod]
		public void TestCaseValueType()
		{
			Process(@"..\..\..\TestCase\ValueType.cs");
		}

		[TestMethod]
		public void TestCaseXElement()
		{
			Process(@"..\..\..\TestCase\XElement.cs");
		}

		void Process(string filePath)
		{
			filePath = Path.GetFullPath(filePath);
			string content = File.ReadAllText(filePath);
			var source = new SourceTextFile(filePath, content);
			Tokenizer.Process(null, source, null, true);
		}
	}
}
