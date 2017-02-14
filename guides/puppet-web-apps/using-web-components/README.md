# Using Web Components

Web Components are loaded by browser with HTML Imports. HTML Import for Polymer in Starcounter should look like this:

```html
<link rel="import" href="/sys/polymer/polymer.html" />
```

## Avoiding loading the same files multiple times

Browser loads HTML Imports only once, not like scripts and styles, which are loaded as many times as many references page has.

How does the browser know which of the components has already been imported then? Browser distinguishes Web Components one from another by their location. 

This will be loaded twice and then lead to a Polymer error:
```html
<link rel="import" href="/bower_components/polymer/polymer.html" />
<link rel="import" href="/sys/polymer/polymer.html" />
```

It is a good practice to avoid double loading of external libraries. Hence, you need to use the same URL pattern as other apps. The recommended way is to use the pattern: `/sys/<dependency-name>/<dependency-files>`.

Starcounter has system folder called `StaticFiles` and located in the installation folder: `C:\Program Files\Starcounter\ClientFiles\StaticFiles`. There you can find `sys` folder, which includes some common components such as Polymer and PuppetJs. You can also [browse out GitHub repo](https://github.com/Starcounter/Starcounter/tree/develop/src/BuildSystem/ClientFiles/StaticFiles/sys) to see what client side libraries are included in that directory.

The benefit of this is that you can rely on having a specific version of Starcounter to include a specific version of Polymer, PuppetJs, etc.

## Adding external dependencies to your apps

In order to add files that match this pattern, simply put a `sys` folder in your `wwwroot` folder that holds the static files for your project.

You should not do that automatically, but use Bower to install such dependencies. A correct Bower configuration consists of two files in your project: `.bowerrc` and `bower.json.

### .bowerrc

The `.bowerrc` file contains Bower configuration. It specifies what is the destination directory and what dependencies should be ignored, because they are delivered as part of Starcounter.

<div class="code-name">
<a href="https://github.com/StarcounterSamples/KitchenSink/blob/master/src/KitchenSink/.bowerrc" target="_blank">.bowerrc</a></div>

```json
{
  "directory": "wwwroot/sys",
  "ignoredDependencies": [
    "webcomponentsjs",
    "PuppetJs",
    "imported-template",
    "puppet-redirect",
    "juicy-redirect",
    "starcounter-debug-aid",
    "puppet-client",
    "bootswatch",
    "polymer"
  ]
}
```

### bower.json

`bower.json` file that keeps the list of your app's client side dependencies. This file should not be created and maintained manually. It should be modified using the command line tool: `bower init`, `bower install paper-dialog --save`.

A sample file looks like this:

<div class="code-name">
<a href="https://github.com/StarcounterSamples/KitchenSink/blob/master/src/KitchenSink/bower.json" target="_blank">bower.json</a></div>

```json
{
  "name": "KitchenSink",
  "version": "1.0.0",
  "homepage": "https://github.com/StarcounterSamples/KitchenSink",
  "authors": [
    "Marcin Warpechowski <warpech@gmail.com>"
  ],
  "license": "MIT",
  "private": true,
  "ignore": [
    "**/.*",
    "node_modules",
    "bower_components",
    "wwwroot/sys",
    "test",
    "tests"
  ],
  "dependencies": {
    "hot-table": "handsontable/hot-table#fix-styles-issue-23",
    "juicy-markdown": "~1.0.0",
    "paper-item": "PolymerElements/paper-item#~1.0.3",
    "paper-menu": "PolymerElements/paper-menu#~1.1.1",
    "paper-radio-button": "PolymerElements/paper-radio-button#~1.0.9",
    "paper-radio-group": "PolymerElements/paper-radio-group#~1.0.5",
    "chart-elements": "robdodson/chart-elements#~2.0.0",
    "google-map": "GoogleWebComponents/google-map#~1.1.4",
    "juicy-select": "Juicy/juicy-select#master",
    "x-breadcrumb": "Juicy/x-breadcrumb#gh-pages",
    "paper-dialog": "PolymerElements/paper-dialog#^1.0.4",
    "paper-toggle-button": "PolymerElements/paper-toggle-button#^1.1.2",
    "starcounter-upload": "Starcounter/starcounter-upload#~0.0.1"
  },
  "resolutions": {
    "polymer": "^1.1.*",
    "pikaday": "^1.3.2"
  }
}
```
 
## Starcounter static file server

The `StaticFiles` folder from Starcounter installation is automatically served as a static content folder. When Starcounter server receives a request for a static file, it searches for the file in all of the static content folders. The project folder has higher priority over internal folder.

Read more here: <a href="https://github.com/Starcounter/Starcounter/wiki/Serving-static-content" target="_blank">https://github.com/Starcounter/Starcounter/wiki/Serving-static-content</a>.

Keep that in mind you can simply use another version of Polymer by putting it into your local `sys` folder. This will affect all other apps, though.