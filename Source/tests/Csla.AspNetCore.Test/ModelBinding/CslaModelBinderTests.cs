//-----------------------------------------------------------------------
// <copyright file="CslaModelBinderTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for the ASP.NET Core CslaModelBinder</summary>
//-----------------------------------------------------------------------

using System.Globalization;
using FluentAssertions;
using Csla.Configuration;
using Csla.Web.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.AspNetCore.Test.ModelBinding;

[TestClass]
public class CslaModelBinderTests
{
  private ServiceProvider _rootProvider;
  private IServiceScope _scope;
  private DefaultHttpContext _httpContext;
  private ModelStateDictionary _modelState;

  [TestInitialize]
  public void Setup()
  {
    var services = new ServiceCollection();
    services.AddLogging();
    services.AddCsla(o => o.AddAspNetCore());
    _rootProvider = services.BuildServiceProvider();
    _scope = _rootProvider.CreateScope();
    _httpContext = new DefaultHttpContext { RequestServices = _scope.ServiceProvider };
    _scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext = _httpContext;
  }

  [TestCleanup]
  public void Cleanup()
  {
    _scope?.Dispose();
    _rootProvider?.Dispose();
  }

  [TestMethod]
  public async Task Bind_NewRoot_SimplePropertiesAreBound()
  {
    var result = await BindAsync<Parent>(new()
    {
      ["Id"] = "42",
      ["Name"] = "root",
    });

    result.Id.Should().Be(42);
    result.Name.Should().Be("root");
  }

  [TestMethod]
  public async Task Bind_NewRoot_BindsChildrenAndGrandChildren()
  {
    var result = await BindAsync<Parent>(new()
    {
      ["Name"] = "root",
      ["Children[0].Name"] = "child0",
      ["Children[0].Quantity"] = "5",
      ["Children[0].GrandChildren[0].Name"] = "gc00",
      ["Children[0].GrandChildren[1].Name"] = "gc01",
      ["Children[1].Name"] = "child1",
      ["Children[1].Quantity"] = "7",
      ["Children[1].GrandChildren[0].Name"] = "gc10",
    });

    result.Name.Should().Be("root");
    result.Children.Should().NotBeNull();
    result.Children.IsChild.Should().BeTrue();
    result.Children.Should().HaveCount(2);
    result.Children[0].Name.Should().Be("child0");
    result.Children[0].Quantity.Should().Be(5);
    result.Children[0].IsChild.Should().BeTrue();
    result.Children[0].GrandChildren.Select(g => g.Name).Should().Equal("gc00", "gc01");
    result.Children[0].GrandChildren[0].IsChild.Should().BeTrue();
    result.Children[1].Name.Should().Be("child1");
    result.Children[1].Quantity.Should().Be(7);
    result.Children[1].GrandChildren.Select(g => g.Name).Should().Equal("gc10");
  }

  [TestMethod]
  public async Task Bind_NewRootWithPrefix_BindsChildrenAndGrandChildren()
  {
    var result = await BindAsync<Parent>(new()
    {
      ["parent.Name"] = "root",
      ["parent.Children[0].Name"] = "child0",
      ["parent.Children[0].GrandChildren[0].Name"] = "gc00",
    }, "parent");

    result.Name.Should().Be("root");
    result.Children.Should().ContainSingle();
    result.Children[0].Name.Should().Be("child0");
    result.Children[0].GrandChildren.Select(g => g.Name).Should().Equal("gc00");
  }

  [TestMethod]
  public async Task Bind_KeysAreCaseInsensitive()
  {
    var result = await BindAsync<Parent>(new()
    {
      ["children[0].name"] = "child0",
      ["children[0].grandchildren[0].name"] = "gc00",
    });

    result.Children.Should().ContainSingle();
    result.Children[0].Name.Should().Be("child0");
    result.Children[0].GrandChildren.Select(g => g.Name).Should().Equal("gc00");
  }

