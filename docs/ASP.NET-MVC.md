# ASP.NET MVC

## ASP.NET Core

CSLA 5 and 6 provide support for ASP.NET Core - MVC and Razor Pages - with various helper types in the `Csla.AspNetCore` namespace.

> ℹ️ If you host code in ASP.NET Core you _must_ add the `Csla.AspNetCore` NuGet package to your project. This is true if you are using MVC, Razor Pages, server-side Blazor, or creating an app server using aspnetcore.

## ASP.NET in .NET Framework

_Please remember that ASP.NET MVC does run in ASP.NET. So nearly all the FAQ items under the [Web Forms](Web-forms.md) topic apply to ASP.NET MVC as well._

## Does CSLA .NET work with ASP.NET MVC?

Yes, in your web project add the NuGet package specific to the version of MVC that you are targeting. That will automatically bring in the CSLA assemblies bound to that version of MVC.

CSLA .NET works great with any of the "M" patterns (MVVM, MVC, MVP, etc), since it is all about helping you create a great Model.

In short, your CSLA .NET business objects are the 'model', and you should use Controller code to link your model to the View. Similarly, for any postbacks that trigger an action, the Controller code should simply invoke appropriate methods on the Model so the business objects do the work.

The result is that CSLA .NET allows you to create a powerful Model, minimizing code in the Controller and allowing the Controller to focus purely on user interaction and orchestration, leaving all business behaviors to the Model where they belong.

CSLA .NET 3.8 introduced a CslaModelBinder class to smooth the one outstanding issue with using ASP.NET MVC with the cool Controller "data binding" capabilities. This is optional, but addresses some issues that come up if you use MVC 2 and DataAnnotations attributes and Controller data binding together.
