# Contribution Instructions

## Creating issues

There are a few things to keep in mind when creating issues here:

1. There are almost no bad issues, they can be created for typos, glitches, ambiguity, questions, and more. 
2. Please link to the relevant page when creating the issue

## Contributing to the Documentation

Changes to this repo are conducted the same way we would make changes to a repo containing code. That means:

1. Clone the repo to your local machine
2. Create a new branch if the changes need to be reviewed
3. Make changes using the text editor of your choice
4. Commit with a descriptive message
5. Push to the remote branch

If you wish to see a preview of your markdown you can use the [GitBook Editor](https://www.gitbook.com/editor). This editor has good integration with git and is easy to use. Although, to use the GitBook Editor you have to go to `File` -> `Preferences..` -> `Git` and uncheck the box `Automatically generate commit message from changes` in order to write your own commit messages. It's worth knowing that GitBook does not apply CSS styles, for that you have to follow the instructions below.

### Checking changes locally

You're encouraged to check for yourself how the changes look before pushing it to the remote branch. This can be accomplished using the GitBook CLI:

1. run `npm install -g gitbook-cli`
2. the first time, run `gitbook install` to install the plugins
3. run `gitbook serve` and go to `localhost:4000` to see how the changes will look when they're live

### Choosing the right branch

The different branches in this repo correspond to the different versions of Starcounter. Thus, it's important to push to the right branches after having made changes.

The branches are the following:

1. Every Release: 2.1.177, 2.2.1834, and 2.2.1.3234
2. Develop
3. RC

If you make a change that only pertains to the current `Develop` version, then that change should only be pushed to the `Develop` branch. If the change is relevant to 2.2.1.3234, then push to that branch and then cherry-pick the commit to `RC` and `Develop`. This is according to the release channels described [here](https://github.com/Starcounter/RebelsLounge/issues/60).

## Adding a new page

1. Read the instructions above
2. Create a folder with the same name as the page in the right folder. For example, if you want to add a page pertaining to SQL in Starcounter, then you should add a folder containing a `README.md` file to in guides/SQL folder. All the content in the new page should be in the `README` file
3. Add your page to `SUMMARY.md` by making it a bullet point under the appropriate section by writing `[Your Page Headline](your-page-headline/README.md)`
