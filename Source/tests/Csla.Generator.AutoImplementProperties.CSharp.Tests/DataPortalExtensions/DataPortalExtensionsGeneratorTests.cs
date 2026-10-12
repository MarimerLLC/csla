//-----------------------------------------------------------------------
// <copyright file="DataPortalExtensionsGeneratorTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
//-----------------------------------------------------------------------
using Csla.Generator.AutoImplementProperties.CSharp.DataPortalExtensions;
using Csla.Generator.AutoImplementProperties.CSharp.DataPortalOperations;
using Csla.Generator.AutoImplementProperties.CSharp.Tests.DataPortalOperations;

namespace Csla.Generator.AutoImplementProperties.CSharp.Tests.DataPortalExtensions
{
  [TestClass]
  public class DataPortalExtensionsGeneratorTests : VerifyBase
  {
    [TestMethod("Async and sync extensions for root operations")]
    public async Task RootOperations()
    {
      var source = """
        using Csla;
        using System.Threading.Tasks;

        namespace TestApp
        {
          public interface IDal { }

          [DataPortalExtensions]
          public partial class PersonEdit : BusinessBase<PersonEdit>
          {
            [Create]
            private void New() { }

            [Fetch]
            private async Task GetById(int id, [Inject] IDal dal) { await Task.CompletedTask; }

            [Delete]
            private Task RemoveAsync(int id) => Task.CompletedTask;

            [RunLocal]
            [Fetch]
            private void GetLocal([Inject] IDal dal, string code) { }
          }
        }
        """;

      await Verify(source);
    }

    [TestMethod("Extensions for child operations")]
    public async Task ChildOperations()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          [DataPortalExtensions]
          public partial class LineItem : BusinessBase<LineItem>
          {
            [CreateChild]
            private void NewChild() { }

            [FetchChild]
            private void LoadChild(int id, string name) { }
          }
        }
        """;

      await Verify(source);
    }

    [TestMethod("Execute operation on a command object")]
    public async Task ExecuteCommand()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          [DataPortalExtensions]
          public class RecalculateCommand : CommandBase<RecalculateCommand>
          {
            [Execute]
            private void Run(int count) { }
          }
        }
        """;

