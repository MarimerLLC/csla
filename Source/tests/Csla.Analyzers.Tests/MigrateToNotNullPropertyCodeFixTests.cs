using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class MigrateToNotNullPropertyCodeFixTests
  {
    [TestMethod]
    public void VerifyGetFixableDiagnosticIds()
    {
      CollectionAssert.AreEqual(new[] { Constants.AnalyzerIdentifiers.MigrateToNotNullProperty },
        new MigrateToNotNullPropertyCodeFix().FixableDiagnosticIds.ToArray());
      CollectionAssert.AreEqual(new[] { Constants.AnalyzerIdentifiers.MigrateToNotNullPropertyWithReview },
        new MigrateToNotNullPropertyWithReviewCodeFix().FixableDiagnosticIds.ToArray());
    }

    [TestMethod]
    public void FixAllIsOfferedOnTheFixAllTierOnly()
    {
      var fixAll = new MigrateToNotNullPropertyCodeFix().GetFixAllProvider();
      Assert.IsNotNull(fixAll);
      var scopes = fixAll.GetSupportedFixAllScopes().ToList();
      Assert.AreEqual(FixAllScope.ContainingType, scopes[0], "first scope");
      CollectionAssert.IsSubsetOf(new[] { FixAllScope.Document, FixAllScope.Project, FixAllScope.Solution }, scopes);

      Assert.IsNull(new MigrateToNotNullPropertyWithReviewCodeFix().GetFixAllProvider());
    }

    private const string Header =
      """
      #nullable enable
      using System;
      using Csla;

      public class Child : BusinessBase<Child> { }

      """;

    private static string BusinessObjectWith(string members) =>
      Header +
      $$"""
      public class A : BusinessBase<A>
      {
        {{members}}
      }
      """;

    private static async Task<List<Diagnostic>> GetMigrationDiagnosticsAsync(Document document)
    {
      var compilation = (await document.Project.GetCompilationAsync())
        .WithAnalyzers([new MigrateToNotNullPropertyAnalyzer()]);
      return (await compilation.GetAnalyzerDiagnosticsAsync()).ToList();
    }

    /// <summary>
    /// Applies the code fix to the single migration diagnostic in <paramref name="code"/>
    /// and returns the changed document, after checking that it compiles without errors
    /// or nullable warnings.
    /// </summary>
    private static async Task<Document> FixAsync(string code, string expectedId)
    {
      var document = TestHelpers.Create(code);
      var diagnostics = await GetMigrationDiagnosticsAsync(document);
      Assert.AreEqual(1, diagnostics.Count, string.Join(Environment.NewLine, diagnostics));
      Assert.AreEqual(expectedId, diagnostics[0].Id);

      CodeFixProvider fix = expectedId == Constants.AnalyzerIdentifiers.MigrateToNotNullProperty
        ? new MigrateToNotNullPropertyCodeFix()
        : new MigrateToNotNullPropertyWithReviewCodeFix();
      var actions = new List<CodeAction>();
      await fix.RegisterCodeFixesAsync(new CodeFixContext(document, diagnostics[0], (action, _) => actions.Add(action), default));
      Assert.AreEqual(1, actions.Count, nameof(actions));

      var operations = await actions[0].GetOperationsAsync(default);
      var changed = ((ApplyChangesOperation)operations[0]).ChangedSolution.GetDocument(document.Id);
      await AssertCompilesCleanlyAsync(changed);
      return changed;
    }

    private static async Task AssertCompilesCleanlyAsync(Document document)
    {
      var compilation = await document.Project.GetCompilationAsync();
      var problems = compilation.GetDiagnostics()
        .Where(_ => _.Severity == DiagnosticSeverity.Error || _.Id.StartsWith("CS86", StringComparison.Ordinal))
        .ToList();
      Assert.AreEqual(0, problems.Count, string.Join(Environment.NewLine, problems) + Environment.NewLine + await GetTextAsync(document));
    }

    private static async Task<string> GetTextAsync(Document document) => (await document.GetTextAsync()).ToString();

    private static async Task AssertFixAsync(string members, string expectedMembers, string expectedId = "CSLA0037")
    {
      var changed = await FixAsync(BusinessObjectWith(members), expectedId);
      Assert.AreEqual(BusinessObjectWith(expectedMembers), await GetTextAsync(changed));
    }

    [TestMethod]
    public async Task MigratesValueType()
    {
      await AssertFixAsync(
        """
        public static readonly PropertyInfo<int> CountProperty = RegisterProperty<int>(nameof(Count));
          public int Count { get => GetProperty(CountProperty); set => SetProperty(CountProperty, value); }
        """,
        """
        public static readonly NotNullPropertyInfo<int> CountProperty = RegisterNotNullProperty<int>(nameof(Count));
          public int Count { get => GetProperty(CountProperty); set => SetProperty(CountProperty, value); }
        """);
    }

    [TestMethod]
    public async Task MigratesStringWithoutDefaultAndRemovesTheSuppression()
    {
      await AssertFixAsync(
        """
        public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
          public string Name { get => GetProperty(NameProperty)!; set => SetProperty(NameProperty, value); }
        """,
        """
        public static readonly NotNullPropertyInfo<string> NameProperty = RegisterNotNullProperty<string>(nameof(Name), null, string.Empty);
          public string Name { get => GetProperty(NameProperty); set => SetProperty(NameProperty, value); }
        """);
    }

    [TestMethod]
    public async Task MigratesStringKeepingTheFriendlyName()
    {
      await AssertFixAsync(
        """
        public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name), "Full name");
          public string Name => GetProperty(NameProperty)!;
        """,
        """
        public static readonly NotNullPropertyInfo<string> NameProperty = RegisterNotNullProperty<string>(nameof(Name), "Full name", string.Empty);
          public string Name => GetProperty(NameProperty);
        """);
    }

    [TestMethod]
    public async Task MigratesStringReplacingANullDefault()
    {
      await AssertFixAsync(
        """
        public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name), "Full name", null);
          public string Name => GetProperty(NameProperty)!;
        """,
        """
        public static readonly NotNullPropertyInfo<string> NameProperty = RegisterNotNullProperty<string>(nameof(Name), "Full name", string.Empty);
          public string Name => GetProperty(NameProperty);
        """);
    }

    [TestMethod]
    public async Task MigratesStringWithNamedArguments()
    {
      await AssertFixAsync(
        """
        public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(propertyName: nameof(Name), friendlyName: "Full name");
          public string Name => GetProperty(NameProperty)!;
        """,
        """
        public static readonly NotNullPropertyInfo<string> NameProperty = RegisterNotNullProperty<string>(propertyName: nameof(Name), friendlyName: "Full name", defaultValue: string.Empty);
          public string Name => GetProperty(NameProperty);
        """);
    }

    [TestMethod]
    public async Task MigratesStringRegisteredWithALambda()
    {
      await AssertFixAsync(
        """
        public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(_ => _.Name);
          public string Name => ReadProperty(NameProperty)!;
        """,
        """
        public static readonly NotNullPropertyInfo<string> NameProperty = RegisterNotNullProperty<string>(_ => _.Name, null, string.Empty);
          public string Name => ReadProperty(NameProperty);
        """);
    }

    [TestMethod]
    public async Task MigratesDroppingRelationshipNone()
    {
      await AssertFixAsync(
        """
        public static readonly PropertyInfo<int> CountProperty = RegisterProperty<int>(nameof(Count), "Count", 0, RelationshipTypes.None);
          public int Count => GetProperty(CountProperty);
        """,
        """
        public static readonly NotNullPropertyInfo<int> CountProperty = RegisterNotNullProperty<int>(nameof(Count), "Count", 0);
          public int Count => GetProperty(CountProperty);
        """);
    }

    [TestMethod]
    public async Task MigratesQualifiedType()
    {
      await AssertFixAsync(
        """
        public static readonly Csla.PropertyInfo<int> CountProperty = RegisterProperty<int>(nameof(Count));
          public int Count => GetProperty(CountProperty);
        """,
        """
        public static readonly Csla.NotNullPropertyInfo<int> CountProperty = RegisterNotNullProperty<int>(nameof(Count));
          public int Count => GetProperty(CountProperty);
        """);
    }

    [TestMethod]
    public async Task MigratesReferenceWithDefault()
    {
      await AssertFixAsync(
        """
        public static readonly PropertyInfo<Child> ItemProperty = RegisterProperty<Child>(nameof(Item), null, new Child());
          public Child Item => GetProperty(ItemProperty)!;
        """,
        """
        public static readonly NotNullPropertyInfo<Child> ItemProperty = RegisterNotNullProperty<Child>(nameof(Item), null, new Child());
          public Child Item => GetProperty(ItemProperty);
        """);
    }

    [TestMethod]
    public async Task MigratesReviewTierDroppingANullDefault()
    {
      await AssertFixAsync(
        """
        public static readonly PropertyInfo<Child> ItemProperty = RegisterProperty<Child>(nameof(Item), "Item", null);
          public Child Item { get => GetProperty(ItemProperty)!; private set => LoadProperty(ItemProperty, value); }
        """,
        """
        public static readonly NotNullPropertyInfo<Child> ItemProperty = RegisterNotNullProperty<Child>(nameof(Item), "Item");
          public Child Item { get => GetProperty(ItemProperty); private set => LoadProperty(ItemProperty, value); }
        """,
        Constants.AnalyzerIdentifiers.MigrateToNotNullPropertyWithReview);
    }

    [TestMethod]
    public async Task KeepsTheSuppressionOnConvertAndOtherProperties()
    {
      await AssertFixAsync(
        """
        public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
          public string Name => GetProperty(NameProperty)!;
          public string Upper => GetPropertyConvert<string, string>(NameProperty)!;
          public static readonly PropertyInfo<string?> NoteProperty = RegisterProperty<string?>(nameof(Note));
          public string Note => GetProperty(NoteProperty)!;
        """,
        """
        public static readonly NotNullPropertyInfo<string> NameProperty = RegisterNotNullProperty<string>(nameof(Name), null, string.Empty);
          public string Name => GetProperty(NameProperty);
          public string Upper => GetPropertyConvert<string, string>(NameProperty)!;
          public static readonly PropertyInfo<string?> NoteProperty = RegisterProperty<string?>(nameof(Note));
          public string Note => GetProperty(NoteProperty)!;
        """);
    }

    [TestMethod]
    public async Task MigratedStringHasADefaultValueAtRunTime()
    {
      var changed = await FixAsync(BusinessObjectWith(
        """
        public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
          public string Name => GetProperty(NameProperty)!;
        """), Constants.AnalyzerIdentifiers.MigrateToNotNullProperty);

      var compilation = await changed.Project.GetCompilationAsync();
      using var stream = new MemoryStream();
      var result = compilation.Emit(stream);
      Assert.IsTrue(result.Success, string.Join(Environment.NewLine, result.Diagnostics));

      var assembly = Assembly.Load(stream.ToArray());
      var info = assembly.GetType("A").GetField("NameProperty").GetValue(null);
      Assert.IsInstanceOfType<NotNullPropertyInfo<string>>(info);
      var notNull = (NotNullPropertyInfo<string>)info;
      Assert.IsTrue(notNull.HasDefaultValue, nameof(NotNullPropertyInfo<string>.HasDefaultValue));
      Assert.AreEqual(string.Empty, notNull.DefaultValue);
    }

    [TestMethod]
    public async Task FixAllInContainingTypeMigratesTheFixAllTierOnly()
    {
      var code = Header +
        """
        public class A : BusinessBase<A>
        {
          public static readonly PropertyInfo<int> CountProperty = RegisterProperty<int>(nameof(Count));
          public int Count => GetProperty(CountProperty);
          public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
          public string Name => GetProperty(NameProperty)!;
          public static readonly PropertyInfo<Child> ItemProperty = RegisterProperty<Child>(nameof(Item));
          public Child Item => GetProperty(ItemProperty)!;
        }

        public class B : BusinessBase<B>
        {
          public static readonly PropertyInfo<int> TotalProperty = RegisterProperty<int>(nameof(Total));
          public int Total => GetProperty(TotalProperty);
        }
        """;
      var document = TestHelpers.Create(code);
      var diagnostics = await GetMigrationDiagnosticsAsync(document);
      Assert.AreEqual(4, diagnostics.Count, string.Join(Environment.NewLine, diagnostics));
      var first = diagnostics.First(_ => _.Id == Constants.AnalyzerIdentifiers.MigrateToNotNullProperty &&
        _.Location.SourceSpan.Start == code.IndexOf("CountProperty =", StringComparison.Ordinal));

      var fix = new MigrateToNotNullPropertyCodeFix();
      var context = new FixAllContext(document, first.Location.SourceSpan, fix, FixAllScope.ContainingType,
        MigrateToNotNullPropertyCodeFixConstants.Title, [Constants.AnalyzerIdentifiers.MigrateToNotNullProperty],
        new TestDiagnosticProvider(diagnostics), default);
      var action = await fix.GetFixAllProvider().GetFixAsync(context);
      Assert.IsNotNull(action);
      var operations = await action.GetOperationsAsync(default);
      var changed = ((ApplyChangesOperation)operations.OfType<ApplyChangesOperation>().Single()).ChangedSolution.GetDocument(document.Id);
      var text = await GetTextAsync(changed);

      StringAssert.Contains(text, "NotNullPropertyInfo<int> CountProperty = RegisterNotNullProperty<int>(nameof(Count));");
      StringAssert.Contains(text, "NotNullPropertyInfo<string> NameProperty = RegisterNotNullProperty<string>(nameof(Name), null, string.Empty);");
      StringAssert.Contains(text, "public string Name => GetProperty(NameProperty);");
      // the review tier and the other type are left alone
      StringAssert.Contains(text, "PropertyInfo<Child> ItemProperty = RegisterProperty<Child>(nameof(Item));");
      StringAssert.Contains(text, "public Child Item => GetProperty(ItemProperty)!;");
      StringAssert.Contains(text, "PropertyInfo<int> TotalProperty = RegisterProperty<int>(nameof(Total));");
      await AssertCompilesCleanlyAsync(changed);
    }

    private sealed class TestDiagnosticProvider(List<Diagnostic> diagnostics) : FixAllContext.DiagnosticProvider
    {
      public override Task<IEnumerable<Diagnostic>> GetAllDiagnosticsAsync(Project project, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<Diagnostic>>(diagnostics);

      public override Task<IEnumerable<Diagnostic>> GetDocumentDiagnosticsAsync(Document document, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<Diagnostic>>(diagnostics);

      public override Task<IEnumerable<Diagnostic>> GetProjectDiagnosticsAsync(Project project, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<Diagnostic>>([]);
    }
  }
}
