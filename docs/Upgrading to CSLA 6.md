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

The first code executed in a Windows Forms app may vary depending on the age of your codebase. Sometimes it might be in a `Program.cs` file, other times it might be in the code-behind for the first window that's displayed. Most likely it is in `Program.cs` though.

Assuming you don't update your app to fully rely on dependency injection, the minimum you must do is initialize a DI provider, configure CSLA, and make the CSLA `ApplicationContext` instance available globally to your code. All this is done in `Program.cs`:

```c#
using System;
using System.Net.Http;
using System.Windows.Forms;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WindowsUI
{
  static class Program
  {
    public static Csla.ApplicationContext ApplicationContext { get; private set; }

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      var services = new ServiceCollection();
      services.AddTransient<HttpClient>();
      services.AddCsla(o => o
        .DataPortal(dp => dp
          .UseHttpProxy(hp => hp
            .DataPortalUrl = "https://localhost:44332/api/dataportal")));
      var provider = services.BuildServiceProvider();
      ApplicationContext = provider.GetService<Csla.ApplicationContext>();

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new Form1());
    }
  }
}
```

Notice that there's a public static `ApplicationContext` property to make the instance globally available to your code:

```c#
    public static Csla.ApplicationContext ApplicationContext { get; private set; }
```

Now, anywhere in your code you can access `ApplicationContext` like:

```c#
  var portal = Program.ApplicationContext.GetRequiredService<IDataPortal<PersonEdit>>();
```

Also notice the creation of a `ServiceCollection`, and how it is used to initialize CSLA (and maybe other services), and to create the DI provider:

```c#
      var services = new ServiceCollection();
      services.AddTransient<HttpClient>();
      services.AddCsla(o => o
        .DataPortal(dp => dp
          .UseHttpProxy(hp => hp
            .DataPortalUrl = "https://localhost:44332/api/dataportal")));
      var provider = services.BuildServiceProvider();
```

In this case CSLA is configured to use a remote data portal.

Finally, the `ApplicationContext` property is set by creating the required service instance using the DI provider:

```c#
      ApplicationContext = provider.GetService<Csla.ApplicationContext>();
```

At this point, without changing the existing codebase substantially, the app has access to CSLA and the `ApplicationContext` instance.

You _will_ need to go through your codebase and update any code that uses the static `ApplicationContext` type so it uses the new instance property. 

And you will need to update any code that uses the data portal as discussed in the section of this document covering the use of the client-side data portal.

### WPF App

In this section I am discussing how to update an existing WPF app without converting the app to fully rely on dependency injection.

The first code executed in a WPF app is usually in the `App.xaml.cs` file. The minimum requirement for using CSLA 6 in WPF is the same as for Windows Forms, so I won't repeat the full explanation of the code. Here's a typical `App.xaml.cs` startup:

```c#
using System;
using System.Net.Http;
using System.Windows;
using Csla;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WpfUI
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    public static ApplicationContext ApplicationContext { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
      var services = new ServiceCollection();
      services.AddTransient<HttpClient>();
      services.AddCsla(o => o
        .DataPortal(dp => dp
          .UseHttpProxy(hp => hp
            .DataPortalUrl = "https://localhost:44332/api/dataportal")));
      var provider = services.BuildServiceProvider();
      ApplicationContext = provider.GetService<ApplicationContext>();

      base.OnStartup(e);
    }
  }
}
```

As with Windows Forms, this code creates a `ServiceCollection` instance, uses that instance to register CSLA and other service types, creates a DI provider, and uses that provider to create an instance of the `ApplicationContext` service. A public static `ApplicationService` property makes this instance available globally to your code.

You can use the `ApplicationContext` property to access the service, and must update any code that used to rely on the static `ApplicationContext` type. For example:

```c#
      ApplicationContext = provider.GetService<Csla.ApplicationContext>();
```

You will also need to update all code that uses the client-side data portal, as described in the section of this document covering the use of the client-side data portal.

### Xamarin.Forms App

In this section I am discussing how to update an existing Xamarin.Forms app without converting the app to fully rely on dependency injection.

The process for a Xamarin.Forms app is identical to the process I described for updating a WPF app, and you should follow those instructions.

### Server-Side Blazor App

Blazor apps already directly rely on dependency injection to function, and so CSLA 6 fits naturally into the existing coding model. All you need to do is configure CSLA during app startup, usually in `Program.cs`.

Here is a typical configuration:

```c#
builder.Services.AddHttpContextAccessor();
builder.Services.AddCsla(options => options
  .AddAspNetCore()
  .AddServerSideBlazor());
```

Notice that the `AddCsla` method is called such that CSLA is configured with all services required by ASP.NET Core and also server-side Blazor. Remember that server-side Blazor apps are _hosted within aspnetcore_, and so both sets of services are required.

Also notice that the `HttpContextAccessor` is registered. This is required by CSLA, and if you skip this step you'll get a runtime exception as the app starts up.

Once this configuration is complete, you can inject CSLA service types such as `ApplicationContext` or `IDataPortal<T>` into your code as you would any other type of service.

