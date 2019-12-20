# Contributing

There are two ways to change the documentation: by editing the markdown files in this repository, or by editing directly in the browser on docs.starcounter.io. For most writing, it's easiest to edit the markdown files locally and then push to this repository.

To edit the markdown files, clone this repository, make your edits, and push. GitBook then picks up the changes and syncs it to the live documentation. For smaller wording changes or clarifications in the default branch, which represents the develop version of Starcounter, this is all you need to know. For all other changes, there are some other things to keep in mind: versioning, reviews, and formatting.

## Versioning

Each branch of the documentation corresponds to one Starcounter version. For example, the 2.3.1 branch corresponds to the 2.3.1 release of Starcounter. When you edit the documentation, you first have to determine in which version to include the changes. The general guideline is the documentation should only change if the corresponding Starcounter version changes. For example, if a new feature is added to 2.3.2, the branches 2.3.2 and 2.4 should reflect this.

Versions that are already released should not be touched. At the time of this writing, this means that changes should never be made to the documentation of the branches 2.2, 2.3.0, and 2.3.1 since these are released Starcounter versions. Thus, all changes should either go into 2.3.2 and 2.4, or only 2.4.

If the change applies to 2.3.2, 2.3.2 should be merged into 2.4 to ensure that 2.4 is always ahead of 2.3.2 - similar to how we treat source-code.

When merging, beware of merge conflicts. There might be structural differences between versions, so merge conflicts might not always be straightforward to resolve. Ask for help from the maintainer (@miyconst) when in doubt.

## Reviews

To maintain high quality and consistency in the documentation, add changes that are not typo fixes or other smaller changes through a pull request.

## Formatting

To make it easier for the reader to skim through the pages, we're trying to keep consistent formatting on the pages. For the pages in the topic guides, this means that the pages start with an introduction section and that the sections are of an appropriate size. For the how-to pages, the structure looks like this:

- Goal
- Steps

  - Step 1
  - Step 2
  - Step 3

- Summary

## Editing on docs.starcounter.io

In addition to pushing to GitHub, it's also possible to edit directly in the browser on docs.starcounter.io. These changes are then automatically synced to GitHub. To edit directly on docs.starcounter.io, you have to have the correct authorization. To get authorization, contact @miyconst.

Editing directly is appropriate when moving pages or making other structural changes. By doing that in the web interface, GitBook will handle paths and other related things. It's also useful to edit in the browser when you're using more visual elements, such as hints, API blocks, or tabs, since that allows you to see how it looks before publishing.
