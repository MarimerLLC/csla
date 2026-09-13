//-----------------------------------------------------------------------
// <copyright file="DataPortalOperationsGeneratorTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
//-----------------------------------------------------------------------
using Csla.Generator.AutoImplementProperties.CSharp.DataPortalOperations;

namespace Csla.Generator.AutoImplementProperties.CSharp.Tests.DataPortalOperations
{
  [TestClass]
  public class DataPortalOperationsGeneratorTests : VerifyBase
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

    [TestMethod("Inject parameter declared before criteria keeps parameter order")]
    public async Task InjectBeforeCriteria()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public interface IDal { }

          public partial class PersonEdit : Csla.BusinessBase<PersonEdit>
          {
            [Fetch]
            private void Fetch([Inject] IDal dal, int id, [Inject(AllowNull = true)] IDal? backup, string name) { }
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("Array, nullable, generic and tuple criteria")]
    public async Task ArrayNullableGenericCriteria()
    {
      var source = """
        using Csla;
        using System.Collections.Generic;

        namespace TestApp
        {
          public partial class PersonEdit : Csla.BusinessBase<PersonEdit>
          {
            [Fetch]
            private void Fetch(int[] ids) { }

            [Fetch]
            private void Fetch(int? id, string? name) { }

            [Fetch]
            private void Fetch(List<int> ids) { }

            [Fetch]
            private void Fetch((int Id, string Name) key) { }

            [Fetch]
            private void Fetch(Dictionary<string, List<int?>>[] map) { }
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("Partial class with operation methods spread across two files")]
    public async Task PartialClassAcrossFiles()
    {
      var source1 = """
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

      var source2 = """
        using Csla;

        namespace TestApp
        {
          public partial class PersonEdit
          {
            [Fetch]
            private void Fetch(int id) { }
          }
        }
        """;

      await TestHelperVerify(source1, source2);
    }

    [TestMethod("Abstract class gets the operations interface only")]
    public async Task AbstractClass()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public abstract partial class PersonBase<T> : Csla.BusinessBase<T>
            where T : PersonBase<T>
          {
            [Fetch]
            private void Fetch(int id) { }
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("Derived class hides the operations interface of a base class in the same assembly")]
    public async Task DerivedFromBaseWithOperations()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public abstract partial class PersonBase<T> : Csla.BusinessBase<T>
            where T : PersonBase<T>
          {
            [Fetch]
            private void Fetch(int id) { }
          }

          public partial class PersonEdit : PersonBase<PersonEdit>
          {
            [Fetch]
            private void Fetch(string name) { }
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("Generic business object excludes type parameter criteria from name dispatch")]
    public async Task GenericBusinessObject()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public partial class Lookup<TKey> : Csla.ReadOnlyBase<Lookup<TKey>>
          {
            [Fetch]
            private void Fetch(TKey key) { }

            [Fetch]
            private void Fetch(int id) { }
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("RunLocal and ValueTask operation methods")]
    public async Task RunLocalAndValueTask()
    {
      var source = """
        using Csla;
        using System.Threading.Tasks;

        namespace TestApp
        {
          public partial class PersonEdit : Csla.BusinessBase<PersonEdit>
          {
            [RunLocal]
            [Create]
            private void Create() { }

            [Fetch]
            private async ValueTask Fetch(int id) { await Task.Yield(); }

            [Fetch]
            private ValueTask<int> Fetch(string name) => new ValueTask<int>(0);
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("Keyed inject parameters")]
    public async Task KeyedInject()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public interface IDal { }

          public enum DalKind { Primary, Secondary }

          public partial class PersonEdit : Csla.BusinessBase<PersonEdit>
          {
            [Fetch]
            private void Fetch(int id, [Inject(Key = "primary")] IDal dal, [Inject(Key = DalKind.Secondary, AllowNull = true)] IDal? secondary) { }
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("Overloads with the same operation name report a diagnostic")]
    public async Task DuplicateOperationName()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public interface IDal { }

          public partial class PersonEdit : Csla.BusinessBase<PersonEdit>
          {
            [Fetch]
            private void Fetch(int id) { }

            [Fetch]
            private void Fetch(int id, [Inject] IDal dal) { }

            [Fetch]
            private void Load(int id) { }
          }
        }
        """;

      await DataPortalOperationsTestHelper<IncrementalDataPortalOperationsGenerator>.VerifyWithDiagnostics(source);
    }

    [TestMethod("Overloads with the same criteria count require exact types in type-based dispatch")]
    public async Task AmbiguousOverloads()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public class Criteria { }

          public partial class PersonEdit : Csla.BusinessBase<PersonEdit>
          {
            [Fetch]
            private void Fetch(object value) { }

            [Fetch]
            private void Fetch(Criteria criteria) { }

            [Fetch]
            private void Fetch(int id) { }
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("Params object array and keyword parameter names")]
    public async Task ParamsArrayAndKeywordNames()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public partial class PersonEdit : Csla.BusinessBase<PersonEdit>
          {
            [Fetch]
            private void Fetch(params object[] values) { }

            [Create]
            private void Create(int @class, string @event = "x") { }
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("Non-partial class generates nothing")]
    public async Task NonPartialClass()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public class PersonEdit : Csla.BusinessBase<PersonEdit>
          {
            [Fetch]
            private void Fetch(int id) { }
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("Class that implements the named mapping itself only gets the operations interface and type mapping")]
    public async Task ExistingNamedMapping()
    {
      var source = """
        using Csla;
        using System;
        using System.Threading.Tasks;

        namespace TestApp
        {
          public partial class PersonEdit : Csla.BusinessBase<PersonEdit>, Csla.Server.IDataPortalOperationNamedMapping
          {
            [Fetch]
            private void Fetch(int id) { }

            public Task InvokeNamedOperationAsync(string operationName, bool isSync, object?[]? criteria, IServiceProvider serviceProvider)
              => Task.CompletedTask;
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("Pipeline stages are cached when the compilation does not change")]
    public void StageCaching()
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
            [FetchChild]
            private async Task Fetch(int id, [Inject] IDal dal) { await Task.CompletedTask; }
          }
        }
        """;

      StageCachingTester.VerifyStageCaching<IncrementalDataPortalOperationsGenerator>(source, typeof(TrackingNames));
    }

    private static async Task TestHelperVerify(string source, params string[]? additionalSources)
    {
      await DataPortalOperationsTestHelper<IncrementalDataPortalOperationsGenerator>.Verify(source, additionalSources);
    }
  }
}
