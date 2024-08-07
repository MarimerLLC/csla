using Csla.Generator.AutoImplementProperties.CSharp.AutoImplement;
using Csla.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;


namespace Csla.Generator.Tests
{
  [TestClass]
  public class IncrementalAutoImplementPropertiesPartialsGeneratorTests
  {
    [TestMethod]
    public void TestInitialize()
    {
      var generator = new IncrementalAutoImplementPropertiesPartialsGenerator();
      var context = new Microsoft.CodeAnalysis.IncrementalGeneratorInitializationContext();
      generator.Initialize(context);
      Assert.IsNotNull(context);
    }

    [TestMethod]
    public async Task TestSourceGeneration()
    {
      var inputSource = """
        namespace Test
        {
          [Csla.Serialization.CslaImplementProperties]
          public partial class ReadOnlyPOCO : Csla.ReadOnlyBase<ReadOnlyPOCO>
          {

            public partial string Name { get; private set; }

          }
        }   
        """;

      var expectedOutput = """
        //<auto-generated/>
        #nullable enable
        using System;
        using Csla;

        namespace Csla.Generator.AutoImplementProperties.CSharp.Test;

        public partial class AddressPOCO
        {
        	public static readonly PropertyInfo<string?> AddressLine1Property = RegisterProperty<string?>(nameof(AddressLine1));
        	public partial string? AddressLine1
        	{
        		get => GetProperty(AddressLine1Property);
        		private set => SetProperty(AddressLine1Property, value);
        	}
        	public static readonly PropertyInfo<string> AddressLine2Property = RegisterProperty<string>(nameof(AddressLine2));
        	public partial string AddressLine2
        	{
        		get => GetProperty(AddressLine2Property);
        		set => SetProperty(AddressLine2Property, value);
        	}
        	public static readonly PropertyInfo<string> TownProperty = RegisterProperty<string>(nameof(Town));
        	public partial string Town
        	{
        		get => GetProperty(TownProperty);
        		set => SetProperty(TownProperty, value);
        	}
        	public static readonly PropertyInfo<string> CountyProperty = RegisterProperty<string>(nameof(County));
        	public partial string County
        	{
        		get => GetProperty(CountyProperty);
        		set => SetProperty(CountyProperty, value);
        	}
        	public static readonly PropertyInfo<string> PostcodeProperty = RegisterProperty<string>(nameof(Postcode));
        	public partial string Postcode
        	{
        		get => GetProperty(PostcodeProperty);
        		set => SetProperty(PostcodeProperty, value);
        	}
        }    
        """;

      await TestHelper<IncrementalAutoImplementPropertiesPartialsGenerator>.Verify(inputSource);
      //("AddressPOCO.cs", SourceText.From(inputSource)), ("AddressPOCO.g.cs", expectedOutput));
    }

  }

  public static class TestHelper<T> where T : IIncrementalGenerator, new()
  {
    public static Task Verify(string source)
    {
      // Parse the provided string into a C# syntax tree
      SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);
      var references = AppDomain.CurrentDomain.GetAssemblies()
    .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
    .Select(a => MetadataReference.CreateFromFile(a.Location))
    .Concat([
            MetadataReference.CreateFromFile(typeof(FetchAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(CslaIgnorePropertyAttribute).Assembly.Location)
    ]);
      // Create a Roslyn compilation for the syntax tree.
      CSharpCompilation compilation = CSharpCompilation.Create(
          assemblyName: "Tests",
          syntaxTrees: new[] { syntaxTree },
          references: references,
          options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));


      // Create an instance of our EnumGenerator incremental source generator
      var generator = new T();

      // The GeneratorDriver is used to run our generator against a compilation
      GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

      // Run the source generator!
      driver = driver.RunGenerators(compilation);
      var result = driver.GetRunResult().Results;
      return Verifier.Verify(driver)

        .UseDirectory("Snapshots");
      // Use verify to snapshot test the source generator output!
    }
  }
}
