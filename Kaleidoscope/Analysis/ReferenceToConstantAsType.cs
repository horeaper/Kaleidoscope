namespace Kaleidoscope.Analysis
{
	public sealed class ReferenceToConstantAsType : ReferenceToConstant
	{
		public readonly ReferenceToType Type;

		public ReferenceToConstantAsType(ReferenceToType type)
			: base(type.Content)
		{
			Type = type;
		}
	}
}
