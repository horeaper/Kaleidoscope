using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.SyntaxObject
{
	/// <summary>
	/// Declaration of a managed object
	/// </summary>
	public abstract class ManagedDeclare
	{
		public Token Name { get; protected set; }
		public string Fullname { get; protected set; }
	}
}
