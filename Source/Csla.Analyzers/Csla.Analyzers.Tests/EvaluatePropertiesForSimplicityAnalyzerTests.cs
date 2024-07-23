﻿using Microsoft.CodeAnalysis;
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
  }
}