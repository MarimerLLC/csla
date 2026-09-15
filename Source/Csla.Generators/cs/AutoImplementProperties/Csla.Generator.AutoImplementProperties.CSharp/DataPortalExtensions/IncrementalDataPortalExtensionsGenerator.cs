//-----------------------------------------------------------------------
// <copyright file="IncrementalDataPortalExtensionsGenerator.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Source generator for strongly typed data portal extension methods</summary>
//-----------------------------------------------------------------------
// Portions adapted from Csla.DataPortalExtensions by Stefan Ossendorf
// (https://github.com/StefanOssendorf/Csla.DataPortalExtensions),
// licensed under the MIT License. Copyright (c) 2023 Stefan Ossendorf.
//-----------------------------------------------------------------------

using System.Text;
using Csla.Generator.AutoImplementProperties.CSharp.DataPortalOperations;
using Csla.Generator.AutoImplementProperties.CSharp.DataPortalOperations.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Csla.Generator.AutoImplementProperties.CSharp.DataPortalExtensions
{
  /// <summary>
  /// Incremental source generator that emits strongly typed extension methods
  /// on <c>IDataPortal&lt;T&gt;</c> and <c>IChildDataPortal&lt;T&gt;</c> for each
  /// data portal operation method of a business class marked with
  /// <c>[DataPortalExtensions]</c>, or of every eligible business class when
  /// the assembly is marked. Diagnostics about the generated methods are
  /// reported by <see cref="DataPortalExtensionsAnalyzer"/>.
  /// </summary>
  [Generator(LanguageNames.CSharp)]
  public class IncrementalDataPortalExtensionsGenerator : IIncrementalGenerator
  {
    /// <summary>
    /// Operation kinds that have a client-side data portal method.
    /// </summary>
    internal static readonly string[] SupportedKinds = ["Create", "Fetch", "Execute", "Delete", "CreateChild", "FetchChild"];

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
      var options = context.CompilationProvider
        .Combine(context.AnalyzerConfigOptionsProvider)
        .Select(static (pair, _) => ExtensionOptions.Create(pair.Left, pair.Right.GlobalOptions))
        .WithTrackingName(TrackingNames.ExtensionOptions);

      var models = IncrementalDataPortalOperationsGenerator.GetOperationTypes(context)
        .SelectMany(static (types, _) => types)
        .Combine(options)
        .Select(static (pair, _) => CreateModel(pair.Left, pair.Right))
        .Where(static m => m is not null)
        .Select(static (m, _) => m!)
        .WithTrackingName(TrackingNames.ExtensionTypes);

      context.RegisterSourceOutput(models, static (spc, model) =>
      {
        var source = DataPortalExtensionsBuilder.Build(model);
        if (source is not null)
          spc.AddSource($"{model.Type.HintName}.DataPortalExtensions.g.cs", SourceText.From(source, Encoding.UTF8));
      });
    }

    /// <summary>
    /// Creates the generation model for a type with operation methods, or
    /// returns null when no extension methods are requested for it.
    /// </summary>
    internal static ExtensionGenerationModel? CreateModel(OperationTypeModel operationType, ExtensionOptions options, LocationInfo? location = null)
    {
      var type = ExtensionTypeModel.Create(operationType.Type, options, location);
      if (type is null)
        return null;

      return new ExtensionGenerationModel(
        type,
        new EquatableArray<OperationMethodModel>(operationType.Methods.Where(m => SupportedKinds.Contains(m.Kind))),
        options);
    }
  }
}
