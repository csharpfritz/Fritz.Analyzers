using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using TestHelper;

namespace Fritz.Analyzers.Test.DependencyInjectionNaming.AddPrefix
{
	[TestFixture]
	public class WhenPresent : CodeFixVerifier
	{

		[Test]
		public void ShouldNotRaiseWarning()
		{

			const string sut = @"
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

			VerifyCSharpDiagnostic(sut, new DiagnosticResult[] { });

		}


		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new Fritz.Analyzers.DependencyInjectionNaming.NamingAnalyzer();
		}


	}

}
