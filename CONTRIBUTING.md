# How to contribute

CSLA .NET is developed and maintained by a [global development team](https://github.com/MarimerLLC/csla/graphs/contributors) composed of volunteers. We welcome your help!

Read this entire document before starting any work.

## Getting started

1. Review the [Code of Conduct](https://github.com/MarimerLLC/csla/blob/master/CODE_OF_CONDUCT.md)
1. Make sure you have a [GitHub account](https://github.com/signup/free)
1. Download, print, sign, scan, and return the contributor agreement (CLA)
1. Engage
   1. Bring up your idea in a [Discussion](https://github.com/MarimerLLC/csla/discussions) and talk it through with the community
   1. Join the [Discord server](https://discord.gg/9ahKjb7ccf) and talk it through there
   1. Submit an [Issue](https://github.com/MarimerLLC/csla/issues) for your suggestion; check first to see if one already exists
1. For beginners we suggest
   1. This online course on [contributing to OSS projects on GitHub](https://egghead.io/series/how-to-contribute-to-an-open-source-project-on-github)
   1. This excellent blog post on [Being a good open source citizen](https://hackernoon.com/being-a-good-open-source-citizen-9060d0ab9732#.4owk5884d)
   1. Information on the [CSLA git flow](https://github.com/MarimerLLC/csla/blob/master/docs/dev/csla-github-flow.md) used on this project

## Contributor Agreement (CLA)

* Print, sign, and email the [contributor agreement](https://github.com/MarimerLLC/csla/blob/master/Support/Contributions/CSLA%20Contributor%20Agreement.pdf?raw=true) document to Marimer LLC
   * Email to rocky at marimer dot llc
   * Include your GitHub username in the email
   * You will recieve an email from GitHub inviting you to the organization and repo once we've reviewed your CLA
* We will not accept large changes without a signed CLA, but we may accept small edits to the existing codebase
   * By submitting a change, large or small, you grant ownership of the code and related IP to Marimer LLC, and you certify that you have the right to transfer such ownership

## Making Changes

We have rules around:

1. Using Git
1. Using GitHub
1. Doing the work
1. Git commits
1. Submitting Pull Requests
1. Reviewing PRs

The rules are detailed below.

### Using Git

* We have a doc with step-by-step instructions on [how to use git and GitHub to contribute](https://github.com/MarimerLLC/csla/blob/master/docs/dev/csla-github-flow.md) to CSLA .NET
* Follow the [CSLA git flow](https://github.com/MarimerLLC/csla/blob/master/docs/dev/csla-github-flow.md) used on this project

### Using GitHub

* Make sure you are working against an [Issue in the backlog](https://github.com/marimerllc/csla/issues)
   * If an issue doesn't exist, engage with the dev team via [Discussions](https://github.com/MarimerLLC/csla/discussions) or [Discord](https://discord.gg/9ahKjb7ccf) to discuss your concern
* Create a fork of the MarimerLLC/csla repo
* In your fork, create a topic/feature/issue branch from where you want to base your work
   * We recommend naming your branch using the format `dev/issueNumber-test` such as `dev/1234-enhance-dataportal`
* Assign the issue
  * If you have permissions, assign the Issue to yourself 
  * If you don't have permissions, **make a comment** in the issue saying that you are working on the issue
* Schedule the issue
  * Issues in progress must be part of a [Project](https://github.com/MarimerLLC/csla/projects)
  * If you have permissions, assign the Issue to a project
  * If you don't have permissions, engage with the dev team via [Discussions](https://github.com/MarimerLLC/csla/discussions) or [Discord](https://discord.gg/9ahKjb7ccf) to discuss

### Doing the Work

* Follow the [CSLA coding standards](https://github.com/MarimerLLC/csla/blob/main/docs/dev/Coding-standards.md)
* Make sure you have added the necessary tests for your changes

### Commit to Git

* Git commit text should include the backlog issue number; for example `#1234 Fixed an issue`

### Submitting a Pull Request

* Pull requests are submitted against a backlog issue; make sure you have an issue number before proceeding
* Submit a pull request (PR) to the repository in the Marimer LLC organization.
   * Normally to the `main` branch
   * If you are changing an older version, submit to the `vX.0` branch (confirm with Rocky that we're still doing releases on older versions _before doing your work_)
* In the PR _description_ use `Closes #1234` or `Fixes #1234` to link the PR to the issue
* Submitting a PR will trigger a CI build; make sure the CI build passes with your changes and new unit tests

### Reviewing a Pull Request

* All PRs require at least one approving review before they can be merged
* You may add your review to a PR, or you may be requested to review someone's PR
* PR reviews should be positive and constructive, but also thorough; don't approve something if you think it is wrong in terms of code, style, testing, etc.

## Additional Resources

* [General GitHub documentation](http://help.github.com/)
* [GitHub pull request documentation](http://help.github.com/send-pull-requests/)
