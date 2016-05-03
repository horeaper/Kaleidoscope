using Kaleidoscope.SyntaxObject.Primitive;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.SyntaxObject
{
	/// <summary>
	/// using LookupTable = System.Collections.Generic.Dictionary&lt;int, string&gt;;
	/// Col = System.Collections.Generic;
	/// </summary>
	public sealed class UsingCSAliasDirective
	{
		public readonly CodeFile Source;
		public readonly Token Name;
		public readonly TokenBlock TypeContent;

		internal UsingCSAliasDirective(CodeFile source, Token name, TokenBlock typeContent)
		{
			Source = source;
			Name = name;
			TypeContent = typeContent;
		}
	}
}