### Blazor WebAssembly App

A Blazor WebAssembly (wasm) app runs entirely on the client device, and so is very similar in many ways to a Windows Forms, WPF, or Xamarin app. However, Blazor already directly relies on dependency injection, and so CSLA 6 fits naturally into the existing app model.

During app startup, in the `Program.cs` file, you must register the CSLA service types:

```c#
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// ...

builder.Services.AddCsla(options => options
  .AddBlazorWebAssembly()
  .DataPortal(dpo => dpo
    .UseHttpProxy(hpo => hpo
      .DataPortalUrl = "/api/DataPortal")));
```

In this example, the client-side data portal is configured to use the `HttpProxy` to communicate with a remote data portal server. I suspect this is the most common scenario when building client-side Blazor apps.

By default, the Blazor wasm project template includes the line of code that registers the `HttpClient` type. I am including that in this document, because if you use the `HttpProxy` data portal configuration, you _must_ also register the `HttpClient` service.

Once this configuration is complete, you can inject CSLA service types such as `ApplicationContext` or `IDataPortal<T>` into your code as you would any other type of service.

#### Configuring an ASP.NET Core Host for Blazor WebAssembly

Blazor WebAssembly apps are downloaded from some sort of web server. Sometimes you might build that web server using aspnetcore.

If all the web site does is download the static files for the Blazor wasm app, then you don't need to do anything special to configure the server. 

However, if your aspnetcore web site is used to download the Blazor wasm app _and also_ to host a data portal endpoint, then you'll need to configure the web site to support the server-side data portal.

To do this, in `Program.cs` you must register the CSLA service types so they are available to the web site code:

```c#
builder.Services.AddHttpContextAccessor();
builder.Services.AddCsla(o => o
  .AddAspNetCore());
```

CSLA 6 requires access to `HttpContext` and so you _must_ register the `HttpContextAccessor` service. Also, when calling the `AddCsla` method, you must ensure that the aspnetcore services are registered by calling the `AddAspNetCore` method.

It is also the case that an aspnetcore web site that provides a data portal endpoint for a Blazor wasm app must be configuring to support CORS. For example, you need code similar to this in your `Program.cs` file:

```c#
string BlazorClientPolicy = "AllowAllOrigins";

builder.Services.AddCors(options =>
{
  options.AddPolicy(BlazorClientPolicy,
    builder =>
    {
      builder
      .AllowAnyOrigin()
      .AllowAnyHeader()
      .AllowAnyMethod();
    });
});
```

This is not specific to CSLA, but is required for a Blazor client app to have access to any controller provided by your web site.

Finally, because the CSLA `MobileFormatter` uses a synchronous API when deserializing data, you must configure aspnetcore to allow the use of synchronous behaviors:

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
```

At this point you can create a server-side data portal endpoint as discussed later in this document.

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

The data portal supports four models for data portal operation implementation:

1. Encapsulated invocation
1. Factory implementation
1. Encapsulated implementation
1. Factory invocation

> ℹ️ These are discussed in the _Using CSLA 4_ book.

I recommend options 1 or 2, and will discuss them here. If you use one of the other models you should be able to extrapolate how to make them work based on the information in this document.

### Encapsulated Data Portal Models

Starting with CSLA 5 you had the ability to inject services into data portal operation methods, and also use data portal operation attributes instead of hard-coded method names. For example:

```c#
  [Fetch]
  private void Fetch(int id, [Inject] IPersonDal dal)
  {
      var data = dal.GetPerson(id);
      using (BypassPropertyChecks)
      {
        // map data into the business object properties
      }
      BusinessRules.CheckRules(); // optionally check rules
  }
```

CSLA 6 removes the vitual "DataPortal_XYZ" methods in all base classes, and you should use the data portal operation attributes as shown here.

Because the data portal is now an _instance_ instead of a static helper, if your data portal operation needs to interact with another root object or a child object, you will need to inject the appropriate service. The services you can inject are:

* `IDataPortal<T>` - interact with root type `T`
* `IChildDataPortal<T>` - interact with child type `T`
* `IDataPortalFactory` - use to create instances of `IDataPortal<T>`

For example:

```c#
  [Fetch]
  private async Task Fetch(
    int id, 
    [Inject] IOrderDal dal),
    [Inject] IChildDataPortal<LineItems> childPortal)
  {
      var data = dal.GetOrder(id);
      using (BypassPropertyChecks)
      {
        // map data into the business object properties
        LineItems = await childPortal.FetchChild(id);
      }
      BusinessRules.CheckRules(); // optionally check rules
  }
```

Notice how an instance of `IChildDataPortal<T>` is injected and then used to fetch a child object as part of the overall data portal operation.

Also, the CSLA base classes no longer generally implement virtual `DataPortal_XYZ` methods for you to override. As a result, if you have a method like this:

```c#
    protected override void DataPortal_Create()
    {
      base.DataPortal_Create();
    }
```

It will need to be reimplemented like this:

```c#
    [Create]
    private void DataPortal_Create()
    {
      BusinessRules.CheckRules();
    }
