# How to contribute

CSLA .NET is developed and maintained by a [global development team](http://www.lhotka.net/Article.aspx?area=4&id=bbe426f7-cd06-482f-bfa7-ec5640296562) composed of volunteers. We welcome your help!

## Getting started

* Make sure you have a [GitHub account](https://github.com/signup/free)
* Review and follow the [Coding standards](https://github.com/MarimerLLC/csla/wiki/Coding-standards) we use to maintain consistent code in the framework
* Submit a ticket for your issue, assuming one does not already exist
* Fork the repository on GitHub

## Making Changes

* Create a topic branch from where you want to base your work.
  * This is usually the master branch.
  * Only target release branches if you are certain your fix must be on that
    branch. Releases are typically in maintenance mode and accept only
    critical bug fixes. Check with the project owners before working on
    anything other than critical bug fixes.
  * Create a branch in which to do your work.  Please avoid working directly on the
    `master` branch.
* Make commits of logical units.
* Make sure your commit messages are in the proper format.

````
    #99999 Make the example in CONTRIBUTING imperative and concrete
````

* Make sure you have added the necessary tests for your changes.
* Run _all_ the tests to assure nothing else was accidentally broken. This is
  particularly important because CSLA .NET is cross-platform (WinRT, .NET, Silverlight,
  mono, mono for Android, etc.) and you _must_ ensure your code compiles and runs on all
  these platforms!

## Submitting Changes

* Print, sign, and email the [contributor agreement](https://github.com/MarimerLLC/csla/blob/master/Support/Contributions/CSLA%20Contributor%20Agreement.pdf?raw=true) document to Marimer LLC
* Push your changes to a topic branch in your fork of the repository.
* Submit a pull request to the repository in the Marimer LLC organization.
* Update your GitHub issue to mark that you have submitted code and are ready for it to be reviewed.
  * Include a link to the pull request in the ticket

# Additional Resources

* [General GitHub documentation](http://help.github.com/)
* [GitHub pull request documentation](http://help.github.com/send-pull-requests/)
* [CSLA .NET forum](http://forums.lhotka.net/forums/5.aspx/)
