using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class EvaluateManagedBackingFieldsAnalayzerTests
  {
    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var analyzer = new EvaluateManagedBackingFieldsAnalayzer();
      var diagnostics = analyzer.SupportedDiagnostics;
      Assert.AreEqual(1, diagnostics.Length);

      var diagnostic = diagnostics.Single(_ => _.Id == Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields);
      Assert.AreEqual(EvaluateManagedBackingFieldsAnalayzerConstants.Title, diagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(EvaluateManagedBackingFieldsAnalayzerConstants.Message, diagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, diagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Error, diagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields, nameof(EvaluateManagedBackingFieldsAnalayzer)),
        diagnostic.HelpLinkUri,
        nameof(DiagnosticDescriptor.HelpLinkUri));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsNotStereotype()
    {
      var code = "public class ClassIsNotStereotype { }";
      await TestHelpers.RunAnalysisAsync<EvaluateManagedBackingFieldsAnalayzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasManagedBackingFieldNotUsedByProperty()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public static readonly PropertyInfo<string> DataProperty =
    RegisterProperty<string>(_ => _.Data);
  public string Data { get; set; }

  public static readonly PropertyInfo<string> ArrowDataProperty =
    RegisterProperty<string>(_ => _.ArrowData);
  public string ArrowData 
  { 
    get => string.Empty; 
    set => ArrowData = value; 
  }

  public static readonly PropertyInfo<string> BlockDataProperty =
    RegisterProperty<string>(_ => _.BlockData);
  public string BlockData 
  { 
    get { return string.Empty; }
    set { BlockData = value; }
  }

  public static readonly PropertyInfo<string> ExpressionDataProperty =
    RegisterProperty<string>(_ => _.ExpressionData);
  public string ExpressionData => string.Empty;
}";
      await TestHelpers.RunAnalysisAsync<EvaluateManagedBackingFieldsAnalayzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasManagedBackingFieldUsedProperty()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public static readonly PropertyInfo<string> DataProperty =
    RegisterProperty<string>(_ => _.Data);
  public string Data
  {
    get => GetProperty(DataProperty);
    set => SetProperty(DataProperty, value);
  }

  public static readonly PropertyInfo<string> ExpressionDataProperty =
    RegisterProperty<string>(_ => _.ExpressionData);
  public string ExpressionData => GetProperty(ExpressionDataProperty);
}";
      await TestHelpers.RunAnalysisAsync<EvaluateManagedBackingFieldsAnalayzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasManagedBackingFieldUsedPropertyAndIsNotPublic()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  static readonly PropertyInfo<string> BlockDataProperty =
    RegisterProperty<string>(_ => _.BlockData);
  public string BlockData
  {
    get { return GetProperty(BlockDataProperty); }
    set { SetProperty(BlockDataProperty, value); }
  }

  static readonly PropertyInfo<string> ArrowDataProperty =
    RegisterProperty<string>(_ => _.ArrowData);
  public string ArrowData
  {
    get => GetProperty(ArrowDataProperty);
    set => SetProperty(ArrowDataProperty, value);
  }

  static readonly PropertyInfo<string> ExpressionDataProperty =
    RegisterProperty<string>(_ => _.ExpressionData);
  public string ExpressionData => GetProperty(ExpressionDataProperty);
}";
      await TestHelpers.RunAnalysisAsync<EvaluateManagedBackingFieldsAnalayzer>(
        code, new[] 
        {
          Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields,
          Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields,
          Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields
        });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasManagedBackingFieldUsedPropertyAndIsNotStatic()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public readonly PropertyInfo<string> BlockDataProperty =
    RegisterProperty<string>(_ => _.BlockData);
  public string BlockData
  {
    get { return GetProperty(BlockDataProperty); }
    set { SetProperty(BlockDataProperty, value); }
  }

  public readonly PropertyInfo<string> ArrowDataProperty =
    RegisterProperty<string>(_ => _.ArrowData);
  public string ArrowData
  {
    get => GetProperty(ArrowDataProperty);
    set => SetProperty(ArrowDataProperty, value);
  }

  public readonly PropertyInfo<string> ExpressionDataProperty =
    RegisterProperty<string>(_ => _.ExpressionData);
  public string ExpressionData => GetProperty(ExpressionDataProperty);
}";
      await TestHelpers.RunAnalysisAsync<EvaluateManagedBackingFieldsAnalayzer>(
        code, new[]
        {
          Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields,
          Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields,
          Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields
        });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasManagedBackingFieldUsedPropertyAndIsNotReadonly()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public static PropertyInfo<string> BlockDataProperty =
    RegisterProperty<string>(_ => _.BlockData);
  public string BlockData
  {
    get { return GetProperty(BlockDataProperty); }
    set { SetProperty(BlockDataProperty, value); }
  }

  public static PropertyInfo<string> DataProperty =
    RegisterProperty<string>(_ => _.Data);
  public string Data
  {
    get => GetProperty(DataProperty);
    set => SetProperty(DataProperty, value);
  }

  public static PropertyInfo<string> ExpressionDataProperty =
    RegisterProperty<string>(_ => _.ExpressionData);
  public string ExpressionData => GetProperty(ExpressionDataProperty);
}";
      await TestHelpers.RunAnalysisAsync<EvaluateManagedBackingFieldsAnalayzer>(
        code, new[]
        {
          Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields,
          Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields,
          Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields
        });
    }

    [TestMethod]
    public async Task AnalyzeWhenCommandHasManagedBackingFieldUsedPropertyAndIsNotReadonly()
    {
      var code =
@"using Csla;

public class A : CommandBase<A>
{
  public static PropertyInfo<string> BlockDataProperty =
    RegisterProperty<string>(_ => _.BlockData);
  public string BlockData
  {
    get { return ReadProperty(BlockDataProperty); }
    set { LoadProperty(BlockDataProperty, value); }
  }

  public static PropertyInfo<string> ArrowDataProperty =
    RegisterProperty<string>(_ => _.ArrowData);
  public string ArrowData
  {
    get => ReadProperty(ArrowDataProperty);
    set => LoadProperty(ArrowDataProperty, value);
  }

  public static PropertyInfo<string> ExpressionDataProperty =
    RegisterProperty<string>(_ => _.ExpressionData);
  public string ExpressionData => ReadProperty(ExpressionDataProperty);
}";
      await TestHelpers.RunAnalysisAsync<EvaluateManagedBackingFieldsAnalayzer>(
        code, new[]
        {
          Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields,
          Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields,
          Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields
        });
    }
  }
}