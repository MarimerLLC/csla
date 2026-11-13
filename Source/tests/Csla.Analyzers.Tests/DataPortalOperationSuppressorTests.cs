using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class DataPortalOperationSuppressorTests
  {
    private const string OperationsCode =
@"using Csla;

public sealed class Customer : BusinessBase<Customer>
{
  [Create]
  private void Create() { }

  [Fetch]
  private void Fetch(int id) { }

  [Insert]
  private void Insert() { }

  [DeleteSelf]
  private void DeleteSelf() { }

  [FetchChild]
  private void FetchChild(int id) { }

  private void Helper() { }

  private void DataPortal_Update() { }
}

public sealed class Command : CommandBase<Command>
{
  [Execute]
  private void Execute() { }
}

public sealed class Items : BusinessListBase<Items, Customer>
{
  protected override void Child_Create() { }
}

public abstract class CustomerBase<T> : BusinessBase<T>
  where T : CustomerBase<T>
{
  [Fetch]
  private void PrivateBaseFetch(int id) { }

  [Fetch]
  protected void ProtectedBaseFetch(string name) { }
}

[Csla.Server.ObjectFactory(""Factories.CustomerFactory, Factories"")]
public sealed class FactoryCustomer : BusinessBase<FactoryCustomer>
{
  [Fetch]
  private void FactoryRootFetch(int id) { }

  [FetchChild]
  private void FactoryChildFetch(int id) { }
}

public sealed class NotAStereotype
{
  [Fetch]
  private void NotAStereotypeFetch(int id) { }
}";

    private static Task<List<Diagnostic>> RunAsync(string code, DiagnosticAnalyzer analyzer) =>
      TestHelpers.GetDiagnosticsWithSuppressorAsync(code, analyzer, new DataPortalOperationSuppressor());

    [TestMethod]
    public void VerifySupportedSuppressions()
    {
      var suppressions = new DataPortalOperationSuppressor().SupportedSuppressions;
      Assert.AreEqual(3, suppressions.Length);

      Assert.AreEqual(Constants.SuppressorIdentifiers.UnusedOperation, suppressions[0].Id);
      Assert.AreEqual("IDE0051", suppressions[0].SuppressedDiagnosticId);
      Assert.AreEqual(Constants.SuppressorIdentifiers.OperationCanBeStatic, suppressions[1].Id);
      Assert.AreEqual("CA1822", suppressions[1].SuppressedDiagnosticId);
      Assert.AreEqual(Constants.SuppressorIdentifiers.UnusedOperationParameter, suppressions[2].Id);
      Assert.AreEqual("IDE0060", suppressions[2].SuppressedDiagnosticId);

      foreach (var suppression in suppressions)
      {
        Assert.IsFalse(string.IsNullOrWhiteSpace(suppression.Justification.ToString()), suppression.Id);
      }
    }

    [TestMethod]
    [DataRow("IDE0051")]
    [DataRow("CA1822")]
    public async Task SuppressOnCalledOperations(string diagnosticId)
    {
      DiagnosticAnalyzer analyzer = diagnosticId == "CA1822"
        ? new MarkMembersAsStaticStubAnalyzer() : new UnusedPrivateMemberStubAnalyzer();
      var diagnostics = await RunAsync(OperationsCode, analyzer);

      foreach (var method in new[] { "Create", "Fetch", "Insert", "DeleteSelf", "FetchChild", "Execute", "Child_Create", "ProtectedBaseFetch", "FactoryChildFetch" })
      {
        Assert.IsTrue(diagnostics.IsSuppressedAt(method), method);
      }
    }

    [TestMethod]
    [DataRow("IDE0051")]
    [DataRow("CA1822")]
    public async Task DoNotSuppressOnMethodsTheDataPortalDoesNotCall(string diagnosticId)
    {
      DiagnosticAnalyzer analyzer = diagnosticId == "CA1822"
        ? new MarkMembersAsStaticStubAnalyzer() : new UnusedPrivateMemberStubAnalyzer();
      var diagnostics = await RunAsync(OperationsCode, analyzer);

      // Not an operation, a legacy name without an attribute, a private operation
      // in an abstract base type, a root operation on an [ObjectFactory] type,
      // and an operation outside a CSLA business type.
      foreach (var method in new[] { "Helper", "DataPortal_Update", "PrivateBaseFetch", "FactoryRootFetch", "NotAStereotypeFetch" })
      {
        Assert.IsFalse(diagnostics.IsSuppressedAt(method), method);
      }
    }

    [TestMethod]
    public async Task DoNotSuppressUnrelatedDiagnosticOnOperation()
    {
      var diagnostics = await RunAsync(OperationsCode, new UnrelatedStubAnalyzer());

      Assert.IsTrue(diagnostics.Count > 0);
      Assert.IsFalse(diagnostics.Any(_ => _.IsSuppressed));
    }

    [TestMethod]
    public async Task SuppressUnusedCriteriaParameterOnOperations()
    {
      var code =
@"using System;
using Csla;

public interface IDal { }

public sealed class Customer : BusinessBase<Customer>
{
  [Create]
  private void Create(bool overloadMarker) { }

  [Fetch]
  private void Fetch(int id, [Inject] IDal dal)
  {
    Func<int, int> lambda = lambdaParameter => 0;
    int Local(int localParameter) => 0;
  }

  private void Helper(int helperParameter) { }

  public Customer(int constructorParameter) { }
}";
      var diagnostics = await RunAsync(code, new UnusedParameterStubAnalyzer());

      Assert.IsTrue(diagnostics.IsSuppressedAt("overloadMarker"), "overloadMarker");
      Assert.IsTrue(diagnostics.IsSuppressedAt("id"), "id");
      // [Inject] parameters don't take part in choosing the operation.
      Assert.IsFalse(diagnostics.IsSuppressedAt("dal"), "dal");
      Assert.IsFalse(diagnostics.IsSuppressedAt("lambdaParameter"), "lambdaParameter");
      Assert.IsFalse(diagnostics.IsSuppressedAt("localParameter"), "localParameter");
      Assert.IsFalse(diagnostics.IsSuppressedAt("helperParameter"), "helperParameter");
      Assert.IsFalse(diagnostics.IsSuppressedAt("constructorParameter"), "constructorParameter");
    }

    [TestMethod]
    public async Task SuppressUnusedMemberOnCalledObjectAuthorizationRulesMethods()
    {
      var code =
@"using Csla;
using Csla.Rules;

public sealed class DerivedRulesAttribute : ObjectAuthorizationRulesAttribute { }

public sealed class ByName : BusinessBase<ByName>
{
  private static void AddObjectAuthorizationRules(IAddObjectAuthorizationRulesContext byNameContext) { }
}

public sealed class ByNameWithDerivedAttributeMethod : BusinessBase<ByNameWithDerivedAttributeMethod>
{
  [DerivedRules]
  public static void DerivedAttributeRules(IAddObjectAuthorizationRulesContext context) { }

  private static void AddObjectAuthorizationRules(IAddObjectAuthorizationRulesContext derivedContext) { }
}

public sealed class Shadowed : BusinessBase<Shadowed>
{
  [ObjectAuthorizationRules]
  public static void SetupRules(IAddObjectAuthorizationRulesContext context) { }

  private static void AddObjectAuthorizationRules(IAddObjectAuthorizationRulesContext shadowedContext) { }
}

public sealed class PrivateByAttribute : BusinessBase<PrivateByAttribute>
{
  [ObjectAuthorizationRules]
  private static void NeverCalledRules(IAddObjectAuthorizationRulesContext context) { }
}

public sealed class NotAStereotype
{
  private static void AddObjectAuthorizationRules(IAddObjectAuthorizationRulesContext notAStereotypeContext) { }
}";
      var unusedMembers = await RunAsync(code, new UnusedPrivateMemberStubAnalyzer());

      bool IsSuppressedIn(string typeName) =>
        unusedMembers.Single(_ => _.Location.SourceTree.GetRoot()
          .FindNode(_.Location.SourceSpan).Ancestors()
          .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax>().First().Identifier.Text == typeName &&
          _.Location.SourceTree.ToString().Substring(_.Location.SourceSpan.Start, _.Location.SourceSpan.Length) == "AddObjectAuthorizationRules")
          .IsSuppressed;

      Assert.IsTrue(IsSuppressedIn("ByName"), "ByName");
      // AuthorizationRuleManager matches the exact attribute type, so a derived attribute doesn't hide the by-name method.
      Assert.IsTrue(IsSuppressedIn("ByNameWithDerivedAttributeMethod"), "ByNameWithDerivedAttributeMethod");
      // A public [ObjectAuthorizationRules] method runs instead of the by-name method.
      Assert.IsFalse(IsSuppressedIn("Shadowed"), "Shadowed");
      Assert.IsFalse(IsSuppressedIn("NotAStereotype"), "NotAStereotype");
      Assert.IsTrue(unusedMembers.IsSuppressedAt("SetupRules"), "SetupRules");
      // AuthorizationRuleManager only finds public methods by attribute, so this one is really unused.
      Assert.IsFalse(unusedMembers.IsSuppressedAt("NeverCalledRules"), "NeverCalledRules");

      // The context parameter is optional at run time, so an unused one is a true positive.
      var unusedParameters = await RunAsync(code, new UnusedParameterStubAnalyzer());
      Assert.IsFalse(unusedParameters.IsSuppressedAt("byNameContext"), "byNameContext");
    }
  }
}
