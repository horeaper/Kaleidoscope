using System.Collections.Generic;
using System.Collections.Immutable;
using Kaleidoscope.Analysis;

namespace Kaleidoscope.SyntaxObject
{
	/// <summary>
	/// A group of usings
	/// </summary>
	public sealed class UsingBlob
	{
		public readonly UsingBlob Enclosing;
		public readonly ImmutableArray<UsingCSNamespaceDirective> UsingCSNamespaceDirectives;
		public readonly ImmutableArray<UsingCSAliasDirective> UsingCSAliasDirectives;
		public readonly ImmutableArray<UsingCppNamespaceDirective> UsingCppNamespaceDirectives;
		public readonly ImmutableArray<UsingCppAliasDirective> UsingCppAliasDirectives;
		public readonly ImmutableArray<UsingStaticDirective> UsingStaticDirectives;

		public UsingBlob(Builder builder)
		{
			Enclosing = new UsingBlob(builder.Enclosing);
			UsingCSNamespaceDirectives = ImmutableArray.CreateRange(builder.UsingCSNamespaceDirectives);
			UsingCSAliasDirectives = ImmutableArray.CreateRange(builder.UsingCSAliasDirectives);
			UsingCppNamespaceDirectives = ImmutableArray.CreateRange(builder.UsingCppNamespaceDirectives);
			UsingCppAliasDirectives = ImmutableArray.CreateRange(builder.UsingCppAliasDirectives);
			UsingStaticDirectives = ImmutableArray.CreateRange(builder.UsingStaticDirectives);
		}

		public sealed class Builder
		{
			public Builder Enclosing;
			public readonly List<UsingCSNamespaceDirective> UsingCSNamespaceDirectives = new List<UsingCSNamespaceDirective>();
			public readonly List<UsingCSAliasDirective> UsingCSAliasDirectives = new List<UsingCSAliasDirective>();
			public readonly List<UsingCppNamespaceDirective> UsingCppNamespaceDirectives = new List<UsingCppNamespaceDirective>();
			public readonly List<UsingCppAliasDirective> UsingCppAliasDirectives = new List<UsingCppAliasDirective>();
			public readonly List<UsingStaticDirective> UsingStaticDirectives = new List<UsingStaticDirective>();
		}
	}
}
