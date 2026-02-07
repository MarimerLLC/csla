//-----------------------------------------------------------------------
// <copyright file="OperationMethodExtractor.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Extract data portal operation methods from a type</summary>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Csla.Generator.DataPortalInterfaces.CSharp.Extractors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Csla.Generator.DataPortalInterfaces.CSharp.Discovery
{

  /// <summary>
  /// Extract data portal operation methods from a type declaration
  /// </summary>
  internal static class OperationMethodExtractor
  {
    /// <summary>
    /// All CSLA data portal operation attribute fully-qualified names
    /// </summary>
    private static readonly HashSet<string> DataPortalAttributeNames = new HashSet<string>
    {
      "Csla.CreateAttribute",
      "Csla.FetchAttribute",
      "Csla.InsertAttribute",
      "Csla.UpdateAttribute",
      "Csla.ExecuteAttribute",
      "Csla.DeleteAttribute",
      "Csla.DeleteSelfAttribute",
      "Csla.CreateChildAttribute",
      "Csla.FetchChildAttribute",
      "Csla.InsertChildAttribute",
      "Csla.UpdateChildAttribute",
      "Csla.DeleteSelfChildAttribute",
      "Csla.ExecuteChildAttribute"
    };

    private const string InjectAttributeName = "Csla.InjectAttribute";

    /// <summary>
    /// Extract all data portal operation methods from a type declaration
    /// </summary>
    public static IReadOnlyList<ExtractedOperationMethod> ExtractOperationMethods(
      DefinitionExtractionContext context,
      ClassDeclarationSyntax classDeclaration)
    {
      var methods = new List<ExtractedOperationMethod>();

      foreach (var member in classDeclaration.Members)
      {
        if (member is MethodDeclarationSyntax methodDecl)
        {
          var operationMethod = TryExtractOperationMethod(context, methodDecl);
          if (operationMethod != null)
          {
            methods.Add(operationMethod);
          }
        }
      }

      return methods;
    }

    private static ExtractedOperationMethod? TryExtractOperationMethod(
      DefinitionExtractionContext context,
      MethodDeclarationSyntax methodDecl)
    {
      var semanticModel = context.SemanticModel;
      var methodSymbol = semanticModel.GetDeclaredSymbol(methodDecl) as IMethodSymbol;
      if (methodSymbol == null)
        return null;

      var operationAttributes = new List<string>();

      foreach (var attrData in methodSymbol.GetAttributes())
      {
        var attrClassName = attrData.AttributeClass?.ToDisplayString();
        if (attrClassName != null && DataPortalAttributeNames.Contains(attrClassName))
        {
          operationAttributes.Add(attrClassName);
        }
      }

      if (operationAttributes.Count == 0)
        return null;

      var result = new ExtractedOperationMethod
      {
        MethodName = methodSymbol.Name,
        IsAsync = IsAsyncMethod(methodSymbol)
      };

      foreach (var attr in operationAttributes)
      {
        result.OperationAttributeNames.Add(attr);
      }

      foreach (var param in methodSymbol.Parameters)
      {
        var extractedParam = ExtractParameter(param);
        if (extractedParam.IsInjected)
        {
          result.InjectParameters.Add(extractedParam);
        }
        else
        {
          result.CriteriaParameters.Add(extractedParam);
        }
      }

      return result;
    }

    private static bool IsAsyncMethod(IMethodSymbol method)
    {
      if (method.ReturnType is INamedTypeSymbol namedType)
      {
        var displayName = namedType.ToDisplayString();
        return displayName == "System.Threading.Tasks.Task" ||
               displayName.StartsWith("System.Threading.Tasks.Task<");
      }
      return false;
    }

    private static ExtractedOperationParameter ExtractParameter(IParameterSymbol param)
    {
      var isInjected = false;
      var allowNull = false;

      foreach (var attrData in param.GetAttributes())
      {
        if (attrData.AttributeClass?.ToDisplayString() == InjectAttributeName)
        {
          isInjected = true;

          // Check for AllowNull = true
          foreach (var namedArg in attrData.NamedArguments)
          {
            if (namedArg.Key == "AllowNull" && namedArg.Value.Value is bool val)
            {
              allowNull = val;
            }
          }
        }
      }

      return new ExtractedOperationParameter
      {
        Name = param.Name,
        TypeFullName = param.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
        TypeDisplayName = param.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
        IsInjected = isInjected,
        AllowNull = allowNull
      };
    }
  }
}
