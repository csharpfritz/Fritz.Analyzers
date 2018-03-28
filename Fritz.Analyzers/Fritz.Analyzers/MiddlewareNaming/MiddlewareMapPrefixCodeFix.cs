using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Fritz.Analyzers.MiddlewareNaming
{

	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiddlewareMapPrefixCodeFix)), Shared]
	public class MiddlewareMapPrefixCodeFix : CodeFixProvider
	{
		public const string Title = "Prefix method with \"Run\"";

		public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(NamingAnalyzer.DiagnosticId);

		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
			var diagnostic = context.Diagnostics.First();
			var diagnosticSpan = diagnostic.Location.SourceSpan;
			var method = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

			if (!(method.ReturnType is PredefinedTypeSyntax) || ((PredefinedTypeSyntax)method.ReturnType).Keyword.ValueText != "void") return;

				// Register a code action that will invoke the fix.
				context.RegisterCodeFix(
				 CodeAction.Create(
					  title: Title,
					  createChangedSolution: c => PrefixMethodWithRun(context.Document, method, c),
					  equivalenceKey: Title),
				 diagnostic);

			context.RegisterCodeFix(
					CodeAction.Create(
						title: "Replace First Word with Run",
						createChangedSolution: c => ReplaceFirstWordWithRun(context.Document, method, c),
						equivalenceKey: "Replace First Word with Run"),
					diagnostic);


		}

		private async Task<Solution> ReplaceFirstWordWithRun(Document document, MethodDeclarationSyntax method, 
			CancellationToken cancellationToken)
		{
			var identifierToken = method.Identifier;


			var reFirstWord = new Regex("^([A-Z]?[a-z]+)");
			var newName = reFirstWord.Replace(identifierToken.Text, "Run");

			var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
			var methodSymbol = semanticModel.GetDeclaredSymbol(method, cancellationToken);

			var originalSolution = document.Project.Solution;
			var optionSet = originalSolution.Workspace.Options;
			var newSolution = await Renamer.RenameSymbolAsync(
				document.Project.Solution, methodSymbol, newName, optionSet, cancellationToken).ConfigureAwait(false);

			return newSolution;
		}

		private async Task<Solution> PrefixMethodWithRun(Document document, MethodDeclarationSyntax method, CancellationToken cancellationToken)
		{
			var identifierToken = method.Identifier;
			var newName = $"Run{identifierToken.Text}";

			var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
			var methodSymbol = semanticModel.GetDeclaredSymbol(method, cancellationToken);

			var originalSolution = document.Project.Solution;
			var optionSet = originalSolution.Workspace.Options;
			var newSolution = await Renamer.RenameSymbolAsync(
				document.Project.Solution, methodSymbol, newName, optionSet, cancellationToken).ConfigureAwait(false);

			return newSolution;
		}


		public sealed override FixAllProvider GetFixAllProvider() =>
			WellKnownFixAllProviders.BatchFixer;
	}
}