  [TestMethod]
  public async Task Bind_NewRoot_BindsSingleChildObject()
  {
    var result = await BindAsync<Parent>(new()
    {
      ["Name"] = "root",
      ["Address.Street"] = "Main St",
      ["Address.City"] = "Eau Claire",
    });

    result.Address.Should().NotBeNull();
    result.Address.IsChild.Should().BeTrue();
    result.Address.Street.Should().Be("Main St");
    result.Address.City.Should().Be("Eau Claire");
  }

  [TestMethod]
  public async Task Bind_ExistingGraph_UpdatesInPlace()
  {
    var existing = await FetchParentAsync();
    var child0 = existing.Children[0];
    var grandChild01 = child0.GrandChildren[1];

    var result = await BindAsync(new()
    {
      ["Name"] = "updated root",
      ["Address.Street"] = "updated street",
      ["Children[0].Name"] = "updated child0",
      ["Children[0].GrandChildren[1].Name"] = "updated gc01",
      ["Children[2].Name"] = "new child",
      ["Children[2].GrandChildren[0].Name"] = "new gc",
    }, model: existing);

    result.Should().BeSameAs(existing);
    result.Name.Should().Be("updated root");
    result.Id.Should().Be(1, "unposted fields of an existing object are left alone");
    result.Address.Street.Should().Be("updated street");
    result.Address.City.Should().Be("city");

    result.Children.Should().HaveCount(3);
    result.Children[0].Should().BeSameAs(child0);
    child0.Name.Should().Be("updated child0");
    child0.Quantity.Should().Be(1);
    child0.IsNew.Should().BeFalse();
    child0.GrandChildren[1].Should().BeSameAs(grandChild01);
    grandChild01.Name.Should().Be("updated gc01");
    child0.GrandChildren[0].Name.Should().Be("child0-grandchild0");

    result.Children[1].Name.Should().Be("child1");
    result.Children[1].IsDirty.Should().BeFalse();

    result.Children[2].Name.Should().Be("new child");
    result.Children[2].IsNew.Should().BeTrue();
    result.Children[2].IsChild.Should().BeTrue();
    result.Children[2].GrandChildren.Select(g => g.Name).Should().Equal("new gc");
  }

  [TestMethod]
  public async Task Bind_ExistingGraph_UnpostedChildrenAreNotCleared()
  {
    var existing = await FetchParentAsync();
    var children = existing.Children;
    var address = existing.Address;

    var result = await BindAsync(new()
    {
      ["Name"] = "updated root",
    }, model: existing);

    result.Name.Should().Be("updated root");
    result.Children.Should().BeSameAs(children);
    result.Children.Should().HaveCount(2);
    result.Address.Should().BeSameAs(address);
    result.IsDirty.Should().BeTrue();
    result.Children.IsDirty.Should().BeFalse();
  }

  [TestMethod]
  public async Task Bind_RootList_BindsItems()
  {
    var result = await BindAsync<RootChildList>(new()
    {
      ["[0].Name"] = "child0",
      ["[0].Quantity"] = "1",
      ["[1].Name"] = "child1",
      ["[1].Quantity"] = "2",
    });

    result.Should().HaveCount(2);
    result.Select(c => c.Name).Should().Equal("child0", "child1");
    result.Select(c => c.Quantity).Should().Equal(1, 2);
  }

  [TestMethod]
  public async Task Bind_RootListWithNestedKeys_ItemCountComesFromIndexes()
  {
    var result = await BindAsync<RootChildList>(new()
    {
      ["list[0].Name"] = "child0",
      ["list[0].GrandChildren[0].Name"] = "gc00",
      ["list[0].GrandChildren[1].Name"] = "gc01",
      ["list[0].GrandChildren[2].Name"] = "gc02",
      ["list[1].Name"] = "child1",
    }, "list");

    result.Should().HaveCount(2);
    result[0].Name.Should().Be("child0");
    result[0].GrandChildren.Select(g => g.Name).Should().Equal("gc00", "gc01", "gc02");
    result[1].Name.Should().Be("child1");
    result[1].GrandChildren.Should().BeNull();
  }

