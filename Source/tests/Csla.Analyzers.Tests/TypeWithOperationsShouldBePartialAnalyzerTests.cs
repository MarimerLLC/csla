using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class TypeWithOperationsShouldBePartialAnalyzerTests
  {
    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var analyzer = new TypeWithOperationsShouldBePartialAnalyzer();
      var diagnostics = analyzer.SupportedDiagnostics;
      Assert.AreEqual(1, diagnostics.Length);

      var diagnostic = diagnostics[0];
      Assert.AreEqual(Constants.AnalyzerIdentifiers.TypeWithOperationsShouldBePartial, diagnostic.Id,
        nameof(DiagnosticDescriptor.Id));
      Assert.AreEqual("Type with data portal operation methods should be partial", diagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual("Type '{0}' has data portal operation methods and should be declared partial so the CSLA source generator can implement its operations interface",
        diagnostic.MessageFormat.ToString(),
        nameof(DiagnosticDescriptor.MessageFormat));
      Assert.AreEqual(Constants.Categories.Usage, diagnostic.Category,
        nameof(DiagnosticDescriptor.Category));
      Assert.AreEqual(DiagnosticSeverity.Warning, diagnostic.DefaultSeverity,
        nameof(DiagnosticDescriptor.DefaultSeverity));
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.TypeWithOperationsShouldBePartial, nameof(TypeWithOperationsShouldBePartialAnalyzer)),
        diagnostic.HelpLinkUri,
        nameof(DiagnosticDescriptor.HelpLinkUri));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsNotPartialAndHasOperationMethod()
    {
      var code =
        """
        using Csla;
        using System;

        [Serializable]
        public class A : BusinessBase<A>
        {
          [Fetch]
          private void Fetch(int id) { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<TypeWithOperationsShouldBePartialAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.TypeWithOperationsShouldBePartial],
        diagnostics =>
        {
          Assert.AreEqual("Type 'A' has data portal operation methods and should be declared partial so the CSLA source generator can implement its operations interface",
            diagnostics[0].GetMessage());
          var location = diagnostics[0].Location;
          Assert.AreEqual("A", location.SourceTree.GetText().ToString(location.SourceSpan));
        });
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsNotPartialAndHasChildOperationMethod()
    {
      var code =
        """
        using Csla;
        using System;

        [Serializable]
        public class A : BusinessBase<A>
        {
          [UpdateChild]
          private void Child_Update() { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<TypeWithOperationsShouldBePartialAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.TypeWithOperationsShouldBePartial]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsNotPartialAndHasMultipleOperationMethods()
    {
      var code =
        """
        using Csla;
        using System;

        [Serializable]
        public class A : BusinessBase<A>
        {
          [Create]
          private void Create() { }

          [Fetch]
          private void Fetch(int id) { }

          [Insert]
          private void Insert() { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<TypeWithOperationsShouldBePartialAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.TypeWithOperationsShouldBePartial]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsPartial()
    {
      var code =
        """
        using Csla;
        using System;

        [Serializable]
        public partial class A : BusinessBase<A>
        {
          [Fetch]
          private void Fetch(int id) { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<TypeWithOperationsShouldBePartialAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasNoOperationMethods()
    {
      var code =
        """
        using Csla;
        using System;

        [Serializable]
        public class A : BusinessBase<A>
        {
          private void DataPortal_Fetch(int id) { }

          public void DoWork() { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<TypeWithOperationsShouldBePartialAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenNestedClassIsPartialButContainingClassIsNot()
    {
      var code =
        """
        using Csla;
        using System;

        public class Outer
        {
          [Serializable]
          public partial class A : BusinessBase<A>
          {
            [Fetch]
            private void Fetch(int id) { }
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<TypeWithOperationsShouldBePartialAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.TypeWithOperationsShouldBePartial],
        diagnostics => Assert.IsTrue(diagnostics[0].GetMessage().Contains("'A'")));
    }

    [TestMethod]
    public async Task AnalyzeWhenNestedClassAndContainingClassesArePartial()
    {
      var code =
        """
        using Csla;
        using System;

        public partial class Outer
        {
          public static partial class Middle
          {
            [Serializable]
            public partial class A : BusinessBase<A>
            {
              [Fetch]
              private void Fetch(int id) { }
            }
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<TypeWithOperationsShouldBePartialAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenNestedClassAndContainingClassAreNotPartial()
    {
      var code =
        """
        using Csla;
        using System;

        public class Outer
        {
          [Serializable]
          public class A : BusinessBase<A>
          {
            [Fetch]
            private void Fetch(int id) { }
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<TypeWithOperationsShouldBePartialAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.TypeWithOperationsShouldBePartial]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassIsAbstract()
    {
      var code =
        """
        using Csla;
        using System;

        [Serializable]
        public abstract class A<T> : BusinessBase<T>
          where T : A<T>
        {
          [Fetch]
          protected void Fetch(int id) { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<TypeWithOperationsShouldBePartialAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.TypeWithOperationsShouldBePartial]);
    }

    [TestMethod]
    public async Task AnalyzeWhenTypeIsRecord()
    {
      var code =
        """
        using Csla;

        public record A
        {
          [Fetch]
          private void Fetch(int id) { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<TypeWithOperationsShouldBePartialAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenCodeIsGenerated()
    {
      var code =
        """
        // <auto-generated/>
        using Csla;
        using System;

        [Serializable]
        public class A : BusinessBase<A>
        {
          [Fetch]
          private void Fetch(int id) { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<TypeWithOperationsShouldBePartialAnalyzer>(code, []);
    }
  }
}
