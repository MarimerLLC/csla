using Csla.Generator.AutoSerialization.CSharp.AutoSerialization;
using Csla.Generator.Tests;

namespace Csla.Generator.AutoSerialization.CSharp.Tests;

[TestClass]
public class AutoSerializationGeneratorTests : VerifyBase
{
  [TestMethod("User class uses file scoped namespace which should be possible to use.")]
  public async Task Case01()
  {
    var source = """
      namespace Test;
      
      [Csla.Serialization.AutoSerializable]
      public partial class AutoSerializableTest
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
      public partial class AutoSerializableTest
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
        public partial class AutoSerializableTest
        {
          public string? Name { get; set; } = "";

          public MobileDictionary<string, Foo<Foo<AutoSerializableTest>>>? Name2 { get; private set; }
        }
      }
      """;

    await TestHelperVerify(source, additionalSources);
  }



  private static async Task TestHelperVerify(string source, params string[]? additionalSources)
  {
    await TestHelper<IncrementalSerializationPartialsGenerator>.Verify(source, additionalSources);
  }
}
