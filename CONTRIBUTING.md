# Contribution Instructions

## Creating issues

There are a few things to keep in mind when creating issues here:

1. There are almost no bad issues, they can be created for typos, glitches, ambiguity, questions, and more. 
2. Please link to the relevant page when creating the issue

## Contributing to the Documentation

Changes to this repo are conducted the same way we would make changes to a repo containing code. In broad strokes, that means:

1. Clone the repo to your local machine
2. Create a new branch
3. Make changes using the text editor of your choice
4. Commit with a descriptive message
5. Push the branch
6. Create a pull request (optional)
7. Merge into one of the branches
8. Merge changes to all other relevant branches

When creating a new branch, do so from the oldest applicable branch. For example, if the change applies to all branches, then the new branch should be created from the oldest branch, which is 2.1.177. If it applies to all versions after 2.2, then the branch should be created from 2.2.1834, and so on. In git, it will look something like this:

```git
git checkout 2.1.177
git checkout -b fix-typo
git add .
git commit -m "Description of the changes"
git push -u origin fix-typo
```

To merge the changes, merge to the branch that the checkout was done from. So if the checkout was done from 2.1.177, as shown above, the following commands should be executed:

```git
git checkout 2.1.177
git merge --no-ff --no-commit fix-typo
```

When the changes have been merged into the oldest applicable branch, the changes should be merged in a cascading fashion to all the newer branches. With the current branches, it looks like this:

```
git checkout 2.2.1834
git merge 2.1.177
git push

git checkout 2.2.1.3234
git merge 2.2.1834
git push

git checkout RC
git merge 2.2.1.3234
git push

git checkout Develop
git merge RC
git push
```

When these changes are pushed, GitBook will sync, build, and upload them to the [Docs](https://docs.starcounter.io/).

The branches that are uploaded to GitBook are:

* Every release:
    * 2.1.177
    * 2.2.1834
    * 2.2.1.3234
* RC
* Develop

### Using the Gitbook editor

If you wish to see a preview of the markdown, the [GitBook Editor](https://www.gitbook.com/editor) can be used. This editor has good integration with git and is relatively easy to learn. Although, to use the GitBook Editor you have to go to `File` -> `Preferences..` -> `Git` and uncheck the box `Automatically generate commit message from changes` in order to write your own commit messages. It's worth knowing that GitBook does not apply CSS styles, for that you have to follow the instructions below.

### Checking changes locally

You're encouraged to check for yourself how the changes look before pushing it to the remote branch. This can be accomplished using the GitBook CLI:

1. run `npm install -g gitbook-cli`
2. the first time, run `gitbook install` to install the plugins
3. run `gitbook serve` and go to `localhost:4000` to see how the changes will look when they're live

## Adding a new page

1. Read the instructions above
2. Create a folder with the same name as the page in the right folder. For example, if you want to add a page pertaining to SQL in Starcounter, then you should add a folder containing a `README.md` file to in guides/SQL folder. All the content in the new page should be in the `README` file
3. Add your page to `SUMMARY.md` by making it a bullet point under the appropriate section by writing `[Your Page Headline](your-page-headline/README.md)`
