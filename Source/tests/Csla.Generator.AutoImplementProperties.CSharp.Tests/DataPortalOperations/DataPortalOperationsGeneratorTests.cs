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

    [TestMethod("Derived class dispatches operation methods inherited from a generic base class")]
    public async Task InheritedOperationsFromGenericBase()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public class MyCriteria<T> { }

          public abstract partial class AppBase<T> : Csla.BusinessBase<T>
            where T : AppBase<T>
          {
            [Create, RunLocal]
            protected virtual void Create(MyCriteria<T> criteria) { }

            [InsertChild]
            protected virtual void InsertChild() { }

            [UpdateChild]
            protected virtual void UpdateChild() { }

            [Fetch]
            private void Fetch(int id) { }
          }

          public partial class Customer : AppBase<Customer>
          {
            [Update]
            protected void Update() { }
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("Inherited method with the name of a method declared by the type is called through base")]
    public async Task InheritedOverloadHiddenByDerived()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public abstract class PersonBase<T> : Csla.BusinessBase<T>
            where T : PersonBase<T>
          {
            [Fetch]
            protected void Fetch(int id) { }
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

    [TestMethod("Inherited method hidden by a member of an intermediate class is left to reflection")]
    public async Task InheritedOverloadHiddenByIntermediate()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public abstract class PersonBase<T> : Csla.BusinessBase<T>
            where T : PersonBase<T>
          {
            [Fetch]
            protected void Fetch(int id) { }

            // Ties with the hidden Fetch(int), so reflection reports an ambiguous match.
            [Fetch]
            protected void Load(int id) { }

            [Delete]
            protected void Delete(int id) { }
          }

          public abstract class PersonMiddle<T> : PersonBase<T>
            where T : PersonMiddle<T>
          {
            protected void Fetch(System.Guid id) { }
          }

          public partial class PersonEdit : PersonMiddle<PersonEdit>
          {
            [Create]
            private void Create() { }
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("Inherited method overridden without the attribute is called virtually at the level of the override")]
    public async Task InheritedOverriddenWithoutAttribute()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public abstract class PersonBase<T> : Csla.BusinessBase<T>
            where T : PersonBase<T>
          {
            [Fetch]
            protected virtual void Fetch(int id) { }
          }

          public partial class PersonEdit : PersonBase<PersonEdit>
          {
            protected override void Fetch(int id) { }

            protected override void Child_Create() { }

            [Create]
            private void Create() { }
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("Inherited method with the criteria count of a method declared by the type is left to reflection")]
    public async Task InheritedSameShapeAcrossLevels()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public abstract class PersonBase<T> : Csla.BusinessBase<T>
            where T : PersonBase<T>
          {
            [Fetch]
            protected void Load(int id) { }
          }

          public partial class PersonEdit : PersonBase<PersonEdit>
          {
            [Fetch]
            private void Fetch(object value) { }
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("Inherited and declared methods with the same operation name are scored like reflection")]
    public async Task InheritedOperationNameScoring()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public interface IDal { }

          public abstract class PersonBase<T> : Csla.BusinessBase<T>
            where T : PersonBase<T>
          {
            [Fetch]
            protected void Load(int id, [Inject] IDal dal) { }

            [Delete]
            protected void Remove(int id) { }
          }

          public partial class PersonEdit : PersonBase<PersonEdit>
          {
            [Fetch]
            private void Fetch(int id) { }

            [Delete]
            private void Delete(int id) { }
          }
        }
        """;

      await DataPortalOperationsTestHelper<IncrementalDataPortalOperationsGenerator>.VerifyWithDiagnostics(source);
    }

    [TestMethod("Inherited method taking a protected nested type is left to reflection")]
    public async Task InheritedProtectedNestedCriteria()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public abstract class PersonBase<T> : Csla.BusinessBase<T>
            where T : PersonBase<T>
          {
            protected class Criteria { }

            [Fetch]
            protected void Fetch(Criteria criteria) { }

            [Delete]
            protected void Delete(int id) { }
          }

          public partial class PersonEdit : PersonBase<PersonEdit>
          {
            [Create]
            private void Create() { }
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("Declared method taking a private nested type is left to reflection")]
    public async Task PrivateNestedCriteria()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public partial class PersonEdit : Csla.BusinessBase<PersonEdit>
          {
            private class Criteria { }

            [Fetch]
            private void Fetch(Criteria criteria) { }

            // Must not capture a Criteria value, which reflection gives to Fetch(Criteria).
            [Fetch]
            private void Fetch(object value) { }
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("Injected parameters of a method further up the chain can outscore a closer method")]
    public async Task InheritedInjectionScoring()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public interface IA { }
          public interface IB { }
          public interface IC { }
          public interface ID { }

          public abstract class PersonBase<T> : Csla.BusinessBase<T>
            where T : PersonBase<T>
          {
            [Fetch]
            protected void Load(object value, [Inject] IA a, [Inject] IB b, [Inject] IC c, [Inject] ID d) { }
          }

          public abstract class PersonMiddle<T> : PersonBase<T>
            where T : PersonMiddle<T>
          {
            // For an int, reflection scores this 3 - 1 = 2 and Load 1 - 2 + 4 = 3.
            [Fetch]
            protected void Fetch(int id) { }
          }

          public partial class PersonEdit : PersonMiddle<PersonEdit>
          {
            [Create]
            private void Create() { }
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("Inherited method marked with a subclass of an operation attribute is dispatched")]
    public async Task InheritedDerivedOperationAttribute()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public class InitializeChildAttribute : CreateChildAttribute { }

          public abstract class PersonBase<T> : Csla.BusinessBase<T>
            where T : PersonBase<T>
          {
            // Closer than BusinessBase.Child_Create, so reflection calls this.
            [InitializeChild]
            protected void Initialize() { }
          }

          public partial class PersonEdit : PersonBase<PersonEdit>
          {
            [Fetch]
            private void Fetch(int id) { }
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("Declared method marked with a subclass of an operation attribute leaves its kind to reflection")]
    public async Task DeclaredDerivedOperationAttribute()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public class InitializeChildAttribute : CreateChildAttribute { }

          public partial class PersonEdit : Csla.BusinessBase<PersonEdit>
          {
            [InitializeChild]
            private void Initialize() { }

            [Fetch]
            private void Fetch(int id) { }
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("Child list dispatches the inherited params object array update")]
    public async Task InheritedParamsArrayUpdateChild()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public partial class LineItem : Csla.BusinessBase<LineItem>
          {
            [FetchChild]
            private void FetchChild(int id) { }
          }

          public partial class LineItems : Csla.BusinessListBase<LineItems, LineItem>
          {
            [FetchChild]
            private void FetchChild(int count) { }
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

            [Create]
            private void Create([Inject(Key = (short)-1)] IDal dal, [Inject(Key = (byte)2)] IDal other) { }
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

    [TestMethod("Overloads tied on injected parameters are left to reflection and reported")]
    public async Task AmbiguousOperationName()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public interface IDal { }

          public partial class PersonEdit : Csla.BusinessBase<PersonEdit>
          {
            [Fetch]
            private void Fetch(int id, [Inject] IDal dal) { }

            [Fetch]
            private void Load(int id, [Inject] IDal dal) { }

            [Fetch]
            private void Fetch(string name) { }
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

    [TestMethod("In criteria parameters and nullable inject parameters")]
    public async Task InCriteriaAndNullableInject()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public interface IDal { }

          public partial class PersonEdit : Csla.BusinessBase<PersonEdit>
          {
            [Fetch]
            private void Fetch(in string? name, in int id) { }

            [Create]
            private void Create([Inject] int? count, [Inject] IDal? dal) { }
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

    [TestMethod("Business objects without operation methods generate nothing")]
    public async Task NoOperationMethods()
    {
      var source = """
        using Csla;

        namespace TestApp
        {
          public partial class PersonEdit : Csla.BusinessBase<PersonEdit>
          {
          }

          public partial class PersonList : Csla.ReadOnlyListBase<PersonList, PersonEdit>
          {
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
