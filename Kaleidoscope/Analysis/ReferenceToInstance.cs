using Kaleidoscope.SyntaxObject;

namespace Kaleidoscope.Analysis
{
	public sealed class ReferenceToInstance
	{
		public readonly TokenBlock Content;
		public readonly bool IsConstantOnly;

		public ReferenceToInstance(TokenBlock content, bool isConstantOnly)
		{
			Content = content;
			IsConstantOnly = isConstantOnly;
		}
	}
}
