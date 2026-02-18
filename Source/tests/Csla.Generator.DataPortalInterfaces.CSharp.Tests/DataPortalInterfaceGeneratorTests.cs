//-----------------------------------------------------------------------
// <copyright file="DataPortalInterfaceGeneratorTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
//-----------------------------------------------------------------------
using Csla.Generator.DataPortalInterfaces.CSharp.Tests.Helpers;

namespace Csla.Generator.DataPortalInterfaces.CSharp.Tests
{
  [TestClass]
  public class DataPortalInterfaceGeneratorTests : VerifyBase
  {
    [TestMethod("Single Create operation with no parameters")]
    public async Task SingleCreateNoParams()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public partial class PersonEdit : Csla.BusinessBase<PersonEdit>
          {
            [Create]
            private void Create() { }
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("Single Fetch operation with one criteria parameter")]
    public async Task SingleFetchWithCriteria()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public partial class PersonEdit : Csla.BusinessBase<PersonEdit>
          {
            [Fetch]
            private void Fetch(int id) { }
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("Operation method with Inject parameter")]
    public async Task OperationWithInjectParam()
    {
      var source = """
        using Csla;
        using System.Threading.Tasks;

        namespace TestApp
        {
          public interface IDal { }

          public partial class PersonEdit : Csla.BusinessBase<PersonEdit>
          {
            [Insert]
            private async Task Insert([Inject] IDal dal) { }
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("Operation method with Inject AllowNull parameter")]
    public async Task OperationWithInjectAllowNull()
    {
      var source = """
        using Csla;
        using System.Threading.Tasks;

        namespace TestApp
        {
          public interface ILogger { }

          public partial class PersonEdit : Csla.BusinessBase<PersonEdit>
          {
            [Fetch]
            private async Task Fetch(int id, [Inject(AllowNull = true)] ILogger? logger) { }
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("Multiple overloads of same Fetch operation")]
    public async Task MultipleOverloads()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public partial class PersonEdit : Csla.BusinessBase<PersonEdit>
          {
            [Fetch]
            private void Fetch(string name) { }

            [Fetch]
            private void Fetch(int id) { }
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("Method with both root and child attributes")]
    public async Task RootAndChildAttributes()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public partial class PersonEdit : Csla.BusinessBase<PersonEdit>
          {
            [Create]
            [CreateChild]
            private void Create() { }
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("Async method returning Task")]
    public async Task AsyncMethod()
    {
      var source = """
        using Csla;
        using System.Threading.Tasks;

        namespace TestApp
        {
          public partial class PersonEdit : Csla.BusinessBase<PersonEdit>
          {
            [Fetch]
            private async Task Fetch(int id) { await Task.CompletedTask; }
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("File-scoped namespace")]
    public async Task FileScopedNamespace()
    {
      var source = """
        using Csla;

        namespace TestApp;

        public partial class PersonEdit : Csla.BusinessBase<PersonEdit>
        {
          [Create]
          private void Create() { }

          [Fetch]
          private void Fetch(int id) { }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("Complete CRUD operations")]
    public async Task CompleteCrud()
    {
      var source = """
        using Csla;
        using System.Threading.Tasks;

        namespace TestApp
        {
          public interface IDal { }

          public partial class PersonEdit : Csla.BusinessBase<PersonEdit>
          {
            [Create]
            private void Create() { }

            [Fetch]
            private void Fetch(int id) { }

            [Insert]
            private async Task Insert([Inject] IDal dal) { await Task.CompletedTask; }

            [Update]
            private async Task Update([Inject] IDal dal) { await Task.CompletedTask; }

            [DeleteSelf]
            private async Task DeleteSelf([Inject] IDal dal) { await Task.CompletedTask; }
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("Delete operation with criteria")]
    public async Task DeleteWithCriteria()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public partial class PersonEdit : Csla.BusinessBase<PersonEdit>
          {
            [Delete]
            private void Delete(int id) { }
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("Child operations")]
    public async Task ChildOperations()
    {
      var source = """
        using Csla;
        using System.Threading.Tasks;

        namespace TestApp
        {
          public interface IDal { }

          public partial class LineItem : Csla.BusinessBase<LineItem>
          {
            [CreateChild]
            private void CreateChild() { }

            [FetchChild]
            private void FetchChild(int id) { }

            [InsertChild]
            private async Task InsertChild([Inject] IDal dal) { await Task.CompletedTask; }

            [UpdateChild]
            private async Task UpdateChild([Inject] IDal dal) { await Task.CompletedTask; }

            [DeleteSelfChild]
            private async Task DeleteSelfChild([Inject] IDal dal) { await Task.CompletedTask; }
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("Execute operation on command object")]
    public async Task ExecuteCommand()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public partial class MyCommand : Csla.CommandBase<MyCommand>
          {
            [Execute]
            private void Execute() { }
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("Nested class with data portal operations")]
    public async Task NestedClass()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public partial class Outer
          {
            public partial class PersonEdit : Csla.BusinessBase<PersonEdit>
            {
              [Fetch]
              private void Fetch(int id) { }
            }
          }
        }
        """;

      await TestHelperVerify(source);
    }

    private static async Task TestHelperVerify(string source, params string[]? additionalSources)
    {
      await TestHelper<IncrementalDataPortalInterfaceGenerator>.Verify(source, additionalSources);
    }
  }
}
