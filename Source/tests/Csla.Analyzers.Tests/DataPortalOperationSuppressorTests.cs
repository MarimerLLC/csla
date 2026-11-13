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
}";

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
    public async Task SuppressUnusedPrivateMemberOnOperations()
    {
      var diagnostics = await TestHelpers.GetDiagnosticsWithSuppressorAsync(
        OperationsCode, new UnusedPrivateMemberStubAnalyzer(), new DataPortalOperationSuppressor());

      Assert.IsTrue(diagnostics.IsSuppressedAt("Create"), "Create");
      Assert.IsTrue(diagnostics.IsSuppressedAt("Fetch"), "Fetch");
      Assert.IsTrue(diagnostics.IsSuppressedAt("Insert"), "Insert");
      Assert.IsTrue(diagnostics.IsSuppressedAt("DeleteSelf"), "DeleteSelf");
      Assert.IsTrue(diagnostics.IsSuppressedAt("FetchChild"), "FetchChild");
      Assert.IsTrue(diagnostics.IsSuppressedAt("Execute"), "Execute");
      Assert.IsTrue(diagnostics.IsSuppressedAt("Child_Create"), "Child_Create");
      Assert.IsFalse(diagnostics.IsSuppressedAt("Helper"), "Helper");
      Assert.IsFalse(diagnostics.IsSuppressedAt("DataPortal_Update"), "DataPortal_Update");
    }

    [TestMethod]
    public async Task SuppressMarkMembersAsStaticOnOperations()
    {
      var diagnostics = await TestHelpers.GetDiagnosticsWithSuppressorAsync(
        OperationsCode, new MarkMembersAsStaticStubAnalyzer(), new DataPortalOperationSuppressor());

      Assert.IsTrue(diagnostics.IsSuppressedAt("Create"), "Create");
      Assert.IsTrue(diagnostics.IsSuppressedAt("Fetch"), "Fetch");
      Assert.IsTrue(diagnostics.IsSuppressedAt("Execute"), "Execute");
      Assert.IsTrue(diagnostics.IsSuppressedAt("Child_Create"), "Child_Create");
      Assert.IsFalse(diagnostics.IsSuppressedAt("Helper"), "Helper");
      Assert.IsFalse(diagnostics.IsSuppressedAt("DataPortal_Update"), "DataPortal_Update");
    }

    [TestMethod]
    public async Task DoNotSuppressUnrelatedDiagnosticOnOperation()
    {
      var diagnostics = await TestHelpers.GetDiagnosticsWithSuppressorAsync(
        OperationsCode, new UnrelatedStubAnalyzer(), new DataPortalOperationSuppressor());

      Assert.IsTrue(diagnostics.Count > 0);
      Assert.IsFalse(diagnostics.Any(_ => _.IsSuppressed));
    }

    [TestMethod]
    public async Task SuppressUnusedParameterOnOperations()
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
      var diagnostics = await TestHelpers.GetDiagnosticsWithSuppressorAsync(
        code, new UnusedParameterStubAnalyzer(), new DataPortalOperationSuppressor());

      Assert.IsTrue(diagnostics.IsSuppressedAt("overloadMarker"), "overloadMarker");
      Assert.IsTrue(diagnostics.IsSuppressedAt("id"), "id");
      Assert.IsTrue(diagnostics.IsSuppressedAt("dal"), "dal");
      Assert.IsFalse(diagnostics.IsSuppressedAt("lambdaParameter"), "lambdaParameter");
      Assert.IsFalse(diagnostics.IsSuppressedAt("localParameter"), "localParameter");
      Assert.IsFalse(diagnostics.IsSuppressedAt("helperParameter"), "helperParameter");
      Assert.IsFalse(diagnostics.IsSuppressedAt("constructorParameter"), "constructorParameter");
    }

    [TestMethod]
    public async Task SuppressOnObjectAuthorizationRulesMethods()
    {
      var code =
@"using Csla;
using Csla.Rules;

public sealed class ByName : BusinessBase<ByName>
{
  private static void AddObjectAuthorizationRules(IAddObjectAuthorizationRulesContext byNameContext) { }
}

public sealed class ByAttribute : BusinessBase<ByAttribute>
{
  [ObjectAuthorizationRules]
  public static void SetupRules(IAddObjectAuthorizationRulesContext context) { }
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
      var unusedMembers = await TestHelpers.GetDiagnosticsWithSuppressorAsync(
        code, new UnusedPrivateMemberStubAnalyzer(), new DataPortalOperationSuppressor());
      var byName = unusedMembers.Where(_ => _.Location.GetLineSpan().StartLinePosition.Line == 5).ToList();

      Assert.AreEqual(1, byName.Count);
      Assert.IsTrue(byName[0].IsSuppressed, "AddObjectAuthorizationRules");
      Assert.IsTrue(unusedMembers.IsSuppressedAt("SetupRules"), "SetupRules");
      // AuthorizationRuleManager only finds public methods by attribute, so this one is really unused.
      Assert.IsFalse(unusedMembers.IsSuppressedAt("NeverCalledRules"), "NeverCalledRules");
      Assert.IsFalse(unusedMembers.Single(_ => _.Location.GetLineSpan().StartLinePosition.Line == 22).IsSuppressed, "NotAStereotype");

      var unusedParameters = await TestHelpers.GetDiagnosticsWithSuppressorAsync(
        code, new UnusedParameterStubAnalyzer(), new DataPortalOperationSuppressor());

      Assert.IsTrue(unusedParameters.IsSuppressedAt("byNameContext"), "byNameContext");
      Assert.IsFalse(unusedParameters.IsSuppressedAt("notAStereotypeContext"), "notAStereotypeContext");
    }
  }
}
