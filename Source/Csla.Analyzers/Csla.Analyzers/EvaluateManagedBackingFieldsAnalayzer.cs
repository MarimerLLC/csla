using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Linq;
using static Csla.Analyzers.Extensions.ITypeSymbolExtensions;

namespace Csla.Analyzers
{
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class EvaluateManagedBackingFieldsAnalayzer
    : DiagnosticAnalyzer
  {
    private static DiagnosticDescriptor mustBePublicStaticAndReadonlyRule = new DiagnosticDescriptor(
      EvaluateManagedBackingFieldsAnalayzerConstants.DiagnosticId,
      EvaluateManagedBackingFieldsAnalayzerConstants.Title,
      EvaluateManagedBackingFieldsAnalayzerConstants.Message,
      EvaluateManagedBackingFieldsAnalayzerConstants.Category,
      DiagnosticSeverity.Error, true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
      get
      {
        return ImmutableArray.Create(EvaluateManagedBackingFieldsAnalayzer.mustBePublicStaticAndReadonlyRule);
      }
    }

    public override void Initialize(AnalysisContext context)
    {
      context.RegisterSyntaxNodeAction<SyntaxKind>(
        EvaluateManagedBackingFieldsAnalayzer.AnalyzeFieldDeclaration, SyntaxKind.FieldDeclaration);
    }

    private static void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
    {
      var fieldNode = (FieldDeclarationSyntax)context.Node;

      if (!fieldNode.ContainsDiagnostics)
      {
        foreach (var variable in fieldNode.Declaration.Variables)
        {
          var fieldSymbol = context.SemanticModel.GetDeclaredSymbol(variable) as IFieldSymbol;
          var classSymbol = fieldSymbol?.ContainingType;

          context.CancellationToken.ThrowIfCancellationRequested();

          if (fieldSymbol != null && classSymbol != null &&
            classSymbol.IsStereotype())
          {
            if (fieldSymbol.Type.IsIPropertyInfo())
            {
              foreach (var classMember in classSymbol.GetMembers())
              {
                if (classMember.Kind == SymbolKind.Property)
                {
                  var classProperty = classMember as IPropertySymbol;

                  if (!classProperty.IsIndexer)
                  {
                    if (EvaluateManagedBackingFieldsAnalayzer.DetermineIfPropertyUsesField(
                      context, fieldSymbol, classProperty))
                    {
                      context.CancellationToken.ThrowIfCancellationRequested();

                      EvaluateManagedBackingFieldsAnalayzer.CheckForDiagnostics(
                        context, fieldNode, fieldSymbol);
                      break;
                    }
                  }
                }
              }
            }
          }
        }
      }
    }

    private static void CheckForDiagnostics(SyntaxNodeAnalysisContext context, FieldDeclarationSyntax fieldNode, IFieldSymbol fieldSymbol)
    {
      var isStatic = fieldSymbol.IsStatic;
      var isPublic = fieldSymbol.DeclaredAccessibility.HasFlag(Accessibility.Public);
      var isReadOnly = fieldSymbol.IsReadOnly;

      if (!isStatic || !isPublic || !isReadOnly)
      {
        context.ReportDiagnostic(Diagnostic.Create(
          EvaluateManagedBackingFieldsAnalayzer.mustBePublicStaticAndReadonlyRule,
          fieldNode.GetLocation()));
      }
    }

    private static bool DetermineIfPropertyUsesField(SyntaxNodeAnalysisContext context, 
      IFieldSymbol fieldSymbol, IPropertySymbol classProperty, 
      Func<PropertyDeclarationSyntax, SyntaxNode> propertyBody)
    {
      var root = context.Node.SyntaxTree.GetRoot();
      var rootSpan = root.FullSpan;
      var classPropertyLocationSpan = classProperty.Locations[0].SourceSpan;

      if (rootSpan.Contains(classPropertyLocationSpan))
      {
        var propertyNode = root.FindNode(classPropertyLocationSpan) as PropertyDeclarationSyntax;

        if (propertyNode != null)
        {
          var getter = propertyBody(propertyNode);

          if (new EvaluateManagedBackingFieldsWalker(getter, context.SemanticModel, fieldSymbol).UsesField)
          {
            return true;
          }
        }
      }

      return false;
    }

    private static bool DetermineIfPropertyUsesField(SyntaxNodeAnalysisContext context,
      IFieldSymbol fieldSymbol, IPropertySymbol classProperty)
    {
      if (classProperty.GetMethod != null)
      {
        return EvaluateManagedBackingFieldsAnalayzer.DetermineIfPropertyUsesField(
          context, fieldSymbol, classProperty,
          propertyNode => propertyNode.ExpressionBody as SyntaxNode ??
            propertyNode.AccessorList.Accessors.Single(
              _ => _.IsKind(SyntaxKind.GetAccessorDeclaration)).Body);
      }

      if (classProperty.SetMethod != null)
      {
        return EvaluateManagedBackingFieldsAnalayzer.DetermineIfPropertyUsesField(
          context, fieldSymbol, classProperty,
          propertyNode => propertyNode.AccessorList.Accessors.Single(
            _ => _.IsKind(SyntaxKind.SetAccessorDeclaration)).Body);
      }

      return false;
    }
  }
}