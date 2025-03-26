﻿//-----------------------------------------------------------------------
// <copyright file="BusyStatusTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System.ComponentModel.DataAnnotations;
using Csla.Configuration;
using Csla.TestHelpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.BizRules
{
  [TestClass]
  public class BusinessRuleTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestInitialize]
    public void Initialize()
    {
      TestResults.Reinitialise();
    }

    [TestMethod]
    public void DefaultDataAnnotationsScan()
    { 
      var portal = _testDIContext.ServiceProvider.GetRequiredService<IDataPortal<TestBusinessRule>>();
      var obj = portal.Create();
      obj.Name = "";
      obj.IsValid.Should().BeFalse();
    }

    [TestMethod]
    public void DisableDataAnnotationsScan()
    {
      var options = _testDIContext.ServiceProvider.GetRequiredService<CslaOptions>();
      options.ScanForDataAnnotations(false);

      var portal = _testDIContext.ServiceProvider.GetRequiredService<IDataPortal<TestBusinessRule>>();
      var obj = portal.Create();
      obj.Name = "";
      obj.IsValid.Should().BeTrue();
    }
  }

  public class TestBusinessRule : BusinessBase<TestBusinessRule>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    [Required]
    public string Name
    {
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }

    [Create]
    private void Create()
    {
      BusinessRules.CheckRules();
    }
  }
}
