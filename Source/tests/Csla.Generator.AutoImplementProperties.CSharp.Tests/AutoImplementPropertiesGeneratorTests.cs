using Csla.Generator.AutoImplementProperties.CSharp.AutoImplement;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;


namespace Csla.Generator.Tests
{
  [TestClass]
  public class AutoImplementPropertiesGeneratorTests : VerifyBase
  {
    [TestMethod("ReadOnlyBase must be generated with LoadProperty for set and GetProperty for get.")]
    public async Task Case01()
    {
      var inputSource = """
        namespace Test
        {
          [Csla.CslaImplementProperties]
          public partial class ReadOnlyPOCO : Csla.ReadOnlyBase<ReadOnlyPOCO>
          {
            public partial string Name { get; private set; }
          }
        }   
        """;

      await TestHelper<IncrementalAutoImplementPropertiesPartialsGenerator>.Verify(inputSource);
    }

    [TestMethod("BusinessBase must be generated with SetProperty for set and GetProperty for get.")]
    public async Task Case02()
    {
      var source = """
        namespace Test
        {
          [Csla.CslaImplementProperties]
          public partial class BusinessBaseTestClass : Csla.BusinessBase<BusinessBaseTestClass>
          {
            public partial int? Age { get; private set; }
          }
        }
        """;

      await TestHelper<IncrementalAutoImplementPropertiesPartialsGenerator>.Verify(source);
    }

    [TestMethod("Property with a type containing a generic parameter.")]
    public async Task Case03()
    {
      var source = """
        namespace Test2
        {
          public class GenericType<T>;
        }

        namespace Test
        {
          using Test2;

          [Csla.CslaImplementProperties]
          public partial class BusinessBaseTestClass : Csla.BusinessBase<BusinessBaseTestClass>
          {
            public partial GenericType<string> Name { get; private set; }
          }
        }
        """;

      await TestHelper<IncrementalAutoImplementPropertiesPartialsGenerator>.Verify(source);
    }

    [TestMethod("Type contained in a file scoped namespace declaration.")]
    public async Task Case04()
    {
      var source = """
        namespace Test;

        [Csla.CslaImplementProperties]
        public partial class BusinessBaseTestClass : Csla.BusinessBase<BusinessBaseTestClass>
        {
          public partial string? Name { get; private set; }
        }
        """;

      await TestHelper<IncrementalAutoImplementPropertiesPartialsGenerator>.Verify(source);
    }

    [TestMethod("Two types with same name but different namespaces.")]
    public async Task Case05()
    {
      var additionalSources = new List<string>
      {
        """
        namespace Test2
        {
          public class UniqueType1
          {
          }
        
          public class NonUniqueType
          {
          }
        }
        """,
        """
        namespace Test3
        {
          public class NonUniqueType
          {
          }
        }
        """
      };

      var source = """
        using Test2;

        namespace Test
        {
          [Csla.CslaImplementProperties]
          public partial class BusinessBaseTestClass : Csla.BusinessBase<BusinessBaseTestClass>
          {
            public partial UniqueType1? UniqueName { get; private set; }
            
            public partial Test3.NonUniqueType? NonUnique { get; set; }
          }
        }
        """;

      await TestHelper<IncrementalAutoImplementPropertiesPartialsGenerator>.Verify(source, additionalSources);
    }

    [TestMethod("Command must be generated with LoadProperty for set and ReadProperty for get.")]
    public async Task Case06()
    {
      var source = """
        namespace Test
        {
          [Csla.CslaImplementProperties]
          public partial class CommandTest : Csla.CommandBase<CommandTest>
          {
            public partial string Name { get; private set; }
          }
        }
        """;

      await TestHelper<IncrementalAutoImplementPropertiesPartialsGenerator>.Verify(source);
    }

    [TestMethod("Property with nested generic type must be generated correctly.")]
    public async Task Case07()
    {
      var source = """
        using Csla;
        using Csla.Core;

        namespace Test;

        public class GenericType<T>;

        [CslaImplementProperties]
        public partial class BOTest : BusinessBase<BOTest>
        {
          public partial GenericType<MobileDictionary<string, int>>? Name { get; private set; }
        }
        """;
      await TestHelper<IncrementalAutoImplementPropertiesPartialsGenerator>.Verify(source);
    }

