# Upgrading to CSLA 6

CSLA 6 is a major release and so there a [number of breaking changes](https://github.com/MarimerLLC/csla/issues?q=is%3Aissue+project%3Amarimerllc%2Fcsla%2F11+is%3Aclosed+label%3A%22flag%2Fbreaking+change%22) compared to CSLA 5 or earlier releases of the framework.

In this document I'll try to highlight the most common changes required when upgrading your codebase from CSLA 5 to CSLA 6.

## Application Configuration

CSLA 6 requires the use of dependency injection (DI). This means that all apps using CSLA code must configure DI on startup, usually in `Program.cs`, `Main.cs`, `App.xaml.cs` or whatever is the very first code that is executed as your app starts up.

### Console App

A CSLA 5 console app had no required startup code.

A CSLA 6 console app must have at least the following:

```c#
using System;
using System.Threading.Tasks;
using Csla;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataPortalFactoryExample
{
  class Program
  {
    static async Task Main(string[] args)
    {
      var services = new ServiceCollection();
      services.AddCsla();
      var provider = services.BuildServiceProvider();
    }
  }
}
```

This code creates a DI `ServiceCollection`, adds the required CSLA services, and creates an `IServiceProvider` instance that is used to create any  services required by the app.

Where you go from here depends a lot on how you structure your app. You can either use DI throughout the app, or just expose a global `ApplicationContext` instance for use in your code.

#### Use DI Throughout

You can build the rest of the console app using DI (which is probably ideal), so the rest of the app's code has access to all services. In this case, all your actual code will go in instance methods of the `Program` class, or classes called from this starting code.

```c#
  public class Program
  {
    static async Task Main(string[] args)
    {
      var services = new ServiceCollection();
      services.AddCsla();
      services.AddTransient<Program>();
      var provider = services.BuildServiceProvider();
      var program = provider.GetRequiredService<Program>();
      await program.Run();
    }
    
    public Program()
    {
        // Inject any required services as parameters to
        // this constructor method
        //
        // Services might include `ApplicationContext` or
        // `IDataPortal<BusinessType>`
    }
    
    public async Task Run()
    {
        // write your code here
    }
  }
```

### ASP.NET Core App

ASP.NET Core apps (MVC or Razor Pages) have required DI for years, and so almost certainly already include a `Program.cs` and/or `Startup.cs` where services are registered and configured.

CSLA 5 had an `AddCsla` method to register services, and a `UseCsla` method to configure services.

CSLA 6 only has the `AddCsla` method, which is used to register and configure the framework services. It is necessary to use this method, and also to specify that the required aspnetcore services be registered.

In .NET 6, aspnetcore apps usually only have a `Program.cs`, and in that file you'll configure CSLA like this:

```c#
// If using Kestrel:
builder.Services.Configure<KestrelServerOptions>(options =>
{
  options.AllowSynchronousIO = true;
});

// If using IIS:
builder.Services.Configure<IISServerOptions>(options =>
{
  options.AllowSynchronousIO = true;
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddCsla(o => o.AddAspNetCore());
```

CSLA uses `MobileFormatter`, which currently uses synchronous calls to serialize and deserialize object graphs. Because of this, it is necessary to configure aspnetcore to `AllowSynchronousIO`.

CSLA relies on `HttpContext` to manage the current user identity and other per-request state. You must call `AddHttpContextAccessor` to configure aspnetcore to enable the use of the `HttpContext` type.

The `AddCsla` method registers CSLA types, and the `AddAspNetCore` method registers and configures types necessary for CSLA to work properly in the aspnetcore runtime environment.

> ⚠️ There are some differences if you are using server-side Blazor, which are covered elsewhere in this document.

### Windows Forms App

### WPF App

### Xamarin.Forms App

### Server-Side Blazor App

### Blazor WebAssembly App

## Using the Client-Side DataPortal

In previous versions of CSLA you would typically use static data portal helper methods to interact with the client-side data portal:

```c#
var person = await DataPortal.FetchAsync<PersonEdit>(42);
```

In CSLA 6 the client-side data portal is now a service you acquire via DI. I'll discuss the `IDataPortal<T>` and `IDataPortalFactory` service types, but first I want to talk about your options for injecting these types into your code so you can use them.

If your app already relies on DI, which is likely for aspnetcore or Blazor apps, you can just inject the required service into your code as normal.

If your existing codebase doesn't rely on DI, such as most Windows Forms, WPF, Xamarin, or Console apps, you have two choices. You can rework your code to use DI throughout, or you can implement a workaround.

Reworking your code to use DI throughout has some advantages, but can be a lot of work and is often not a viable option. In this document I'll focus on how to use CSLA 6 if you can't inject services throughout your codebase.

### Accessing the Client-Side DataPortal Without DI

In most Windows Forms, WPF, Xamarin, UWP, and Console apps your existing code probably does not rely on DI. As a result, you can't easily inject services into your pages, viewmodels, or other classes like you can in aspnetcore or Blazor.

The simplest way to upgrade such a codebase for CSLA 6 is to expose a global static `ApplicationContext` property for use throughout the app.

> ℹ️ This is only works for code that runs in a single process or AppDomain, and _will not work_ in multi-user server scenarios such as aspnetcore!

Earlier in this document I showed now to configure dependency injection and to call the `AddCsla` method to register and configure CSLA types for DI. 

It turns out that you can use methods on the `ApplicationContext` object to access all required CSLA services, including the client-side data portal. To do this, you must provide a global reference to the `ApplicationContext` instance for the app.

In WPF, UWP, and Xamarin you typically have an `App` class, represented by `App.xaml.cs`. In Windows Forms there is no `App` class, so you will need to add one to your project.

Whether the `App` class exists or you add it, this class will define a static `ApplicationContext` property:

```c#
  public class App
  {
    public static Csla.ApplicationContext ApplicationContext { get; internal set; }
  }
```

Again, your `App` class may have other code, especially in XAML-based apps. What you are adding is this new static property.

Then, in your startup code where you call `AddCsla`, it is necessary to set this new `ApplicationContext` property. For example:

```c#
      var services = new ServiceCollection();
      services.AddCsla();
      var provider = services.BuildServiceProvider();
      App.ApplicationContext = provider.GetRequiredService<ApplicationContext>();
```

At this point you have used the DI provider to create an instance of the `ApplicationContext` service, and that instance is now available globally to your app code via the `App` type.

Now, anywhere that you need a client-side data portal instance, you can use the `App.ApplicationContext` to create the required data portal service:

```c#
  var portal = App.ApplicationContext.GetRequiredService<IDataPortal<PersonEdit>>();
```

Again, in an aspnetcore or Blazor app (or any other app where DI is used throughout), you can simply inject the data portal service normally. What I'm showing here is a workaround to make it easier to upgrade existing codebases to CSLA 6 without major rework.

Now that you understand how to access the client-side data portal services, I'll discuss the `IDataPortal<T>` and `IDataPortalFactory` service types.

### IDataPortal<T> Service

In CSLA 6 the client-side data portal is an instance instead of a set of static methods like in previous versions of CSLA. The client-side data portal is now represented as an `IDataPortal<T>` service that you get via dependency injection.

The `IDataPortal<T>` expects that `T` is a business domain type, such as an editable object, read-only object, editable list, etc.

Once you get an instance of the client-side data portal service, you can use its methods to create, fetch, update, delete, or execute your business objects as in previous versions of CSLA.

For example, in CSLA 5 you might do this:

```c#
  var person = await DataPortal.FetchAsync<PersonEdit>(42);
```

In CSLA 6, assuming you've injected or acquired an instance of `IDataPortal<PersonEdit>` and have that service in a field named `portal`, your code will look like this:

```c#
  var person = await portal.FetchAsync(42);
```

Notice that you don't provide the generic type on the method call, because the `portal` service instance is already strongly typed to only work with the `PersonEdit` type.

For most existing code, once you provide access to the client-side data portal instance, changing the existing static method calls to instance method calls is a straightforward process.

### IDataPortalFactory Service

Sometimes you might have code that interacts with many different business domain types, such as a page that loads the primary business object, plus multiple read-only lists to populate combobox controls.

You can certainly inject all the necessary `IDataPortal<T>` types into such a page or method. The potential drawback with doing so, is that if your code only uses some of those data portal instances sometimes, and doesn't use them other times, then you've required the creation of services that you won't use.

A more efficient solution may be to inject an `IDataPortalFactory` service, and to use that factory service to create client-side data portal services as needed. 

For example, here's code where an `IDataPortalFactory` service was injected in the field `factory` and used:

```c#
  var personPortal = factory.GetPortal<PersonEdit>();
  var person = await personPortal.FetchAsync(42);
  var orderPortal = factory.GetPortal<OrderEdit>();
  var person = await orderPortal.FetchAsync(1701);
```

There is no requirement to use the `IDataPortalFactory` service. It is a convenience to simplify some coding scenarios, and possibly to allow you to optimize when or if client-side data portal instances are created.

### Using the Child Data Portal on the Client

Sometimes it is necessary to create new instances of child business domain types on the client, outside of a server-side data portal operation method. The most common example is in the `AddNew` or `AddNewCore` methods of a `BusinessListBase` subclass.

There is an `IChildDataPortal<T>` service you can use for this purpose. The `BusinessListBase` and other list base classes expose a protected `ApplicationContext` property you can use to access this service.

```c#
  protexted override ChildType AddNew()
  {
    var portal = ApplicationContext.GetRequiredService<IChildDataPortal<ChildType>>();
    Add(portal.Create());
  }
```

As in previous versions of CSLA, this only works if the child type's `ChildCreate` operation can run on the client device (meaning it doesn't use server-side resources such as a database).

## Implementing Data Portal Operations

### Encapsulated Data Portal Models

### Factory Data Portal Models
