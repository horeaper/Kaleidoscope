using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public abstract class ReferenceToConstant
	{
		public readonly TokenBlock Content;
		public string Text => Content.Text;

		protected ReferenceToConstant(TokenBlock content)
		{
			Content = content;
		}
	}
}
