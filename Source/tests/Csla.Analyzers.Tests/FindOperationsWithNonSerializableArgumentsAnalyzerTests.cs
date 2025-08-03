﻿using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class FindOperationsWithNonSerializableArgumentsAnalyzerTests
  {
    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var analyzer = new FindOperationsWithNonSerializableArgumentsAnalyzer();
      var diagnostics = analyzer.SupportedDiagnostics;
      Assert.AreEqual(1, diagnostics.Length);

      var diagnostic = diagnostics[0];
      Assert.AreEqual(Constants.AnalyzerIdentifiers.FindOperationsWithNonSerializableArguments, diagnostic.Id,
        nameof(DiagnosticDescriptor.Id));
      Assert.AreEqual("Find Operation Arguments That Are Not Serializable", diagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual("Operation argument types should be serializable", diagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Design, diagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Warning, diagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.FindOperationsWithNonSerializableArguments, nameof(FindOperationsWithNonSerializableArgumentsAnalyzer)),
        diagnostic.HelpLinkUri,
        nameof(DiagnosticDescriptor.HelpLinkUri));
    }

    [TestMethod]
    public async Task AnalyzeWithNotMobileObject()
    {
      var code = "public class A { }";
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsNotOperation()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          public void Foo() { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsRootOperationWithNoArguments()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          [Fetch]
          private void Fetch() { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsRootOperationWithSerializableArgument()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          [Fetch]
          private void Fetch(int x) { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsRootOperationWithNonSerializableArgument()
    {
      var code =
        """
        using Csla;

        public class A { }

        public class B : BusinessBase<B>
        {
          [Fetch]
          private void Fetch(A x) { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(
        code, [Constants.AnalyzerIdentifiers.FindOperationsWithNonSerializableArguments]);
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsRootOperationWithMobileObjectArgument()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A> { }

        public class B : BusinessBase<B>
        {
          [Fetch]
          private void Fetch(A x) { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsRootOperationWithNonSerializableArgumentThatIsInjectable()
    {
      var code =
        """
        using Csla;

        public class A { }

        public class B : BusinessBase<B>
        {
          [Fetch]
          private void Fetch([Inject] A x) { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsChildOperationWithNoArguments()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          [FetchChild]
          private void FetchChild() { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsChildOperationWithSerializableArgument()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          [FetchChild]
          private void FetchChild(int x) { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWithMobileObjectAndMethodIsChildOperationWithNonSerializableArgument()
    {
      var code =
        """
        using Csla;

        public class A { }

        public class B : BusinessBase<B>
        {
          [FetchChild]
          private void FetchChild(A x) { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(code, []);
    }

    [DataTestMethod]
    [DataRow("bool")]
    [DataRow("byte")]
    [DataRow("sbyte")]
    [DataRow("char")]
    [DataRow("decimal")]
    [DataRow("double")]
    [DataRow("float")]
    [DataRow("int")]
    [DataRow("uint")]
    [DataRow("long")]
    [DataRow("ulong")]
    [DataRow("short")]
    [DataRow("ushort")]
    public async Task AnalyzeWithNullableSerializablePrimitiveArgument(string primitiveType)
    {
      var code =
        $$"""

          using Csla;

          namespace TestingNamespace {
          
            public class A : BusinessBase<Foo>
            {
              [Create]
              private void Create({{primitiveType}}? a) { }
            }

          }
          """;

      await TestHelpers.RunAnalysisAsync<FindOperationsWithNonSerializableArgumentsAnalyzer>(code, []);
    }
  }
}