using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.CodeAnalysis
{
	internal static class ISymbolExtensions
	{

		/// <summary>
		/// The the complete namespace for a symbol
		/// </summary>
		/// <param name="theSymbol">The symbol to inspect</param>
		/// <returns>The full namespace of the symbol</returns>
		internal static string GetFullNamespace(this ISymbol theSymbol)
		{
			var namespaces = new List<string>();
			var @namespace = theSymbol.ContainingNamespace;

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
