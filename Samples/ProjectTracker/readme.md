# ProjectTracker Reference App

[ProjectTracker](https://github.com/MarimerLLC/csla/tree/main/Samples/ProjectTracker) is the primary reference app for CSLA .NET. 

The `/Samples` directory contains numerous other samples, many of which are much simpler and more targeted at demonstrating specific deployment models or features of CSLA .NET.

## How to run the Blazor UI

In order to run/debug the sample you need to launch not just the Blazor client, but also the app server project.

Right-click on the solution, select 'Set Startup Projects'
When dialog appears select the radio button  'Multiple startup Projects'.

Set Action for the project 'ProjectTracker.AppServerCore' to 'Start'.
Also set Action for the project 'ProjectTracker.Ui.Blazor' to 'Start'.
