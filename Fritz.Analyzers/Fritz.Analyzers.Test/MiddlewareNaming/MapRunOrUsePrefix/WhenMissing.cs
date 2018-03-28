using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TestHelper;

namespace Fritz.Analyzers.Test.MiddlewareNaming.MapRunOrUsePrefix
{

	[TestFixture]
	public class WhenMissing : CodeFixVerifier
	{

		const string sut = @"
using Microsoft.AspNetCore.Builder;
using System;

namespace SampleLibrary
{
	public static class MiddlewareExtension
	{

		public static void MyMiddleware(this IApplicationBuilder app)
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
				Id = "FRITZ002",
				Message = "Middleware methods should start with Map, Run, or Use",
				Severity = DiagnosticSeverity.Warning,
				Locations =
				new[] {
					new DiagnosticResultLocation("Test0.cs", 10, 22)
				}
			};

			VerifyCSharpDiagnostic(sut, expected);

		}

		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{

			return new Fritz.Analyzers.MiddlewareNaming.NamingAnalyzer();

		}


	}

}
