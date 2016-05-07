using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public abstract class ReferenceToType
	{
		public readonly TokenBlock Content;
		public string Text => Content.Text;

		protected ReferenceToType(TokenBlock content)
		{
			Content = content;
		}
	}
}
