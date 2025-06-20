//-----------------------------------------------------------------------
// <copyright file="IncrementalAutoImplementPropertiesPartialsGenerator.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Source generator for partial properties implementation</summary>
//-----------------------------------------------------------------------
using System.Text;
using Csla.Generator.AutoImplementProperties.CSharp.AutoImplement.Discovery;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Csla.Generator.AutoImplementProperties.CSharp.AutoImplement
{
  /// <summary>
  /// Generates partial classes for incremental properties implementation based on the presence of the Csla.CslaImplementPropertiesAttribute attribute.
  /// </summary>
  [Generator]
  public class IncrementalAutoImplementPropertiesPartialsGenerator : IIncrementalGenerator
  {
    /// <summary>
    /// Initializes the incremental generator for properties implementation based on partial properties generator.
    /// </summary>
    /// <param name="context">The initialization context.</param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
      // Register a syntax receiver to collect information during the initial parsing phase
      var classDeclarations = context.SyntaxProvider.ForAttributeWithMetadataName(
              DefinitionExtractionContext.CslaImplementPropertiesAttributeFullName,

              predicate: static (s, _) => (s is ClassDeclarationSyntax || s is StructDeclarationSyntax) && s is TypeDeclarationSyntax type && type.Modifiers.Any(SyntaxKind.PartialKeyword),
              transform: static (ctx, _) =>
              {
                if (ctx.TargetNode is TypeDeclarationSyntax typeDeclarationSyntax)
                {
                  return TypeDefinitionExtractor.ExtractTypeDefinition(new DefinitionExtractionContext(ctx.SemanticModel, false, true), typeDeclarationSyntax);
                }
                return default!;
              })
          .Where(static m => m is not null).WithTrackingName(TrackingNames.ExtractClasses);

      var compilation = context.CompilationProvider.Select((c, _) => new { ((CSharpCompilation)c).LanguageVersion, Nullable = c.Options.NullableContextOptions.AnnotationsEnabled() }).WithTrackingName(TrackingNames.SelectCompilation);
      // Combine the collected syntax nodes with the semantic model
      var compilationAndClasses = compilation.Combine(classDeclarations.Collect()).WithTrackingName(TrackingNames.Combine);

      // Set up the generation phase
      context.RegisterSourceOutput(compilationAndClasses, static (spc, source) =>
      {
        var (nullable, classes) = source;
        SerializationPartialBuilder builder = new SerializationPartialBuilder(nullable.Nullable);
        foreach (var typeDefinition in classes)
        {
          // Build the text for the generated type using the builder
          var generationResults = builder.BuildPartialTypeDefinition(typeDefinition);

          // Add the generated source to the output
          spc.AddSource($"{generationResults.FullyQualifiedName}.g.cs",
              SourceText.From(generationResults.GeneratedSource, Encoding.UTF8));
        }
      });
    }
  }
}