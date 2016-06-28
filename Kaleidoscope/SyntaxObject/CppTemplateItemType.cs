namespace Kaleidoscope.SyntaxObject
{
	public sealed class CppTemplateItemType : CppTemplateItem
	{
		public readonly CppTypeReference Target;
		public override string DisplayName => Target.DisplayName;

		public CppTemplateItemType(CppTypeReference.Builder target)
		{
			Target = new CppTypeReference(target);
		}
	}
}
