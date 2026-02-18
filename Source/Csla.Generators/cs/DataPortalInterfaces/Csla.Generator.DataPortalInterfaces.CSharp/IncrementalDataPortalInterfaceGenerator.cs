//-----------------------------------------------------------------------
// <copyright file="IncrementalDataPortalInterfaceGenerator.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Source generator for data portal operation interface implementations</summary>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Csla.Generator.DataPortalInterfaces.CSharp.Discovery;
using Csla.Generator.DataPortalInterfaces.CSharp.Extractors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Csla.Generator.DataPortalInterfaces.CSharp
{

  /// <summary>
  /// Incremental source generator that creates explicit implementations of
  /// IDataPortalOperationMapping for business types with data portal operation methods.
  /// </summary>
  [Generator]
  public class IncrementalDataPortalInterfaceGenerator : IIncrementalGenerator
  {
    /// <summary>
    /// The short attribute names to look for on methods (syntactic check)
    /// </summary>
    private static readonly HashSet<string> AttributeShortNames = new HashSet<string>
    {
      "Create", "CreateAttribute",
      "Fetch", "FetchAttribute",
      "Insert", "InsertAttribute",
      "Update", "UpdateAttribute",
      "Execute", "ExecuteAttribute",
      "Delete", "DeleteAttribute",
      "DeleteSelf", "DeleteSelfAttribute",
      "CreateChild", "CreateChildAttribute",
      "FetchChild", "FetchChildAttribute",
      "InsertChild", "InsertChildAttribute",
      "UpdateChild", "UpdateChildAttribute",
      "DeleteSelfChild", "DeleteSelfChildAttribute",
      "ExecuteChild", "ExecuteChildAttribute"
    };

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
      var classDeclarations = context.SyntaxProvider.CreateSyntaxProvider(
        predicate: static (s, _) => IsCandidateClass(s),
        transform: static (ctx, _) => TransformCandidate(ctx)
      ).Where(static m => m is not null);

      context.RegisterSourceOutput(classDeclarations, static (spc, typeDefinition) =>
      {
        if (!typeDefinition!.IsPartial)
        {
          // Emit diagnostic for non-partial class
          var diagnostic = Diagnostic.Create(
            Diagnostics.NonPartialClassWarning,
            Location.None,
            typeDefinition.TypeName);
          spc.ReportDiagnostic(diagnostic);
          return;
        }

        var builder = new DataPortalInterfaceBuilder();
        var generationResults = builder.BuildPartialTypeDefinition(typeDefinition);

        spc.AddSource(
          $"{generationResults.FullyQualifiedName}.DataPortalOperations.g.cs",
          SourceText.From(generationResults.GeneratedSource, Encoding.UTF8));
      });
    }

    private static bool IsCandidateClass(SyntaxNode node)
    {
      if (node is not ClassDeclarationSyntax classDecl)
        return false;

      // Skip abstract classes - they can't be directly instantiated by the data portal
      if (classDecl.Modifiers.Any(SyntaxKind.AbstractKeyword))
        return false;

      // Check if any method has a data portal operation attribute
      foreach (var member in classDecl.Members)
      {
        if (member is MethodDeclarationSyntax methodDecl)
        {
          foreach (var attrList in methodDecl.AttributeLists)
          {
            foreach (var attr in attrList.Attributes)
            {
              var name = GetAttributeName(attr);
              if (AttributeShortNames.Contains(name))
                return true;
            }
          }
        }
      }

      return false;
    }

    private static string GetAttributeName(AttributeSyntax attr)
    {
      switch (attr.Name)
      {
        case SimpleNameSyntax simpleName:
          return simpleName.Identifier.Text;
        case QualifiedNameSyntax qualifiedName:
          return qualifiedName.Right.Identifier.Text;
        default:
          return attr.Name.ToString();
      }
    }

    private static ExtractedTypeDefinition? TransformCandidate(GeneratorSyntaxContext ctx)
    {
      var classDecl = (ClassDeclarationSyntax)ctx.Node;
      var extractionContext = new DefinitionExtractionContext(ctx.SemanticModel);
      var typeDefinition = TypeDefinitionExtractor.ExtractTypeDefinition(extractionContext, classDecl);

      // Verify at least one method actually has a valid CSLA attribute (semantic check)
      if (typeDefinition.OperationMethods.Count == 0)
        return null;

      return typeDefinition;
    }
  }

  /// <summary>
  /// Diagnostic descriptors for the data portal interface generator
  /// </summary>
  internal static class Diagnostics
  {
    /// <summary>
    /// Warning when data portal operations are found on a non-partial class
    /// </summary>
    public static readonly DiagnosticDescriptor NonPartialClassWarning = new DiagnosticDescriptor(
      id: "CSLADP001",
      title: "Data portal operation class should be partial",
      messageFormat: "Class '{0}' has data portal operation methods but is not partial. The source generator cannot generate the IDataPortalOperationMapping implementation.",
      category: "Csla.DataPortal",
      defaultSeverity: DiagnosticSeverity.Warning,
      isEnabledByDefault: true);
  }
}
