using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TestHelper;

namespace Fritz.Analyzers.Test.DependencyInjectionNaming.AddPrefix
{

	[TestFixture]
	public class WhenMissing : CodeFixVerifier
	{

		const string sut = @"
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SampleLibrary
{
	public static class ServiceCollectionExtension
	{

		public static void DoSomethingCool(this IServiceCollection services)
		{

		}

		static void Main(string[] args) {}

	}
}";

		[Test]
		public void ShouldRaiseWarning()
		{

			var expected = new DiagnosticResult
			{
				Id = "FRITZ001",
				Message = "IServiceCollection methods should start with Add",
				Severity = DiagnosticSeverity.Warning,
				Locations =
				new[] {
					new DiagnosticResultLocation("Test0.cs", 10, 22)
				}
			};

			VerifyCSharpDiagnostic(sut, expected);

		}

		[Test]
		public void ShouldPrefixWithAdd()
		{

			const string expected = @"
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SampleLibrary
{
	public static class ServiceCollectionExtension
	{

		public static void AddDoSomethingCool(this IServiceCollection services)
		{

		}

		static void Main(string[] args) {}

	}
}";

			VerifyCSharpFix(sut, expected, 0);


		}

		[Test]
		public void ShouldReplaceFirstWordWithAdd()
		{

			const string expected = @"
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SampleLibrary
{
	public static class ServiceCollectionExtension
	{

		public static void AddSomethingCool(this IServiceCollection services)
		{

		}

		static void Main(string[] args) {}

	}
}";

			VerifyCSharpFix(sut, expected, 1);

		}



		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new Fritz.Analyzers.DependencyInjectionNaming.NamingAnalyzer();
		}

		protected override CodeFixProvider GetCSharpCodeFixProvider()
		{
			return new Fritz.Analyzers.DependencyInjectionNaming.NamingAddPrefixCodeFix();
		}

	}

}