  [TestMethod]
  public async Task Bind_NonFormRequest_ReturnsNewObject()
  {
    var actionContext = new ActionContext(_httpContext, new RouteData(), new ControllerActionDescriptor());
    var metadata = new EmptyModelMetadataProvider().GetMetadataForType(typeof(Parent));
    var bindingContext = DefaultModelBindingContext.CreateBindingContext(actionContext, new CompositeValueProvider(), metadata, bindingInfo: null, string.Empty);
    var binder = new CslaModelBinder(_scope.ServiceProvider.GetRequiredService<ApplicationContext>());

    await binder.BindModelAsync(bindingContext);

    bindingContext.Result.IsModelSet.Should().BeTrue();
    var result = (Parent)bindingContext.Result.Model;
    result.Id.Should().Be(0);
    result.Children.Should().BeNull();
  }

  [TestMethod]
  public async Task Bind_ListWithAllowNewFalse_IgnoresNewItems()
  {
    // an invalid value shows the ignored item isn't created or bound
    var result = await BindAsync<NoNewItemsRootChildList>(new()
    {
      ["[0].Name"] = "child0",
      ["[0].Quantity"] = "not a number",
    });

    result.Should().BeEmpty();
    _modelState.ErrorCount.Should().Be(0);
  }

  [TestMethod]
  public async Task Bind_BindNeverProperty_IsNotBound()
  {
    var result = await BindAsync<Parent>(new()
    {
      ["Name"] = "root",
      ["Secret"] = "posted",
    });

    result.Name.Should().Be("root");
    result.Secret.Should().NotBe("posted");
  }

  [TestMethod]
  public async Task Bind_ExistingGraph_BindNeverPropertyIsNotBound()
  {
    var existing = await FetchParentAsync();

    await BindAsync(new()
    {
      ["Name"] = "updated root",
      ["Secret"] = "posted",
    }, model: existing);

    existing.Name.Should().Be("updated root");
    existing.Secret.Should().Be("secret");
  }

  [TestMethod]
  public async Task Bind_ChildTypeWithBindAttribute_OnlyBindsIncludedProperties()
  {
    var result = await BindAsync<Parent>(new()
    {
      ["MailingAddress.Street"] = "Main St",
      ["MailingAddress.City"] = "posted",
    });

    result.MailingAddress.Street.Should().Be("Main St");
    result.MailingAddress.City.Should().NotBe("posted");
  }

  [TestMethod]
  public async Task Bind_WaitsForAsyncRulesInWholeGraph()
  {
    var result = await BindAsync<Parent>(new()
    {
      ["Name"] = "root",
      ["Children[0].Name"] = "child0",
      ["Children[0].GrandChildren[0].Name"] = "bad",
    });

    result.IsBusy.Should().BeFalse();
    result.Children[0].GrandChildren[0].IsValid.Should().BeFalse();
    result.IsValid.Should().BeFalse();
  }

  [TestMethod]
  public async Task Bind_RootList_WaitsForAsyncRules()
  {
    var result = await BindAsync<RootChildList>(new()
    {
      ["[0].Name"] = "child0",
      ["[0].GrandChildren[0].Name"] = "bad",
    });

    result.IsBusy.Should().BeFalse();
    result[0].GrandChildren[0].IsValid.Should().BeFalse();
  }

  [TestMethod]
  public async Task TryUpdateModelAsync_WithIncludedProperties_OnlyBindsThoseProperties()
  {
    var existing = await FetchParentAsync();
    SetForm(new()
    {
      ["Name"] = "updated root",
      ["Id"] = "99",
      ["Address.Street"] = "updated street",
      ["Children[0].Name"] = "updated child0",
    });

    await CreateController().TryUpdateModelAsync(existing, string.Empty, p => p.Name);

    existing.Name.Should().Be("updated root");
    existing.Id.Should().Be(1);
    existing.Address.Street.Should().Be("street");
    existing.Children[0].Name.Should().Be("child0");
  }

