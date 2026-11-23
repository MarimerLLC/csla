//-----------------------------------------------------------------------
// <copyright file="NotNullRuleContextTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for the IRuleContext members that take a NotNullPropertyInfo</summary>
//-----------------------------------------------------------------------

#nullable enable

using Csla.Core;
using Csla.Rules;
using Csla.Testing;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.NotNull
{
  [TestClass]
  public class NotNullRuleContextTests
  {
    private static CslaTestHost _testHost = default!;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testHost = CslaTestHost.Create();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
      _testHost?.Dispose();
    }

    private static RuleContext NewContext(params (IPropertyInfo Property, object? Value)[] inputs)
    {
      var values = inputs.ToDictionary(_ => _.Property, _ => _.Value);
      return new RuleContext(_testHost.ApplicationContext, _ => { }, new NotNullRuleContextRoot.NoOpRule(NotNullRuleContextRoot.TriggerProperty), null, values);
    }

    // In a business object

    [TestMethod]
    public void TypedMembers_InARule_ReadInputAndWriteOutput()
    {
      var obj = _testHost.ApplicationContext.CreateInstanceDI<NotNullRuleContextRoot>();
      obj.LoadName("abc");

      obj.Trigger = 1;

      obj.Copy.Should().Be("abc!");
    }

    // GetInputValue

    [TestMethod]
    public void GetInputValue_NotNullProperty_ReturnsValue()
    {
      var context = NewContext((NotNullRuleContextRoot.NameProperty, "abc"));

      string value = context.GetInputValue(NotNullRuleContextRoot.NameProperty);

      value.Should().Be("abc");
    }

    [TestMethod]
    public void GetInputValue_UnloadedPropertyWithoutDefault_ReturnsNullWithoutThrowing()
    {
      // an incomplete object, for example a rule that runs before the property is loaded
      var context = NewContext((NotNullRuleContextRoot.DataProperty, null));

      var value = (object?)context.GetInputValue(NotNullRuleContextRoot.DataProperty);

      value.Should().BeNull();
    }

    [TestMethod]
    public void GetInputValue_PropertyThatIsNotAnInput_ThrowsKeyNotFoundException()
    {
      var context = NewContext();

      context.Invoking(c => c.GetInputValue(NotNullRuleContextRoot.NameProperty))
        .Should().ThrowExactly<KeyNotFoundException>();
    }

    [TestMethod]
    public void GetInputValue_NullProperty_ThrowsArgumentNullException()
    {
      var context = NewContext();

      context.Invoking(c => c.GetInputValue((NotNullPropertyInfo<string>)null!))
        .Should().ThrowExactly<ArgumentNullException>();
    }

    // TryGetInputValue

    [TestMethod]
    public void TryGetInputValue_NotNullPropertyWithValue_ReturnsTrueAndValue()
    {
      var context = NewContext((NotNullRuleContextRoot.NameProperty, "abc"));

      var found = context.TryGetInputValue(NotNullRuleContextRoot.NameProperty, out var value);

      using (new AssertionScope())
      {
        found.Should().BeTrue();
        value.Should().Be("abc");
      }
    }

    [TestMethod]
    public void TryGetInputValue_NullValue_ReturnsFalse()
    {
      var context = NewContext((NotNullRuleContextRoot.DataProperty, null));

      var found = context.TryGetInputValue(NotNullRuleContextRoot.DataProperty, out var value);

      using (new AssertionScope())
      {
        found.Should().BeFalse();
        value.Should().BeNull();
      }
    }

    [TestMethod]
    public void TryGetInputValue_PropertyThatIsNotAnInput_ReturnsFalse()
    {
      var context = NewContext();

      var found = context.TryGetInputValue(NotNullRuleContextRoot.NameProperty, out _);

      found.Should().BeFalse();
    }

    [TestMethod]
    public void TryGetInputValue_NullProperty_ThrowsArgumentNullException()
    {
      var context = NewContext();

      context.Invoking(c => c.TryGetInputValue((NotNullPropertyInfo<string>)null!, out _))
        .Should().ThrowExactly<ArgumentNullException>();
    }

    // AddOutValue

    [TestMethod]
    public void AddOutValue_NotNullProperty_StoresValue()
    {
      var context = NewContext();
      var data = new byte[] { 1 };

      context.AddOutValue(NotNullRuleContextRoot.DataProperty, data);

      context.OutputPropertyValues[NotNullRuleContextRoot.DataProperty].Should().BeSameAs(data);
    }

    [TestMethod]
    public void AddOutValue_NullString_StoresEmptyString()
    {
      var context = NewContext();

      context.AddOutValue(NotNullRuleContextRoot.NameProperty, null!);

      context.OutputPropertyValues[NotNullRuleContextRoot.NameProperty].Should().Be(string.Empty);
    }

    [TestMethod]
    public void AddOutValue_NullForPropertyWithDefault_StoresDefault()
    {
      var context = NewContext();

      context.AddOutValue(NotNullRuleContextRoot.CodeProperty, null!);

      context.OutputPropertyValues[NotNullRuleContextRoot.CodeProperty].Should().Be("unknown");
    }

    [TestMethod]
    public void AddOutValue_NullForPropertyWithoutDefault_ThrowsArgumentNullException()
    {
      var context = NewContext();

      context.Invoking(c => c.AddOutValue(NotNullRuleContextRoot.DataProperty, null!))
        .Should().ThrowExactly<ArgumentNullException>();
    }

    [TestMethod]
    public void AddOutValue_NullProperty_ThrowsArgumentNullException()
    {
      var context = NewContext();

      context.Invoking(c => c.AddOutValue((NotNullPropertyInfo<string>)null!, "x"))
        .Should().ThrowExactly<ArgumentNullException>();
    }
  }

  [Serializable]
  public class NotNullRuleContextRoot : BusinessBase<NotNullRuleContextRoot>
  {
    public static readonly PropertyInfo<int> TriggerProperty = RegisterProperty<int>(nameof(Trigger));
    public int Trigger
    {
      get => GetProperty(TriggerProperty);
      set => SetProperty(TriggerProperty, value);
    }

    public static readonly NotNullPropertyInfo<string> NameProperty = RegisterNotNullProperty<string>(nameof(Name), null, string.Empty);
    public string Name => GetProperty(NameProperty);

    public static readonly NotNullPropertyInfo<byte[]> DataProperty = RegisterNotNullProperty<byte[]>(nameof(Data));
    public byte[] Data => GetProperty(DataProperty);

    public static readonly NotNullPropertyInfo<string> CodeProperty = RegisterNotNullProperty<string>(nameof(Code), null, "unknown");
    public string Code => GetProperty(CodeProperty);

    public static readonly NotNullPropertyInfo<string> CopyProperty = RegisterNotNullProperty<string>(nameof(Copy), null, string.Empty);
    public string Copy => GetProperty(CopyProperty);

    public void LoadName(string value) => LoadProperty(NameProperty, value);

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new CopyNameRule(TriggerProperty));
    }

    private class CopyNameRule : BusinessRule
    {
      public CopyNameRule(IPropertyInfo primaryProperty)
        : base(primaryProperty)
      {
        InputProperties.Add(NameProperty);
        AffectedProperties.Add(CopyProperty);
      }

      protected override void Execute(IRuleContext context)
      {
        string name = context.GetInputValue(NameProperty);
        context.AddOutValue(CopyProperty, name + "!");
      }
    }

    internal class NoOpRule : BusinessRule
    {
      public NoOpRule(IPropertyInfo primaryProperty)
        : base(primaryProperty)
      {
      }

      protected override void Execute(IRuleContext context)
      {
      }
    }
  }
}
