﻿using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
      Assert.AreEqual("Find CSLA Business Objects That Have Constructors With Parameters", ctorHasParametersDiagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual("CSLA business objects should not have public constructors with parameters", ctorHasParametersDiagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, ctorHasParametersDiagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Warning, ctorHasParametersDiagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.ConstructorHasParameters, nameof(CheckConstructorsAnalyzer)), 
        ctorHasParametersDiagnostic.HelpLinkUri,
        nameof(DiagnosticDescriptor.HelpLinkUri));

      var publicNoArgsCtorDiagnostic = diagnostics.Single(_ => _.Id == Constants.AnalyzerIdentifiers.PublicNoArgumentConstructorIsMissing);
      Assert.AreEqual("Find CSLA Business Objects That do not Have Public No-Arugment Constructors", publicNoArgsCtorDiagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual("CSLA business objects must have a public constructor with no arguments", publicNoArgsCtorDiagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, publicNoArgsCtorDiagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Error, publicNoArgsCtorDiagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.PublicNoArgumentConstructorIsMissing, nameof(CheckConstructorsAnalyzer)),
        publicNoArgsCtorDiagnostic.HelpLinkUri,
        nameof(DiagnosticDescriptor.HelpLinkUri));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsNotStereotype()
    {
      var code = "public class A { }";
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasPublicNoArgumentConstructor()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A> { }
        """;
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasPrivateNoArgumentConstructor()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          private A() { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.PublicNoArgumentConstructorIsMissing],
        diagnostics => Assert.AreEqual(true.ToString(), diagnostics[0].Properties[PublicNoArgumentConstructorIsMissingConstants.HasNonPublicNoArgumentConstructor]));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasPrivateConstructorWithArguments()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          private A(int a) { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.PublicNoArgumentConstructorIsMissing],
        diagnostics => Assert.AreEqual(false.ToString(), diagnostics[0].Properties[PublicNoArgumentConstructorIsMissingConstants.HasNonPublicNoArgumentConstructor]));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasPublicNoArgumentConstructorAndPublicConstructorWithArguments()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          public A() { }
          public A(int a) { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.ConstructorHasParameters],
        diagnostics => Assert.AreEqual(0, diagnostics[0].Properties.Count));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasNoPublicNoArgumentConstructorAndPublicConstructorWithArguments()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          public A(int a) { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.ConstructorHasParameters, Constants.AnalyzerIdentifiers.PublicNoArgumentConstructorIsMissing],
        diagnostics => Assert.AreEqual(0, diagnostics[0].Properties.Count));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsStereotypeAndHasStaticConstructor()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          static A() { }
        
          private A(int a) { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.PublicNoArgumentConstructorIsMissing],
        diagnostics => Assert.AreEqual(false.ToString(), diagnostics[0].Properties[PublicNoArgumentConstructorIsMissingConstants.HasNonPublicNoArgumentConstructor]));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsBusinessListBaseAndHasPublicConstructorWithArguments()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A> { }

        public class B : BusinessListBase<B, A>
        {
          public B() { }
        
          public B(int a) { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.ConstructorHasParameters],
        diagnostics => Assert.AreEqual(0, diagnostics[0].Properties.Count));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsDynamicListBaseAndHasPublicConstructorWithArguments()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A> { }

        public class B : DynamicListBase<A>
        {
          public B() { }
        
          public B(int a) { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.ConstructorHasParameters],
        diagnostics => Assert.AreEqual(0, diagnostics[0].Properties.Count));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsBusinessBindingListBaseAndHasPublicConstructorWithArguments()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A> { }

        public class B
          : BusinessBindingListBase<B, A>
        {
          public B() { }
        
          public B(int a) { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.ConstructorHasParameters],
        diagnostics => Assert.AreEqual(0, diagnostics[0].Properties.Count));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsCommandBaseAndHasPublicConstructorWithArguments()
    {
      var code =
        """
        using Csla;

        public class A : CommandBase<A>
        {
          public A() { }
        
          public A(int a) { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<CheckConstructorsAnalyzer>(code, []);
    }
  }
}