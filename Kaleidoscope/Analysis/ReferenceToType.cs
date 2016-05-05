using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public abstract class ReferenceToType
	{
		public readonly TokenBlock Content;

		protected ReferenceToType(TokenBlock content)
		{
			Content = content;
		}
	}
}
