# Contribution Instructions

## General instructions

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

If you make a change that only pertains to the current `Develop` version, then that change should only be pushed to the `Develop` branch. If the change is relevant to 2.2.1.3234, then push to that branch and then merge to `RC` and `RC` to `Develop`. This is according to the release channels described [here](https://github.com/Starcounter/RebelsLounge/issues/60).

### Deciding if a review is neccesary

Here are some cases where a review is appropriate:

* Creating a completely new page
* Significantly changing the content of an existing page
* Saking refactoring changes spanning over several pages

Some cases when a review is not neccesary:

* Fixing typos
* Refactoring within one page

When choosing who will review the page, please consider who are most familiar with the content. If you're changing a page that was initially written by someone else, then the initial author should be assigned as the reviewer.

[@mackiovello](# Contribution Instructions

## General instructions

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

If you make a change that only pertains to the current `Develop` version, then that change should only be pushed to the `Develop` branch. If the change is relevant to 2.2.1.3234, then push to that branch and then merge to `RC` and `RC` to `Develop`. This is according to the release channels described [here](https://github.com/Starcounter/RebelsLounge/issues/60).

### Deciding if a review is neccesary

Here are some cases where a review is appropriate:

* Creating a completely new page
* Significantly changing the content of an existing page
* Saking refactoring changes spanning over several pages

Some cases when a review is not neccesary:

* Fixing typos
* Refactoring within one page

When choosing who will review the page, please consider who are most familiar with the content. If you're changing a page that was initially written by someone else, then the initial author should be assigned as the reviewer.

[@Mackiovello](https://github.com/Mackiovello) is glad to help out reviewing if there is an already heavy workload on the reviwer or if there's a need for proofreading.

## Editing already existing pages

To edit an already existing page you simply follow the steps the "General Instructions" above. The two things you should keep in mind before making a change is:
1. Should my changes be reviewed?
2. What branches are my changes relevant to?

If the changes are significant enough to justify a review, then create a new branch, make a pull request, and assign someone to review.

## Adding a new page

1. Read the instructions above
2. Add a new page in the correct folder. For example, if you want to add a page pertaining to SQL in Starcounter, then you should add a markdown page to in guides/SQL folder.
3. Add your page to `SUMMARY.md` by making it a bullet point under the appropriate section by writing `[Your Page Headline](you-page-headline.md)`
 is glad to help out reviewing if there is an already heavy workload on the reviwer or if there's a need for proofreading.

## Editing already existing pages

To edit an already existing page you simply follow the steps the "General Instructions" above. The two things you should keep in mind before making a change is:
1. Should my changes be reviewed?
2. What branches are my changes relevant to?

If the changes are significant enough to justify a review, then create a new branch, make a pull request, and assign someone to review.

## Adding a new page

1. Read the instructions above
2. Add a new page in the correct folder. For example, if you want to add a page pertaining to SQL in Starcounter, then you should add a markdown page to in guides/SQL folder.
3. Add your page to `SUMMARY.md` by making it a bullet point under the appropriate section by writing `[Your Page Headline](you-page-headline.md)`
