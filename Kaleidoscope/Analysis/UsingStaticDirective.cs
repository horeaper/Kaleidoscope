using System.Collections.Immutable;
using System.Text;
using Kaleidoscope.Tokenizer;

namespace Kaleidoscope.Analysis
{
	/// <summary>
	/// using static System.Math;
	/// </summary>
	public sealed class UsingStaticDirective
	{
		public readonly ImmutableArray<Token> OwnerNamespace; 
		public readonly ImmutableArray<Token> TypeContent;
		readonly string m_displayName;

		public UsingStaticDirective(Token[] ownerNamespace, Token[] typeContent)
		{
			OwnerNamespace = ImmutableArray.Create(ownerNamespace);
			TypeContent = ImmutableArray.Create(typeContent);

			var builder = new StringBuilder();
			for (int cnt = 0; cnt < TypeContent.Length; ++cnt) {
				builder.Append(TypeContent[cnt].Text);
				if (cnt < TypeContent.Length - 1) {
					builder.Append('.');
				}
			}
			m_displayName = builder.ToString();
		}

		public override string ToString()
		{
			return $"[UsingStaticDirective] using static {m_displayName};";
		}
	}
}
