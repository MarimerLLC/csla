//-----------------------------------------------------------------------
// <copyright file="NotNullPropertyGenerationTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for the generation of not-null properties (#5047)</summary>
//-----------------------------------------------------------------------

using Csla.Generator.AutoImplementProperties.CSharp.AutoImplement;
using Csla.Generator.AutoImplementProperties.CSharp.Tests.DataPortalOperations;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Csla.Generator.AutoImplementProperties.CSharp.Tests
{
  [TestClass]
  public class NotNullPropertyGenerationTests
  {
    private const string SwitchOff = "build_property.CslaGenerateNotNullProperties";

    private const string MixedTypes = """
      namespace Test;

      public class Address { }
      public enum Status { Open, Closed }

      [Csla.CslaImplementProperties]
      public partial class Person : Csla.BusinessBase<Person>
      {
        public partial string Name { get; set; }
        public partial string? Nickname { get; set; }
        public partial Address Home { get; set; }
        public partial Address? Work { get; set; }
        public partial int Age { get; set; }
        public partial int? Score { get; set; }
        public partial Status State { get; set; }
        public partial byte[] Photo { get; set; }
        public partial System.Collections.Generic.List<string?> Tags { get; set; }
      }
      """;

    private static string Generate<TGenerator>(string source, IEnumerable<string>? additionalSources = null,
      Dictionary<string, string>? options = null, NullableContextOptions nullable = NullableContextOptions.Enable)
      where TGenerator : IIncrementalGenerator, new()
    {
      var optionsProvider = options is null ? null : new TestAnalyzerConfigOptionsProvider(options);
      var (driver, compilation) = DataPortalOperationsTestHelper<TGenerator>.Setup(source, additionalSources, optionsProvider: optionsProvider);
      var input = compilation.WithOptions(compilation.Options.WithNullableContextOptions(nullable));
      driver = driver.RunGeneratorsAndUpdateCompilation(input, out var output, out var diagnostics);

      diagnostics.Should().BeEmpty();
      output.GetDiagnostics()
        .Where(d => d.Severity == DiagnosticSeverity.Error
          || (d.Severity == DiagnosticSeverity.Warning && d.Location.SourceTree?.FilePath.EndsWith(".g.cs", StringComparison.Ordinal) == true))
        .Should().BeEmpty();

      return driver.GetRunResult().GeneratedTrees.Single().ToString();
    }

    private static string Generate(string source, Dictionary<string, string>? options = null, NullableContextOptions nullable = NullableContextOptions.Enable)
      => Generate<IncrementalAutoImplementPropertiesPartialsGenerator>(source, options: options, nullable: nullable);

    [TestMethod]
    public void Generator_NonNullableTypes_AreRegisteredAsNotNull()
    {
      var generated = Generate(MixedTypes);

      generated.Should().Contain("public static readonly NotNullPropertyInfo<string> NameProperty = RegisterNotNullProperty<string>(nameof(Name), null, string.Empty);")
        .And.Contain("public static readonly NotNullPropertyInfo<global::Test.Address> HomeProperty = RegisterNotNullProperty<global::Test.Address>(nameof(Home));")
        .And.Contain("public static readonly NotNullPropertyInfo<int> AgeProperty = RegisterNotNullProperty<int>(nameof(Age));")
        .And.Contain("public static readonly NotNullPropertyInfo<global::Test.Status> StateProperty = RegisterNotNullProperty<global::Test.Status>(nameof(State));")
        .And.Contain("public static readonly NotNullPropertyInfo<byte[]> PhotoProperty = RegisterNotNullProperty<byte[]>(nameof(Photo));")
        .And.Contain("public static readonly NotNullPropertyInfo<global::System.Collections.Generic.List<string?>> TagsProperty = RegisterNotNullProperty<global::System.Collections.Generic.List<string?>>(nameof(Tags));")
        .And.Contain("get => GetProperty(NameProperty);")
        .And.Contain("get => GetProperty(HomeProperty);");
    }

    [TestMethod]
    public void Generator_NullableTypes_KeepRegisterProperty()
    {
      var generated = Generate(MixedTypes);

      generated.Should().Contain("public static readonly PropertyInfo<string?> NicknameProperty = RegisterProperty<string?>(nameof(Nickname));")
        .And.Contain("public static readonly PropertyInfo<global::Test.Address?> WorkProperty = RegisterProperty<global::Test.Address?>(nameof(Work));")
        .And.Contain("public static readonly PropertyInfo<int?> ScoreProperty = RegisterProperty<int?>(nameof(Score));");
    }

    [TestMethod]
    public void Generator_NullableDisabledAtDeclaration_KeepsRegisterProperty()
    {
      var source = """
        namespace Test;

        #nullable disable
        [Csla.CslaImplementProperties]
        public partial class Person : Csla.BusinessBase<Person>
        {
          public partial string Name { get; set; }
          public partial int Age { get; set; }
        }
        """;

      var generated = Generate(source);

      generated.Should().NotContain("NotNull")
        .And.Contain("public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));")
        .And.Contain("get => GetProperty(NameProperty)!;");
    }

    [TestMethod]
    public void Generator_NullableEnabledAtDeclarationInDisabledProject_RegistersNotNull()
    {
      var source = """
        namespace Test;

        #nullable enable
        [Csla.CslaImplementProperties]
        public partial class Person : Csla.BusinessBase<Person>
        {
          public partial string Name { get; set; }
        }
        """;

      var generated = Generate(source, nullable: NullableContextOptions.Disable);

      generated.Should().Contain("public static readonly NotNullPropertyInfo<string> NameProperty = RegisterNotNullProperty<string>(nameof(Name), null, string.Empty);");
    }

    [TestMethod]
    public void Generator_TypeParameters_AreNotNullOnlyWhenConstrained()
    {
      var source = """
        namespace Test;

        public class Address { }

        [Csla.CslaImplementProperties]
        public partial class Holder<TFree, TNotNull, TClass, TMaybeClass, TStruct, TBase> : Csla.BusinessBase<Holder<TFree, TNotNull, TClass, TMaybeClass, TStruct, TBase>>
          where TNotNull : notnull
          where TClass : class
          where TMaybeClass : class?
          where TStruct : struct
          where TBase : Address
        {
          public partial TFree Free { get; set; }
          public partial TNotNull NotNull { get; set; }
          public partial TClass Class { get; set; }
          public partial TMaybeClass MaybeClass { get; set; }
          public partial TStruct Struct { get; set; }
          public partial TBase Base { get; set; }
        }
        """;

      var generated = Generate(source);

      generated.Should().Contain("PropertyInfo<TFree> FreeProperty = RegisterProperty<TFree>(nameof(Free));")
        .And.Contain("NotNullPropertyInfo<TNotNull> NotNullProperty = RegisterNotNullProperty<TNotNull>(nameof(NotNull));")
        .And.Contain("NotNullPropertyInfo<TClass> ClassProperty = RegisterNotNullProperty<TClass>(nameof(Class));")
        .And.Contain("PropertyInfo<TMaybeClass> MaybeClassProperty = RegisterProperty<TMaybeClass>(nameof(MaybeClass));")
        .And.Contain("NotNullPropertyInfo<TStruct> StructProperty = RegisterNotNullProperty<TStruct>(nameof(Struct));")
        .And.Contain("NotNullPropertyInfo<TBase> BaseProperty = RegisterNotNullProperty<TBase>(nameof(Base));");
    }

    [TestMethod]
    [DataRow("ReadOnlyBase", "GetProperty", "LoadProperty")]
    [DataRow("CommandBase", "ReadProperty", "LoadProperty")]
    public void Generator_OtherBaseClasses_UseTheNotNullOverloads(string baseType, string getter, string setter)
    {
      var source = $$"""
        namespace Test;

        public class Address { }

        [Csla.CslaImplementProperties]
        public partial class Item : Csla.{{baseType}}<Item>
        {
          public partial Address Home { get; private set; }
        }
        """;

      var generated = Generate(source);

      generated.Should().Contain("NotNullPropertyInfo<global::Test.Address> HomeProperty = RegisterNotNullProperty<global::Test.Address>(nameof(Home));")
        .And.Contain($"get => {getter}(HomeProperty);")
        .And.Contain($"private set => {setter}(HomeProperty, value);");
    }

    [TestMethod]
    public void Generator_SwitchOff_GeneratesThePreviousOutput()
    {
      var source = """
        namespace Test;

        public class Address { }

        [Csla.CslaImplementProperties]
        public partial class Person : Csla.BusinessBase<Person>
        {
          public partial string Name { get; set; }
          public partial Address Home { get; set; }
          public partial int Age { get; set; }
        }
        """;

      var generated = Generate(source, new() { [SwitchOff] = "False" });

      generated.Should().Be("""
        //<auto-generated/>
        #nullable enable
        using System;
        using Csla;

        namespace Test;

        public partial class Person
        {
        	public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
        	public partial string Name
        	{
        		get => GetProperty(NameProperty)!;
        		set => SetProperty(NameProperty, value);
        	}
        	public static readonly PropertyInfo<global::Test.Address> HomeProperty = RegisterProperty<global::Test.Address>(nameof(Home));
        	public partial global::Test.Address Home
        	{
        		get => GetProperty(HomeProperty)!;
        		set => SetProperty(HomeProperty, value);
        	}
        	public static readonly PropertyInfo<int> AgeProperty = RegisterProperty<int>(nameof(Age));
        	public partial int Age
        	{
        		get => GetProperty(AgeProperty);
        		set => SetProperty(AgeProperty, value);
        	}
        }

        """.ReplaceLineEndings(Environment.NewLine));
    }

    [TestMethod]
    [DataRow("true")]
    [DataRow("")]
    [DataRow("yes")]
    public void Generator_SwitchOtherThanFalse_GeneratesNotNull(string value)
    {
      var generated = Generate(MixedTypes, new() { [SwitchOff] = value });

      generated.Should().Contain("RegisterNotNullProperty<string>(nameof(Name), null, string.Empty)");
    }

    [TestMethod]
    public void Generator_ChangingTheSwitch_Regenerates()
    {
      var optionsOn = new TestAnalyzerConfigOptionsProvider([]);
      var optionsOff = new TestAnalyzerConfigOptionsProvider(new() { [SwitchOff] = "false" });
      var (driver, compilation) = DataPortalOperationsTestHelper<IncrementalAutoImplementPropertiesPartialsGenerator>.Setup(MixedTypes, optionsProvider: optionsOn, trackSteps: true);

      driver = driver.RunGenerators(compilation);
      var on = driver.GetRunResult().GeneratedTrees.Single().ToString();
      driver = driver.WithUpdatedAnalyzerConfigOptions(optionsOff).RunGenerators(compilation);
      var result = driver.GetRunResult();

      on.Should().Contain("RegisterNotNullProperty");
      result.GeneratedTrees.Single().ToString().Should().NotContain("RegisterNotNullProperty");
      result.Results[0].TrackedSteps[TrackingNames.ExtractClasses]
        .SelectMany(step => step.Outputs)
        .Should().OnlyContain(output => output.Reason == IncrementalStepRunReason.Cached || output.Reason == IncrementalStepRunReason.Unchanged,
          "only the build property changed, so the extracted classes are reused");
    }

    [TestMethod]
    public void InterfaceGenerator_AllowNullOrMaybeNull_KeepsRegisterPropertyAndTheAttribute()
    {
      // the class implements the interface, so a generated setter stricter than
      // the interface member would warn CS8767 (Generate fails on that warning)
      var source = """
        namespace Test;

        public class Address { }

        [Csla.CslaImplementPropertiesInterface<IPerson>]
        public partial class Person : Csla.BusinessBase<Person>, IPerson
        {
        }

        public interface IPerson
        {
          [System.Diagnostics.CodeAnalysis.AllowNull]
          string Name { get; set; }

          [System.Diagnostics.CodeAnalysis.MaybeNull]
          Address Home { get; set; }

          [System.Diagnostics.CodeAnalysis.DisallowNull]
          [System.ComponentModel.Description("The code")]
          string Code { get; set; }
        }
        """;

      var generated = Generate<IncrementalAutoImplementInterfacePartialsGenerator>(source);

      generated.Should().Contain("PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));")
        .And.Contain("[global::System.Diagnostics.CodeAnalysis.AllowNullAttribute(")
        .And.Contain("PropertyInfo<global::Test.Address> HomeProperty = RegisterProperty<global::Test.Address>(nameof(Home));")
        .And.Contain("[global::System.Diagnostics.CodeAnalysis.MaybeNullAttribute(")
        .And.Contain("NotNullPropertyInfo<string> CodeProperty = RegisterNotNullProperty<string>(nameof(Code), null, string.Empty);")
        .And.Contain("[global::System.Diagnostics.CodeAnalysis.DisallowNullAttribute(", "attributes that agree with not-null are kept")
        .And.Contain("[global::System.ComponentModel.DescriptionAttribute(\"The code\"");
    }

    [TestMethod]
    public void Generator_AllowNullOnPartialProperty_KeepsRegisterProperty()
    {
      // the generated setter passes value (maybe null under [AllowNull]) on; the
      // not-null overload would warn CS8604 in the generated code
      var source = """
        namespace Test;

        [Csla.CslaImplementProperties]
        public partial class Person : Csla.BusinessBase<Person>
        {
          [System.Diagnostics.CodeAnalysis.AllowNull]
          public partial string Name { get; set; }
        }
        """;

      var generated = Generate(source);

      generated.Should().Contain("PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));");
    }

    [TestMethod]
    public void InterfaceGenerator_SwitchOff_KeepsNullabilityAttributes()
    {
      var source = """
        namespace Test;

        [Csla.CslaImplementPropertiesInterface<IPerson>]
        public partial class Person : Csla.BusinessBase<Person>
        {
        }

        public interface IPerson
        {
          [System.Diagnostics.CodeAnalysis.AllowNull]
          string Name { get; set; }
        }
        """;

      var generated = Generate<IncrementalAutoImplementInterfacePartialsGenerator>(source, options: new() { [SwitchOff] = "false" });

      generated.Should().Contain("RegisterProperty<string>(nameof(Name))")
        .And.Contain("[global::System.Diagnostics.CodeAnalysis.AllowNullAttribute(");
    }
  }
}
