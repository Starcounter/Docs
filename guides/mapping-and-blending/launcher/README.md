# Launcher

A launcher is an app that gives a common UI feeling to multiple apps running simultaneously.

What it does specifically:

<ul>
<li>create a new session</li>
<li>initialize a Puppet connnection</li>
<li>load a global stylesheet</li>
<li>provide features to switch between apps</li>
</ul>

<a href="https://github.com/StarcounterSamples/Launcher">Launcher project</a> is free for forking and modifying. You can create your own Launcher that does the same but looks differently. Or looks differently and acts differently!

<h2>Launcher features</h2>

<h3>Create a new session</h3>

Launcher creates a new session, so your app doesn't have to.

<h3>Initialize a Puppet connection</h3>

Launcher initializes a Puppet connnection, that syncs the view-model between the server and the client.

<h3>Load a global stylesheet</h3>

Launcher loads a stylesheet that gives a uniform look to the apps. By default it is using a Bootstrap Bootswatch Paper theme. You can override default styles by placing your own <code>bootstrap.html</code> into <code>/sys/</code> folder.

<h3>Provide features to switch between apps</h3>

Launcher provides a concept of workspaces. Only the response for the currently displayed URL is visible on the screen. Other responses are hidden using CSS.

<h4>Launchpad</h4>

The icons in Launcher's Launchpad are composed from the responses to <code>/sc/mapping/app-name</code>.

<h4>Left menu area</h4>

The links in Launcher's left hand side menu are composed from the responses to <code>/sc/mapping/menu</code>.

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

You can put your app's response there by mapping a HTTP GET request handler to this URI using <a href="/guides/mapping-and-blending/uri-mapping">UriMapping.Map</a>.

<h4>User icons area</h4>

The icons in Launcher's top right hand side of the screen are composed from the responses to <code>/sc/mapping/user</code>.

You can put your app's response there by mapping a HTTP GET request handler to this URI using <a href="/guides/mapping-and-blending/uri-mapping">UriMapping.Map</a>.

<a href="https://github.com/StarcounterSamples/SignIn" target="_blank">SignIn</a> application uses this section to show login popup and details of signed in user.

<h4>Settings area</h4>

The in the top right corner of Launcher there is a cog icon that brings the Settings page. Clicking on it displays all partials mapped to <code>/sc/mapping/settings</code>.

You can put your app's response there by mapping a HTTP GET request handler to this URI using <a href="/guides/mapping-and-blending/uri-mapping">UriMapping.Map</a>.

<h4>Search</h4>

Whatever you type to Launcher's edit field results in a HTTP GET request to <code>/sc/mapping/search?query=?</code>.

You may present search results by mapping a HTTP GET request handler to this URI using <a href="/guides/mapping-and-blending/uri-mapping">UriMapping.Map</a>.