    [TestMethod("Property with nested generic nullable types must be generated correctly.")]
    public async Task Case08()
    {
      var source = """
        using Csla;
        using Csla.Core;

        namespace Test;

        [CslaImplementProperties]
        public partial class BOTest : BusinessBase<BOTest>
        {
          public partial MobileDictionary<string, MobileList<int?>?>? Name { get; private set; }
        }
        """;

      await TestHelper<IncrementalAutoImplementPropertiesPartialsGenerator>.Verify(source);
    }

    [TestMethod("Attributes on partial properties are preserved via the compiler's partial property mechanism.")]
    public async Task Case10()
    {
      var source = """
        using System.ComponentModel;
        using System.Diagnostics.CodeAnalysis;

        namespace Test;

        [Csla.CslaImplementProperties]
        public partial class BusinessBaseTestClass : Csla.BusinessBase<BusinessBaseTestClass>
        {
          [Description("The name")]
          public partial string? Name { get; set; }

          [StringSyntax(StringSyntaxAttribute.Regex)]
          public partial string? Pattern { get; set; }
        }
        """;

      await TestHelper<IncrementalAutoImplementPropertiesPartialsGenerator>.Verify(source);
    }

    [TestMethod("A generic class keeps its type parameter in the generated partial declaration (#5042).")]
    public async Task GenericClass()
    {
      var source = """
        namespace Test;

        public abstract class MyBusinessBase<T> : Csla.BusinessBase<T> where T : MyBusinessBase<T>
        {
        }

        [Csla.CslaImplementProperties]
        public abstract partial class PersonBase<T> : MyBusinessBase<T> where T : PersonBase<T>
        {
          public partial string PersonDisplayName { get; set; }
          public partial T? Self { get; set; }
        }
        """;

      await TestHelper<IncrementalAutoImplementPropertiesPartialsGenerator>.Verify(source);
    }

    [TestMethod("Generic classes with several type parameters, and generic and non-generic classes sharing a name, each get their own file.")]
    public async Task GenericClassesWithSameName()
    {
      var source = """
        namespace Test;

        [Csla.CslaImplementProperties]
        public partial class Pair : Csla.BusinessBase<Pair>
        {
          public partial string Name { get; set; }
        }

        [Csla.CslaImplementProperties]
        public partial class Pair<TKey, TValue> : Csla.ReadOnlyBase<Pair<TKey, TValue>>
        {
          public partial TKey Key { get; private set; }
          public partial TValue Value { get; private set; }
        }
        """;

      await TestHelper<IncrementalAutoImplementPropertiesPartialsGenerator>.Verify(source);
    }

    [TestMethod("The property methods come from the CSLA base class inherited through an intermediate base class whose name doesn't contain the CSLA base class's name (#5042).")]
    public async Task IntermediateBaseClass()
    {
      var source = """
        namespace Test;

        public abstract class EntityBase<T> : Csla.ReadOnlyBase<T> where T : EntityBase<T>
        {
        }

        public abstract class PersonBase<T> : EntityBase<T> where T : PersonBase<T>
        {
        }

        [Csla.CslaImplementProperties]
        public partial class Person : PersonBase<Person>
        {
          public partial string Name { get; private set; }
        }
        """;

      await TestHelper<IncrementalAutoImplementPropertiesPartialsGenerator>.Verify(source);
    }

    [TestMethod("[CslaImplementPropertiesInterface<T>] on a generic class keeps its type parameter (#5042).")]
    public async Task GenericClassImplementingInterface()
    {
      var source = """
        using Csla;

        namespace Test;

        public abstract class MyBusinessBase<T> : Csla.BusinessBase<T> where T : MyBusinessBase<T>
        {
        }

        [CslaImplementPropertiesInterface<IPerson>]
        public abstract partial class PersonBase<T> : MyBusinessBase<T> where T : PersonBase<T>
        {
        }

        public interface IPerson
        {
          string Name { get; set; }
        }
        """;

      await TestHelper<IncrementalAutoImplementInterfacePartialsGenerator>.Verify(source);
    }

    [TestMethod("Type parameter shapes: struct-constrained T?, arrays of T, jagged and multi-dimensional arrays, and an @-escaped type parameter.")]
    public async Task TypeParameterShapes()
    {
      var source = """
        namespace Test;

        [Csla.CslaImplementProperties]
        public partial class Measure<T, @class> : Csla.BusinessBase<Measure<T, @class>> where T : struct
        {
          public partial T? Amount { get; set; }
          public partial T[] Items { get; set; }
          public partial T[][] Jagged { get; set; }
          public partial int[,] Grid { get; set; }
          public partial string?[]? Tags { get; set; }
          public partial @class Value { get; set; }
        }
        """;

      await TestHelper<IncrementalAutoImplementPropertiesPartialsGenerator>.Verify(source);
    }

