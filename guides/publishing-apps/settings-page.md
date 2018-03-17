# Settings page

If your app is configurable, it's good for to provide a user interface for changing the app settings.

By having a settings page mapped to the common token, your configuration UI can appear along settings pages from other apps. This is good because it gives the end user a single go-to place to configure all apps.

The common pattern is to have the settings page addressable by `/<appname>/settings`, blended to the token `settings`.

The settings page might contain a button to do a "Factory Reset" of the app \(restores required data\) or a "Sample data" button that populates the database with sample data.

Sample and prefab apps that have a settings page:

* [https://github.com/StarcounterApps/Products](https://github.com/StarcounterApps/Products)
* [https://github.com/StarcounterApps/SignIn](https://github.com/StarcounterApps/SignIn)

