# ASP.NET MVC

## ASP.NET Core

CSLA 5 and higher supports ASP.NET Core - MVC and Razor Pages - with various helper types in the `Csla.AspNetCore` namespace.

CSLA 6 and higher supports Blazor WebAssembly, and CSLA 8 and higher supports all Blazor models (Server, WebAssembly, and Auto).

CSLA 10 adds OpenTelemetry instrumentation for the data portal, enabling integration with observability platforms like Prometheus, Grafana, Azure Monitor, and .NET Aspire dashboards. See the [OpenTelemetry Dashboard](OpenTelemetry-Dashboard.md) documentation for more information.

> ℹ️ If you host code in ASP.NET Core you _must_ add the `Csla.AspNetCore` NuGet package to your project. This is true if you are using MVC, Razor Pages, server-side Blazor, or creating an app server using aspnetcore.

### Binding editable object graphs

`AddAspNetCore()` registers `CslaModelBinder` for editable business objects and editable lists. The binder binds the whole posted object graph. That includes the root object's properties, editable child objects and editable child lists, nested to any depth. Form field names follow the standard MVC conventions, which the tag helpers and `Html.*For` helpers generate when you render the graph in loops:

```html
<input asp-for="Name" />
<input asp-for="Address.Street" />
@for (var i = 0; i < Model.Children.Count; i++)
{
  <input asp-for="Children[i].Name" />
  @for (var j = 0; j < Model.Children[i].GrandChildren.Count; j++)
  {
    <input asp-for="Children[i].GrandChildren[j].Name" />
  }
}
```

There are two ways to bind.

**As an action parameter.** The binder creates a new object graph from the posted values:

```csharp
[HttpPost]
public async Task<ActionResult> Create(OrderEdit order)
```

Every object the binder creates is new (`IsNew` is `true`), including the children. This suits creating new objects. For editing existing data, prefer the next approach, so child objects keep their state and save as updates.

**With `TryUpdateModelAsync`.** Fetch the object graph, then bind the posted values into it:

```csharp
[HttpPost]
public async Task<ActionResult> Edit(int id)
{
  var order = await orderPortal.FetchAsync(id);
  if (await TryUpdateModelAsync(order) && order.IsValid)
  {
    order = await order.SaveAsync();
    return RedirectToAction(nameof(Index));
  }
  return View(order);
}
```

When it binds into an existing object, the binder:
* updates the existing objects in place;
* only sets values that were posted, leaving other properties and children untouched;
* adds a new child item for a posted list index past the end of the list.

In both approaches, the binder:
* waits for async business rules anywhere in the graph to finish, so the object isn't busy when your action runs;
* doesn't bind properties marked `[BindNever]`;
* binds only the root properties you allow, when you pass an include list to `TryUpdateModelAsync` (for example `TryUpdateModelAsync(order, "", o => o.Name)`) or put `[Bind]` on the action parameter. An included child list or child object is bound as a whole;
* applies `[Bind]` on a child's class to that child's properties.

Limits:
* New list items and child objects are created using the property's or list's declared type. Polymorphic children are bound correctly only when they already exist, as in the `TryUpdateModelAsync` approach.
* Posting a list never removes items. Remove deleted items in your controller code.
* Lists whose `AllowNew` is `false` ignore posted items beyond their existing ones.
* Read-only child objects aren't bound.

## ASP.NET in .NET Framework

CSLA 10 continues to support ASP.NET MVC 5 projects on .NET Framework 4.6.2-4.8. You must use dependency injection in your project for CSLA to work correctly.

_Please remember that ASP.NET MVC does run in ASP.NET. So nearly all the FAQ items under the [Web Forms](Web-forms.md) topic apply to ASP.NET MVC as well._

## Does CSLA .NET work with ASP.NET MVC?

Yes, in your web project add the NuGet package specific to the version of MVC that you are targeting. That will automatically bring in the CSLA assemblies bound to that version of MVC.

CSLA .NET works great with any of the "M" patterns (MVVM, MVC, MVP, etc), since it is all about helping you create a great Model.

In short, your CSLA .NET business objects are the 'model', and you should use Controller code to link your model to the View. Similarly, for any postbacks that trigger an action, the Controller code should simply invoke appropriate methods on the Model so the business objects do the work.

The result is that CSLA .NET allows you to create a powerful Model, minimizing code in the Controller and allowing the Controller to focus purely on user interaction and orchestration, leaving all business behaviors to the Model where they belong.

CSLA .NET 3.8 introduced a CslaModelBinder class to smooth the one outstanding issue with using ASP.NET MVC with the cool Controller "data binding" capabilities. This is optional, but addresses some issues that come up if you use MVC 2 and DataAnnotations attributes and Controller data binding together.
