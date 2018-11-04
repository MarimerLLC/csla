# How to contribute

CSLA .NET is developed and maintained by a [global development team](https://github.com/MarimerLLC/csla/graphs/contributors) composed of volunteers. We welcome your help!

## Getting started

* Make sure you have a [GitHub account](https://github.com/signup/free)
* For beginners we suggest
  * This online course on [contributing to OSS projects on GitHub](https://egghead.io/series/how-to-contribute-to-an-open-source-project-on-github)
  * This excellent blog post on [Being a good open source citizen](https://hackernoon.com/being-a-good-open-source-citizen-9060d0ab9732#.4owk5884d)
  * [A simple flow for using git and pull requests](http://www.lhotka.net/weblog/ASimpleFlowForUsingGitAndPullRequests.aspx)
* Download, print, sign, scan, and return the [contributor agreement document](https://github.com/MarimerLLC/csla/blob/master/Support/Contributions/CSLA%20Contributor%20Agreement.pdf).
* Review the [Code of Conduct](https://github.com/MarimerLLC/csla/blob/master/code_of_conduct.md)
* Review and follow the [Coding standards](https://github.com/MarimerLLC/csla/blob/master/docs/Coding-standards.md) we use to maintain consistent code in the framework
* Submit a ticket for your issue, assuming one does not already exist
* Fork the repository on GitHub

## Making Changes

* Create a topic/feature/issue branch from where you want to base your work.
  * This is usually the MarimerLLC/master branch.
  * Only target release branches if you are certain your fix must be on that
    branch. Releases are typically in maintenance mode and accept only
    critical bug fixes. Check with the project owners before working on
    anything other than critical bug fixes.
  * Create a feature branch in which to do your work. Avoid working directly on the
    `master` branch to make your own life easier.
* Make commits of logical units.
* Make sure your commit messages are in the proper format.

````
    #999 Make the example in CONTRIBUTING imperative and concrete
````

or

````
   Fixes #999 Describe the change made in a concise manner
   Closes #999 Describe the change made in a concise manner
````

* Make sure you have added the necessary tests for your changes.
* Run _all_ the tests to assure nothing else was accidentally broken. This is
  particularly important because CSLA .NET is cross-platform (.NET Core, mono, 
  .NET Framework, Xamarin, etc.) and you _must_ ensure your code compiles and 
  runs on all these platforms!

## Submitting Changes

* Print, sign, and email the [contributor agreement](https://github.com/MarimerLLC/csla/blob/master/Support/Contributions/CSLA%20Contributor%20Agreement.pdf?raw=true) document to Marimer LLC (rocky at lhotka dot net)
 * We will not accept large changes without a signed contributor agreement, but we may accept small edits to the existing codebase
 * By submitting a change, large or small, you grant ownership of the code and related IP to Marimer LLC, and you certify that you have the right to transfer such ownership
* Push your changes to a topic branch in your fork of the repository.
* Submit a pull request (PR) to the repository in the Marimer LLC organization.
  * Make sure the issue number (e.g. #999) is in the _description_ of the PR so GitHub can automatically link the PR to the issue.

# Additional Resources

* [General GitHub documentation](http://help.github.com/)
* [GitHub pull request documentation](http://help.github.com/send-pull-requests/)
