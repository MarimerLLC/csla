# Upgrading to CSLA 8

CSLA 8 is a major release and so there a number of breaking changes.

In this document I'll try to highlight the most common changes required when upgrading your codebase from CSLA 6 or CSLA 7 to CSLA 8.

If you are upgrading from a version of CSLA prior to 6, you should review the [Upgrading to CSLA 6](https://github.com/MarimerLLC/csla/blob/main/docs/Upgrading%20to%20CSLA%206.md) document, as most of its contents are relevant. This document only covers the changes from CSLA 6 or CSLA 7 to CSLA 8.

## Platform Support

CSLA 8 removes support for .NET Core 3.1 and for Xamarin. If you are using these platforms you will need to stay on an older version of CSLA until you can upgrade to a newer platform.

## Application Configuration

There are some changes to CSLA configuration in CSLA 8. These changes will impact you if you have an existing .NET 6 or .NET 7 Blazor app and just upgrade that app to .NET 8 and CSLA 8. They will also impact you if you are building a new .NET 8 app with the new Blazor solution template.

### Blazor App (legacy style server)

Starting with CSLA version 8.1.0 you no longer need to configure Kestrel or IIS to allow synchronous IO. This is because CSLA now supports async serialization in aspnetcore. This means that you can remove the `UseSynchronousIO` call from your `Program.cs` file.

```csharp
// Required by CSLA data portal controller. If using Kestrel:
builder.Services.Configure<KestrelServerOptions>(options =>
{
  options.AllowSynchronousIO = true;
});

// Required by CSLA data portal controller. If using IIS:
builder.Services.Configure<IISServerOptions>(options =>
{
  options.AllowSynchronousIO = true;
});
```

Because CSLA now defaults to using a new state management solution for Blazor 8, if you are using a _pure_ Blazor server project style (no pre-rendering or InteractiveAuto render modes), then you'll need to configure CSLA to use the in-memory state management solution. This is done in the `Program.cs` file.

```csharp
builder.Services.AddCsla(o => o
  .AddAspNetCore()
  .AddServerSideBlazor(ssb => ssb.UseInMemoryApplicationContextManager = true));
```

### Blazor App (legacy style WebAssembly client)

There should be no configuration changes necessary for a pure Blazor WebAssembly client app (no server pre-rendering or InteractiveAuto render modes).

### Blazor App (new solution template)

Starting with CSLA version 8.1.0 you no longer need to configure Kestrel or IIS to allow synchronous IO. This is because CSLA now supports async serialization in aspnetcore. This means that you can remove the `UseSynchronousIO` call from your `Program.cs` file.

```csharp
// Required by CSLA data portal controller. If using Kestrel:
builder.Services.Configure<KestrelServerOptions>(options =>
{
  options.AllowSynchronousIO = true;
});

// Required by CSLA data portal controller. If using IIS:
builder.Services.Configure<IISServerOptions>(options =>
{
  options.AllowSynchronousIO = true;
});
```

CSLA 8 supports a new state management subsyste for Blazor 8. This subsystem is designed to streamline the process of using the new InteractiveAuto render mode, and other new render modes, in Blazor 8. It helps ensure that `LocalContext`, `ClientContext`, and the current user identity are all available to you on server-static pages, server-interactive pages, and WebAssembly-interactive pages. As a result, you can also use the InteractiveAuto render mode, because that mode uses those other render modes on your behalf.

You must do some configuration if your Blazor 8 app includes server-side and client-side page render modes. If your app is server-side only, the new subsystem is automatically used.

Here's a typical server-side configuration in `Program.cs`:

```csharp
builder.Services.AddCsla(o => o
  .AddAspNetCore()
  .AddServerSideBlazor());
```

If you do have a "client" project in your solution, you are probably supporting WebAssembly. In that case, you need to configure the new state management subsystem to support the WebAssembly client. Here's a typical client-side configuration in `Program.cs`:

```csharp
builder.Services.AddCsla(o => o
  .AddBlazorWebAssembly(o => o.SyncContextWithServer = true)
  .DataPortal(o => o.ClientSideDataPortal(o => o
    .UseHttpProxy(o => o.DataPortalUrl = "/api/dataportal"))));
```

For this to work, there are two things you must have on the Blazor server side:

1. The data portal controller
2. The state management subsystem controller

The data portal controller is a CSLA controller that handles data portal requests. You can add it to your project by adding a new controller class that inherits from `Csla.Server.Hosts.AspNetCore.CslaDataPortalController`. The state management subsystem controller is a CSLA controller that handles state management requests. You can add it to your project by adding a new controller class that inherits from `Csla.Server.Hosts.AspNetCore.CslaStateController`. For example:

```c#
using Csla;
using Microsoft.AspNetCore.Mvc;

namespace ProjectTracker.Blazor.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DataPortalController(ApplicationContext applicationContext) 
    : Csla.Server.Hosts.HttpPortalController(applicationContext)
  {
    [HttpGet]
    public string Get()
    {
      return "DataPortal running...";
    }
  }
}
```

This is a standard data portal server endpoint.

The state management subsystem controller defaults to being named `CslaStateController`. You can add it to your project by adding a new controller class that inherits from `Csla.Server.Hosts.AspNetCore.CslaStateController`. For example:

```c#
using Microsoft.AspNetCore.Mvc;
using Csla;
using Csla.State;

namespace ProjectTracker.Blazor.Controllers
{
  /// <summary>
  /// Gets and puts the current user session data
  /// from the Blazor wasm client components.
  /// </summary>
  /// <param name="applicationContext"></param>
  /// <param name="sessionManager"></param>
  [ApiController]
  [Route("[controller]")]
  public class CslaStateController(ApplicationContext applicationContext, ISessionManager sessionManager) :
      Csla.AspNetCore.Blazor.State.StateController(applicationContext, sessionManager)
  {  }
}
```

If you change the name of this controller, you must also update the `AddBlazorWebAssembly` configuration to use the new name by setting the `StateControllerName` property.

The state controller is called by the client-side state management subsystem to get and save the current user session data. This is necessary because the client-side state management subsystem is running in the browser, and so it can't directly access the server-side session data. The state controller is a server-side endpoint that the client-side state management subsystem can call to get and put the session data.

This controller can optionally also flow a copy of the server-side user identity (`ClaimsPrincipal`) to the WebAssembly client. If you want to use any authorization rules or UI behaviors in the WebAssembly client pages or objects, you must ensure that the user identity is available on the client.

In any page, server-static, server-interactive, or WebAssembly-interactive, you can access the current user identity using the `Csla.ApplicationContext.User` property. This property is automatically set by the state management subsystem, and is available to you in your code. The same is true for the `LocalContext` and `ClientContext` objects.

_Before a page can access this state_ it must initialize the state management subsystem for the page. This is done by adding code to the page's `OnInitializedAsync` method. For example:

```c#
    protected override async Task OnInitializedAsync()
    {
        // Every page _must_ initialize the state manager
        await StateManager.InitializeAsync();
    }
```

This initialization is required for every page that uses CSLA objects, because the state management subsystem is responsible for ensuring that the `LocalContext`, `ClientContext`, and user identity are available to the page.

If you _change_ any state in `LocalContext` or `ClientContext` it is _your responsibility_ to call `StateManager.SaveState()` to ensure that the changes are saved to the server. This is because the state management subsystem is designed to be as efficient as possible, and so it only saves state when you tell it to do so.

### ASP.NET Core

Starting with CSLA version 8.1.0 you no longer need to configure Kestrel or IIS to allow synchronous IO. This is because CSLA now supports async serialization in aspnetcore. This means that you can remove the `UseSynchronousIO` call from your `Program.cs` file.

```csharp
// Required by CSLA data portal controller. If using Kestrel:
builder.Services.Configure<KestrelServerOptions>(options =>
{
  options.AllowSynchronousIO = true;
});

// Required by CSLA data portal controller. If using IIS:
builder.Services.Configure<IISServerOptions>(options =>
{
  options.AllowSynchronousIO = true;
});
```

### MAUI Android

It turns out that the `HttpClient` behavior on Android is unable to transfer binary data over http via a `POST` operation. This is how the data portal works. Because of this, you must configure the Android client app to use text encoding for the data portal. This is done in the `MauiProgram.cs` file.

```csharp
  builder.Services.AddCsla(o => o
      .DataPortal(o => o
          .ClientSideDataPortal(o => o
              .UseHttpProxy(o =>
              {
                  o.UseTextSerialization = true;
                  o.DataPortalUrl = $"https://{ServerName}/dataportal";
              }
      ))));
```

This also requires that your data portal endpoint accept text data. This is done by configuring the data portal controller to use text serialization. For example:

```csharp
  public DataPortalController(ApplicationContext applicationContext)
    : base(applicationContext)
  { 
      UseTextSerialization = true;
  }
```

If your data portal is being used by multiple clients, such as Android and also Blazor or iOS, etc. then you will need two different data portal controllers, one for text serialization and one for binary serialization.

## New Cache API

CSLA 8 adds a new caching capability to the client-side data portal. This is a new feature that allows you to cache objects on the client, so that they can be reused without having to go back to the server. This is particularly useful in any smart-client app, where network latency can be high.

There is a new interface, `IDataPortalCache`, that you can implement in your business objects. This interface has one method: `GetDataPortalResultAsync`. Here is a simple implementation of this interface that uses a standard `IMemoryCache`:

```csharp
using Csla;
using Csla.Server;
using Csla.DataPortalClient;
using ProjectTracker.Library;
using Microsoft.Extensions.Caching.Memory;
using System.Text;

namespace ProjectTracker.Blazor.Client
{
  public class DataPortalCache(IMemoryCache cache) : IDataPortalCache
  {
    private readonly IMemoryCache _cache = cache;

    public async Task<DataPortalResult> GetDataPortalResultAsync(Type objectType, object criteria, DataPortalOperations operation, Func<Task<DataPortalResult>> portal)
    {
      if (operation == DataPortalOperations.Fetch && objectType == typeof(RoleList))
      {
        // this operation + type is cached
        return await GetResultAsync(objectType, criteria, operation, portal);
      }
      else
      {
        // the result isn't cached
        return await portal();
      }
    }

    private async Task<DataPortalResult> GetResultAsync(Type objectType, object criteria, DataPortalOperations operation, Func<Task<DataPortalResult>> portal)
    {
      DataPortalResult? result;
      var key = GetKey(objectType, criteria, operation);
      result = await _cache.GetOrCreateAsync(key, async (v) =>
      {
        var obj = await portal();
        v.AbsoluteExpiration = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(5);
        return obj;
      });
      if (result != null)
        return result;
      else
        return await portal();
    }

    private static string GetKey(Type objectType, object criteria, DataPortalOperations operation)
    {
      var builder = new StringBuilder();
      // requested type
      builder.Append(objectType.FullName);
      builder.Append('|');

      // criteria values (each criteria has 'valid' ToString)
      var criteriaList = Csla.Server.DataPortal.GetCriteriaArray(criteria);
      foreach (var item in criteriaList)
      {
        builder.Append(item.ToString());
        builder.Append('|');
      }

      // operation
      builder.Append(operation.ToString());
      return builder.ToString();
    }
  }
}
```

In the `GetDataPortalResultAsync` method, you can decide whether to cache the result or not. In this example, the `RoleList` object is cached for the fetch operation. The cache is stored in the `IMemoryCache` object, and is set to expire after 5 seconds.

Configure the client-side data portal to use this cache in the `Program.cs` file:

```csharp
builder.Services.AddCsla(o => o
  .AddBlazorWebAssembly(o => o.SyncContextWithServer = true)
  .DataPortal(o => o.ClientSideDataPortal(o =>
  { 
    o.DataPortalCacheType = typeof(DataPortalCache);
    o.UseHttpProxy(o => o.DataPortalUrl = "/api/dataportal";
  }
  )));
```

The `DataPortalCacheType` property is used to specify the type of cache to use. This type must implement the `IDataPortalCache` interface.

## Rules Engine Changes

The rules engine has changed how it cascades rules based on input properties.

https://github.com/MarimerLLC/csla/issues/3616

If your existing codebase relies on cascading rules based on input properties you may need to update your rules to ensure they are cascading correctly.
