using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.FindSymbols;
using System.Collections.Immutable;
using System.Linq;
using static Csla.Analyzers.Extensions.ITypeSymbolExtensions;

namespace Csla.Analyzers
{
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class EvaluateManagedBackingFieldsAnalayzer
    : DiagnosticAnalyzer
  {
    private static DiagnosticDescriptor mustBePublicRule = new DiagnosticDescriptor(
      EvaluateManagedBackingFieldsAnalayzerMustBePublicConstants.DiagnosticId,
      EvaluateManagedBackingFieldsAnalayzerMustBePublicConstants.Title,
      EvaluateManagedBackingFieldsAnalayzerMustBePublicConstants.Message,
      EvaluateManagedBackingFieldsAnalayzerMustBePublicConstants.Category,
      DiagnosticSeverity.Error, true);

    private static DiagnosticDescriptor mustBeStaticRule = new DiagnosticDescriptor(
      EvaluateManagedBackingFieldsAnalayzerMustBeStaticConstants.DiagnosticId,
      EvaluateManagedBackingFieldsAnalayzerMustBeStaticConstants.Title,
      EvaluateManagedBackingFieldsAnalayzerMustBeStaticConstants.Message,
      EvaluateManagedBackingFieldsAnalayzerMustBeStaticConstants.Category,
      DiagnosticSeverity.Error, true);

    private static DiagnosticDescriptor mustBeReadOnlyRule = new DiagnosticDescriptor(
      EvaluateManagedBackingFieldsAnalayzerMustBeReadOnlyConstants.DiagnosticId,
      EvaluateManagedBackingFieldsAnalayzerMustBeReadOnlyConstants.Title,
      EvaluateManagedBackingFieldsAnalayzerMustBeReadOnlyConstants.Message,
      EvaluateManagedBackingFieldsAnalayzerMustBeReadOnlyConstants.Category,
      DiagnosticSeverity.Error, true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
      get
      {
        return ImmutableArray.Create(EvaluateManagedBackingFieldsAnalayzer.mustBePublicRule,
          EvaluateManagedBackingFieldsAnalayzer.mustBeReadOnlyRule,
          EvaluateManagedBackingFieldsAnalayzer.mustBeStaticRule);
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

      if(!fieldNode.ContainsDiagnostics)
      {
        foreach(var variable in fieldNode.Declaration.Variables)
        {
          var fieldSymbol = context.SemanticModel.GetDeclaredSymbol(variable) as IFieldSymbol;
          var classSymbol = fieldSymbol?.ContainingType;

          if (fieldSymbol != null && classSymbol != null &&
            classSymbol.IsStereotype())
          {
            if(fieldSymbol.Type.IsIPropertyInfo())
            {
              bool usesSymbol = false;

              foreach(var classMember in classSymbol.GetMembers())
              {
                if(classMember.Kind == SymbolKind.Property)
                {
                  var classProperty = classMember as IPropertySymbol;
                  if(!classProperty.IsIndexer)
                  {
                    if(classProperty.GetMethod != null)
                    {
                      var propertyNode = context.Node.SyntaxTree.GetRoot().FindNode(
                        classProperty.Locations[0].SourceSpan) as PropertyDeclarationSyntax;

                      var getter = propertyNode.AccessorList.Accessors.Single(
                        _ => _.IsKind(SyntaxKind.GetAccessorDeclaration)).Body;

                      usesSymbol |= new EvaluateManagedBackingFieldsWalker(getter, context.SemanticModel, fieldSymbol).UsesField;
                    }

                    if (classProperty.SetMethod != null)
                    {
                      var propertyNode = context.Node.SyntaxTree.GetRoot().FindNode(
                        classProperty.Locations[0].SourceSpan) as PropertyDeclarationSyntax;

                      var setter = propertyNode.AccessorList.Accessors.Single(
                        _ => _.IsKind(SyntaxKind.SetAccessorDeclaration)).Body;

                     usesSymbol |= new EvaluateManagedBackingFieldsWalker(setter, context.SemanticModel, fieldSymbol).UsesField;
                    }
                  }
                }
              }

              if(usesSymbol)
              {
                if (!fieldSymbol.IsStatic)
                {
                  context.ReportDiagnostic(Diagnostic.Create(
                    EvaluateManagedBackingFieldsAnalayzer.mustBeStaticRule,
                    fieldNode.GetLocation()));
                }

                if (!fieldSymbol.DeclaredAccessibility.HasFlag(Accessibility.Public))
                {
                  context.ReportDiagnostic(Diagnostic.Create(
                    EvaluateManagedBackingFieldsAnalayzer.mustBePublicRule,
                    fieldNode.GetLocation()));
                }

                if (!fieldSymbol.IsReadOnly)
                {
                  context.ReportDiagnostic(Diagnostic.Create(
                    EvaluateManagedBackingFieldsAnalayzer.mustBeReadOnlyRule,
                    fieldNode.GetLocation()));
                }
              }
            }
          }
        }
      }
    }
  }
}