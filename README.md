This is a GitBook to test the feasibility of using this for the Starcounter documentation

## Steps to move from WordPress to GitBook

#### Setup
1. Download GitBook desktop client from [here](https://www.gitbook.com/editor)
2. Open GitBook and click GitBook.com in the top right corner
3. Click on the book for Docs and click `clone` when the dialogue box opens
4. Open Docs from the `Local Library` tab
5. Go to `File` -> `Preferences..` -> `Git` 
6. Uncheck the `Automatically generate commit message from changes`

#### Adding pages

1. In the files tree section of GitBook, if there's already a directory where the page goes, simple add a file and name it the same as the original page, i.e `commit-hooks.md`. Otherwise create a directory where the page should be.
2. Go to `SUMMARY.md` and add a reference to the page in the correct place like this: `[<my-page-name>](<page-directory>/<page-file>)`
3. Copy the markdown from WordPress into the newly created markdown file.
4. Add a headline of the size `<h1>`
5. Remove all the `fusion` tags that come from the WordPress theme
6. Fix paths to images
    * Download the image from the original
    * Simply drag the file into GitBook and give it a descriptive name
7. Click save and add a commit message

#### To do later
* Fix all the internal links
* Host this GitBook on docs.starcounter.com
* Deal with ugly code boxes
* Implement custom CSS