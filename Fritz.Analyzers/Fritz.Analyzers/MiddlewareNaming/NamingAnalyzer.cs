using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Fritz.Analyzers.MiddlewareNaming
{

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class NamingAnalyzer : DiagnosticAnalyzer
	{

		public const string DiagnosticId = "FRITZ002";
		private const string Title = "Middleware Naming";
		public const string MessageFormat = "Middleware methods should start with Map, Run, or Use";

		private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, "Naming", DiagnosticSeverity.Warning, isEnabledByDefault: true);
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSymbolAction(AnalyzeMethod, SymbolKind.Method);
		}
		  

		private void AnalyzeMethod(SymbolAnalysisContext context)
		{

			var method = (IMethodSymbol)context.Symbol;

			if (method.IsExtensionMethod)
			{
				var thisArgumentType = method.Parameters[0].Type;

				if (thisArgumentType.Name == "IApplicationBuilder" &&
					thisArgumentType.GetFullNamespace() == "Microsoft.AspNetCore.Builder" &&
					thisArgumentType.ContainingAssembly.Name == "Microsoft.AspNetCore.Http.Abstractions" &&
					!(method.Name.StartsWith("Map") || method.Name.StartsWith("Run") || method.Name.StartsWith("Use")))
				{
					context.ReportDiagnostic(Diagnostic.Create(
						Rule, method.Locations[0]));
				}
			}

		}

	}

}
