//-----------------------------------------------------------------------
// <copyright file="MobileSerializerRegistrationGeneratorTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
//-----------------------------------------------------------------------
using Csla.Generator.AutoImplementProperties.CSharp.MobileSerializers;
using Csla.Generator.AutoImplementProperties.CSharp.Tests.DataPortalOperations;
using FluentAssertions;

namespace Csla.Generator.AutoImplementProperties.CSharp.Tests.MobileSerializers
{
  [TestClass]
  public class MobileSerializerRegistrationGeneratorTests : VerifyBase
  {
    private const string Serializers = """

      namespace TestApp
      {
        public class MoneySerializer : Csla.Serialization.Mobile.IMobileSerializer
        {
          public object Deserialize(Csla.Serialization.Mobile.SerializationInfo info) => new Money();
          public void Serialize(object obj, Csla.Serialization.Mobile.SerializationInfo info) { }
        }
      }
      """;

    [TestMethod("Type-side attribute")]
    public async Task TypeSide()
    {
      var source = """
        using Csla.Serialization.Mobile;
        using Csla.Serialization.Mobile.CustomSerializers;

        namespace TestApp
        {
          [MobileSerializer(typeof(MoneySerializer))]
          public class Money { }

          [MobileSerializer(typeof(PocoSerializer<Customer>))]
          public partial class Customer { }

          public partial class Customer { public string Name { get; set; } = ""; }

          [MobileSerializer(typeof(MoneySerializer))]
          internal struct Amount { }
        }
        """ + Serializers;

      await Verify(source);
    }

    [TestMethod("Serializer-side attribute with several types")]
    public async Task SerializerSide()
    {
      var source = """
        using Csla.Serialization.Mobile;
        using ThirdParty;

        namespace ThirdParty
        {
          public class Widget { }
          public class Gizmo { }
          public class Outer { public class Nested { } }
        }

        namespace TestApp
        {
          [MobileSerializerFor(typeof(Widget), typeof(Gizmo), typeof(Outer.Nested))]
          internal class WidgetSerializer : IMobileSerializer
          {
            public object Deserialize(SerializationInfo info) => new Widget();
            public void Serialize(object obj, SerializationInfo info) { }
          }
        }
        """;

      await Verify(source);
    }

    [TestMethod("Records as serializers and serialized types")]
    public async Task Records()
    {
      var source = """
        using Csla.Serialization.Mobile;

        namespace TestApp
        {
          [MobileSerializer(typeof(MoneySerializer))]
          public record Money(decimal Amount);

          public record class Widget(string Name);

          public record struct Point(int X, int Y);

          [MobileSerializerFor(typeof(Widget), typeof(Point))]
          public record WidgetSerializer : IMobileSerializer
          {
            public object Deserialize(SerializationInfo info) => new Widget("");
            public void Serialize(object obj, SerializationInfo info) { }
          }

          public record Price(decimal Amount);

          [MobileSerializerFor(typeof(Price))]
          public record class PriceSerializer : IMobileSerializer
          {
            public object Deserialize(SerializationInfo info) => new Price(0);
            public void Serialize(object obj, SerializationInfo info) { }
          }
        }
        """ + Serializers.Replace("new Money()", "new Money(0)");

      await Verify(source);
    }

    [TestMethod("Both attributes, with a pair declared twice")]
    public async Task Mixed()
    {
      var source = """
        using Csla.Serialization.Mobile;

        namespace TestApp
        {
          [MobileSerializer(typeof(MoneySerializer2))]
          public class Money { }

          public class Price { }

          [MobileSerializerFor(typeof(Money), typeof(Price))]
          public class MoneySerializer2 : MoneySerializer { }
        }
        """ + Serializers;

      await Verify(source);
    }

    [TestMethod("Invalid declarations are skipped")]
    public async Task InvalidSkipped()
    {
      var source = """
        using System.Collections.Generic;
        using Csla.Serialization.Mobile;

        namespace TestApp
        {
          [MobileSerializer(typeof(MoneySerializer))]
          public class Money { }

          [MobileSerializer(typeof(AbstractSerializer))]
          public class SkippedAbstract { }

          [MobileSerializer(typeof(NotASerializer))]
          public class SkippedNotSerializer { }

          [MobileSerializer(typeof(MoneySerializer))]
          public class SkippedGeneric<T> { }

          public class Outer
          {
            [MobileSerializer(typeof(MoneySerializer))]
            private class SkippedPrivate { }
          }

          public abstract class AbstractSerializer : MoneySerializer { }

          public class NotASerializer { }

          [MobileSerializerFor(typeof(List<int>), typeof(int[]), typeof(System.Guid), typeof(string), typeof(MobileType))]
          public class SkippedTargets : MoneySerializer { }

          public class MobileType : Csla.Core.MobileObject { }

          [MobileSerializerFor(typeof(Money))]
          public class SkippedOpen<T> : MoneySerializer { }
        }
        """ + Serializers;

      await Verify(source);
    }

    [TestMethod("No attributes, no output")]
    public void NoAttributes()
    {
      var source = """
        namespace TestApp
        {
          public class Money { }
        }
        """;

      GetGeneratedSources(source).Should().BeEmpty();
    }

    [TestMethod("Only invalid declarations, no output")]
    public void OnlyInvalid()
    {
      var source = """
        using Csla.Serialization.Mobile;

        namespace TestApp
        {
          [MobileSerializer(typeof(NotASerializer))]
          public class Money { }

          public class NotASerializer { }
        }
        """;

      GetGeneratedSources(source).Should().BeEmpty();
    }

    [TestMethod("Pipeline stages are cacheable")]
    public void StageCaching()
    {
      var source = """
        using Csla.Serialization.Mobile;

        namespace TestApp
        {
          [MobileSerializer(typeof(MoneySerializer))]
          public class Money { }

          public class Price { }

          [MobileSerializerFor(typeof(Price))]
          public class PriceSerializer : MoneySerializer { }
        }
        """ + Serializers;

      StageCachingTester.VerifyStageCaching<IncrementalMobileSerializerRegistrationGenerator>(source, typeof(TrackingNames));
    }

    private static string[] GetGeneratedSources(string source)
    {
      var (driver, compilation) = DataPortalOperationsTestHelper<IncrementalMobileSerializerRegistrationGenerator>.Setup(source);
      var result = driver.RunGenerators(compilation).GetRunResult();
      result.Diagnostics.Should().BeEmpty();
      return result.GeneratedTrees.Select(t => t.ToString()).ToArray();
    }

    private static Task Verify(string source, [System.Runtime.CompilerServices.CallerFilePath] string sourceFile = "")
      => DataPortalOperationsTestHelper<IncrementalMobileSerializerRegistrationGenerator>.Verify(source, sourceFile: sourceFile);
  }
}
