using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	/// <summary>
	/// Declare of a managed object
	/// </summary>
	public abstract class ManagedDeclare
	{
		public readonly TokenIdentifier Name;

		protected ManagedDeclare(TokenIdentifier name)
		{
			Name = name;
		}
	}
}
