# Mapping file

Each app that have blendable views should have a `<AppName>.map.md` file which contains the list of them.

The file should be provided at the root of the App Warehouse package. If the app is open source, the file should be provided in the code repository, preferably at the root.

Every view must be explained in the file and preferably illustrated with a screenshot.

In the future, the information in this file is used for automated blending as well as for [testing purposes](https://github.com/Starcounter/Guidelines/issues/26).

Sample `Images.map.md` ([source](https://github.com/StarcounterApps/Images/blob/develop/Images.map.md)):

```txt
# Blendable views

## /images/partials/contents/{[Simplified.Ring1.Content](https://github.com/StarcounterApps/Simplified/blob/master/Ring1/Content.cs)}

Shows a simple page for `Content` preview, image or video. In case of unexisting
content, shows empty file preview image.

![screenshot](docs/screenshot-content.png)
```
