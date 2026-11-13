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

  [CreateChild]
  private void FactoryCreateChild() { }

  [MyCreateChild]
  private void FactoryDerivedCreateChild() { }
}

[Csla.Server.ObjectFactory(""Factories.CustomerFactory, Factories"")]
public sealed partial class PartialFactoryCustomer : BusinessBase<PartialFactoryCustomer>
{
  [Fetch]
  private void PartialFactoryRootFetch(int id) { }

  [FetchChild]
  private void PartialFactoryChildFetch(int id) { }

  [MyFetchChild]
  private void PartialFactoryDerivedChildFetch(int id) { }
}

public sealed class CustomFactoryAttribute : Csla.Server.ObjectFactoryAttribute
{
  public CustomFactoryAttribute() : base(""Factories.CustomerFactory, Factories"") { }
}

[CustomFactory]
public sealed class CustomFactoryCustomer : BusinessBase<CustomFactoryCustomer>
{
  [Fetch]
  private void CustomFactoryRootFetch(int id) { }
}

public sealed class StaticOperation : BusinessBase<StaticOperation>
{
  [Fetch]
  private static void StaticFetch(int id) { }
}

public sealed class MyFetchChildAttribute : FetchChildAttribute { }

public sealed class MyCreateChildAttribute : CreateChildAttribute { }

public sealed class PublicOperations : BusinessBase<PublicOperations>
{
  [Fetch]
  public void PublicFetch(int id) { }
}

public sealed partial class PartialPublicOperations : BusinessBase<PartialPublicOperations>
{
  [Fetch]
  public void PartialPublicFetch(int id) { }
}

public sealed class NotInvoked : BusinessBase<NotInvoked>
{
  [DataPortalChildOperation]
  private void BaseAttributeLoad(int id) { }

  [Fetch]
  private void GenericFetch<TX>(int id) { }

  [MyFetchChild]
  private void DerivedFetchChild(int id) { }
}

public sealed class CustomCslaObject : Csla.Core.ICslaObject
{
  [Fetch]
  private void CustomObjectFetch(int id) { }
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

      foreach (var method in new[] { "Create", "Fetch", "Insert", "DeleteSelf", "FetchChild", "Execute", "Child_Create", "ProtectedBaseFetch", "PrivateBaseFetch", "FactoryCreateChild", "PartialFactoryChildFetch", "FactoryDerivedCreateChild", "PartialPublicFetch", "DerivedFetchChild", "CustomObjectFetch" })
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

      // Not an operation, a legacy name without an attribute, root operations on
      // [ObjectFactory] types (including a derived factory attribute), a child
      // operation on a factory type without a generated mapping, an operation
      // outside a CSLA object, a static operation, a public operation without a
      // generated mapping (reflection only binds non-public methods), a base
      // operation attribute the data portal never looks for, a generic method,
      // and a derived attribute the generator doesn't see on a factory type.
      foreach (var method in new[] { "Helper", "DataPortal_Update", "FactoryRootFetch", "FactoryChildFetch", "PartialFactoryRootFetch", "CustomFactoryRootFetch", "NotAStereotypeFetch", "StaticFetch", "PublicFetch", "BaseAttributeLoad", "GenericFetch", "PartialFactoryDerivedChildFetch" })
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
}

public sealed class NoParameters : BusinessBase<NoParameters>
{
  private static void AddObjectAuthorizationRules() { }
}

public abstract class AbstractRules<T> : BusinessBase<T>
  where T : AbstractRules<T>
{
  private static void AddObjectAuthorizationRules(IAddObjectAuthorizationRulesContext abstractContext) { }
}

public sealed class Overloaded : BusinessBase<Overloaded>
{
  private static void AddObjectAuthorizationRules() { }

  private static void AddObjectAuthorizationRules(IAddObjectAuthorizationRulesContext overloadContext) { }
}

public sealed class WrongSignature : BusinessBase<WrongSignature>
{
  private static void AddObjectAuthorizationRules(int wrongParameter) { }
}";
      var unusedMembers = await RunAsync(code, new UnusedPrivateMemberStubAnalyzer());

      // Every AddObjectAuthorizationRules method in the type must be suppressed (or none).
      bool IsSuppressedIn(string typeName)
      {
        var matches = unusedMembers.Where(_ => _.Location.SourceTree.GetRoot()
          .FindNode(_.Location.SourceSpan).Ancestors()
          .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax>().First().Identifier.Text == typeName &&
          _.Location.SourceTree.ToString().Substring(_.Location.SourceSpan.Start, _.Location.SourceSpan.Length) == "AddObjectAuthorizationRules")
          .ToList();
        Assert.IsTrue(matches.Count > 0, typeName);
        Assert.IsTrue(matches.All(_ => _.IsSuppressed) || matches.All(_ => !_.IsSuppressed), typeName);
        return matches[0].IsSuppressed;
      }

      Assert.IsTrue(IsSuppressedIn("NoParameters"), "NoParameters");
      // Type.GetMethod on the concrete type doesn't return private static methods of a base type.
      Assert.IsFalse(IsSuppressedIn("AbstractRules"), "AbstractRules");
      // Type.GetMethod throws when the name is overloaded.
      Assert.IsFalse(IsSuppressedIn("Overloaded"), "Overloaded");
      // CSLA calls the method with no arguments or with the context only.
      Assert.IsFalse(IsSuppressedIn("WrongSignature"), "WrongSignature");

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
