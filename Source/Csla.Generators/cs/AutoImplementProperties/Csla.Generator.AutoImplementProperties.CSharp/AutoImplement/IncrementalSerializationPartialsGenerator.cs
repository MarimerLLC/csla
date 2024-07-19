using System.Text;
using Csla.Generator.AutoImplementProperties.CSharp.AutoSerialization.Discovery;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Csla.Generator.AutoImplementProperties.CSharp.AutoSerialization
{
  [Generator]
  public class IncrementalSerializationPartialsGenerator : IIncrementalGenerator
  {
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
      // Register a syntax receiver to collect information during the initial parsing phase
      var classDeclarations = context.SyntaxProvider.ForAttributeWithMetadataName(
              "Csla.Serialization.AutoImplementPropertiesAttribute",

              predicate: static (s, _) => (s is ClassDeclarationSyntax || s is StructDeclarationSyntax) && s is TypeDeclarationSyntax type && type.Modifiers.Any(SyntaxKind.PartialKeyword),
              transform: static (ctx, _) =>
              {
                if (ctx.TargetNode is TypeDeclarationSyntax typeDeclarationSyntax)
                {
                  return TypeDefinitionExtractor.ExtractTypeDefinition(new DefinitionExtractionContext(ctx.SemanticModel), typeDeclarationSyntax);
                }
                return null;
              })
          .Where(static m => m is not null);

      var compilation = context.CompilationProvider.Select((c, _) => new { ((CSharpCompilation)c).LanguageVersion, Nullable = c.Options.NullableContextOptions.AnnotationsEnabled() });
      // Combine the collected syntax nodes with the semantic model
      var compilationAndClasses = compilation.Combine(classDeclarations.Collect());

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