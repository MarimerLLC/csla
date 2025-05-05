using Csla.Generator.AutoImplementProperties.CSharp.AutoImplement;
using Microsoft.VisualStudio.TestTools.UnitTesting;


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
            public partial string Name { get; private set; }
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
          public partial string Name { get; private set; }
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
            public partial UniqueType1 UniqueName { get; private set; }
            
            public partial Test3.NonUniqueType NonUnique { get; set; }
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
    public async Task MyTestMethod()
    {
      var source = """
        using Csla;
        using Csla.Core;

        namespace Test;

        public class GenericType<T>;

        [CslaImplementProperties]
        public partial class BOTest : BusinessBase<BOTest>
        {
          public partial GenericType<MobileDictionary<string, int>> Name { get; private set; }
        }
        """;
      await TestHelper<IncrementalAutoImplementPropertiesPartialsGenerator>.Verify(source);
    }
  }
}
