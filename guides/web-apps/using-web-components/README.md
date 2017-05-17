# Using Web Components

Web Components are loaded by browser with HTML Imports. HTML Import for Polymer in Starcounter should look like this:

```html
<link rel="import" href="/sys/polymer/polymer.html" />
```

## Avoiding Loading the Same Files Multiple Times

Browser loads HTML Imports only once, not like scripts and styles, which are loaded as many times as many references page has.

How does the browser know which of the components has already been imported then? Browser distinguishes Web Components one from another by their location. 

This will be loaded twice and then lead to a Polymer error:
```html
<link rel="import" href="/bower_components/polymer/polymer.html" />
<link rel="import" href="/sys/polymer/polymer.html" />
```

It is a good practice to avoid double loading of external libraries. Hence, you need to use the same URL pattern as other apps. The recommended way is to use the pattern: `/sys/<dependency-name>/<dependency-files>`.

Starcounter has system folder called `StaticFiles` and located in the installation folder: `C:\Program Files\Starcounter\ClientFiles\StaticFiles`. There you can find `sys` folder, which includes some common components such as Polymer and PuppetJs. You can also look at the code example under `PartialToStandaloneHtmlProvider` on the [middleware page](/guides/network/middleware/) to see what client side libraries are included in that directory by default.

The benefit of this is that you can rely on having a specific version of Starcounter to include a specific version of Polymer, PuppetJs, etc.

## Adding External Dependencies to Your Apps

In order to add files that match this pattern, simply put a `sys` folder in your `wwwroot` folder that holds the static files for your project.

You should not do that automatically, but use Bower to install such dependencies. A correct Bower configuration consists of two files in your project: `.bowerrc` and `bower.json.

### .bowerrc

The `.bowerrc` file contains Bower configuration. It specifies what is the destination directory and what dependencies should be ignored, because they are delivered as part of Starcounter. An example of this can be found in the [KitchenSink app](https://github.com/StarcounterApps/KitchenSink/blob/master/src/KitchenSink/.bowerrc).

### bower.json

`bower.json` file that keeps the list of your app's client side dependencies. This file should not be created and maintained manually. It should be modified using the command line tool: `bower init`, `bower install paper-dialog --save`.

A sample file can be found in the [KitchenSink app](https://github.com/StarcounterApps/KitchenSink/blob/master/src/KitchenSink/bower.json).
 
## Starcounter Static File Server

The `StaticFiles` folder from Starcounter installation is automatically served as a static content folder. When Starcounter server receives a request for a static file, it searches for the file in all of the static content folders. The project folder has higher priority over internal folder.

Read more [here](/guides/network/static-file-server/).

Keep that in mind you can simply use another version of Polymer by putting it into your local `sys` folder. This will affect all other apps, though.

## Open Source Web Components

A vast library of open source Web Components are available at [webcomponents.org](https://www.webcomponents.org/). 