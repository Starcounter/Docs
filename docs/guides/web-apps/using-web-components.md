# Using Web Components

Web Components are loaded by browser with HTML Imports. HTML Import for Polymer in Starcounter should look like this:

```markup
<link rel="import" href="/sys/polymer/polymer.html" />
```

### Avoiding Loading the Same Files Multiple Times

Browser loads HTML Imports only once, not like scripts and styles, which are loaded as many times as many references page has.

How does the browser know which of the components has already been imported then? Browser distinguishes Web Components one from another by their location.

This will be loaded twice and then lead to a Polymer error:

```markup
<link rel="import" href="/bower_components/polymer/polymer.html" />
<link rel="import" href="/sys/polymer/polymer.html" />
```

It is a good practice to avoid double loading of external libraries. Hence, you need to use the same URL pattern as other apps. The recommended way is to use the pattern: `/sys/<dependency-name>/<dependency-files>`.

Starcounter has system folder called `StaticFiles` and located in the installation folder: `C:\Program Files\Starcounter\ClientFiles\StaticFiles`. There you can find `sys` folder, which includes some common components such as Polymer and Palindrom. You can also look at the code example under `PartialToStandaloneHtmlProvider` on the [middleware page](../network/middleware.md) to see what client side libraries are included in that directory by default.

The benefit of this is that you can rely on having a specific version of Starcounter to include a specific version of Polymer, Palindrom, etc.

### Adding External Dependencies to Your Apps

In order to add files that match this pattern, simply put a `sys` folder in your `wwwroot` folder that holds the static files for your project.

You should not do that automatically, but use Bower to install such dependencies. A correct Bower configuration consists of two files in your project: `.bowerrc` and \`bower.json.

### .bowerrc

The `.bowerrc` file contains the Bower configuration. It specifies the destination directory and what dependencies should be ignored, because they are delivered with Starcounter. An example of this can be found in the [KitchenSink app](https://github.com/StarcounterApps/KitchenSink/blob/master/src/KitchenSink/.bowerrc).

To find the specific dependencies that are delivered with Starcounter, go to `C:\Program Files\Starcounter\ClientFiles\bower-list.txt`. For Starcounter 2.3.1.6694, it looks like this:



```text
bower check-new     Checking for new versions of the project dependencies...
sys#1.0.0 C:\Users\omer\Desktop\shared folder\new SC\level1\src\BuildSystem\ClientFiles
├─┬ PuppetJs#2.5.0
│ ├── fast-json-patch#1.1.8 (1.2.2 available)
│ ├── json-patch-ot#61992acdfd
│ ├─┬ json-patch-ot-agent#49d5d2cee5
│ │ └── json-patch-queue#2a579f94db
│ └── json-patch-queue#2a579f94db
├── array.observe#0.0.1 extraneous
├─┬ bootswatch#3.3.7
│ └── bootstrap not installed
├── dom-bind-notifier#a43453ceb9
├─┬ imported-template#1.5.0
│ └── juicy-html#1.2.0
├── juicy-redirect#0.4.2 extraneous
├── object.observe#0.2.6 extraneous
├── polymer#1.9.1 (2.0.1 available)
├─┬ puppet-client#3.2.2 (3.3.0 available)
│ ├── PuppetJs#2.5.0
│ └─┬ polymer#1.9.1 (latest is 2.0.1)
│   └── webcomponentsjs#0.7.24 (latest is 1.0.1)
├── puppet-redirect#0.4.3 (latest is 0.5.0)
├─┬ starcounter-debug-aid#2.0.10
│ ├─┬ juicy-jsoneditor#1.1.0 (1.1.1 available)
│ │ ├── fast-json-patch#1.1.8 incompatible with ~1.0.0 (1.0.1 available, latest is 1.2.2)
│ │ ├── jsoneditor#5.5.11 (5.7.0 available)
│ │ └── polymer#1.9.1 (2.0.1 available)
│ └── polymer#1.9.1 (2.0.1 available)
├─┬ starcounter-include#2.3.2
│ ├─┬ imported-template#1.5.0
│ │ └── juicy-html#1.2.0
│ └─┬ juicy-composition#0.1.1
│   ├── translate-shadowdom#0.0.5
│   └── webcomponentsjs#0.7.24 (latest is 1.0.1)
└── webcomponentsjs#0.7.24 (latest is 1.0.1)
```

### bower.json

`bower.json` file that keeps the list of your app's client side dependencies. This file should not be created and maintained manually. It should be modified using the command line tool: `bower init`, `bower install paper-dialog --save`.

A sample file can be found in the [KitchenSink app](https://github.com/StarcounterApps/KitchenSink/blob/master/src/KitchenSink/bower.json).

### Starcounter Static File Server

The `StaticFiles` folder from Starcounter installation is automatically served as a static content folder. When Starcounter server receives a request for a static file, it searches for the file in all of the static content folders. The project folder has higher priority over internal folder.

Read more [here](../network/static-file-server.md).

Keep that in mind you can simply use another version of Polymer by putting it into your local `sys` folder. This will affect all other apps, though.

### Open Source Web Components

A vast library of open source Web Components are available at [webcomponents.org](https://www.webcomponents.org/).