  [TestMethod]
  public async Task TryUpdateModelAsync_UpdatesFetchedGraph()
  {
    var existing = await FetchParentAsync();
    var child1 = existing.Children[1];
    SetForm(new()
    {
      ["Name"] = "updated root",
      ["Children[1].Name"] = "updated child1",
      ["Children[1].GrandChildren[0].Name"] = "updated gc10",
    });
    await CreateController().TryUpdateModelAsync(existing);

    existing.Name.Should().Be("updated root");
    existing.Children[1].Should().BeSameAs(child1);
    child1.Name.Should().Be("updated child1");
    child1.GrandChildren[0].Name.Should().Be("updated gc10");
    existing.Children[0].Name.Should().Be("child0");
  }

  [TestMethod]
  public async Task Bind_WriteDeniedProperty_PostedOnNewObject_IsNotStoredAndAddsModelStateError()
  {
    var result = await BindAsync<Employee>(new()
    {
      ["Name"] = "employee",
      ["Salary"] = "999999",
    });

    result.Name.Should().Be("employee");
    result.Salary.Should().Be(0m);
    _modelState.IsValid.Should().BeFalse();
    _modelState["Salary"].Errors.Should().ContainSingle()
      .Which.ErrorMessage.Should().Be(Csla.Properties.Resources.PropertySetNotAllowed);
  }

  [TestMethod]
  public async Task Bind_WriteDeniedProperty_PostedOnExistingObject_IsNotStoredAndAddsModelStateError()
  {
    var existing = await FetchEmployeeAsync();

    await BindAsync(new()
    {
      ["Salary"] = "999999",
    }, model: existing);

    existing.Salary.Should().Be(100m);
    existing.IsDirty.Should().BeFalse();
    _modelState["Salary"].Errors.Should().ContainSingle();
  }

  [TestMethod]
  public async Task Bind_WriteDeniedProperty_NotPostedOnNewObject_AddsNoModelStateError()
  {
    var result = await BindAsync<Employee>(new()
    {
      ["Name"] = "employee",
    });

    result.Name.Should().Be("employee");
    result.Salary.Should().Be(0m);
    _modelState.ErrorCount.Should().Be(0);
  }

  [TestMethod]
  public async Task Bind_SetterThrows_PostedValueIsNotStoredAndAddsModelStateError()
  {
    var existing = await FetchEmployeeAsync();

    await BindAsync(new()
    {
      ["Name"] = "updated",
      ["Code"] = "bad",
    }, model: existing);

    existing.Name.Should().Be("updated");
    existing.Code.Should().Be("code");
    var expected = string.Format(CultureInfo.CurrentCulture, Csla.Properties.Resources.ModelBinderPropertySetFailed, "Code", Employee.BadCodeMessage);
    _modelState["Code"].Errors.Should().ContainSingle()
      .Which.ErrorMessage.Should().Be(expected);
  }

  [TestMethod]
  public async Task Bind_SetterAccepts_PostedValueIsStored()
  {
    var result = await BindAsync<Employee>(new()
    {
      ["Code"] = "good",
    });

    result.Code.Should().Be("good");
    _modelState.ErrorCount.Should().Be(0);
  }

  [TestMethod]
  public async Task Bind_PrivateSetterProperty_IsNotBound()
  {
    var created = await BindAsync<Employee>(new()
    {
      ["Rating"] = "5",
    });
    created.Rating.Should().Be(0);

    var existing = await FetchEmployeeAsync();
    await BindAsync(new()
    {
      ["Rating"] = "5",
    }, model: existing);
    existing.Rating.Should().Be(3);
    existing.IsDirty.Should().BeFalse();
    _modelState.ErrorCount.Should().Be(0);
  }

