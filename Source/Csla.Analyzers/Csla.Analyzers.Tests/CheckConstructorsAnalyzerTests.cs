using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class CheckConstructorsAnalyzerTests
  {
    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var analyzer = new CheckConstructorsAnalyzer();
      var diagnostics = analyzer.SupportedDiagnostics;
      Assert.AreEqual(2, diagnostics.Length);

      var ctorHasParametersDiagnostic = diagnostics.Single(_ => _.Id == Constants.AnalyzerIdentifiers.ConstructorHasParameters);
      Assert.AreEqual(ConstructorHasParametersConstants.Title, ctorHasParametersDiagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(ConstructorHasParametersConstants.Message, ctorHasParametersDiagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, ctorHasParametersDiagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Warning, ctorHasParametersDiagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));

      var publicNoArgsCtorDiagnostic = diagnostics.Single(_ => _.Id == Constants.AnalyzerIdentifiers.PublicNoArgumentConstructorIsMissing);
      Assert.AreEqual(PublicNoArgumentConstructorIsMissingConstants.Title, publicNoArgsCtorDiagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual(PublicNoArgumentConstructorIsMissingConstants.Message, publicNoArgsCtorDiagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, publicNoArgsCtorDiagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Error, publicNoArgsCtorDiagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsNotStereotype()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class ClassIsNotStereotype { }
}
";
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasPublicNoArgumentConstructor()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class ClassIsStereotypeAndHasPublicNoArgumentConstructor
    : BusinessBase<ClassIsStereotypeAndHasPublicNoArgumentConstructor> { }
}";
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(code, Array.Empty<string>());
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasPrivateNoArgumentConstructor()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class ClassIsStereotypeAndHasPrivateNoArgumentConstructor
    : BusinessBase<ClassIsStereotypeAndHasPrivateNoArgumentConstructor>
  {
    private ClassIsStereotypeAndHasPrivateNoArgumentConstructor() { }
  }
}";
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(code, 
        new[] { Constants.AnalyzerIdentifiers.PublicNoArgumentConstructorIsMissing },
        diagnostics => Assert.AreEqual(true.ToString(), diagnostics[0].Properties[PublicNoArgumentConstructorIsMissingConstants.HasNonPublicNoArgumentConstructor]));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasPrivateConstructorWithArguments()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class ClassIsStereotypeAndHasPrivateConstructorWithArguments
    : BusinessBase<ClassIsStereotypeAndHasPrivateConstructorWithArguments>
  {
    private ClassIsStereotypeAndHasPrivateConstructorWithArguments(int a) { }
  }
}";
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(code,
        new[] { Constants.AnalyzerIdentifiers.PublicNoArgumentConstructorIsMissing },
        diagnostics => Assert.AreEqual(false.ToString(), diagnostics[0].Properties[PublicNoArgumentConstructorIsMissingConstants.HasNonPublicNoArgumentConstructor]));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasPublicNoArgumentConstructorAndPublicConstructorWithArguments()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class ClassIsStereotypeAndHasPublicNoArgumentConstructorAndPublicConstructorWithArguments
    : BusinessBase<ClassIsStereotypeAndHasPublicNoArgumentConstructorAndPublicConstructorWithArguments>
  {
    public ClassIsStereotypeAndHasPublicNoArgumentConstructorAndPublicConstructorWithArguments() { }
    public ClassIsStereotypeAndHasPublicNoArgumentConstructorAndPublicConstructorWithArguments(int a) { }
  }
}";
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(code, 
        new[] { Constants.AnalyzerIdentifiers.ConstructorHasParameters },
        diagnostics => Assert.AreEqual(0, diagnostics[0].Properties.Count));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasNoPublicNoArgumentConstructorAndPublicConstructorWithArguments()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class ClassIsStereotypeAndHasNoPublicNoArgumentConstructorAndPublicConstructorWithArguments
    : BusinessBase<ClassIsStereotypeAndHasNoPublicNoArgumentConstructorAndPublicConstructorWithArguments>
  {
    public ClassIsStereotypeAndHasNoPublicNoArgumentConstructorAndPublicConstructorWithArguments(int a) { }
  }
}";

      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(code,
        new[] { Constants.AnalyzerIdentifiers.ConstructorHasParameters, Constants.AnalyzerIdentifiers.PublicNoArgumentConstructorIsMissing },
        diagnostics => Assert.AreEqual(0, diagnostics[0].Properties.Count));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasStaticConstructor()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassIsStereotypeAndHasStaticConstructor
    : BusinessBase<AnalyzeWhenClassIsStereotypeAndHasStaticConstructor>
  {
    static AnalyzeWhenClassIsStereotypeAndHasStaticConstructor() { }

    private AnalyzeWhenClassIsStereotypeAndHasStaticConstructor(int a) { }
  }
}";
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(code,
        new[] { Constants.AnalyzerIdentifiers.PublicNoArgumentConstructorIsMissing },
        diagnostics => Assert.AreEqual(false.ToString(), diagnostics[0].Properties[PublicNoArgumentConstructorIsMissingConstants.HasNonPublicNoArgumentConstructor]));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsBusinessListBaseAndHasPublicConstructorWithArguments()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassIsBusinessListBaseAndHasPublicConstructorWithArguments
    : BusinessBase<AnalyzeWhenClassIsBusinessListBaseAndHasPublicConstructorWithArguments>
  { }

  public class AnalyzeWhenClassIsBusinessListBaseAndHasPublicConstructorWithArgumentsList
    : BusinessListBase<AnalyzeWhenClassIsBusinessListBaseAndHasPublicConstructorWithArgumentsList, AnalyzeWhenClassIsBusinessListBaseAndHasPublicConstructorWithArguments>
  {
    public AnalyzeWhenClassIsBusinessListBaseAndHasPublicConstructorWithArgumentsList() { }

    public AnalyzeWhenClassIsBusinessListBaseAndHasPublicConstructorWithArgumentsList(int a) { }
  }
}";
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(code,
        new[] { Constants.AnalyzerIdentifiers.ConstructorHasParameters },
        diagnostics => Assert.AreEqual(0, diagnostics[0].Properties.Count));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsDynamicListBaseAndHasPublicConstructorWithArguments()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassIsDynamicListBaseAndHasPublicConstructorWithArguments
    : BusinessBase<AnalyzeWhenClassIsDynamicListBaseAndHasPublicConstructorWithArguments>
  { }

  public class AnalyzeWhenClassIsDynamicListBaseAndHasPublicConstructorWithArgumentsList
    : DynamicListBase<AnalyzeWhenClassIsDynamicListBaseAndHasPublicConstructorWithArguments>
  {
    public AnalyzeWhenClassIsDynamicListBaseAndHasPublicConstructorWithArgumentsList() { }

    public AnalyzeWhenClassIsDynamicListBaseAndHasPublicConstructorWithArgumentsList(int a) { }
  }
}";
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(code,
        new[] { Constants.AnalyzerIdentifiers.ConstructorHasParameters },
        diagnostics => Assert.AreEqual(0, diagnostics[0].Properties.Count));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsBusinessBindingListBaseAndHasPublicConstructorWithArguments()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassIsBusinessBindingListBaseAndHasPublicConstructorWithArguments
    : BusinessBase<AnalyzeWhenClassIsBusinessBindingListBaseAndHasPublicConstructorWithArguments>
  { }

  public class AnalyzeWhenClassIsBusinessBindingListBaseAndHasPublicConstructorWithArgumentsList
    : BusinessBindingListBase<AnalyzeWhenClassIsBusinessBindingListBaseAndHasPublicConstructorWithArgumentsList, AnalyzeWhenClassIsDynamicListBaseAndHasPublicConstructorWithArguments>
  {
    public AnalyzeWhenClassIsBusinessBindingListBaseAndHasPublicConstructorWithArgumentsList() { }

    public AnalyzeWhenClassIsBusinessBindingListBaseAndHasPublicConstructorWithArgumentsList(int a) { }
  }
}";
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(code,
        new[] { Constants.AnalyzerIdentifiers.ConstructorHasParameters },
        diagnostics => Assert.AreEqual(0, diagnostics[0].Properties.Count));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsCommandBaseAndHasPublicConstructorWithArguments()
    {
      var code =
@"namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassIsCommandBaseAndHasPublicConstructorWithArguments
    : CommandBase<AnalyzeWhenClassIsCommandBaseAndHasPublicConstructorWithArguments>
  {
    public AnalyzeWhenClassIsCommandBaseAndHasPublicConstructorWithArguments() { }

    public AnalyzeWhenClassIsCommandBaseAndHasPublicConstructorWithArguments(int a) { }
  }
}";
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(code, Array.Empty<string>());
    }
  }
}