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
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsNotStereotype()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.EvaluateManagedBackingFieldsAnalayzerTests
{
  public class ClassIsNotStereotype { }
}";
      await TestHelpers.RunAnalysisAsync<EvaluateManagedBackingFieldsAnalayzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasManagedBackingFieldNotUsedByProperty()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.EvaluateManagedBackingFieldsAnalayzerTests
{
  public class AnalyzeWhenClassHasManagedBackingFieldNotUsedByProperty
    : BusinessBase<AnalyzeWhenClassHasManagedBackingFieldNotUsedByProperty>
  {
    public static readonly PropertyInfo<string> DataProperty =
      RegisterProperty<string>(_ => _.Data);
    public string Data { get; set; }

    public static readonly PropertyInfo<string> ExpressionDataProperty =
      RegisterProperty<string>(_ => _.ExpressionData);
    public string ExpressionData => string.Empty;
  }
}";
      await TestHelpers.RunAnalysisAsync<EvaluateManagedBackingFieldsAnalayzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasManagedBackingFieldUsedProperty()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.EvaluateManagedBackingFieldsAnalayzerTests
{
  public class AnalyzeWhenClassHasManagedBackingFieldUsedProperty
    : BusinessBase<AnalyzeWhenClassHasManagedBackingFieldUsedProperty>
  {
    public static readonly PropertyInfo<string> DataProperty =
      RegisterProperty<string>(_ => _.Data);
    public string Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }

    public static readonly PropertyInfo<string> ExpressionDataProperty =
      RegisterProperty<string>(_ => _.ExpressionData);
    public string ExpressionData => GetProperty(ExpressionDataProperty);
  }
}";
      await TestHelpers.RunAnalysisAsync<EvaluateManagedBackingFieldsAnalayzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasManagedBackingFieldUsedPropertyAndIsNotPublic()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.EvaluateManagedBackingFieldsAnalayzerTests
{
  public class AnalyzeWhenClassHasManagedBackingFieldUsedPropertyAndIsNotPublic
    : BusinessBase<AnalyzeWhenClassHasManagedBackingFieldUsedPropertyAndIsNotPublic>
  {
    static readonly PropertyInfo<string> DataProperty =
      RegisterProperty<string>(_ => _.Data);
    public string Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }

    static readonly PropertyInfo<string> ExpressionDataProperty =
      RegisterProperty<string>(_ => _.ExpressionData);
    public string ExpressionData => GetProperty(ExpressionDataProperty);
  }
}";
      await TestHelpers.RunAnalysisAsync<EvaluateManagedBackingFieldsAnalayzer>(
        code, new[] { Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields, Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasManagedBackingFieldUsedPropertyAndIsNotStatic()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.EvaluateManagedBackingFieldsAnalayzerTests
{
  public class AnalyzeWhenClassHasManagedBackingFieldUsedPropertyAndIsNotStatic
    : BusinessBase<AnalyzeWhenClassHasManagedBackingFieldUsedPropertyAndIsNotStatic>
  {
    public readonly PropertyInfo<string> DataProperty =
      RegisterProperty<string>(_ => _.Data);
    public string Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }

    public readonly PropertyInfo<string> ExpressionDataProperty =
      RegisterProperty<string>(_ => _.ExpressionData);
    public string ExpressionData => GetProperty(ExpressionDataProperty);
  }
}";
      await TestHelpers.RunAnalysisAsync<EvaluateManagedBackingFieldsAnalayzer>(
        code, new[] { Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields, Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasManagedBackingFieldUsedPropertyAndIsNotReadonly()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.EvaluateManagedBackingFieldsAnalayzerTests
{
  public class AnalyzeWhenClassHasManagedBackingFieldUsedPropertyAndIsNotReadonly
    : BusinessBase<AnalyzeWhenClassHasManagedBackingFieldUsedPropertyAndIsNotReadonly>
  {
    public static PropertyInfo<string> DataProperty =
      RegisterProperty<string>(_ => _.Data);
    public string Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }

    public static PropertyInfo<string> ExpressionDataProperty =
      RegisterProperty<string>(_ => _.ExpressionData);
    public string ExpressionData => GetProperty(ExpressionDataProperty);
  }
}";
      await TestHelpers.RunAnalysisAsync<EvaluateManagedBackingFieldsAnalayzer>(
        code, new[] { Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields, Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields });
    }

    [TestMethod]
    public async Task AnalyzeWhenCommandHasManagedBackingFieldUsedPropertyAndIsNotReadonly()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.EvaluateManagedBackingFieldsAnalayzerTests
{
  public class AnalyzeWhenCommandHasManagedBackingFieldUsedPropertyAndIsNotReadonly
    : CommandBase<AnalyzeWhenCommandHasManagedBackingFieldUsedPropertyAndIsNotReadonly>
  {
    public static PropertyInfo<string> DataProperty =
      RegisterProperty<string>(_ => _.Data);
    public string Data
    {
      get { return ReadProperty(DataProperty); }
      set { LoadProperty(DataProperty, value); }
    }

    public static PropertyInfo<string> ExpressionDataProperty =
      RegisterProperty<string>(_ => _.ExpressionData);
    public string ExpressionData => ReadProperty(ExpressionDataProperty);
  }
}";
      await TestHelpers.RunAnalysisAsync<EvaluateManagedBackingFieldsAnalayzer>(
        code, new[] { Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields, Constants.AnalyzerIdentifiers.EvaluateManagedBackingFields });
    }
  }
}