  [TestMethod]
  public async Task Bind_GetOnlyProperty_IsNotBound()
  {
    var existing = await FetchEmployeeAsync();

    await BindAsync(new()
    {
      ["Badge"] = "posted",
    }, model: existing);

    existing.Badge.Should().Be("badge");
    existing.IsDirty.Should().BeFalse();
    _modelState.ErrorCount.Should().Be(0);
  }

  [TestMethod]
  public async Task Bind_WriteDeniedPropertyWithPrefix_ModelStateKeyIncludesPrefix()
  {
    var result = await BindAsync<Employee>(new()
    {
      ["emp.Salary"] = "999999",
    }, "emp");

    result.Salary.Should().Be(0m);
    _modelState.ContainsKey("emp.Salary").Should().BeTrue();
    _modelState["emp.Salary"].Errors.Should().ContainSingle();
  }

  [TestMethod]
  public async Task Bind_WriteDeniedProperty_PostedUnchanged_AddsNoModelStateError()
  {
    var existing = await FetchEmployeeAsync();

    await BindAsync(new()
    {
      ["Name"] = "updated",
      ["Salary"] = "100",
    }, model: existing);

    existing.Name.Should().Be("updated");
    existing.Salary.Should().Be(100m);
    _modelState.ErrorCount.Should().Be(0);
  }

  [TestMethod]
  public async Task Bind_WriteDeniedProperty_PostedDefaultOnNewObject_AddsNoModelStateError()
  {
    var result = await BindAsync<Employee>(new()
    {
      ["Salary"] = "0",
    });

    result.Salary.Should().Be(0m);
    _modelState.ErrorCount.Should().Be(0);
  }

  [TestMethod]
  public async Task Bind_SetPropertyThrows_MessageIsUnwrapped()
  {
    var result = await BindAsync<Employee>(new()
    {
      ["Tag"] = "bad",
    });

    result.Tag.Should().NotBe("bad");
    var expected = string.Format(CultureInfo.CurrentCulture, Csla.Properties.Resources.ModelBinderPropertySetFailed, "Tag", Employee.BadTagMessage);
    _modelState["Tag"].Errors.Should().ContainSingle()
      .Which.ErrorMessage.Should().Be(expected);
  }

  [TestMethod]
  public async Task Bind_SetterThrowsWithReflectionFallback_MessageIsUnwrapped()
  {
    var original = ApplicationContext.UseReflectionFallback;
    ApplicationContext.UseReflectionFallback = true;
    try
    {
      var result = await BindAsync<Employee>(new()
      {
        ["Code"] = "bad",
      });

      result.Code.Should().NotBe("bad");
      var expected = string.Format(CultureInfo.CurrentCulture, Csla.Properties.Resources.ModelBinderPropertySetFailed, "Code", Employee.BadCodeMessage);
      _modelState["Code"].Errors.Should().ContainSingle()
        .Which.ErrorMessage.Should().Be(expected);
    }
    finally
    {
      ApplicationContext.UseReflectionFallback = original;
    }
  }

  [TestMethod]
  public async Task Bind_SetterThrowsUnexpectedException_MessageHidesDetails()
  {
    var result = await BindAsync<Employee>(new()
    {
      ["Notes"] = "boom",
    });

    result.Notes.Should().NotBe("boom");
    var expected = string.Format(CultureInfo.CurrentCulture, Csla.Properties.Resources.ModelBinderPropertyValueNotAccepted, "Notes");
    _modelState["Notes"].Errors.Should().ContainSingle()
      .Which.ErrorMessage.Should().Be(expected);
  }

  [TestMethod]
  public async Task Bind_UnpostedSetterThrowsUnexpectedException_Throws()
  {
    var act = () => BindAsync<NullRejectingSetter>(new());

    await act.Should().ThrowAsync<InvalidOperationException>();
  }

