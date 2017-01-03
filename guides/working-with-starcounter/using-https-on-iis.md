# Using HTTPS on IIS

This article explains how to use Microsoft [Internet Information Server](https://www.iis.net/) (IIS) as an HTTPS proxy on top of Starcounter. While NGINX might be a better web server overall, it requires a separate Linux machine. IIS can be installed alongside Starcounter on the same Windows machine.

**Note:** IIS splits big web socket messages into chunks, but this is not yet supported by Starcounter. Usually it only affects file upload over web socket. See this issue: [Add support for frame fragmentation in WebSockets](https://github.com/Starcounter/Starcounter/issues/3837). Temporary solution is to reduce file upload chunk size to `500 bytes`.

## Installing IIS

IIS is available from the `Turn Windows features on or off` console.

The following items should be installed.

<a href="http://starcounter.io/wp-content/uploads/2016/10/starcounter-https-iis-0.png" rel="attachment wp-att-15862"><img src="http://starcounter.io/wp-content/uploads/2016/10/starcounter-https-iis-0.png" alt="Installing IIS features" width="415" height="870" class="alignnone size-full wp-image-15862" /></a>

You can add any extra features if needed.

## Setting up IIS

Open Internet Information Services (IIS) Manager from the Start menu. And install the following components with `Get New Web Platform Components`.

- URL Rewrite 2.0
- Application Request Routing 3.0

<a href="http://starcounter.io/wp-content/uploads/2016/10/starcounter-https-iis-1.png" rel="attachment wp-att-15863"><img src="http://starcounter.io/wp-content/uploads/2016/10/starcounter-https-iis-1.png" alt="IIS Url Rewrite &amp; Application Request Routing" width="686" height="473" class="alignnone size-full wp-image-15863" /></a>

Enable `system.webServer/webSocket` configuration section via Configuration Editor.

<a href="http://starcounter.io/wp-content/uploads/2016/10/starcounter-https-iis-2.png" rel="attachment wp-att-15864"><img src="http://starcounter.io/wp-content/uploads/2016/10/starcounter-https-iis-2.png" alt="IIS - system.webServer/webSocket" width="1071" height="633" class="alignnone size-full wp-image-15864" /></a>

Restart the machine.

Open `Application Request Routing Cache` section and go to it's settings page.

<a href="http://starcounter.io/wp-content/uploads/2016/10/starcounter-https-iis-3.png" rel="attachment wp-att-15865"><img src="http://starcounter.io/wp-content/uploads/2016/10/starcounter-https-iis-3.png" alt="IIS - Application Request Routing Cache" width="896" height="625" class="alignnone size-full wp-image-15865" /></a>

<a href="http://starcounter.io/wp-content/uploads/2016/10/starcounter-https-iis-4.png" rel="attachment wp-att-15866"><img src="http://starcounter.io/wp-content/uploads/2016/10/starcounter-https-iis-4.png" alt="IIS - Application Request Routing Cache" width="903" height="633" class="alignnone size-full wp-image-15866" /></a>

Enable proxy, update timeout to `600` seconds, and click the `Apply` button.

<a href="http://starcounter.io/wp-content/uploads/2016/10/starcounter-https-iis-5.png" rel="attachment wp-att-15867"><img src="http://starcounter.io/wp-content/uploads/2016/10/starcounter-https-iis-5.png" alt="IIS - Application Request Routing Cache Settings" width="904" height="632" class="alignnone size-full wp-image-15867" /></a>

## Setting up IIS website

By default there should be a `Default Web Site` item in the `Sites` section of IIS. The website should listen on the `80` port. Check that by opening `http://localhost/` in your browser. It should show the default IIS webpage.

Create and configure a new website if the default one is missing or you want to listen on another port rather than `80`.

[Create a self signed HTTPS certificate](https://technet.microsoft.com/en-us/library/cc753127%28v=ws.10%29.aspx) and assign it to the website which should be used as a proxy.

Open `https://localhost/` in your browser and make sure that the default IIS webpage is also shown.

Open or create `web.config` file in the root folder of the website and update it's content to the following.

```xml
<?xml version="1.0" encoding="UTF-8"?>
<configuration>
    <system.webServer>
		<webSocket enabled="true" receiveBufferLimit="4194304" />
        <rewrite>
            <rules>
                <clear />
                <rule name="HTTP -> HTTPS" enabled="true" stopProcessing="true">
                    <match url="(.*)" />
                    <conditions logicalGrouping="MatchAll" trackAllCaptures="false">
                        <add input="{HTTPS}" pattern="^OFF$" />
                    </conditions>
                    <action type="Redirect" url="https://{HTTP_HOST}/{R:1}" redirectType="Temporary" />
                </rule>
                <rule name="80 -> 8080 port" patternSyntax="ECMAScript" stopProcessing="true">
                    <match url="(.*)" />
                    <conditions logicalGrouping="MatchAll" trackAllCaptures="false">
                        <add input="{HTTPS}" pattern="^OFF$" />
                        <add input="{CACHE_URL}" pattern="^(.+)[:][/][/]" />
                    </conditions>
                    <action type="Rewrite" url="{C:1}://localhost:8080/{R:0}" />
                </rule>
                <rule name="80 -> 8080 port, https">
                    <match url="(.*)" />
                    <conditions logicalGrouping="MatchAll" trackAllCaptures="false">
                        <add input="{HTTPS}" pattern="^ON$" />
                    </conditions>
                    <action type="Rewrite" url="http://localhost:8080/{R:0}" />
                </rule>
            </rules>
        </rewrite>
    </system.webServer>
</configuration>
```

**Note:** the rule names can be anything, but better keep it meaningful. The `8080` port should be updated with the port of the Starcounter database IIS should redirect to.

Make sure that Starcounter database is running and the proxy is working.

- `http://localhost/` should redirect to `https://localhost/`
- `https://localhost/` should open `http://localhost:8080/`