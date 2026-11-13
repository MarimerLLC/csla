using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
      Assert.AreEqual("Evaluate Properties for Simplicity", ctorHasParametersDiagnostic.Title.ToString(),
        nameof(DiagnosticDescriptor.Title));
      Assert.AreEqual("Properties that use managed backing fields should only use Get/Set/Read/Load methods and nothing else", ctorHasParametersDiagnostic.MessageFormat.ToString(),
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
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasAbstractProperty()
    {
      var code =
        """
        using Csla;

        public abstract class A : BusinessBase<A>
        {
          public abstract string Data { get; set; }
        }
        """;
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasStaticProperty()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          public static string Data { get; set; }
        }
        """;
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasGetterWithNoMethodCall()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          public string Data { get; }
        
          public string ExpressionData => string.Empty;
        }
        """;
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasGetterWithMethodCallButIsNotCslaPropertyMethod()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          public string Data { get { return this.GetProperty(); } }
        
          public string GetProperty() => null;
        }
        """;
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasGetterWithMethodCallAndMultipleStatements()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);
          private string _x;
        
          public string Data { get { _x = "44"; return this.GetProperty(DataProperty); } }
        
          public string GetX() { return _x; }
        }
        """;
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        code, [Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasGetterWithMethodCallAndReturnButNoDirectInvocationExpression()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);
          public string Data { get { return "x" + this.GetProperty(DataProperty); } }
        
          public static readonly PropertyInfo<string> ExpressionDataProperty = RegisterProperty<string>(_ => _.ExpressionData);
          public string ExpressionData => "x" + this.GetProperty(DataProperty);
        }
        """;
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(code,
      [
        Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule,
          Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule
      ]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasGetterWithMethodCallAndReturnAndDirectInvocationExpression()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);
          public string Data { get { return this.GetProperty(DataProperty); } }
        
          public static readonly PropertyInfo<string> ExpressionDataProperty = RegisterProperty<string>(_ => _.ExpressionData);
          public string ExpressionData => this.GetProperty(DataProperty);
        }
        """;
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasExpressionBodiedGetterWithPropertyMethodNestedInAnotherInvocation()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);
          public string Data => GetProperty(DataProperty).ToUpper();

          public static readonly PropertyInfo<string> CodeProperty = RegisterProperty<string>(_ => _.Code);
          public string Code => string.Concat("x", GetProperty(CodeProperty));
        }
        """;
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(code,
      [
        Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule,
        Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule
      ]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasGetAccessorExpressionWithPropertyMethodNestedInAnotherInvocation()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);
          public string Data { get => GetProperty(DataProperty).ToUpper(); }

          public static readonly PropertyInfo<string> CodeProperty = RegisterProperty<string>(_ => _.Code);
          public string Code { get => string.Concat("x", GetProperty(CodeProperty)); }
        }
        """;
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(code,
        [
          Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule,
          Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule
        ],
        diagnostics => AssertLocations(diagnostics,
          "=> GetProperty(DataProperty).ToUpper()",
          "=> string.Concat(\"x\", GetProperty(CodeProperty))"));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasGetAccessorBodyWithPropertyMethodNestedInAnotherInvocation()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);
          public string Data { get { return GetProperty(DataProperty).ToUpper(); } }

          public static readonly PropertyInfo<string> CodeProperty = RegisterProperty<string>(_ => _.Code);
          public string Code { get { return string.Concat("x", GetProperty(CodeProperty)); } }
        }
        """;
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(code,
        [
          Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule,
          Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule
        ],
        diagnostics => AssertLocations(diagnostics,
          "{ return GetProperty(DataProperty).ToUpper(); }",
          "{ return string.Concat(\"x\", GetProperty(CodeProperty)); }"));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasSetAccessorBodyWithPropertyMethodNestedInAnotherInvocation()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);
          public string Data
          {
            get => GetProperty(DataProperty);
            set { Run(() => SetProperty(DataProperty, value)); }
          }

          private void Run(System.Action action) => action();
        }
        """;
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(code,
        [Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule],
        diagnostics => AssertLocations(diagnostics,
          "{ Run(() => SetProperty(DataProperty, value)); }"));
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasPropertyMethodsWithPropertyMethodsInTheirArguments()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);
          public string Data => LazyGetProperty(DataProperty, () => ReadProperty(NameProperty));

          public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(_ => _.Name);
          public string Name
          {
            get { return GetProperty(NameProperty); }
            set { SetProperty(NameProperty, value ?? ReadProperty(NameProperty)); }
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(code, []);
    }

    private static void AssertLocations(List<Diagnostic> diagnostics, params string[] expectedLocations)
    {
      var actualLocations = diagnostics
        .Select(_ => _.Location.SourceTree.ToString().Substring(_.Location.SourceSpan.Start, _.Location.SourceSpan.Length))
        .OrderBy(_ => _, StringComparer.Ordinal)
        .ToArray();
      CollectionAssert.AreEqual(expectedLocations.OrderBy(_ => _, StringComparer.Ordinal).ToArray(), actualLocations);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasSetterWithNoMethodCall()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          public string Data { set { } }
        }
        """;
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasSetterWithMethodCallButIsNotCslaPropertyMethod()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          public string Data { set { this.SetProperty(); } }
        
          public void SetProperty() { }
        }
        """;
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasSetterWithMethodCallAndMultipleStatements()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);
          private string _x;
        
          public string Data { get { return null; } set { _x = "44"; this.SetProperty(DataProperty, value); } }
        
          public string GetX() { return _x; }
        }
        """;
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(
        code,
        [Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule,Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasSetterWithMethodCallAndDirectInvocationExpression()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);
        
          public string Data
          {
            get { return null; }
            set { this.SetProperty(DataProperty, value); }
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(code, [Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule]);
    }
    [TestMethod]
    public async Task AnalyzeWhenClassHasEmptyGetterAndSetter()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);
        
          public string Data
          {
            get;
            set;
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(code, [Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule, Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule]);
    }
    [TestMethod]
    public async Task AnalyzeWhenClassHasEmptyGetter()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);
        
          public string Data
          {
            get;
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(code, [Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule]);
    }
    [TestMethod]
    public async Task AnalyzeWhenClassHasPrivateField()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          public static readonly PropertyInfo<string> CityProperty = RegisterProperty<string>(nameof(City), RelationshipTypes.PrivateField);
          private string _city = CityProperty.DefaultValue;
          public string City
          {
              get => GetProperty(CityProperty, _city);
              set => SetProperty(CityProperty, ref _city, value);
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(code, []);
    }
    [TestMethod]
    public async Task AnalyzeWhenClassHasMethodExpression()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          public static readonly PropertyInfo<string> CityProperty = RegisterProperty<string>(nameof(City));

          public string City
          {
              get => GetProperty(CityProperty);
              set => SetProperty(CityProperty, value);
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(code, []);
    }
    [TestMethod]
    public async Task AnalyzeWhenClassHasGetterExpression()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          public static readonly PropertyInfo<string> CityProperty = RegisterProperty<string>(nameof(City));

          public string City => GetProperty(CityProperty);
        }
        """;
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(code, []);
    }
    [TestMethod]
    public async Task AnalyzeWhenClassBody()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          public static readonly PropertyInfo<string> CityProperty = RegisterProperty<string>(nameof(City));

          public string City
          {
              get { return GetProperty(CityProperty); }
              set { SetProperty(CityProperty, value); }
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(code, []);
    }
    [TestMethod]
    public async Task AnalyzeWhenGetterUsesNullForgivingOperator()
    {
      var code =
        """
        #nullable enable
        using Csla;

        public class A : BusinessBase<A>
        {
          public static readonly PropertyInfo<string> CityProperty = RegisterProperty<string>(nameof(City));
          public string City
          {
            get => GetProperty(CityProperty)!;
            set => SetProperty(CityProperty, value);
          }

          public static readonly PropertyInfo<string> StateProperty = RegisterProperty<string>(nameof(State));
          public string State
          {
            get { return GetProperty(StateProperty)!; }
            set { SetProperty(StateProperty, value); }
          }

          public static readonly PropertyInfo<string> CountryProperty = RegisterProperty<string>(nameof(Country));
          public string Country => ReadProperty(CountryProperty)!;

          public static readonly PropertyInfo<string> RegionProperty = RegisterProperty<string>(nameof(Region), RelationshipTypes.LazyLoad);
          public string Region => LazyGetProperty(RegionProperty, () => "North")!;
        }
        """;
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task AnalyzeWhenGetterUsesNullForgivingOperatorWithOtherLogic()
    {
      var code =
        """
        #nullable enable
        using Csla;

        public class A : BusinessBase<A>
        {
          public static readonly PropertyInfo<string> CityProperty = RegisterProperty<string>(nameof(City));
          public string City
          {
            get => ("x" + GetProperty(CityProperty))!;
            set => SetProperty(CityProperty, value);
          }

          public static readonly PropertyInfo<string> StateProperty = RegisterProperty<string>(nameof(State));
          public string State
          {
            get { return GetProperty(StateProperty)!.Trim(); }
            set { SetProperty(StateProperty, value); }
          }

          public static readonly PropertyInfo<string> CountryProperty = RegisterProperty<string>(nameof(Country));
          public string Country => "x" + GetProperty(CountryProperty)!;
        }
        """;
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(code,
      [
        Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule,
        Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule,
        Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule
      ]);
    }

    [TestMethod]
    public async Task AnalyzeWhenClassHasOnlySetMethodExpression()
    {
      var code =
        """
        using Csla;

        public class A : BusinessBase<A>
        {
          public static readonly PropertyInfo<string> CityProperty = RegisterProperty<string>(nameof(City));

          public string City
          {
              set => SetProperty(CityProperty, value);
          }
        }
        """;
      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(code, [Constants.AnalyzerIdentifiers.OnlyUseCslaPropertyMethodsInGetSetRule]);
    }
    [TestMethod]
    public async Task SkipWhenDeclaringTypeIsAnInterface()
    {
      var code =
        """
        using Csla;
        
        public class A : IFoo
        {
          public string City { get; set; }
        }

        public interface IFoo
        {
          public string City { get; set; }
        }

        """;

      await TestHelpers.RunAnalysisAsync<EvaluatePropertiesForSimplicityAnalyzer>(code, []);
    }
  }
}