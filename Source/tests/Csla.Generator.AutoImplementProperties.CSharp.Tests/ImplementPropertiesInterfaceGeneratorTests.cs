using Csla.Generator.AutoImplementProperties.CSharp.AutoImplement;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Csla.Generator.Tests
{
  /// <summary>
  /// Tests for the code that [CslaImplementPropertiesInterface&lt;T&gt;] generates.
  /// </summary>
  [TestClass]
  public class ImplementPropertiesInterfaceGeneratorTests : VerifyBase
  {
    private const string InterfaceMembers = """
            int Id { get; }
            [Description("The name")]
            string Name { get; set; }
            string? Description { get; set; }

            [Csla.CslaIgnoreProperty]
            string IgnoredProperty { get; set; }
      """;

    [TestMethod]
    public async Task ShortAttributeName()
    {
      await TestHelper<IncrementalAutoImplementInterfacePartialsGenerator>.Verify(NestedInterfaceSource("CslaImplementPropertiesInterface<ITestClass>"));
    }

    // The other forms of the attribute name must generate the same code as the
    // short form, whose output the ShortAttributeName snapshot verifies.
    [TestMethod]
    [DataRow("Csla.CslaImplementPropertiesInterface<ITestClass>")]
    [DataRow("Csla.CslaImplementPropertiesInterfaceAttribute<ITestClass>")]
    [DataRow("global::Csla.CslaImplementPropertiesInterface<ITestClass>")]
    [DataRow("CslaImplementPropertiesInterfaceAttribute<ITestClass>")]
    public void QualifiedAttributeNameGeneratesSameCodeAsShortName(string attribute)
    {
      var expected = Generate(NestedInterfaceSource("CslaImplementPropertiesInterface<ITestClass>"));

      Generate(NestedInterfaceSource(attribute)).Should().Be(expected);
    }

    [TestMethod]
    public void AliasedAttributeNameGeneratesSameCodeAsShortName()
    {
      // An alias can't name a private nested type, so the interface is declared in the namespace
      var source = $$"""
        using System.ComponentModel;
        using ImplementProperties = Csla.CslaImplementPropertiesInterfaceAttribute<Test.ITestClass>;

        namespace Test;

        [ImplementProperties]
        public partial class TestClass : Csla.BusinessBase<TestClass>
        {
        }

        internal interface ITestClass
        {
        {{InterfaceMembers}}
        }
        """;

      var expected = Generate(NestedInterfaceSource("CslaImplementPropertiesInterface<ITestClass>"));

      Generate(source).Should().Be(expected);
    }

    // An attribute on an interface member is copied to the generated property by the
    // attribute type's fully qualified name, however the attribute name is written.
    [TestMethod]
    [DataRow("", "System.ComponentModel.Description(\"The name\")")]
    [DataRow("", "global::System.ComponentModel.DescriptionAttribute(\"The name\")")]
    [DataRow("using System.ComponentModel;", "DescriptionAttribute(\"The name\")")]
    [DataRow("using Desc = System.ComponentModel.DescriptionAttribute;", "Desc(\"The name\")")]
    public void MemberAttributeNameGeneratesSameCodeAsShortName(string usingDirective, string attribute)
    {
      var expected = Generate(MemberAttributeSource("using System.ComponentModel;", "Description(\"The name\")"));

      Generate(MemberAttributeSource(usingDirective, attribute)).Should().Be(expected);
    }

    [TestMethod]
    public async Task MemberAttributesOfNestedAndGenericTypes()
    {
      var source = """
        namespace Test
        {
          [Csla.CslaImplementPropertiesInterface<ITestClass>]
          public partial class TestClass : Csla.BusinessBase<TestClass>
          {
            private interface ITestClass
            {
              [Outer.Marker]
              string Name { get; set; }
              [Tag<int>(Label = "count")]
              int Count { get; set; }
            }
          }

          public class Outer
          {
            [System.AttributeUsage(System.AttributeTargets.Property)]
            public class MarkerAttribute : System.Attribute { }
          }

          [System.AttributeUsage(System.AttributeTargets.Property)]
          public class TagAttribute<T> : System.Attribute
          {
            public string Label { get; set; } = "";
          }
        }
        """;

      await TestHelper<IncrementalAutoImplementInterfacePartialsGenerator>.Verify(source);
    }

    [TestMethod]
    [DataRow("NotAnInterface", "public class NotAnInterface { }")]
    [DataRow("System.IServiceProvider", "")]
    public void TypeArgumentWithoutSourceInterfaceGeneratesNothing(string typeArgument, string declaration)
    {
      var source = $$"""
        namespace Test;

        [Csla.CslaImplementPropertiesInterface<{{typeArgument}}>]
        public partial class TestClass : Csla.BusinessBase<TestClass>
        {
        }

        {{declaration}}
        """;

      var result = CSharpGeneratorDriver.Create(new IncrementalAutoImplementInterfacePartialsGenerator().AsSourceGenerator())
        .RunGenerators(TestCompilation.Create([source]))
        .GetRunResult();

      result.Diagnostics.Should().BeEmpty();
      result.GeneratedTrees.Should().BeEmpty();
    }

    /// <summary>
    /// Runs the generator and returns the text of the one file it generates,
    /// checking that the result compiles.
    /// </summary>
    private static string Generate(string source)
    {
      var compilation = TestCompilation.Create([source]);
      CSharpGeneratorDriver.Create(new IncrementalAutoImplementInterfacePartialsGenerator().AsSourceGenerator())
        .RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

      diagnostics.Should().BeEmpty();
      outputCompilation.GetDiagnostics().Should().BeEmpty();
      outputCompilation.SyntaxTrees.Should().HaveCount(2, "the generator should add one file");
      return outputCompilation.SyntaxTrees.Last().ToString();
    }

    private static string MemberAttributeSource(string usingDirective, string attribute) => $$"""
      {{usingDirective}}

      namespace Test;

      [Csla.CslaImplementPropertiesInterface<ITestClass>]
      public partial class TestClass : Csla.BusinessBase<TestClass>
      {
        private interface ITestClass
        {
          [{{attribute}}]
          string Name { get; set; }
        }
      }
      """;

    private static string NestedInterfaceSource(string attribute) => $$"""
      using System.ComponentModel;
      using Csla;

      namespace Test;

      [{{attribute}}]
      public partial class TestClass : Csla.BusinessBase<TestClass>
      {
        private interface ITestClass
        {
      {{InterfaceMembers}}
        }
      }
      """;
  }
}
