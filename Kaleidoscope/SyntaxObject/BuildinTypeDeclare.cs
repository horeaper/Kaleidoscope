using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.SyntaxObject
{
	public sealed class BuildinTypeDeclare : ManagedDeclare
	{
		BuildinTypeDeclare(KeywordType keyword, string fullname)
		{
			var dummySource = new SourceTextFile("", keyword.ToString());
			Name = new TokenKeyword(dummySource, 0, dummySource.Length, keyword);
			Fullname = fullname;
		}

		public static BuildinTypeDeclare Void = new BuildinTypeDeclare(KeywordType.@void, "System.Void");
		public static BuildinTypeDeclare Boolean = new BuildinTypeDeclare(KeywordType.@bool, "System.Boolean");
		public static BuildinTypeDeclare Int8 = new BuildinTypeDeclare(KeywordType.@sbyte, "System.Int8");
		public static BuildinTypeDeclare UInt8 = new BuildinTypeDeclare(KeywordType.@byte, "System.UInt8");
		public static BuildinTypeDeclare Int16 = new BuildinTypeDeclare(KeywordType.@short, "System.Int16");
		public static BuildinTypeDeclare UInt16 = new BuildinTypeDeclare(KeywordType.@ushort, "System.UInt16");
		public static BuildinTypeDeclare Int32 = new BuildinTypeDeclare(KeywordType.@int, "System.Int32");
		public static BuildinTypeDeclare UInt32 = new BuildinTypeDeclare(KeywordType.@uint, "System.UInt32");
		public static BuildinTypeDeclare Int64 = new BuildinTypeDeclare(KeywordType.@long, "System.Int64");
		public static BuildinTypeDeclare UInt64 = new BuildinTypeDeclare(KeywordType.@ulong, "System.UInt64");
		public static BuildinTypeDeclare Single = new BuildinTypeDeclare(KeywordType.@float, "System.Single");
		public static BuildinTypeDeclare Double = new BuildinTypeDeclare(KeywordType.@double, "System.Double");
		public static BuildinTypeDeclare Char = new BuildinTypeDeclare(KeywordType.@void, "System.Char");
	}
}
