using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class EvaluateManagedBackingFieldsWalkerTests
  {
    private static async Task<EvaluateManagedBackingFieldsWalker> GetWalker(string code)
    {
      var document = TestHelpers.Create(code);
      var root = await document.GetSyntaxRootAsync();
      var model = await document.GetSemanticModelAsync();

      var fieldNode = root.DescendantNodes().OfType<FieldDeclarationSyntax>().Single();
      var fieldSymbol = model.GetDeclaredSymbol(fieldNode.Declaration.Variables[0]) as IFieldSymbol;
      var propertyNode = root.DescendantNodes().OfType<PropertyDeclarationSyntax>().Single();

      var getter = propertyNode.AccessorList.Accessors.Single(
        _ => _.IsKind(SyntaxKind.GetAccessorDeclaration)).Body;

      return new EvaluateManagedBackingFieldsWalker(getter, model, fieldSymbol);
    }

    [TestMethod]
    public async Task WalkWhenFieldIsUsedByPropertyInfoManagement()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public static readonly PropertyInfo<string> DataProperty =
    RegisterProperty<string>(_ => _.Data);

  public string Data
  {
    get { return GetProperty(DataProperty); }
    set { SetProperty(DataProperty, value); }
  }
}";
      var walker = await GetWalker(code);
      Assert.IsTrue(walker.UsesField);
    }

    [TestMethod]
    public async Task WalkWhenFieldIsNotUsedByPropertyInfoManagement()
    {
      var code =
@"using Csla;

public class A : BusinessBase<A>
{
  public static readonly PropertyInfo<string> DataProperty =
    RegisterProperty<string>(_ => _.Data);
  public string Data { get; set; }
}";
      var walker = await GetWalker(code);
      Assert.IsFalse(walker.UsesField);
    }
  }
}