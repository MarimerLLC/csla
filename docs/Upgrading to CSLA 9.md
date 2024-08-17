# Upgrading to CSLA 9

CSLA 9 is a major release and so there a number of breaking changes.

In this document I'll try to highlight the most common changes required when upgrading your codebase from CSLA 8 to CSLA 9.

If you are upgrading from a version of CSLA prior to 8, you should review the [Upgrading to CSLA 8](https://github.com/MarimerLLC/csla/blob/main/docs/Upgrading%20to%20CSLA%208.md) document, as most of its contents are relevant. This document only covers the changes from CSLA 8 to CSLA 9.

## Platform Support

CSLA 9 removes support for .NET 6, UWP, and Xamarin. If you are using these platforms you will need to stay on an older version of CSLA until you can upgrade to a newer platform.

The following are supported:

* .NET 8 and .NET 9
* Microsoft .NET Framework 4.6.2, 4.7.2, 4.8
* Blazor
* ASP.NET Core
* MAUI (Android, iOS, Mac Catalyst, Windows)
* WPF, Windows Forms
* ASP.NET MVC 5, WebForms

It is likely that CSLA will work with:

* Platform.Uno
* Avalonia

## Application Configuration

There are some changes to CSLA configuration in CSLA 9, and these changes will affect most applications.

### Blazor App (modern solution template)

The modern solution template generates two projects, a server-side project and a client-side project.

By default, the modern solution template enables the use of all Blazor render modes, including server-static, server-interactive, WebAssembly-interactive, and Auto. Because of this, the default CSLA configuration is to use the state management subsystem introduced in CSLA 8.

#### Server-side project startup

A typical `Program.cs` file for the Blazor server project might look like this:

```csharp
builder.Services.AddCsla(o => o
  .AddAspNetCore()
  .AddServerSideBlazor(o => o.UseInMemoryApplicationContextManager = false)
  .Security(so => so.FlowSecurityPrincipalFromClient = true)
```

#### Client-side project startup

A typical `Startup.cs` file for the Blazor client project might look like this:

```csharp
builder.Services.AddCsla(o => o
  .AddBlazorWebAssembly(o => o.SyncContextWithServer = true)
  .Security(o => o.FlowSecurityPrincipalFromClient = true)
  .DataPortal(o => o
    .UseClientSideDataPortal(o => o
      .UseHttpProxy(o => o.DataPortalUrl = "/api/DataPortal"))));
```

### Blazor App (legacy style server)

A typical `Program.cs` file for a Blazor server app might look like this:

```csharp
builder.Services.AddCsla(o => o
  .AddAspNetCore()
  .AddServerSideBlazor(o => o
    .UseInMemoryApplicationContextManager = true));
```

### Blazor App (legacy style WebAssembly client)

A typical `Program.cs` file for a Blazor WebAssembly only app might look like this:

```csharp
builder.Services.AddCsla(o => o
  .AddClientSideBlazor());
```

### MAUI Android

CSLA 9 fixes the issue with transfering binary data between the client and server in a MAUI Android app. This means that the default configuration for CSLA now works in a MAUI Android app.

```csharp
  services.AddCsla(o => o
    .DataPortal(o => o
      .UseClientSideDataPortal(o => o
        .UseHttpProxy(o => o.DataPortalUrl = "https://server/api/DataPortal"))));
```

## BinaryFormatter and NetDataContractSerializer Removed

CSLA 9 removes support for the `BinaryFormatter` serialization format. This format is not secure and should not be used.

The `NetDataContractSerializer` (NDCS) is also removed, as WCF is no longer part of modern .NET.

## MobileFormatter Custom Serializers

`MobileFormatter` now supports the concept of custom serializers for types that are not normally serializable.

Two custom serializers are included in CSLA 9: one for `ClaimsPrincipal` and one that uses JSON to serialize any POCO type.

The `ClaimsPrincipalSerializer` is configured and active by default, so `ClaimsPrincipal` types can be serialized and deserialized by the `MobileFormatter`.

### Using the POCO Serializer

The POCO serializer is a generic type that can be used to serialize any simple C# class that has public read/write properties.

Configure it in your program's startup code like this:

```csharp
builder.Services.AddCsla(o => o
  .Serialization(s => s
    .UseMobileFormatter(m => m
      .CustomSerializers.Add(
        new TypeMap<MyType, PocoSerializer<MyType>>(PocoSerializer<MyType>.CanSerialize))
    ))));
```

The `TypeMap` class is used to map a specific type to a specific serializer. The `PocoSerializer` class is a generic type that can serialize any simple C# class that has public read/write properties.

The `PocoSerializer` class has a static `CanSerialize` method that returns true if the type can be serialized by the serializer.

You can add multiple custom serializers to the `MobileFormatter` configuration, each with its own `ITypeMap` instance to map the original type to the serializer type, and to provide a `Func` method reference to determine if the serializer can serialize the type.

By convention, we recommend implementing the `CanSerialize` method as a static method on the serializer class, but you can use any method you like.

### Creating Your Own Serializer

To create your own serializer, create a class that implements the `IMobileSerializer` interface. This interface has two methods: `Serialize` and `Deserialize`.

The `Serialize` method is provided with a reference to the original object instance that otherwise couldn't be serialized, and a `SerializationInfo` instnace that can be used to store the serialized data.

The `Deserialize` method is provided with a `SerializationInfo` instance that contains the serialized data, and should return a new instance of the original object type.

_How_ you implement these methods is up to you, but you should be aware that the `SerializationInfo` class is a key/value store, so you can store any "primitive" data in it that can be normally serialized using `MobileFormatter`.

This includes:

* All .NET primitive types
* `string`
* `DateTime`, `TimeSpan`, `DateTimeOffset`, `DateOnly`, `TimeOnly`
* `Guid`
* `byte[]`
* `char[]`
* `List<int>`

If you look at the implementation of [PocoSerializer](https://github.com/MarimerLLC/csla/blob/main/Source/Csla/Serialization/Mobile/CustomSerializers/PocoSerializer.cs) you'll see that it relies on `System.Text.Json` to serialize and deserialize the object, and stores the JSON string in the `SerializationInfo` instance.

The [ClaimsPrincipalSerializer](https://github.com/MarimerLLC/csla/blob/main/Source/Csla/Serialization/Mobile/CustomSerializers/ClaimsPrincipalSerializer.cs) type has two implementations.

The one for modern .NET relies on the ability of `ClaimsPrincipal` to serialize and deserialize itself using a byte array. This array is stored in the `SerializationInfo` instance.

The one for the .NET Framework copies the `ClaimsPrincipal` values into a DTO object graph and then uses `JsonSerializer` to serialize the DTO to a JSON string. This string is stored in the `SerializationInfo` instance.

By convention, we implement a static `CanSerialize` method on the serializer class that returns true if the serializer can serialize the type. For example:

```csharp
public static bool CanSerialize(Type type) => type == typeof(ClaimsPrincipal);
```

## RabbitMq Data Portal Channel

The RabbitMQ data portal channel has been updated to work with dependency injection. You can explore the `RabbitMqExample` project in the `Samples` folder for an example of how to use the RabbitMQ data portal channel.

## Nullable Reference Types

CSLA 9 supports the use of nullable reference types in your code. This means that you can use the `#nullable enable` directive in your code and CSLA will work correctly.

### API Changes

Supporting nullable types means that some APIs have changed to support nullable types.

* The `User` and `Principal` properties of `ApplicationContext` no longer return null

#### Using Timeout in `HttpProxy` and `HttpCompressionProxy`

##### Overview

The `HttpProxy` and `HttpCompressionProxy` classes now support configuring timeouts through the `HttpProxyOptions` class. This allows for more flexible and centralized configuration of HTTP request timeouts.

##### Configuration

To configure the timeout for `HttpProxy` and `HttpCompressionProxy`, you need to set the `Timeout` and `ReadWriteTimeout` properties in the `HttpProxyOptions` class.

##### Example

Below is an example of how to configure and use the timeout settings in `HttpProxy` and `HttpCompressionProxy`.

###### Configure `HttpProxyOptions`

```csharp
services.AddCsla(
    o => o.DataPortal(
        o2 => o2.AddClientSideDataPortal(
            cso => cso.UseHttpProxy(
                hpo => hpo.WithTimeout(TimeSpan.FromSeconds(30)).WithReadWriteTimeout(TimeSpan.FromSeconds(30))
            )
        )
    )
);

```


##### Summary

By configuring the `HttpProxyOptions` class, you can easily set the timeout values for `HttpProxy` and `HttpCompressionProxy`. This approach centralizes the configuration and makes it easier to manage timeout settings across your application.