      await Verify(source);
    }

    [TestMethod("Prefix gives extensions distinct names")]
    public async Task Prefix()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          [DataPortalExtensions(Prefix = "Portal")]
          public partial class PersonEdit : BusinessBase<PersonEdit>
          {
            [Create]
            private void Create() { }

            [Fetch]
            private void Fetch(int id) { }
          }
        }
        """;

      await Verify(source);
    }

    [TestMethod("Names that would be hidden by data portal instance methods are reported")]
    public async Task HiddenNames()
    {
      var source = """
        using Csla;
        using System.Threading.Tasks;

        namespace TestApp
        {
          [DataPortalExtensions]
          public partial class PersonEdit : BusinessBase<PersonEdit>
          {
            [Fetch]
            private void Fetch(int id) { }

            [Fetch]
            private Task FetchAsync(string name) => Task.CompletedTask;

            [Fetch]
            private void GetByCode(string code, int version) { }
          }
        }
        """;

      await VerifyWithDiagnostics(source);
    }

    [TestMethod("Hidden names are checked against the receiving portal interface only")]
    public async Task ReceiverSpecificHiddenNames()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          [DataPortalExtensions]
          public partial class PersonEdit : BusinessBase<PersonEdit>
          {
            [Create]
            private void CreateChild(int id) { }

            [FetchChild]
            private void Fetch(int id) { }
          }
        }
        """;

      await Verify(source);
    }

    [TestMethod("Keyword method names and parameters colliding with generated names")]
    public async Task KeywordAndCollidingNames()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          [DataPortalExtensions]
          public partial class PersonEdit : BusinessBase<PersonEdit>
          {
            [Fetch]
            private void @class(int portal, int __portal, int __invoker) { }
          }
        }
        """;

      await Verify(source);
    }

    [TestMethod("A prefix that is not a valid identifier is reported")]
    public async Task InvalidPrefix()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          [DataPortalExtensions(Prefix = "My-")]
          public partial class PersonEdit : BusinessBase<PersonEdit>
          {
            [Fetch]
            private void GetById(int id) { }
          }
        }
        """;

      await VerifyWithDiagnostics(source);
    }

    [TestMethod("NoDataPortalExtension excludes a method")]
    public async Task NoDataPortalExtension()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          [DataPortalExtensions]
          public partial class PersonEdit : BusinessBase<PersonEdit>
          {
            [Fetch]
            private void GetById(int id) { }

            [NoDataPortalExtension]
            [Fetch]
            private void GetByName(string name) { }
          }
        }
        """;

      await Verify(source);
    }

    [TestMethod("Internal parameter types make the extension internal and private types are reported")]
    public async Task ParameterAccessibility()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          internal class InternalCriteria { }

          [DataPortalExtensions]
          public partial class PersonEdit : BusinessBase<PersonEdit>
          {
            private class PrivateCriteria { }

            [Fetch]
            private void GetByInternal(InternalCriteria criteria) { }

            [Fetch]
            private void GetByPrivate(PrivateCriteria criteria) { }
          }
        }
        """;

      await VerifyWithDiagnostics(source);
    }

    [TestMethod("Default parameter values including enums")]
    public async Task DefaultValues()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public enum Color { Red, Green = 5 }

          [DataPortalExtensions]
          public partial class PersonEdit : BusinessBase<PersonEdit>
          {
            [Fetch]
            private void GetById(int id = 5, string? name = "x\"y", Color color = Color.Green, decimal amount = 1.5m, double? ratio = null, float scale = 2.25f, long big = -3, bool active = true, char marker = 'a') { }
          }
        }
        """;

      await Verify(source);
    }

    [TestMethod("Array and params criteria")]
    public async Task ArrayCriteria()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          [DataPortalExtensions]
          public partial class PersonEdit : BusinessBase<PersonEdit>
          {
            [Fetch]
            private void GetByNames(string[] names) { }

            [Create]
            private void NewWith(params int[] values) { }
          }
        }
        """;

      await Verify(source);
    }

    [TestMethod("Nested business type")]
    public async Task NestedType()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public partial class Outer
          {
            [DataPortalExtensions]
            public partial class PersonEdit : BusinessBase<PersonEdit>
            {
              [Fetch]
              private void GetById(int id) { }
            }
          }
        }
        """;

      await Verify(source);
    }

    [TestMethod("Business types mapping to the same extension class name merge into one partial class")]
    public async Task ExtensionClassNameCollision()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          [DataPortalExtensions]
          public partial class Outer_Inner : BusinessBase<Outer_Inner>
          {
            [Fetch]
            private void GetById(int id) { }
          }

          internal partial class Outer
          {
            [DataPortalExtensions]
            public partial class Inner : BusinessBase<Inner>
            {
              [Fetch]
              private void GetById(int id) { }
            }
          }
        }
        """;

      await Verify(source);
    }

    [TestMethod("Internal business type generates an internal extension class")]
    public async Task InternalType()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          [DataPortalExtensions]
          internal partial class PersonEdit : BusinessBase<PersonEdit>
          {
            [Fetch]
            private void GetById(int id) { }
          }
        }
        """;

      await Verify(source);
    }

    [TestMethod("Generic business type is skipped")]
    public async Task GenericTypeSkipped()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          [DataPortalExtensions]
          public partial class Lookup<TKey> : ReadOnlyBase<Lookup<TKey>>
          {
            [Fetch]
            private void GetByKey(TKey key) { }
          }
        }
        """;

      await VerifyWithDiagnostics(source);
    }

    [TestMethod("Abstract and non-CSLA types are reported")]
    public async Task InvalidTargets()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          [DataPortalExtensions]
          public abstract partial class PersonBase : BusinessBase<PersonBase>
          {
            [Fetch]
            private void GetById(int id) { }
          }

          [DataPortalExtensions]
          public class NotABusinessObject
          {
            [Fetch]
            private void GetById(int id) { }
          }
        }
        """;

      await VerifyWithDiagnostics(source);
    }

    [TestMethod("Operations producing the same extension signature are reported")]
    public async Task DuplicateExtension()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          [DataPortalExtensions]
          public partial class PersonEdit : BusinessBase<PersonEdit>
          {
            [Fetch]
            private void Get(int id) { }

            [Create]
            private void Get(int id, [Inject] System.IServiceProvider services) { }
          }
        }
        """;

      await VerifyWithDiagnostics(source);
    }

    [TestMethod("Pipeline stages are cached when the compilation does not change")]
    public void StageCaching()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          [DataPortalExtensions(Prefix = "Portal")]
          public partial class PersonEdit : BusinessBase<PersonEdit>
          {
            [Create]
            private void Create() { }

            [FetchChild]
            [Fetch]
            private void Fetch(int id) { }
          }
        }
        """;

      StageCachingTester.VerifyStageCaching<IncrementalDataPortalExtensionsGenerator>(source, typeof(TrackingNames));
    }

    [TestMethod("Only async extensions are generated when CslaGenerateSyncDataPortalExtensions is false")]
    public async Task AsyncOnly()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          [DataPortalExtensions]
          public partial class PersonEdit : BusinessBase<PersonEdit>
          {
            [Create]
            private void Create() { }

            [Fetch]
            private void GetById(int id) { }

            [Delete]
            private void Remove(int id) { }

            [FetchChild]
            private void LoadChild(int id) { }

            [Fetch]
            private void Fetch(string name) { }
          }
        }
        """;

      await VerifyWithDiagnostics(source, SyncDisabled);
    }

    [TestMethod("The assembly attribute requests extensions for every eligible type")]
    public async Task AssemblyAttribute()
    {
      var source = """
        using Csla;

        [assembly: DataPortalExtensions(Prefix = "Portal")]

        namespace TestApp
        {
          public partial class PersonEdit : BusinessBase<PersonEdit>
          {
            [Fetch]
            private void Fetch(int id) { }
          }

          [DataPortalExtensions(Prefix = "My")]
          public partial class OrderEdit : BusinessBase<OrderEdit>
          {
            [Create]
            private void Create() { }
          }

          [DataPortalExtensions]
          public partial class CustomerEdit : BusinessBase<CustomerEdit>
          {
            [Fetch]
            private void GetById(int id) { }
          }

          [NoDataPortalExtension]
          public partial class Excluded : BusinessBase<Excluded>
          {
            [Fetch]
            private void Fetch(int id) { }
          }

          public abstract partial class PersonBase : BusinessBase<PersonBase>
          {
            [Fetch]
            private void Fetch(int id) { }
          }

          public partial class Lookup<TKey> : ReadOnlyBase<Lookup<TKey>>
          {
            [Fetch]
            private void Fetch(TKey key) { }
          }

          public class NotABusinessObject
          {
            [Fetch]
            private void Fetch(int id) { }
          }
        }
        """;

      await VerifyWithDiagnostics(source);
    }

    [TestMethod("An invalid assembly prefix is reported on the assembly attribute")]
    public async Task AssemblyAttributeInvalidPrefix()
    {
      var source = """
        using Csla;

        [assembly: DataPortalExtensions(Prefix = "My-")]

        namespace TestApp
        {
          public partial class PersonEdit : BusinessBase<PersonEdit>
          {
            [Fetch]
            private void GetById(int id) { }
          }

          public partial class OrderEdit : BusinessBase<OrderEdit>
          {
            [Fetch]
            private void GetById(int id) { }
          }
        }
        """;

      await VerifyWithDiagnostics(source);
    }

    [TestMethod("CslaDataPortalExtensionsAsyncSuffix sets the async method name suffix")]
    public async Task CustomAsyncSuffix()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          [DataPortalExtensions]
          public partial class PersonEdit : BusinessBase<PersonEdit>
          {
            [Fetch]
            private void GetById(int id) { }
          }
        }
        """;

      await VerifyWithDiagnostics(source, Options(("CslaDataPortalExtensionsAsyncSuffix", "Task")));
    }

    [TestMethod("With no async suffix and sync methods turned off, async methods have no suffix")]
    public async Task NoAsyncSuffix()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          [DataPortalExtensions]
          public partial class PersonEdit : BusinessBase<PersonEdit>
          {
            [Fetch]
            private void GetById(int id) { }

            [FetchChild]
            private System.Threading.Tasks.Task LoadAsync(int id) => System.Threading.Tasks.Task.CompletedTask;
          }
        }
        """;

      await VerifyWithDiagnostics(source, Options(("CslaDataPortalExtensionsAsyncSuffix", "none"), ("CslaGenerateSyncDataPortalExtensions", "false")));
    }

    [TestMethod("With no async suffix, sync methods are not generated and a warning is reported")]
    public async Task NoAsyncSuffixWithSync()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          [DataPortalExtensions]
          public partial class PersonEdit : BusinessBase<PersonEdit>
          {
            [Fetch]
            private void GetById(int id) { }
          }
        }
        """;

      await VerifyWithDiagnostics(source, Options(("CslaDataPortalExtensionsAsyncSuffix", "none")));
    }

    [TestMethod("An invalid async suffix is reported and the Async suffix is used")]
    public async Task InvalidAsyncSuffix()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          [DataPortalExtensions]
          public partial class PersonEdit : BusinessBase<PersonEdit>
          {
            [Fetch]
            private void GetById(int id) { }
          }
        }
        """;

      await VerifyWithDiagnostics(source, Options(("CslaDataPortalExtensionsAsyncSuffix", "As-ync")));
    }

    private static TestAnalyzerConfigOptionsProvider Options(params (string Name, string Value)[] properties)
      => new(properties.ToDictionary(p => "build_property." + p.Name, p => p.Value));

    private static TestAnalyzerConfigOptionsProvider SyncDisabled { get; } =
      new(new Dictionary<string, string> { ["build_property.CslaGenerateSyncDataPortalExtensions"] = "false" });

    private static Task Verify(string source, [System.Runtime.CompilerServices.CallerFilePath] string sourceFile = "")
      => DataPortalOperationsTestHelper<IncrementalDataPortalExtensionsGenerator>.Verify(source, sourceFile: sourceFile);

    private static Task VerifyWithDiagnostics(string source, TestAnalyzerConfigOptionsProvider? optionsProvider = null,
      [System.Runtime.CompilerServices.CallerFilePath] string sourceFile = "")
      => DataPortalOperationsTestHelper<IncrementalDataPortalExtensionsGenerator>.VerifyWithDiagnostics(source, optionsProvider: optionsProvider, sourceFile: sourceFile);
  }
}
