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
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule, nameof(EvaluatePropertiesForSimplicityAnalyzer)),
        ctorHasParametersDiagnostic.HelpLinkUri,
        nameof(DiagnosticDescriptor.HelpLinkUri));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsNotStereotype()
    {
      var code = "public class A { }";
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasAbstractProperty()
    {
      var code =
@"using Csla;

public abstract class A : BusinessBase<A>
{
  public abstract string Data { get; set; }
}";
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasStaticProperty()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public static string Data { get; set; }
}";
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasGetterWithNoMethodCall()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public string Data { get; }

  public string ExpressionData => string.Empty;
}";
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasGetterWithMethodCallButIsNotCslaPropertyMethod()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public string Data { get { return this.GetProperty(); } }

  public string GetProperty() => null;
}";
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasGetterWithMethodCallAndMultipleStatements()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);
  private string _x;

  public string Data { get { _x = ""44""; return this.GetProperty(DataProperty); } }

  public string GetX() { return _x; }
}";
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        code, new[] { Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasGetterWithMethodCallAndReturnButNoDirectInvocationExpression()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);
  public string Data { get { return ""x"" + this.GetProperty(DataProperty); } }

  public static readonly PropertyInfo<string> ExpressionDataProperty = RegisterProperty<string>(_ => _.ExpressionData);
  public string ExpressionData => ""x"" + this.GetProperty(DataProperty);
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
@"using Csla;

public class A : BusinessBase<A>
{
  public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);
  public string Data { get { return this.GetProperty(DataProperty); } }

  public static readonly PropertyInfo<string> ExpressionDataProperty = RegisterProperty<string>(_ => _.ExpressionData);
  public string ExpressionData => this.GetProperty(DataProperty);
}";
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasSetterWithNoMethodCall()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public string Data { set { } }
}";
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasSetterWithMethodCallButIsNotCslaPropertyMethod()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public string Data { set { this.SetProperty(); } }

  public void SetProperty() { }
}";
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasSetterWithMethodCallAndMultipleStatements()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);
  private string _x;

  public string Data { get { return null; } set { _x = ""44""; this.SetProperty(DataProperty, value); } }

  public string GetX() { return _x; }
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
@"using Csla;

public class A : BusinessBase<A>
{
  public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);

  public string Data
  {
    get { return null; }
    set { this.SetProperty(DataProperty, value); }
  }
}";
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        code, Array.Empty<string>());
    }
  }
}