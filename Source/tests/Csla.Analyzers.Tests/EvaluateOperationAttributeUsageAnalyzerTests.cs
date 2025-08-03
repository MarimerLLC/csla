﻿using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class EvaluateOperationAttributeUsageAnalyzerTests
  {
    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var analyzer = new EvaluateOperationAttributeUsageAnalyzer();
      var diagnostics = analyzer.SupportedDiagnostics;
      Assert.AreEqual(1, diagnostics.Length);

      var diagnostic = diagnostics.Single(_ => _.Id == Constants.AnalyzerIdentifiers.IsOperationAttributeUsageCorrect);
      Assert.AreEqual("Find Operation Attributes In Incorrect Places", diagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual("Operation attributes should only be used on methods within sterotypes or ObjectFactory and should not be static", diagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, diagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Error, diagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.IsOperationAttributeUsageCorrect, nameof(EvaluateOperationAttributeUsageAnalyzer)),
        diagnostic.HelpLinkUri,
        nameof(DiagnosticDescriptor.HelpLinkUri));
    }

    [TestMethod]
    public async Task AnalyzeWhenTypeIsNotStereotype()
    {
      var code =
        """
        using Csla;

        public class A
        {
          [Fetch]
          private void Fetch() { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<EvaluateOperationAttributeUsageAnalyzer>(
        code, [Constants.AnalyzerIdentifiers.IsOperationAttributeUsageCorrect]);
    }

    [TestMethod]
    public async Task AnalyzeWhenTypeIsStereotypeAndOperationIsStatic()
    {
      var code =
        """
        using Csla;
        using System;

        [Serializable]
        public class A
          : BusinessBase<A>
        {
          [Fetch]
          private static void Fetch() { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<EvaluateOperationAttributeUsageAnalyzer>(
        code, [Constants.AnalyzerIdentifiers.IsOperationAttributeUsageCorrect]);
    }

    [TestMethod]
    public async Task AnalyzeWhenTypeIsStereotypeAndOperationIsInstance()
    {
      var code =
        """
        using Csla;
        using System;

        [Serializable]
        public class A
          : BusinessBase<A>
        {
          [Fetch]
          private void Fetch() { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<EvaluateOperationAttributeUsageAnalyzer>(code, []);
    }
  }
}
