using Csla.Generator.AutoImplementProperties.CSharp.AutoImplement;
using Csla.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;


namespace Csla.Generator.Tests
{
  [TestClass]
  public class IncrementalAutoImplementPropertiesPartialsGeneratorTests
  {
    [Ignore]
    [TestMethod]
    public void TestInitialize()
    {
      var generator = new IncrementalAutoImplementPropertiesPartialsGenerator();
      var context = new Microsoft.CodeAnalysis.IncrementalGeneratorInitializationContext();
      generator.Initialize(context);
      Assert.IsNotNull(context);
    }

    [Ignore]
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