    [TestMethod("A class nested in a generic class is generated inside its containing type.")]
    public async Task NestedInGenericClass()
    {
      var source = """
        namespace Test;

        public partial class Outer<T>
        {
          [Csla.CslaImplementProperties]
          public partial class Line : Csla.BusinessBase<Line>
          {
            public partial T? Value { get; set; }
          }
        }
        """;

      await TestHelper<IncrementalAutoImplementPropertiesPartialsGenerator>.Verify(source);
    }

    [TestMethod("Classes with the same name in different namespaces each get their own file.")]
    public async Task SameNameInDifferentNamespaces()
    {
      var source = """
        namespace Sales
        {
          [Csla.CslaImplementProperties]
          public partial class Order : Csla.BusinessBase<Order>
          {
            public partial string Name { get; set; }
          }
        }

        namespace Purchasing
        {
          [Csla.CslaImplementProperties]
          public partial class Order : Csla.BusinessBase<Order>
          {
            public partial string Name { get; set; }
          }
        }
        """;

      await TestHelper<IncrementalAutoImplementPropertiesPartialsGenerator>.Verify(source);
    }

    [TestMethod("[CslaImplementPropertiesInterface<T>] reads an interface declared in another file.")]
    public async Task InterfaceInAnotherFile()
    {
      var source = """
        using Csla;

        namespace Test;

        [CslaImplementPropertiesInterface<IPerson>]
        public partial class Person : Csla.BusinessBase<Person>
        {
        }
        """;
      var interfaceSource = """
        namespace Test;

        public interface IPerson
        {
          string Name { get; set; }
        }
        """;

      await TestHelper<IncrementalAutoImplementInterfacePartialsGenerator>.Verify(source, [interfaceSource]);
    }

    [TestMethod("A class whose base class only has a CSLA base class's name gets no file generated.")]
    public void BaseClassOutsideCslaGeneratesNoProperties()
    {
      var source = """
        namespace MyCompany
        {
          public class BusinessBase<T>
          {
          }
        }

        namespace Test
        {
          [Csla.CslaImplementProperties]
          public partial class TestClass : MyCompany.BusinessBase<TestClass>
          {
            public partial string Name { get; set; }
          }
        }
        """;

      var compilation = CSharpCompilation.Create(
        "Tests",
        [CSharpSyntaxTree.ParseText(source)],
        [MetadataReference.CreateFromFile(typeof(object).Assembly.Location), MetadataReference.CreateFromFile(typeof(CslaImplementPropertiesAttribute).Assembly.Location)],
        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
      var result = CSharpGeneratorDriver.Create(new IncrementalAutoImplementPropertiesPartialsGenerator().AsSourceGenerator())
        .RunGenerators(compilation)
        .GetRunResult();

      Assert.IsNull(result.Results.Single().Exception);
      Assert.AreEqual(0, result.GeneratedTrees.Length);
    }

    public static IEnumerable<object[]> Case09TestData()
    {
      var nonNullableBuiltInTypes = new string[] { "bool", "byte", "sbyte", "char", "decimal", "double", "float", "int", "uint", "long", "ulong", "short", "ushort" };

      foreach (var item in nonNullableBuiltInTypes)
      {
        yield return new object[] { item };
        yield return new object[] { $"{item}?" };
        yield return new object[] { $"{item}[]" };
        yield return new object[] { $"{item}[]?" };
      }

      var nullableBuiltInTypes = new string[] { "string", "object" };

      foreach (var item in nullableBuiltInTypes)
      {
        yield return new object[] { item };
        yield return new object[] { $"{item}?" };
        yield return new object[] { $"{item}[]" };
        yield return new object[] { $"{item}[]?" };
      }
    }

    [DataTestMethod]
    [DynamicData(nameof(Case09TestData))]
    public async Task Case09(string type)
    {
      var source = $$"""
        namespace Test;

        [Csla.CslaImplementProperties]
        public partial class BusinessBaseTestClass : Csla.BusinessBase<BusinessBaseTestClass>
        {
          public partial {{type}} Name { get; private set; }
        }
        """;

      await TestHelper<IncrementalAutoImplementPropertiesPartialsGenerator>.Verify(source);
    }
  }
}