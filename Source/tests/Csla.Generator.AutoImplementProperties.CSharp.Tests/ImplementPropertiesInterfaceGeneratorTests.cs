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
