# ProjectTracker Reference App

[ProjectTracker](https://github.com/MarimerLLC/csla/tree/master/Samples/ProjectTracker) is the primary reference app for CSLA .NET. 

It is used as part of the *Using CSLA* book series from https://store.lhotka.net.

The `/Samples` directory contains numerous other samples, many of which are much simpler and more targeted at demonstrating specific deployment models or features of CSLA .NET.

## How to run the Blazor Client UI

In order to run/debug the sample you need to launch not just the Blazor client, but also the app server project.

Right-click on the solution, select 'Set Startup Projects'
When dialog appears select the radio button  'Multiple startup Projects'.

Set Action for the project 'ProjectTracker.AppServerCore' to 'Start'.
Also set Action for the project 'ProjectTracker.Ui.Blazor.Client' to 'Start'.

## How to run the Blazor Server UI

In order to run/debug the sample you need to launch not just the Blazor server, but also the app server project.

Right-click on the solution, select 'Set Startup Projects'
When dialog appears select the radio button  'Multiple startup Projects'.

Set Action for the project 'ProjectTracker.AppServerCore' to 'Start'.
Also set Action for the project 'ProjectTracker.Ui.Blazor.Server' to 'Start'.

## How to run the WPF UI

In order to run/debug the sample you need to launch not just the WPF client, but also the app server project.

Right-click on the solution, select 'Set Startup Projects'
When dialog appears select the radio button  'Multiple startup Projects'.

Set Action for the project 'ProjectTracker.AppServerCore' to 'Start'.
Also set Action for the project 'ProjectTracker.Ui.WPF' to 'Start'.

## How to run the Windows Forms UI

In order to run/debug the sample you need to launch not just the Windows Forms client, but also the app server project.

Right-click on the solution, select 'Set Startup Projects'
When dialog appears select the radio button  'Multiple startup Projects'.

Set Action for the project 'ProjectTracker.AppServerCore' to 'Start'.
Also set Action for the project 'ProjectTracker.Ui.WinForms' to 'Start'.

## How to run the UWP UI

In order to run/debug the sample you need to launch not just the UWP client, but also the app server project.

Right-click on the solution, select 'Set Startup Projects'
When dialog appears select the radio button  'Multiple startup Projects'.

Set Action for the project 'ProjectTracker.AppServerCore' to 'Start'.
Also set Action for the project 'ProjectTracker.Ui.UWP' to 'Start'.

## How to run the Xamarin Android UI

The Android UI app will run in an Android emulator on your PC. The emulator does not have access to localhost, and so can not use a locally-hosted appserver.

You will need to deploy the appserver to Azure or some other cloud or server location that has a publicly available IP address that can be reached by the Android emulator.

Then you must edit the `App.xaml.cs` file in the `ProjectTracker.Ui.Xamarin` project to update the data portal server URL to match the location where you deployed the app server.

The lines in `App.xaml.cs` will look like this:

```c#
  .DataPortal()
    .DefaultProxy(typeof(Csla.DataPortalClient.HttpProxy), 
                  "https://ptrackerserver.azurewebsites.net/api/dataportal");
```

You can now set the Android UI project as the startup project and press F5.

## How to run the Xamarin iOS UI

The iOS UI app will run in an iOS simulator on your Mac. The emulator does not have access to localhost, and so can not use a locally-hosted appserver.

You will need to deploy the appserver to Azure or some other cloud or server location that has a publicly available IP address that can be reached by the iOS simulator.

Then you must edit the `App.xaml.cs` file in the `ProjectTracker.Ui.Xamarin` project to update the data portal server URL to match the location where you deployed the app server.

The lines in `App.xaml.cs` will look like this:

```c#
  .DataPortal()
    .DefaultProxy(typeof(Csla.DataPortalClient.HttpProxy), 
                  "https://ptrackerserver.azurewebsites.net/api/dataportal");
```

You can now set the iOS UI project as the startup project and press F5.
