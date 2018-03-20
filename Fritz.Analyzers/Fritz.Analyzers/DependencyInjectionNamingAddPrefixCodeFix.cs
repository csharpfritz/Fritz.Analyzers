using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fritz.Analyzers
{

	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DependencyInjectionNamingAddPrefixCodeFix)), Shared]
	public class DependencyInjectionNamingAddPrefixCodeFix : CodeFixProvider
	{
		public const string Title = "Prefix method with \"Add\"";

		public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(DependencyInjectionNamingAnalyzer.DiagnosticId);

		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
			var diagnostic = context.Diagnostics.First();
			var diagnosticSpan = diagnostic.Location.SourceSpan;
			var method = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

			// Register a code action that will invoke the fix.
			context.RegisterCodeFix(
				 CodeAction.Create(
					  title: Title,
					  createChangedSolution: c => PrefixMethodWithAdd(context.Document, method, c),
					  equivalenceKey: Title),
				 diagnostic);

			context.RegisterCodeFix(
					CodeAction.Create(
						title: "Replace First Word with Add",
						createChangedSolution: c => ReplaceFirstWordWithAdd(context.Document, method, c),
						equivalenceKey: "Replace First Word with Add"),
					diagnostic);


		}

		private async Task<Solution> ReplaceFirstWordWithAdd(Document document, MethodDeclarationSyntax method, CancellationToken cancellationToken)
		{
			var identifierToken = method.Identifier;

			// TODO: Replace first word in method name
			var newName = $"Add{identifierToken.Text}";

			var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
			var methodSymbol = semanticModel.GetDeclaredSymbol(method, cancellationToken);

			var originalSolution = document.Project.Solution;
			var optionSet = originalSolution.Workspace.Options;
			var newSolution = await Renamer.RenameSymbolAsync(
				document.Project.Solution, methodSymbol, newName, optionSet, cancellationToken).ConfigureAwait(false);

			return newSolution;
		}

		private async Task<Solution> PrefixMethodWithAdd(Document document, MethodDeclarationSyntax method, CancellationToken cancellationToken)
		{
			var identifierToken = method.Identifier;
			var newName = $"Add{identifierToken.Text}";

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
