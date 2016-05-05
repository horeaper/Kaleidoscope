using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.SyntaxObject
{
	public sealed class PrimitiveTypeDeclare : ManagedDeclare
	{
		PrimitiveTypeDeclare(KeywordType keyword, string fullname)
		{
			var dummySource = new SourceTextFile("", keyword.ToString());
			Name = new TokenKeyword(dummySource, 0, dummySource.Length, keyword);
			Fullname = fullname;
		}

		public static PrimitiveTypeDeclare Void = new PrimitiveTypeDeclare(KeywordType.@void, "System.Void");
		public static PrimitiveTypeDeclare Boolean = new PrimitiveTypeDeclare(KeywordType.@bool, "System.Boolean");
		public static PrimitiveTypeDeclare Int8 = new PrimitiveTypeDeclare(KeywordType.@sbyte, "System.Int8");
		public static PrimitiveTypeDeclare UInt8 = new PrimitiveTypeDeclare(KeywordType.@byte, "System.UInt8");
		public static PrimitiveTypeDeclare Int16 = new PrimitiveTypeDeclare(KeywordType.@short, "System.Int16");
		public static PrimitiveTypeDeclare UInt16 = new PrimitiveTypeDeclare(KeywordType.@ushort, "System.UInt16");
		public static PrimitiveTypeDeclare Int32 = new PrimitiveTypeDeclare(KeywordType.@int, "System.Int32");
		public static PrimitiveTypeDeclare UInt32 = new PrimitiveTypeDeclare(KeywordType.@uint, "System.UInt32");
		public static PrimitiveTypeDeclare Int64 = new PrimitiveTypeDeclare(KeywordType.@long, "System.Int64");
		public static PrimitiveTypeDeclare UInt64 = new PrimitiveTypeDeclare(KeywordType.@ulong, "System.UInt64");
		public static PrimitiveTypeDeclare Single = new PrimitiveTypeDeclare(KeywordType.@float, "System.Single");
		public static PrimitiveTypeDeclare Double = new PrimitiveTypeDeclare(KeywordType.@double, "System.Double");
		public static PrimitiveTypeDeclare Char = new PrimitiveTypeDeclare(KeywordType.@void, "System.Char");
		//decimal, object, string 会直接引用源类型
		//IntPtr， UIntPtr, 虽然定义为PrimitiveType，但因为本身就没有alias，仍然当做源类型来处理，只是增加一个特殊的native转换规则
	}
}
