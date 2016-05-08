using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	/// <summary>
	/// Declare of a managed object
	/// </summary>
	public abstract class ManagedDeclare
	{
		public readonly TokenIdentifier Name;
		public abstract string Fullname { get; }

		protected ManagedDeclare(TokenIdentifier name)
		{
			Name = name;
		}
	}
}
