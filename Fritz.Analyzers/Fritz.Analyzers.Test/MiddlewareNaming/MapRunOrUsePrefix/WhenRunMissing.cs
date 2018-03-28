using Fritz.Analyzers.MiddlewareNaming;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TestHelper;

namespace Fritz.Analyzers.Test.MiddlewareNaming.MapRunOrUsePrefix
{

	[TestFixture]
	public class WhenRunMissing : CodeFixVerifier
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

		[Test]
		public void ShouldPrefixWithRun()
		{

			const string expected = @"
using Microsoft.AspNetCore.Builder;
using System;

namespace SampleLibrary
{
	public static class MiddlewareExtension
	{

		public static void RunMyMiddleware(this IApplicationBuilder app)
		{

		}

		static void Main(string[] args) {}

	}
}";

			VerifyCSharpFix(sut, expected, 0);


		}

		[Test]
		public void ShouldReplaceFirstWordWithRun()
		{

			const string expected = @"
using Microsoft.AspNetCore.Builder;
using System;

namespace SampleLibrary
{
	public static class MiddlewareExtension
	{

		public static void RunMiddleware(this IApplicationBuilder app)
		{

		}

		static void Main(string[] args) {}

	}
}";

			VerifyCSharpFix(sut, expected, 1);

		}

		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{

			return new NamingAnalyzer();

		}

		protected override CodeFixProvider GetCSharpCodeFixProvider()
		{

			return new MiddlewareMapPrefixCodeFix();

		}

	}

}
