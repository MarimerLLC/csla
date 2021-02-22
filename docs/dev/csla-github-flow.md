Git Flow for CSLA Development
-----------------------------
In response to a lot of questions about how to work with git and GitHub when contributing to CSLA I wrote a blog post [Simple Flow for Using Git and Pull Requests](http://lhotka.net/weblog/ASimpleFlowForUsingGitAndPullRequests.aspx). That post is fairly long and includes explanations about why things work as they do.

Here's a tl;dr summary if you just want to get working without all the background. 

> I'm assuming the use of Git Bash or WSL command line. You'll need to translate if you are using cmd/ps1 or a GUI tool.

## Getting started
1. Create a fork of MarimerLLC/csla using the GitHub web UI
   1. Go to `https://github.com/MarimerLLC/csla`
   1. Click the fork button in the upper-right
1. Clone your fork to your dev workstation
   1. cd to directory where you want git to create a new `csla` sub-directory
   1. `git clone https://github.com/yourname/csla.git` 
   1. Your GitHub fork is now known as `origin` within your local clone
1. Work in the `csla` directory
   1. `cd csla`
1. Add an upstream remote so your local clone has access to the "real" main (not just your fork)
   1. `git remote add marimer https://github.com/MarimerLLC/csla.git`
   1. The upstream GitHub repo is now known as `marimer` within your local clone
   
## Create a branch to do some work
1. Make sure your local clone is updated with MarimerLLC/csla
   1. `git fetch marimer`
1. Create a feature branch (work area) based on the upstream main
   1. `git checkout -b 123-feature-branch marimer/main`
   1. Your local workspace is now in a feature branch based on the latest code in MarimerLLC/csla main

## Create a branch to do some work _against a maintenance branch_
1. Make sure your local clone is updated with MarimerLLC/csla
   1. `git fetch marimer`
1. Create a feature branch (work area) based on the upstream main
   1. `git checkout -b 123-feature-branch marimer/<maintenance-branch-name>`
   1. Your local workspace is now in a feature branch based on the latest code in the maintenance branch
1. **Important:** when you submit your PR (later in this doc) make sure the target of your PR is _maintenance-branch-name_, not main

## Do your work
1. Edit code, and do other stuff
1. Commit your changes to your local clone
   1. `git add .` - add all new/modified files to the git index
   1. `git commit -m '#123 your comment here'`
1. Rinse and repeat as you work
   1. It is a good idea to commit frequently so you can roll back to a previous state in case of badness
   1. Committing only updates your _local clone_, it has no impact on anything in the cloud until you push (next step)
1. Watch for changes from marimer/main
   1. `git fetch marimer` - will fetch changes from marimer. This does not update your local branch. It only updates your local repository.
   1. `git log ..marimer/main` - will log all commits that have been made to marimer/main that are **not** in your branch. This can tell you if you need to update your branch. If there are no incoming changes, this prints nothing and there is no need to do the next step.
   1. `git pull marimer main` - will update your local branch with changes that have been made to marimer/main. This is how you keep your local branch up to date.
   
## Upload your work to the cloud
1. Push your local clone to your GitHub fork
   1. `git push origin`
   1. Your GitHub fork now has a copy of your feature branch from your local clone
   1. You can push your local clone to GitHub as often as you'd like, this acts as a backup, and allows for collaboration (other people can see your changes via the GitHub web UI)

## Create a Pull Request (PR)
1. Make sure your feature branch is current with MarimerLLC/main
   1. `git pull marimer main`
   1. That may result in a need to merge changes; resolve any merge conflicts
   1. Make sure your updated/merged feature branch still builds
   1. Make sure your unit tests still pass
   1. `git push origin`
1. Create a PR from your GitHub fork to MarimerLLC/csla
   1. Use the GitHub web UI to create a PR from your fork to MarimerLLC/csla
   1. Navigate to your fork and branch in GitHub, then click the button to create a pull request, the defaults should be correct
   1. Double-check to make sure MarimerLLC/csla-main is on the left, and yourname/csla-yourbranch is on the right
   1. Creating the PR will trigger a CI build
      1. We will only accept a PR if it can be automatically merged (green checkmark)
      1. We will only accept a PR if the build/tests all pass (green checkmark from CI build)
   1. Your PR _has no effect_ on main or production, it is a _pending_ change
   1. Subsequent changes to your feature branch in your GitHub fork automatically become part of your PR; each time you push to your GitHub fork's feature branch triggers a CI build of the PR
   
## React to review comments on your PR
1. People can comment on, or review your PR, indicating changes you need to make before it is accepted
   1. This is a _dialog_ so please engage with the reviewer(s) as appropriate
1. Edit code or other stuff in your feature branch on your local clone
1. Commit your changes to your local clone
   1. `git add .`
   1. `git commit -m '#123 your comment'`
1. Push your local clone to your GitHub fork
   1. `git push origin`
   1. Any changes pushed to the feature branch in your GitHub fork are _automatically_ incorporated into your PR (and a CI build is triggered)

## Cleanup after PR acceptance/rejection
1. Remove feature branch in your GitHub fork
   1. Use the GitHub web UI to remove the feature branch
1. Remove feature branch in your local clone
   1. `git checkout main` (switch out of the feature branch)
   1. `git branch -D 123-feature-branch`

## Ask other people to collaborate on your PR
I'm not going to repeat all the commands here - they are the same as the steps above, but using your GitHub repo as the "upstream" instead of using MarimerLLC/csla as the upstream.

1. Have other person create a fork from your fork using GitHub web UI
1. Have other person clone their fork to their workstation
1. Have other person add your fork as an upstream remote to their local clone
1. Have other person update their local clone from your fork
1. Have other person create working branch based on your feature branch
1. Have other person do their code or other changes
1. Have other person commit changes to their local clone
1. Have other person push their local clone to their fork
1. Have other person create a PR from their fork to your fork
1. Accept their PR to merge their changes to your fork's feature branch (which automatically updates your PR and triggers a CI build)
