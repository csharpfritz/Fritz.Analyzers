using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;

namespace TestHelper
{

	public abstract partial class DiagnosticVerifier
	{

		private static readonly List<MetadataReference> MyReferences = new List<MetadataReference>
		{
			MetadataReference.CreateFromFile(typeof(IServiceCollection).Assembly.Location),
			MetadataReference.CreateFromFile(typeof(IApplicationBuilder).Assembly.Location)
		};

	}

}