  [TestMethod]
  public async Task Bind_ValueNotConvertible_AddsModelStateError()
  {
    var result = await BindAsync<Parent>(new()
    {
      ["Id"] = "not a number",
      ["Name"] = "root",
    });

    result.Id.Should().Be(0);
    result.Name.Should().Be("root");
    var expected = string.Format(CultureInfo.CurrentCulture, Csla.Properties.Resources.ModelBinderPropertyValueNotAccepted, "Id");
    _modelState["Id"].Errors.Should().ContainSingle()
      .Which.ErrorMessage.Should().Be(expected);
  }

  [TestMethod]
  public async Task Bind_AsyncWriteRule_NewObjectBindsWithoutThrowing()
  {
    var result = await BindAsync<AsyncAuthEmployee>(new()
    {
      ["Name"] = "employee",
    });

    result.Name.Should().Be("employee");
    _modelState.ErrorCount.Should().Be(0);
  }

  [TestMethod]
  public async Task Bind_AsyncWriteRule_PostedChangeIsNotStored()
  {
    var result = await BindAsync<AsyncAuthEmployee>(new()
    {
      ["Salary"] = "999999",
    });

    result.Salary.Should().Be(0m);
    _modelState["Salary"].Errors.Should().ContainSingle()
      .Which.ErrorMessage.Should().Be(Csla.Properties.Resources.PropertySetNotAllowed);
  }

  [TestMethod]
  [DataRow("2026-11-23")]
  [DataRow("2026-11-23T14:05")]
  [DataRow("2026-11-23T14:05:30")]
  [DataRow("2026-11-23T14:05:30.123")]
  public async Task Bind_WriteDeniedDate_PostedBackAtLowerPrecision_AddsNoModelStateError(string posted)
  {
    var existing = await FetchEmployeeAsync();

    await BindAsync(new()
    {
      ["HiredOn"] = posted,
    }, model: existing);

    existing.HiredOn.Should().Be(Employee.FetchedHiredOn);
    _modelState.ErrorCount.Should().Be(0);
  }

  [TestMethod]
  [DataRow("2026-11-24")]
  [DataRow("2026-11-23T14:06")]
  public async Task Bind_WriteDeniedDate_PostedChanged_AddsModelStateError(string posted)
  {
    var existing = await FetchEmployeeAsync();

    await BindAsync(new()
    {
      ["HiredOn"] = posted,
    }, model: existing);

    existing.HiredOn.Should().Be(Employee.FetchedHiredOn);
    _modelState["HiredOn"].Errors.Should().ContainSingle();
  }

  [TestMethod]
  public async Task Bind_WriteDeniedNullString_PostedBackEmpty_AddsNoModelStateError()
  {
    var existing = await FetchEmployeeAsync();

    await BindAsync(new()
    {
      ["Grade"] = "",
    }, model: existing);

    _modelState.ErrorCount.Should().Be(0);
  }

  [TestMethod]
  public async Task Bind_WriteDeniedDecimal_PostedBackInOtherCulture_AddsNoModelStateError()
  {
    var culture = CultureInfo.CurrentCulture;
    CultureInfo.CurrentCulture = new CultureInfo("de-DE");
    try
    {
      var existing = await FetchEmployeeAsync();

      await BindAsync(new()
      {
        ["Rate"] = "100.5",
      }, model: existing);

      existing.Rate.Should().Be(100.5m);
      _modelState.ErrorCount.Should().Be(0);
    }
    finally
    {
      CultureInfo.CurrentCulture = culture;
    }
  }

