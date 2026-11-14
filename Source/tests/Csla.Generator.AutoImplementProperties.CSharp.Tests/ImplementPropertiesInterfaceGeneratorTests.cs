using Csla.Generator.AutoImplementProperties.CSharp.AutoImplement;
using Csla.Generator.AutoImplementProperties.CSharp.Tests;
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
            string? Code { get; }

            [Csla.CslaIgnoreProperty]
            string IgnoredProperty { get; set; }
      """;

    [TestMethod]
    public async Task ShortAttributeName()
    {
      await TestHelper<IncrementalAutoImplementInterfacePartialsGenerator>.Verify(NestedInterfaceSource("CslaImplementPropertiesInterface<ITestClass>"));
    }

    // Each base type has its own property methods (SetProperty, LoadProperty, ...);
    // the ShortAttributeName snapshot verifies BusinessBase
    [TestMethod]
    [DataRow("ReadOnlyBase")]
    [DataRow("CommandBase")]
    public async Task BaseTypes(string baseType)
    {
      await TestHelper<IncrementalAutoImplementInterfacePartialsGenerator>.Verify(NestedInterfaceSource("CslaImplementPropertiesInterface<ITestClass>", baseType));
    }

    // A class can also implement the interface: the generated properties must then
    // have the public setters the interface's set accessors require
    [TestMethod]
    public async Task ReadOnlyBaseImplementingInterface()
    {
      var source = """
        namespace Test;

        [Csla.CslaImplementPropertiesInterface<ITestClass>]
        public partial class TestClass : Csla.ReadOnlyBase<TestClass>, ITestClass
        {
        }

        public interface ITestClass
        {
          int Id { get; }
          string Name { get; set; }
        }
        """;

      await TestHelper<IncrementalAutoImplementInterfacePartialsGenerator>.Verify(source);
    }

    // Uses the member attributes with typeof, nameof, enum and constant arguments,
    // which are the values most likely to hold Roslyn objects that can't be cached
    [TestMethod("Pipeline stages are cacheable")]
    public void StageCaching()
    {
      StageCachingTester.VerifyStageCaching<IncrementalAutoImplementInterfacePartialsGenerator>(QualifiedArgumentsSource, typeof(TrackingNames), [QualifiedArgumentsAttributes]);
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
    public void InterfaceDeclaredInAnotherFileGeneratesSameCode()
    {
      var classSource = """
        namespace Test;

        [Csla.CslaImplementPropertiesInterface<ITestClass>]
        public partial class TestClass : Csla.BusinessBase<TestClass>
        {
        }
        """;
      var interfaceSource = $$"""
        using System.ComponentModel;

        namespace Test;

        public partial class TestClass
        {
          private interface ITestClass
          {
        {{InterfaceMembers}}
          }
        }
        """;

      var expected = Generate(NestedInterfaceSource("CslaImplementPropertiesInterface<ITestClass>"));

      Generate(classSource, interfaceSource).Should().Be(expected);
    }

    // Attribute arguments are copied to the generated property, which is in another file
    // without the interface file's using directives, so the names in them must be qualified.
    [TestMethod]
    public async Task MemberAttributeArgumentsAreQualified()
    {
      await TestHelper<IncrementalAutoImplementInterfacePartialsGenerator>.Verify(QualifiedArgumentsSource, [QualifiedArgumentsAttributes]);
    }

    private const string QualifiedArgumentsAttributes = """
      using System;

      namespace Test.Attributes
      {
        [AttributeUsage(AttributeTargets.Property)]
        public class InfoAttribute : Attribute
        {
          public InfoAttribute() { }
          public InfoAttribute(string name, int order = 0) { }
          public Type? ResourceType { get; set; }
          public string? ResourceName { get; set; }
          public int Limit { get; set; }
          public Mode Mode { get; set; }
        }

        public enum Mode { Slow, Fast }

        public static class Limits
        {
          public const int Max = 50;
        }

        public class Strings
        {
          public static string CustomerName => "";
        }

        public class Outer<T>
        {
          public class Inner<U> { }
          public class Plain { }
        }
      }
      """;

    private const string QualifiedArgumentsSource = """
      using System.Collections.Generic;
      using Test.Attributes;
      using Label = Test.Attributes.Strings;

      namespace Test;

      [Csla.CslaImplementPropertiesInterface<ITestClass>]
      public partial class TestClass : Csla.BusinessBase<TestClass>
      {
        private interface ITestClass
        {
          const int MaxLength = 20;

          [Info(ResourceType = typeof(Strings), ResourceName = nameof(Strings.CustomerName))]
          string Name { get; set; }
          [Info(Limit = Limits.Max, Mode = Mode.Fast)]
          int Count { get; set; }
          [Info(Limit = MaxLength, ResourceType = typeof(List<>))]
          string Code { get; set; }
          [Info(order: 2, name: nameof(Code))]
          string Ordered { get; set; }
          // nameof an alias is the alias name, not the name of the type it stands for
          [Info(ResourceType = typeof(Outer<>.Inner<>), ResourceName = nameof(Label))]
          string Nested { get; set; }
          [Info(ResourceType = typeof(Outer<>.Plain))]
          string Plain { get; set; }
        }
      }
      """;

    [TestMethod]
    public void InterfaceDeclaredInReferencedCompilationGeneratesNothing()
    {
      // In the IDE a project reference is a compilation reference, whose symbols have
      // syntax in the other compilation; the generator can't bind that syntax.
      var library = TestCompilation.Create(["""
        namespace Library;

        public interface ICustomer
        {
          string Name { get; set; }
        }
        """]).WithAssemblyName("Library");
      var source = """
        namespace Test;

        [Csla.CslaImplementPropertiesInterface<Library.ICustomer>]
        public partial class TestClass : Csla.BusinessBase<TestClass>
        {
        }
        """;
      var compilation = TestCompilation.Create([source]).AddReferences(library.ToMetadataReference());

      var result = CSharpGeneratorDriver.Create(new IncrementalAutoImplementInterfacePartialsGenerator().AsSourceGenerator())
        .RunGenerators(compilation)
        .GetRunResult();

      result.Results.Should().ContainSingle().Which.Exception.Should().BeNull();
      result.Diagnostics.Should().BeEmpty();
      result.GeneratedTrees.Should().BeEmpty();
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
    private static string Generate(params string[] sources)
    {
      var compilation = TestCompilation.Create(sources);
      CSharpGeneratorDriver.Create(new IncrementalAutoImplementInterfacePartialsGenerator().AsSourceGenerator())
        .RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

      diagnostics.Should().BeEmpty();
      outputCompilation.GetDiagnostics().Should().BeEmpty();
      outputCompilation.SyntaxTrees.Should().HaveCount(sources.Length + 1, "the generator should add one file");
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

    private static string NestedInterfaceSource(string attribute, string baseType = "BusinessBase") => $$"""
      using System.ComponentModel;
      using Csla;

      namespace Test;

      [{{attribute}}]
      public partial class TestClass : Csla.{{baseType}}<TestClass>
      {
        private interface ITestClass
        {
      {{InterfaceMembers}}
        }
      }
      """;
  }
}
