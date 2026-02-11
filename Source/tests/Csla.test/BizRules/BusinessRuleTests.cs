//-----------------------------------------------------------------------
// <copyright file="BusinessRuleTests.cs" company="Marimer LLC">
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
      SetScanForDataAnnotations(true);

      var portal = _testDIContext.ServiceProvider.GetRequiredService<IDataPortal<TestBusinessRule>>();
      var obj = portal.Create();
      obj.Name = "";
      obj.IsValid.Should().BeFalse();
    }

    [TestMethod]
    [TestCategory("SkipOnCIServer")]
    public void DisableDataAnnotationsScan()
    {
      SetScanForDataAnnotations(false);

      // use different type to avoid caching
      var portal = _testDIContext.ServiceProvider.GetRequiredService<IDataPortal<TestBusinessRule2>>();
      var obj = portal.Create();
      obj.Name = "";
      obj.IsValid.Should().BeTrue();
    }

    [TestMethod]
    [TestCategory("ThreadSafety")]
    public void CheckBrokenRulesForThreadSafety()
    {
      var tasks = new List<Task>();

      SetScanForDataAnnotations(true);

      // use different type to avoid caching
      var portal = _testDIContext.ServiceProvider.GetRequiredService<IDataPortal<TestBusinessRule3>>();
      var obj = portal.Create();

      tasks.Add(Task.Run(() =>
      {
        for (int i = 0; i < 10000; i++)
        {
          obj.FirstName = (i % 2 == 0) ? "" : "Drop Dead";
          obj.LastName = (i % 2 == 0) ? "" : "Fred";
        }
      }));
      tasks.Add(Task.Run(() =>
      {
        try
        {
          for (int i = 0; i < 10000; i++)
          {
            var list = obj.GetBrokenRules().ToThreadsafeList();
            foreach (var item in list)
            {
              var property = item.Property;
              var description = item.Description;
            }
          }
        }
        catch (Exception ex)
        {
          Assert.Fail($"Exception thrown during GetBrokenRules(): {ex}");
        }
      }
        ));

      Task.WaitAll(tasks);
    }

    private void SetScanForDataAnnotations(bool enable)
    {
      var options = _testDIContext.ServiceProvider.GetRequiredService<CslaOptions>();
      options.ScanForDataAnnotations(enable);
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

  public class TestBusinessRule2 : BusinessBase<TestBusinessRule2>
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

  public class TestBusinessRule3 : BusinessBase<TestBusinessRule3>
  {
    public static readonly PropertyInfo<string> FirstNameProperty = RegisterProperty<string>(nameof(FirstName));
    [Required]
    public string FirstName
    {
      get => GetProperty(FirstNameProperty);
      set => SetProperty(FirstNameProperty, value);
    }

    public static readonly PropertyInfo<string> LastNameProperty = RegisterProperty<string>(nameof(LastName));
    [Required]
    public string LastName
    {
      get => GetProperty(LastNameProperty);
      set => SetProperty(LastNameProperty, value);
    }

    [Create]
    private void Create()
    {
      BusinessRules.CheckRules();
    }
  }
}