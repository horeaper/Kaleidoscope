namespace Kaleidoscope.Analysis
{
	public sealed class DestructorDeclare : MethodDeclare
	{
		readonly string m_displayName;

		public DestructorDeclare(Builder builder, InstanceTypeDeclare owner)
			: base(builder, owner)
		{
			m_displayName = $"[Destructor] ~{Name.Text}()";
		}

		public override string ToString()
		{
			return m_displayName;
		}

		public new sealed class Builder : MethodDeclare.Builder
		{
		}
	}
}
