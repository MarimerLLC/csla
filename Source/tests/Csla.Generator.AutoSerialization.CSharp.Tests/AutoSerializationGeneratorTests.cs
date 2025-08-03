//-----------------------------------------------------------------------
// <copyright file="AutoSerializationGeneratorTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests of serialization behaviour on the AutoSerializable struct Point</summary>
//-----------------------------------------------------------------------
using Csla.Generator.AutoSerialization.CSharp.AutoSerialization;
using Csla.Generator.AutoSerialization.CSharp.Tests.Helpers;

namespace Csla.Generator.AutoSerialization.CSharp.Tests
{
  [TestClass]
  public class AutoSerializationGeneratorTests : VerifyBase
  {
    [TestMethod("User class uses file scoped namespace which should be possible to use.")]
    public async Task Case01()
    {
      var source = """
        namespace Test;
      
        [Csla.Serialization.AutoSerializable]
        public partial class AutoSerializableTest1
        {
          public string Name { get; private set; } = "";
        }
      
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("User class uses non file scoped namespace which should be possible to use.")]
    public async Task Case02()
    {
      var source = """
        namespace Test
        {
          [Csla.Serialization.AutoSerializable]
          public partial class AutoSerializableTest2
          {
            public string Name { get; private set; } = "";
          }
        }
        """;

      await TestHelperVerify(source);
    }

    [TestMethod("Property with a type containing generics must be generated correctly to be used.")]
    public async Task Case03()
    {
      var additionalSources = """
        namespace Test2;
        public class Foo<T>;
        """;

      var source = """
        using Csla.Core;
        using Test2;

        namespace Test
        {
          [Csla.Serialization.AutoSerializable]
          public partial class AutoSerializableTest3
          {
            public string? Name { get; set; } = "";
            public MobileDictionary<string, Foo<Foo<AutoSerializableTest3>>>? Name2 { get; private set; }
          }
        }
        """;

      await TestHelperVerify(source, additionalSources);
    }

    public static IEnumerable<object[]> Case04TestData()
    {
      var nonNullableBuiltInTypes = new string[] { "bool", "byte", "sbyte", "char", "decimal", "double", "float", "int", "uint", "long", "ulong", "short", "ushort" };

      foreach (var item in nonNullableBuiltInTypes)
      {
        yield return new object[] { item, false };
        yield return new object[] { $"{item}?", false };
        yield return new object[] { $"{item}[]", true };
        yield return new object[] { $"{item}[]?", false };
      }

      var nullableBuiltInTypes = new string[] { "string", "object" };

      foreach (var item in nullableBuiltInTypes)
      {
        yield return new object[] { item, true };
        yield return new object[] { $"{item}?", false };
        yield return new object[] { $"{item}[]", true };
        yield return new object[] { $"{item}[]?", false };
      }
    }

    [DataTestMethod("Property with built in type should be generated as expected.")]
    [DynamicData(nameof(Case04TestData))]
    public async Task Case04(string typeToTest, bool notNull)
    {
      string suppressNull = "";
      if (notNull)
      {
        suppressNull = "= default!;";
      }

      var source = $$"""
        namespace Test;

        [Csla.Serialization.AutoSerializable]
        public partial class AutoSerializableTest4
        {
          public {{typeToTest}} TestProperty { get; set; } {{suppressNull}}
        }
        """;

      await TestHelperVerify(source);
    }



    private static async Task TestHelperVerify(string source, params string[]? additionalSources)
    {
      await TestHelper<IncrementalSerializationPartialsGenerator>.Verify(source, additionalSources);
    }
  }
}
