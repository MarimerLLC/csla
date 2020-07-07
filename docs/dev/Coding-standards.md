# Contribution guidelines and standards

The following are instructions for setting up an environment for CSLA .NET development.

## Getting started

* Review the [contributor guidelines](https://github.com/MarimerLLC/csla/blob/master/CONTRIBUTING.md)
* You must have a GitHub account to fork the repository and create pull requests

## Dev environment setup

You will need to set up your development workstation with the following
* Visual Studio 2019
  * Make sure Visual Studio is running the latest updates from Microsoft, CSLA .NET is almost always at or ahead of any current release of Visual Studio tooling
  * Workloads
    * Windows client development (for Windows Forms, WPF, UWP)
    * Mobile development (for Xamarin)
    * Web development (for ASP.NET)
    * .NET Core and ASP.NET Core
* Git client tooling of your choice
  * Git for Windows
  * TortoiseGit
  * GitHub for Windows
  * etc.

## Getting the project

Once you have that all installed, and you have your GitHub credentials, you’ll need to do the following:

* Fork the Marimer LLC csla project
* Clone your fork to your dev workstation
* Create a feature branch in which to do your work

## Coding standards

As far as coding standards – follow the code style you see in CSLA .NET. Some of the basics are covered by the `editorconfig` file in the repo. Here are some other basic guidelines/rules:

* Casing and naming
  * Use _fieldName for all instance fields
  * Use ClassName
  * Use PropertyName and MethodName
  * Use parameterName for parameters
* When you do a Commit make sure to follow the proper format for the commit description (see below)
* When you create a Pull Request it will trigger a continuous integration build via Appveyor. If that build fails, correct any issues and Push changes to your branch - that will automatically trigger a new CI build
 * ⚠ Make sure to include the issue number in your PR description (not just the title) so GitHub links the PR and issue
 * If your PR closes one or more issues, use the "Closes #123" or "Fixes #123" phrase in your PR description, as when the PR is accepted this will auto-close your issue(s)

````
#999 Detailed description of your change here
````
