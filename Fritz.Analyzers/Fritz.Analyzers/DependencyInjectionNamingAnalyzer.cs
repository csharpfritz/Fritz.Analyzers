using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Fritz.Analyzers
{

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class DependencyInjectionNamingAnalyzer : DiagnosticAnalyzer
	{

		public const string DiagnosticId = "FRITZ001";
		private const string Title = "Dependency Injection Naming";
		private const string MessageFormat = "IServiceCollection methods should start with Add";

		private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, "Naming", DiagnosticSeverity.Warning, isEnabledByDefault: true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>  ImmutableArray.Create(Rule);

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

				if (thisArgumentType.Name == "IServiceCollection" &&
					thisArgumentType.GetFullNamespace() == "Microsoft.Extensions.DependencyInjection" &&
					thisArgumentType.ContainingAssembly.Name == "Microsoft.Extensions.DependencyInjection.Abstractions" &&
					!method.Name.StartsWith("Add"))
				{
					context.ReportDiagnostic(Diagnostic.Create(
						Rule, method.Locations[0]));
				}
			}
		}
	}

	internal static class ISymbolExtensions
	{
		internal static string GetFullNamespace(this ISymbol @this)
		{
			var namespaces = new List<string>();
			var @namespace = @this.ContainingNamespace;

			while (@namespace != null)
			{
				if (!string.IsNullOrWhiteSpace(@namespace.Name))
				{
					namespaces.Add(@namespace.Name);
				}

				@namespace = @namespace.ContainingNamespace;
			}

			namespaces.Reverse();

			return string.Join(".", namespaces);
		}
	}

}
