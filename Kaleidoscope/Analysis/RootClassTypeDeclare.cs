using System.Text;

namespace Kaleidoscope.Analysis
{
	public sealed class RootClassTypeDeclare : ClassTypeDeclare
	{
		public readonly bool IsPublic;
		public override string Fullname { get; }

		public RootClassTypeDeclare(Builder builder)
			: base(builder)
		{
			IsPublic = builder.IsPublic;

			var fullname = new StringBuilder();
			foreach (var item in Namespace) {
				fullname.Append(item.Text);
				fullname.Append(".");
			}
			fullname.Append(Name.Text);
			Fullname = fullname.ToString();
		}

		public new sealed class Builder : ClassTypeDeclare.Builder
		{
			public bool IsPublic;
		}
	}
}
