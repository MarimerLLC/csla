using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class MobileSerializerAttributeUsageAnalyzerTests
  {
    private const string Usings =
      """
      using System.Collections.Generic;
      using Csla.Serialization.Mobile;
      using Csla.Serialization.Mobile.CustomSerializers;

      """;

    private const string Serializers =
      """

      public class S1 : IMobileSerializer
      {
        public object Deserialize(SerializationInfo info) => null;
        public void Serialize(object obj, SerializationInfo info) { }
      }

      public class S2 : IMobileSerializer
      {
        public object Deserialize(SerializationInfo info) => null;
        public void Serialize(object obj, SerializationInfo info) { }
      }
      """;

    [TestMethod]
    public void VerifySupportedDiagnostics()
    {
      var analyzer = new MobileSerializerAttributeUsageAnalyzer();
      var diagnostics = analyzer.SupportedDiagnostics;
      Assert.AreEqual(2, diagnostics.Length);

      var invalid = diagnostics.Single(d => d.Id == Constants.AnalyzerIdentifiers.InvalidCustomSerializerDeclaration);
      Assert.AreEqual("Custom serializer declaration is not valid", invalid.Title.ToString());
      Assert.AreEqual(Constants.Categories.Usage, invalid.Category);
      Assert.AreEqual(DiagnosticSeverity.Error, invalid.DefaultSeverity);
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.InvalidCustomSerializerDeclaration, nameof(MobileSerializerAttributeUsageAnalyzer)),
        invalid.HelpLinkUri);

      var conflict = diagnostics.Single(d => d.Id == Constants.AnalyzerIdentifiers.ConflictingCustomSerializers);
      Assert.AreEqual("Type has more than one custom serializer", conflict.Title.ToString());
      Assert.AreEqual(Constants.Categories.Usage, conflict.Category);
      Assert.AreEqual(DiagnosticSeverity.Warning, conflict.DefaultSeverity);
      Assert.AreEqual(HelpUrlBuilder.Build(Constants.AnalyzerIdentifiers.ConflictingCustomSerializers, nameof(MobileSerializerAttributeUsageAnalyzer)),
        conflict.HelpLinkUri);
    }

    [TestMethod]
    public async Task ValidDeclarations()
    {
      var code = Usings +
        """
        [assembly: MobileSerializerRegistration(typeof(Gadget), typeof(PocoSerializer<Gadget>))]

        [MobileSerializer(typeof(S1))]
        public class Money { }

        [MobileSerializer(typeof(PocoSerializer<Customer>))]
        public struct Customer { }

        public class Widget { }
        public class Gadget { }

        [MobileSerializerFor(typeof(Widget))]
        internal class WidgetSerializer : IMobileSerializer
        {
          public object Deserialize(SerializationInfo info) => null;
          public void Serialize(object obj, SerializationInfo info) { }
        }

        public class Outer
        {
          [MobileSerializer(typeof(S1))]
          internal class Inner { }
        }
        """ + Serializers;

      await TestHelpers.RunAnalysisAsync<MobileSerializerAttributeUsageAnalyzer>(code, []);
    }

    [DataTestMethod]
    [DataRow("public class Bad { }", "the serializer 'Bad' does not implement IMobileSerializer")]
    [DataRow("public abstract class Bad : S1 { }", "the serializer 'Bad' is abstract")]
    [DataRow("public class Bad : S1 { private Bad() { } }", "the serializer 'Bad' has no public constructor")]
    [DataRow("public struct Bad { }", "the serializer 'Bad' is not a class")]
    [DataRow("public interface Bad : IMobileSerializer { }", "the serializer 'Bad' is not a class")]
    public async Task TypeSideWithInvalidSerializer(string serializer, string problem)
    {
      var code = Usings +
        $$"""
        {{serializer}}

        [MobileSerializer(typeof(Bad))]
        public class Money { }
        """ + Serializers;

      await AssertInvalidAsync(code, problem);
    }

    [TestMethod]
    public async Task TypeSideWithOpenGenericSerializer()
    {
      var code = Usings +
        """
        [MobileSerializer(typeof(PocoSerializer<>))]
        public class Money { }
        """;

      await AssertInvalidAsync(code, "the serializer 'Csla.Serialization.Mobile.CustomSerializers.PocoSerializer<>' is an open generic type");
    }

    [TestMethod]
    public async Task TypeSideOnGenericType()
    {
      var code = Usings +
        """
        [MobileSerializer(typeof(S1))]
        public class Money<T> { }
        """ + Serializers;

      await AssertInvalidAsync(code, "the type 'Money<T>' is generic, and custom serializers can be registered only for non-generic types");
    }

    [TestMethod]
    public async Task TypeSideOnPrivateNestedType()
    {
      var code = Usings +
        """
        public class Outer
        {
          [MobileSerializer(typeof(S1))]
          private class Money { }
        }
        """ + Serializers;

      await AssertInvalidAsync(code, "the type 'Outer.Money' is not accessible to an assembly-level attribute");
    }

    [TestMethod]
    public async Task TypeSideWithPrivateNestedSerializer()
    {
      var code = Usings +
        """
        public class Outer
        {
          [MobileSerializer(typeof(Bad))]
          public class Money { }

          [MobileSerializerFor(typeof(Widget))]
          private class Bad : S1 { }
        }

        public class Widget { }
        """ + Serializers;

      // Reported for both the type-side and the serializer-side declaration.
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new MobileSerializerAttributeUsageAnalyzer());
      Assert.AreEqual(2, diagnostics.Count);
      Assert.IsTrue(diagnostics.All(d => d.Id == Constants.AnalyzerIdentifiers.InvalidCustomSerializerDeclaration));
      Assert.IsTrue(diagnostics.All(d => d.GetMessage().Contains("the serializer 'Outer.Bad' is not accessible to an assembly-level attribute")));
    }

    [TestMethod]
    public async Task SerializerSideOnGenericSerializer()
    {
      var code = Usings +
        """
        public class Widget { }

        [MobileSerializerFor(typeof(Widget))]
        public class Bad<T> : S1 { }
        """ + Serializers;

      await AssertInvalidAsync(code, "the serializer 'Bad<T>' is an open generic type");
    }

    [TestMethod]
    public async Task SerializerSideNotImplementingInterface()
    {
      var code = Usings +
        """
        public class Widget { }

        [MobileSerializerFor(typeof(Widget))]
        public class Bad { }
        """;

      await AssertInvalidAsync(code, "the serializer 'Bad' does not implement IMobileSerializer");
    }

    [TestMethod]
    public async Task SerializerSideWithOneInvalidType()
    {
      var code = Usings +
        """
        public class Widget { }

        [MobileSerializerFor(typeof(Widget), typeof(List<int>), typeof(int[]))]
        public class WidgetSerializer : S1 { }
        """ + Serializers;

      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new MobileSerializerAttributeUsageAnalyzer());
      Assert.AreEqual(2, diagnostics.Count);
      Assert.IsTrue(diagnostics.All(d => d.Id == Constants.AnalyzerIdentifiers.InvalidCustomSerializerDeclaration));

      var generic = diagnostics.Single(d => d.GetMessage().Contains("'System.Collections.Generic.List<int>' is generic"));
      Assert.AreEqual("typeof(List<int>)", generic.Location.SourceTree.GetText().ToString(generic.Location.SourceSpan));
      var array = diagnostics.Single(d => d.GetMessage().Contains("'int[]' is not a class or struct"));
      Assert.AreEqual("typeof(int[])", array.Location.SourceTree.GetText().ToString(array.Location.SourceSpan));
    }

    [TestMethod]
    public async Task AssemblyRegistrationWithInvalidSerializer()
    {
      var code = Usings +
        """
        [assembly: MobileSerializerRegistration(typeof(Widget), typeof(Bad))]

        public class Widget { }

        public class Bad { }
        """;

      await AssertInvalidAsync(code, "the serializer 'Bad' does not implement IMobileSerializer");
    }

    [TestMethod]
    public async Task TypeSideConflictsWithAssemblyRegistration()
    {
      var code = Usings +
        """
        [assembly: MobileSerializerRegistration(typeof(Money), typeof(S2))]

        [MobileSerializer(typeof(S1))]
        public class Money { }
        """ + Serializers;

      // In a real build the generator adds Money -> S1 to the assembly registrations;
      // here only the hand-written registration is visible, so the conflict is
      // reported on the type-side attribute.
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new MobileSerializerAttributeUsageAnalyzer());
      Assert.AreEqual(1, diagnostics.Count);
      Assert.AreEqual(Constants.AnalyzerIdentifiers.ConflictingCustomSerializers, diagnostics[0].Id);
      Assert.AreEqual(
        "The type 'Money' is registered with more than one custom serializer ('S1', 'S2'); MobileFormatter uses whichever is registered first",
        diagnostics[0].GetMessage());
    }

    [TestMethod]
    public async Task AssemblyRegistrationsConflict()
    {
      var code = Usings +
        """
        [assembly: MobileSerializerRegistration(typeof(Widget), typeof(S1))]
        [assembly: MobileSerializerRegistration(typeof(Widget), typeof(S2))]

        public class Widget { }
        """ + Serializers;

      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new MobileSerializerAttributeUsageAnalyzer());
      Assert.AreEqual(2, diagnostics.Count);
      Assert.IsTrue(diagnostics.All(d => d.Id == Constants.AnalyzerIdentifiers.ConflictingCustomSerializers));
    }

    [TestMethod]
    public async Task SameSerializerDeclaredTwiceIsNotAConflict()
    {
      var code = Usings +
        """
        [assembly: MobileSerializerRegistration(typeof(Money), typeof(S1))]

        [MobileSerializer(typeof(S1))]
        public class Money { }
        """ + Serializers;

      await TestHelpers.RunAnalysisAsync<MobileSerializerAttributeUsageAnalyzer>(code, []);
    }

    [TestMethod]
    public async Task SerializerSideConflictsWithCslaClaimsPrincipalSerializer()
    {
      var code = Usings +
        """
        [MobileSerializerFor(typeof(System.Security.Claims.ClaimsPrincipal))]
        public class MyPrincipalSerializer : S1 { }
        """ + Serializers;

      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new MobileSerializerAttributeUsageAnalyzer());
      Assert.AreEqual(1, diagnostics.Count);
      Assert.AreEqual(Constants.AnalyzerIdentifiers.ConflictingCustomSerializers, diagnostics[0].Id);
      StringAssert.Contains(diagnostics[0].GetMessage(), "'Csla.Serialization.Mobile.CustomSerializers.ClaimsPrincipalSerializer'");
      StringAssert.Contains(diagnostics[0].GetMessage(), "'MyPrincipalSerializer'");
      StringAssert.Contains(diagnostics[0].GetMessage(), "'System.Security.Claims.ClaimsPrincipal'");
    }

    private static async Task AssertInvalidAsync(string code, string problem)
    {
      var diagnostics = await TestHelpers.GetDiagnosticsAsync(code, new MobileSerializerAttributeUsageAnalyzer());
      Assert.AreEqual(1, diagnostics.Count, string.Join(Environment.NewLine, diagnostics));
      Assert.AreEqual(Constants.AnalyzerIdentifiers.InvalidCustomSerializerDeclaration, diagnostics[0].Id);
      Assert.AreEqual($"The custom serializer declaration is not valid because {problem}", diagnostics[0].GetMessage());
    }
  }
}