```

You don't need to retain the `DataPortal_XYZ` method naming convention, and I typically name a method like this `Create` so it fits into a more normal method naming model.

### Factory Data Portal Models

The _factory implementation_ model separates the code for the data access layer out of the business class entirely, and into its own factory class.

For example, a business class just specifies the type of the factory to be invoked by the data portal:

```c#
  [Serializable]
  [Csla.Server.ObjectFactory(typeof(PersonFactory))]
  public class PersonEdit : BusinessBase<PersonEdit>
```

In CSLA 5 the `PersonFactory` class implements methods to create, fetch, insert, update, and delete instances of the business class. 

In CSLA 6, two changes are required.

1. The `ObjectFactory` subclass must implement a constructor so the `ApplicationContext` can be injected.
1. You must use the `CreateInstanceDI` method of the `ApplicationContext` object to create instances of business domain types.

So in CSLA 6 a factory looks like this:

```c#
  public class PersonFactory : Csla.Server.ObjectFactory
  {
    public PersonFactory(ApplicationContext applicationContext)
      : base(applicationContext) { }

    public PersonEdit Create()
    {
      var result = ApplicationContext.CreateInstanceDI<PersonEdit>();
      LoadProperty(result, PersonEdit.NameProperty, "Xiaoping");
      CheckRules(result);
      MarkNew(result);
      return result;
    }
  }
```

This is nearly identical to CSLA 5 code, but with a constructor:

```c#
    public PersonFactory(ApplicationContext applicationContext)
      : base(applicationContext) { }
```

And the use of `CreateInstanceDI` to create an instance of the business domain type:

```c#
      var result = ApplicationContext.CreateInstanceDI<PersonEdit>();
```

This is necessary, because in CSLA 6 business domain types must be initialized by the `ApplicationContext` to operate properly within the context of the CSLA framework.

If you use `Activator.CreateInstance` or the `new` keyword, the domain object will not be properly initialized and you will get runtime exceptions.

## Implementing a Remote Data Portal Host

Every remote data portal host must configure DI, including registering CSLA types and configuring them _for the server environment_. The idea of the server having different configuration from the client is not new, and has been consistent in all versions of CSLA. What is new and different is that the configuration flows from DI and the `AddCsla` method.

### ASP.NET Core Endpoint

The most common host for a remote data portal is ASP.NET Core, using a controller as an endpoint for the data portal.

> ℹ️ A Blazor WebAssembly client app needs a special endpoint configuration as discussed in the next section of this document.

In CSLA 5 the controller would look something like this:

```c#
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BlazorExample.Server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DataPortalController : Csla.Server.Hosts.HttpPortalController
  {
    [HttpGet]
    public string Get()
    {
      return "DataPortal is running";
    }
  }
}
```

In CSLA 6 the controller needs access to the `ApplicationContext` service, and so it looks like this:

```c#
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BlazorExample.Server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DataPortalController : Csla.Server.Hosts.HttpPortalController
  {
    public DataPortalController(Csla.ApplicationContext applicationContext)
      : base(applicationContext) { }

    [HttpGet]
    public string Get() => "DataPortal is running";
  }
}
```

Notice the constructor that requires the `ApplicationContext` service and provides it to the base implementation.

### ASP.NET Core Endpoint for Blazor WebAssembly

Blazor WebAssembly (wasm) has a limitation where it can't transfer a binary data stream to or from the server. 

When you register the CSLA 6 services in a Blazor wasm app, it defaults to configuring `HttpProxy` to use a text-based protocol. Your data portal server endpoint must match that protocol.

This means that you need a text-based endpoint to support Blazor wasm client apps, even if you already have a "normal" binary-based endpoint for other client app types.

To create such an endpoint, add a controller to the `Controllers` folder in the aspnetcore project:

```c#
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BlazorExample.Server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DataPortalTextController : Csla.Server.Hosts.HttpPortalController
  {
    public DataPortalTextController(Csla.ApplicationContext applicationContext)
      : base(applicationContext) 
    { 
      UseTextSerialization = true;
    }

    [HttpGet]
    public string Get() => "DataPortal is running";
  }
}
```

The controller is named to make it clear that it is a text-based endpoint, and in the constructor the `UseTextSerialization` property is set to `true` so the data portal requires text-based data streams.

The behavior of the data portal is unaffected by this change, only the byte steam being transferred over the network is affected.

> ℹ️ The byte stream is actually still a binary stream, it is just base64 encoded so it flows through the Blazor wasm http API as text.

### WCF Endpoint

Microsoft has chosen to not carry WCF forward into the modern .NET world, so it doesn't exist in .NET Core or .NET 6.

As a result, you should replace all use of the WCF data portal channel (i.e. `WcfProxy`) with another data portal channel such as the HTTP channel (i.e. `HttpProxy`).

> ℹ There is an open source WCF project to provide WCF support in modern .NET: https://github.com/CoreWCF/CoreWCF. If you really need to use WCF, you could build a data portal channel (proxy/host) based on this open source project.
> 
> There is an open backlog item in CSLA to support this scenario: https://github.com/MarimerLLC/csla/issues/1183