  [TestMethod]
  public async Task Bind_InvariantField_ParsedWithInvariantCulture()
  {
    var culture = CultureInfo.CurrentCulture;
    CultureInfo.CurrentCulture = new CultureInfo("de-DE");
    try
    {
      var invariant = await BindAsync<Employee>(new()
      {
        ["Bonus"] = "12.5",
        ["__Invariant"] = "Bonus",
      });
      invariant.Bonus.Should().Be(12.5m);

      var local = await BindAsync<Employee>(new()
      {
        ["Bonus"] = "12,5",
      });
      local.Bonus.Should().Be(12.5m);
    }
    finally
    {
      CultureInfo.CurrentCulture = culture;
    }
  }

  [TestMethod]
  public async Task Bind_SetterFailsAfterWritingValue_ValueIsRestored()
  {
    var existing = await FetchEmployeeAsync();

    await BindAsync(new()
    {
      ["Mood"] = "angry",
    }, model: existing);

    existing.Mood.Should().Be("calm");
    _modelState["Mood"].Errors.Should().ContainSingle();
  }

  [TestMethod]
  public async Task Bind_ModelStateError_KeepsAttemptedValue()
  {
    await BindAsync<Parent>(new()
    {
      ["Id"] = "not a number",
    });

    _modelState["Id"].AttemptedValue.Should().Be("not a number");
    _modelState["Id"].RawValue.Should().NotBeNull();
  }

  [TestMethod]
  public async Task Bind_InitOnlyProperty_IsNotBound()
  {
    var existing = await FetchEmployeeAsync();

    await BindAsync(new()
    {
      ["ExternalId"] = "evil",
    }, model: existing);

    existing.ExternalId.Should().Be("external");
    _modelState.ErrorCount.Should().Be(0);
  }

  private async Task<Employee> FetchEmployeeAsync()
  {
    var portal = _scope.ServiceProvider.GetRequiredService<IDataPortal<Employee>>();
    var result = await portal.FetchAsync();
    result.IsDirty.Should().BeFalse();
    return result;
  }

  private async Task<Parent> FetchParentAsync()
  {
    var portal = _scope.ServiceProvider.GetRequiredService<IDataPortal<Parent>>();
    var result = await portal.FetchAsync(1);
    result.IsNew.Should().BeFalse();
    result.IsDirty.Should().BeFalse();
    return result;
  }

  private void SetForm(Dictionary<string, string> form)
  {
    var fields = new Dictionary<string, StringValues>(StringComparer.OrdinalIgnoreCase);
    foreach (var item in form)
      fields.Add(item.Key, new StringValues(item.Value));
    _httpContext.Request.ContentType = "application/x-www-form-urlencoded";
    _httpContext.Request.Form = new FormCollection(fields);
  }

  private async Task<T> BindAsync<T>(Dictionary<string, string> form, string modelName = "", T model = null)
    where T : class
  {
    SetForm(form);
    var actionContext = new ActionContext(_httpContext, new RouteData(), new ControllerActionDescriptor());
    var metadata = _scope.ServiceProvider.GetRequiredService<IModelMetadataProvider>().GetMetadataForType(typeof(T));
    var valueProvider = new FormValueProvider(BindingSource.Form, _httpContext.Request.Form, CultureInfo.InvariantCulture);
    var bindingContext = DefaultModelBindingContext.CreateBindingContext(actionContext, valueProvider, metadata, bindingInfo: null, modelName);
    bindingContext.Model = model;

    var binder = new CslaModelBinder(_scope.ServiceProvider.GetRequiredService<ApplicationContext>());
    await binder.BindModelAsync(bindingContext);

    _modelState = bindingContext.ModelState;
    bindingContext.Result.IsModelSet.Should().BeTrue();
    return (T)bindingContext.Result.Model;
  }

  private TestController CreateController()
  {
    var actionContext = new ActionContext(_httpContext, new RouteData(), new ControllerActionDescriptor());
    return new TestController
    {
      ControllerContext = new ControllerContext(actionContext)
      {
        ValueProviderFactories = [new FormValueProviderFactory()]
      }
    };
  }

  private sealed class TestController : ControllerBase
  {
  }
}
