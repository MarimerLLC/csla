# Contribution guidelines and standards

The following are instructions for setting up an environment for CSLA .NET development.

## Getting started

* Review the [contributor guidelines](https://github.com/MarimerLLC/csla/blob/master/CONTRIBUTING.md)
* You must have a GitHub account to fork the repository and create pull requests

## Dev environment setup

You will need to set up your development workstation with the following
* Visual Studio 2015
  * Important: You need to set your global tab size to 2, and choose the option to replace tabs with spaces (no tabs allowed!)
  * Make sure Visual Studio is running the latest updates from Microsoft, CSLA .NET is almost always at or ahead of any current release of Visual Studio tooling
  * WP8 SDK and tools
  * Windows 10 UWP SDK and tools
  * Xamarin (to work with the iOS and Android projects)
* Git client tooling of your choice
  * Git for Windows
  * TortoiseGit
  * Git tool from GitHub

## Getting the project

Once you have that all installed, and you have your GitHub credentials, you’ll need to do the following:

* Fork the Marimer LLC csla project
* Clone your fork to your dev workstation
* Create a branch in which to do your work

## Coding standards

As far as coding standards – follow the code style you see in CSLA .NET. Some of the basics are covered by the `editorconfig` file in the repo. Here are some other basic guidelines/rules:

* Casing and stuff
  * Use _fieldName for all instance fields
  * Use ClassName
  * Use PropertyName and MethodName
  * Use parameterName for parameters
* When you do a Commit make sure to follow the proper format for the commit description (see below)
* When you create a Pull Request it will trigger a continuous integration build via Appveyor. If that build fails, correct any issues and Push changes to your branch - that will automatically trigger a new CI build
 * ⚠ Make sure to include the issue number in your PR description so GitHub links the PR and issue
 * If your PR closes one or more issues, use the "Closes #123" or "Fixes #123" phrase in your PR description, as when the PR is accepted this will auto-close your issue(s)

````
#999 Detailed description of your change here
````
