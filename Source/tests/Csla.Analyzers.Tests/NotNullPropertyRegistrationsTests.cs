using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class NotNullPropertyRegistrationsTests
  {
    private const string Code =
      """
      #nullable enable
      using Csla;
      using Csla.Core;

      public class Child : BusinessBase<Child> { }

      public class A : BusinessBase<A>
      {
        public static readonly NotNullPropertyInfo<Child> NameOnly = RegisterNotNullProperty<Child>(nameof(NameOnlyName));
        public static readonly NotNullPropertyInfo<Child> LiteralName = RegisterNotNullProperty<Child>("Literal");
        public static readonly NotNullPropertyInfo<Child> LambdaName = RegisterNotNullProperty<Child>(_ => _.LambdaTarget);
        public static readonly NotNullPropertyInfo<Child> FriendlyName = RegisterNotNullProperty<Child>(nameof(FriendlyNameName), "Friendly");
        public static readonly NotNullPropertyInfo<Child> WithDefault = RegisterNotNullProperty<Child>(nameof(WithDefaultName), null, new Child());
        public static readonly NotNullPropertyInfo<Child> ImplicitNewDefault = RegisterNotNullProperty<Child>(nameof(ImplicitNewDefaultName), null, new());
        public static readonly NotNullPropertyInfo<Child> Reordered = RegisterNotNullProperty<Child>(friendlyName: "Friendly", propertyName: nameof(ReorderedName));
        public static readonly NotNullPropertyInfo<Child> InfoReordered = RegisterNotNullProperty(new NotNullPropertyInfo<Child>(friendlyName: "Friendly", name: nameof(InfoReorderedName)));
        public static readonly NotNullPropertyInfo<Child> NamedDefault = RegisterNotNullProperty<Child>(nameof(NamedDefaultName), defaultValue: new Child());
        public static readonly NotNullPropertyInfo<Child> LambdaDefault = RegisterNotNullProperty<Child>(_ => _.LambdaTarget, null, new Child());
        public static readonly NotNullPropertyInfo<string> StringEmpty = RegisterNotNullProperty<string>(nameof(StringEmptyName), null, string.Empty);
        public static readonly NotNullPropertyInfo<Child> InfoNoDefault = RegisterNotNullProperty(new NotNullPropertyInfo<Child>(nameof(InfoNoDefaultName)));
        public static readonly NotNullPropertyInfo<Child> InfoFriendly = RegisterNotNullProperty(new NotNullPropertyInfo<Child>(nameof(InfoFriendlyName), "Friendly", typeof(A)));
        public static readonly NotNullPropertyInfo<Child> InfoDefault = RegisterNotNullProperty(new NotNullPropertyInfo<Child>(nameof(InfoDefaultName), null, typeof(A), new Child()));
        public static readonly NotNullPropertyInfo<Child> InfoImplicit = RegisterNotNullProperty<Child>(new(nameof(InfoImplicitName)));
        public static readonly NotNullPropertyInfo<Child> TypeAndInfo = RegisterNotNullProperty(typeof(A), new NotNullPropertyInfo<Child>(nameof(TypeAndInfoName), null, typeof(A), new Child()));
        public static readonly NotNullPropertyInfo<Child> Factory = RegisterNotNullProperty(PropertyInfoFactory.Factory.CreateNotNull<Child>(typeof(A), nameof(FactoryName)));
        public static readonly NotNullPropertyInfo<Child> FactoryDefault = RegisterNotNullProperty(PropertyInfoFactory.Factory.CreateNotNull<Child>(typeof(A), nameof(FactoryDefaultName), null, new Child()));
        public static readonly NotNullPropertyInfo<Child> Suppressed = RegisterNotNullProperty<Child>(nameof(SuppressedName))!;
        public static readonly NotNullPropertyInfo<Child> FromVariable = RegisterNotNullProperty(Existing);
        public static readonly NotNullPropertyInfo<Child> FromMethod = Make();
        public static readonly NotNullPropertyInfo<int> ValueType = RegisterNotNullProperty<int>(nameof(ValueTypeName));
        public static readonly NotNullPropertyInfo<Child> NoInitializer;
        public static readonly PropertyInfo<Child> Plain = RegisterProperty<Child>(nameof(PlainName));
        private static readonly NotNullPropertyInfo<Child> Existing = new(nameof(ExistingName));
        private static NotNullPropertyInfo<Child> Make() => Existing;

        public Child LambdaTarget => GetProperty(LambdaName);
      }
      """;

    private static async Task<IFieldSymbol> GetFieldAsync(string name)
    {
      var compilation = await TestHelpers.Create(Code).Project.GetCompilationAsync();
      return compilation.GetTypeByMetadataName("A").GetMembers(name).OfType<IFieldSymbol>().Single();
    }

    [DataTestMethod]
    [DataRow("NameOnly", false)]
    [DataRow("LiteralName", false)]
    [DataRow("LambdaName", false)]
    [DataRow("FriendlyName", false)]
    [DataRow("WithDefault", true)]
    [DataRow("ImplicitNewDefault", true)]
    [DataRow("Reordered", false)]
    [DataRow("InfoReordered", false)]
    [DataRow("NamedDefault", true)]
    [DataRow("LambdaDefault", true)]
    [DataRow("StringEmpty", true)]
    [DataRow("InfoNoDefault", false)]
    [DataRow("InfoFriendly", false)]
    [DataRow("InfoDefault", true)]
    [DataRow("InfoImplicit", false)]
    [DataRow("TypeAndInfo", true)]
    [DataRow("Factory", false)]
    [DataRow("FactoryDefault", true)]
    [DataRow("Suppressed", false)]
    [DataRow("ValueType", false)]
    public async Task TryGetHasDefaultReadsKnownShapes(string fieldName, bool expected)
    {
      var field = await GetFieldAsync(fieldName);
      Assert.IsTrue(NotNullPropertyRegistrations.TryGetHasDefault(field, default, out var hasDefault), "known shape");
      Assert.AreEqual(expected, hasDefault, nameof(hasDefault));
    }

    [DataTestMethod]
    [DataRow("FromVariable")]
    [DataRow("FromMethod")]
    [DataRow("NoInitializer")]
    public async Task TryGetHasDefaultRejectsUnknownShapes(string fieldName)
    {
      var field = await GetFieldAsync(fieldName);
      Assert.IsFalse(NotNullPropertyRegistrations.TryGetHasDefault(field, default, out _));
      Assert.IsFalse(NotNullPropertyRegistrations.IsNoDefaultReference(field, default));
    }

    [DataTestMethod]
    [DataRow("NameOnly", "NameOnlyName")]
    [DataRow("LiteralName", "Literal")]
    [DataRow("LambdaName", "LambdaTarget")]
    [DataRow("InfoNoDefault", "InfoNoDefaultName")]
    [DataRow("InfoImplicit", "InfoImplicitName")]
    [DataRow("TypeAndInfo", "TypeAndInfoName")]
    [DataRow("Factory", "FactoryName")]
    [DataRow("Reordered", "ReorderedName")]
    [DataRow("InfoReordered", "InfoReorderedName")]
    [DataRow("FromMethod", "FromMethod")]
    [DataRow("NoInitializer", "NoInitializer")]
    public async Task GetRegisteredNameReadsTheNameArgument(string fieldName, string expected)
    {
      var field = await GetFieldAsync(fieldName);
      Assert.AreEqual(expected, NotNullPropertyRegistrations.GetRegisteredName(field, default));
    }

    [TestMethod]
    public async Task GetRegisteredNameFallsBackToTheFieldName()
    {
      var compilation = await TestHelpers.Create(
        """
        using Csla;
        public class A : BusinessBase<A>
        {
          public static readonly NotNullPropertyInfo<A> NameProperty = Make();
          private static NotNullPropertyInfo<A> Make() => null!;
        }
        """).Project.GetCompilationAsync();
      var field = compilation.GetTypeByMetadataName("A").GetMembers("NameProperty").OfType<IFieldSymbol>().Single();
      Assert.AreEqual("Name", NotNullPropertyRegistrations.GetRegisteredName(field, default));
    }

    [DataTestMethod]
    [DataRow("NameOnly", true)]
    [DataRow("WithDefault", false)]
    [DataRow("ValueType", false)]
    [DataRow("Plain", false)]
    public async Task IsNoDefaultReference(string fieldName, bool expected)
    {
      var field = await GetFieldAsync(fieldName);
      Assert.AreEqual(expected, NotNullPropertyRegistrations.IsNoDefaultReference(field, default));
    }
  }
}
