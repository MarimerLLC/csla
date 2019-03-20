using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class EvaluatePropertiesForSimplicityAnalyzerTests
  {
    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var analyzer = new EvaluatePropertiesForSimplicityAnalyzer();
      var diagnostics = analyzer.SupportedDiagnostics;
      Assert.AreEqual(1, diagnostics.Length);

      var ctorHasParametersDiagnostic = diagnostics.Single(_ => _.Id == Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule);
      Assert.AreEqual(OnlyUseCslaPropertyMethodsInGetSetRuleConstants.Title, ctorHasParametersDiagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(OnlyUseCslaPropertyMethodsInGetSetRuleConstants.Message, ctorHasParametersDiagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, ctorHasParametersDiagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Warning, ctorHasParametersDiagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsNotStereotype()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.EvaluatePropertiesForSimplicityAnalyzerTests
{
  public class ClassIsNotStereotype { }
}";
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasAbstractProperty()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public abstract class AnalyzeWhenClassHasAbstractProperty
    : BusinessBase<AnalyzeWhenClassHasAbstractProperty>
  {
    public abstract string Data { get; set; }
  }
}";
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasStaticProperty()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassHasStaticProperty
    : BusinessBase<AnalyzeWhenClassHasStaticProperty>
  {
    public static string Data { get; set; }
  }
}";
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasGetterWithNoMethodCall()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassHasGetterWithNoMethodCall
    : BusinessBase<AnalyzeWhenClassHasGetterWithNoMethodCall>
  {
    public string Data { get; }

    public string ExpressionData => string.Empty;
  }
}";
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasGetterWithMethodCallButIsNotCslaPropertyMethod()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassHasGetterWithMethodCallButIsNotCslaPropertyMethod
    : BusinessBase<AnalyzeWhenClassHasGetterWithMethodCallButIsNotCslaPropertyMethod>
  {
    public string Data { get { return this.GetProperty(); } }

    public string GetProperty()
    {
      return null;
    }
  }
}";
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasGetterWithMethodCallAndMultipleStatements()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassHasGetterWithMethodCallAndMultipleStatements
    : BusinessBase<AnalyzeWhenClassHasGetterWithMethodCallAndMultipleStatements>
  {
    public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);
    private string _x;

    public string Data { get { _x = ""44""; return this.GetProperty(DataProperty); } }

    public string GetX() { return _x; }
  }
}";
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        code, new[] { Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasGetterWithMethodCallAndReturnButNoDirectInvocationExpression()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassHasGetterWithMethodCallAndReturnButNoDirectInvocationExpression
    : BusinessBase<AnalyzeWhenClassHasGetterWithMethodCallAndReturnButNoDirectInvocationExpression>
  {
    public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);
    public string Data { get { return ""x"" + this.GetProperty(DataProperty); } }

    public static readonly PropertyInfo<string> ExpressionDataProperty = RegisterProperty<string>(_ => _.ExpressionData);
    public string ExpressionData => ""x"" + this.GetProperty(DataProperty);
  }
}";
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(code, 
        new[] 
        {
          Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule,
          Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule
        });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasGetterWithMethodCallAndReturnAndDirectInvocationExpression()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassHasGetterWithMethodCallAndReturnAndDirectInvocationExpression
    : BusinessBase<AnalyzeWhenClassHasGetterWithMethodCallAndReturnAndDirectInvocationExpression>
  {
    public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);
    public string Data { get { return this.GetProperty(DataProperty); } }

    public static readonly PropertyInfo<string> ExpressionDataProperty = RegisterProperty<string>(_ => _.ExpressionData);
    public string ExpressionData => this.GetProperty(DataProperty);
  }
}";
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasSetterWithNoMethodCall()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassHasSetterWithNoMethodCall
    : BusinessBase<AnalyzeWhenClassHasSetterWithNoMethodCall>
  {
    public string Data { set { } }
  }
}";
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasSetterWithMethodCallButIsNotCslaPropertyMethod()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassHasSetterWithMethodCallButIsNotCslaPropertyMethod
    : BusinessBase<AnalyzeWhenClassHasSetterWithMethodCallButIsNotCslaPropertyMethod>
  {
    public string Data { set { this.SetProperty(); } }

    public void SetProperty() { }
  }
}";
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasSetterWithMethodCallAndMultipleStatements()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassHasSetterWithMethodCallAndMultipleStatements
    : BusinessBase<AnalyzeWhenClassHasSetterWithMethodCallAndMultipleStatements>
  {
    public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);
    private string _x;

    public string Data { get { return null; } set { _x = ""44""; this.SetProperty(DataProperty, value); } }

    public string GetX() { return _x; }
  }
}";
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        code, new[] 
        {
          Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule
        });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasSetterWithMethodCallAndDirectInvocationExpression()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassHasSetterWithMethodCallAndDirectInvocationExpression
    : BusinessBase<AnalyzeWhenClassHasSetterWithMethodCallAndDirectInvocationExpression>
  {
    public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);

    public string Data
    {
      get { return null; }
      set { this.SetProperty(DataProperty, value); }
    }
  }
}";
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        code, Array.Empty<string>());
    }
  }
}