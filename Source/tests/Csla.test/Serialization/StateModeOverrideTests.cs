//-----------------------------------------------------------------------
// <copyright file="StateModeOverrideTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests that overrides of the obsolete StateMode overloads still run</summary>
//-----------------------------------------------------------------------

using Csla.Serialization.Mobile;
using Csla.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.Serialization
{
  [TestClass]
  public class StateModeOverrideTests
  {
    private static CslaTestHost _testHost;

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

    [TestMethod]
    public void ObsoleteOverride_RoundTripsCustomState()
    {
      var root = _testHost.GetDataPortal<LegacyStateRoot>().Fetch("legacy");

      var clone = root.Clone();

      clone.Custom.Should().Be("legacy");
    }

    [TestMethod]
    public void ObsoleteOverride_RunsAfterBaseStateIsRestored()
    {
      var root = _testHost.GetDataPortal<LegacyStateRoot>().Fetch("legacy");
      root.IsNew.Should().BeFalse();

      var clone = root.Clone();

      clone.IsNewAfterBaseSetState.Should().BeFalse();
    }

    [TestMethod]
    public void NewOverride_RoundTripsCustomState()
    {
      var root = _testHost.GetDataPortal<StateRoot>().Fetch("current");

      var clone = root.Clone();

      clone.Custom.Should().Be("current");
      clone.IsNewAfterBaseSetState.Should().BeFalse();
    }
  }

#pragma warning disable CS0618, CS0672 // the obsolete StateMode overloads are what this type tests
  [Serializable]
  public partial class LegacyStateRoot : BusinessBase<LegacyStateRoot>
  {
    private string _custom;

    public string Custom => _custom;

    [NonSerialized]
    private bool? _isNewAfterBaseSetState;

    public bool? IsNewAfterBaseSetState => _isNewAfterBaseSetState;

    protected override void OnGetState(SerializationInfo info, Core.StateMode mode)
    {
      base.OnGetState(info, mode);
      info.AddValue("_custom", _custom);
    }

    protected override void OnSetState(SerializationInfo info, Core.StateMode mode)
    {
      base.OnSetState(info, mode);
      _isNewAfterBaseSetState = IsNew;
      _custom = info.GetValue<string>("_custom");
    }

    [Fetch]
    private void Fetch(string custom)
    {
      _custom = custom;
    }
  }
#pragma warning restore CS0618, CS0672

  [Serializable]
  public partial class StateRoot : BusinessBase<StateRoot>
  {
    private string _custom;

    public string Custom => _custom;

    [NonSerialized]
    private bool? _isNewAfterBaseSetState;

    public bool? IsNewAfterBaseSetState => _isNewAfterBaseSetState;

    protected override void OnGetState(SerializationInfo info)
    {
      base.OnGetState(info);
      info.AddValue("_custom", _custom);
    }

    protected override void OnSetState(SerializationInfo info)
    {
      base.OnSetState(info);
      _isNewAfterBaseSetState = IsNew;
      _custom = info.GetValue<string>("_custom");
    }

    [Fetch]
    private void Fetch(string custom)
    {
      _custom = custom;
    }
  }
}
