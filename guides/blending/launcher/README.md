# Launcher

A launcher is an app that gives a common UI feeling to multiple apps running simultaneously.

What it does specifically:

* create a new session
* initialize a Puppet connnection
* load a global stylesheet
* provide features to switch between apps

[Launcher project](https://github.com/StarcounterApps/Launcher) is free for forking and modifying. You can create your own Launcher that does the same but looks differently. Or looks differently and acts differently!

## Launcher features

### Create a new session

Launcher creates a new session, so your app doesn't have to.

### Initialize a Puppet connection

Launcher initializes a Puppet connnection, that syncs the view-model between the server and the client.

### Load a global stylesheet

Launcher loads a stylesheet that gives a uniform look to the apps. By default it is using a Bootstrap Bootswatch Paper theme. You can override default styles by placing your own `bootstrap.html` into `/sys/` folder.

### Provide features to switch between apps

Launcher provides a concept of workspaces. Only the response for the currently displayed URL is visible on the screen. Other responses are hidden using CSS.

#### Launchpad

The icons in Launcher's Launchpad are composed from the responses to the string token `app-name`.

#### Left menu area

The links in Launcher's left hand side menu are composed from the responses to the string token `menu`.

A typical menu item response looks like:

```html
<template>
  <li>
    <a href="/products">
      <i class="glyphicon glyphicon-oil"></i>
      <span>Products</span>
    </a>
  </li>
</template>
```

You can put your app's response there by mapping a HTTP GET request handler to this URI using [Blender.MapUri](/guides/blending/blending).

#### User icons area

The icons in Launcher's top right hand side of the screen are composed from the responses to the string token `user`.

You can put your app's response there by mapping a HTTP GET request handler to this URI using [Blender.MapUri](/guides/blending/blending).

[SignIn](https://github.com/StarcounterApps/SignIn) application uses this section to show login popup and details of signed in user.

#### Settings area

The in the top right corner of Launcher there is a cog icon that brings the Settings page. Clicking on it displays all partials mapped to the string token `settings`.

You can put your app's response there by mapping a HTTP GET request handler to this URI using [Blender.MapUri](/guides/blending/blending).

#### Search

Whatever you type to Launcher's edit field results in a HTTP GET request to the string token `search`.

You may present search results by mapping a HTTP GET request handler to this URI using [Blender.MapUri](/guides/blending/blending).