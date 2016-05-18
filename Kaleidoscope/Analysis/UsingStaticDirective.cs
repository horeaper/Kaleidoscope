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
		public readonly ImmutableArray<TokenIdentifier> OwnerNamespace; 
		public readonly ReferenceToManagedType Type;

		public UsingStaticDirective(ImmutableArray<TokenIdentifier>.Builder ownerNamespace, ReferenceToManagedType type)
		{
			OwnerNamespace = ownerNamespace.MoveToImmutable();
			Type = type;
		}

		public override string ToString()
		{
			return $"[UsingStaticDirective] using static {Type.Text};";
		}
	}
}
