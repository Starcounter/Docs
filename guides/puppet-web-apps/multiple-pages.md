# Multiple pages

Typically, whenever you move from one page to another page in a browser, the browser loses all state apart from the set cookies. This means that all JavaScript code, any DOM elements and ongoing animations are lost. This makes moving between pages slower than it could be.

This is solved in single page applications (SPA) using [`history.pushState`](https://developer.mozilla.org/en-US/docs/Web/Guide/API/DOM/Manipulating_the_browser_history). For instance, GMail allows the user to bookmark emails and use the navigation buttons (back and forward) while, technically, everything is going on on a single page. In this way, the user gets the best of two worlds, the speed of SPA and the expected behaviour of a multipage application. Starcounter has built in support for creating SPA applications that supports multiple pages.

## Example

Let's assume that we have an email application. On the left is a list of all emails and on the right is a single focused email. You can use an URL to navigate to any particular email (i.e. `www.mysampleapp.com/emails/123`). In this way, each particular email works as a separate web page. However, when you step between the emails in the list on the left, you don't want the browser to load a completely new page. This would make the email application slow and any DOM that is outside of the email page (i.e. the page inside the main page) would be reset.

In Starcounter, you can create master pages and sub pages that are handled on the browser side. In this way, the part of the master page that does not change between pages is actually the *same* DOM when you move in-between pages. The JavaScript instance is the same and any ongoing animations work fluently. Above all, performance is stellar.

## Example source code

<div class="code-name">Program.cs</div>

```cs
using Starcounter;

namespace MultiplePagesDemo
{
    [Database]
    public class Mail
    {
        public string Title;
        public string Content;
    }

    public class Program
    {
        static void Main()
        {
            Handle.GET("/MultiplePagesDemo/mails", () =>
            {
                if (Session.Current != null)
                {
                    return Session.Current.Data;
                }
                MailsPage m = new MailsPage() { Html = "/MailsPage.html" };
                m.Session = new Session(SessionOptions.PatchVersioning);
                m.Mails = Db.SQL<Mail>("SELECT m FROM MultiplePagesDemo.Mail m LIMIT ?", 10);
                m.Focused.Data = m.Mails[0].Data;

                return m;
            });

            Handle.GET("/MultiplePagesDemo/mails/{?}", (string id) =>
            {
                Mail mail = Db.SQL<Mail>("SELECT m FROM MultiplePagesDemo.Mail m WHERE ObjectID=?", id).First;
                MailsPage m = Self.GET<MailsPage>("/MultiplePagesDemo/mails");
                m.Focused.Data = mail;
                return m;
            });

            Db.Transact(() =>
            {
                Db.SlowSQL("DELETE FROM MultiplePagesDemo.Mail");

                new Mail()
                {
                    Title = "Hello World",
                    Content = "This is my first email"
                };

                new Mail()
                {
                    Title = "Hi Alexey!",
                    Content = "How are you? Regards Jack"
                };
            });
        }
    }
}
```

<div class="code-name">MailsPage.json</div>

```json
{
  "Mails":[{
    "Title":"",
    "Url":"",
    "$Url":{
      "Bind":"GetUrl"
    }
  }],
  "Focused":{
    "Html": "", 
    "Title": "", 
    "Content": ""
  }
}
```

<div class="code-name">MailsPage.json.cs</div>

```cs
using Starcounter;

namespace MultiplePagesDemo
{
    [MailsPage_json]
    partial class MailsPage : Page
    {
    }

    [MailsPage_json.Mails]
    partial class MailsElement : Json, IBound<Mail>
    {
        public string GetUrl
        {
            get 
            { 
                return "/MultiplePagesDemo/mails/" + this.Data.GetObjectID(); 
            }
        }
    }
}

```

<div class="code-name">wwwroot/MailsPage.html</div>

```html
<!DOCTYPE html>
<html>
<head>
    <title>Mails</title>
    <meta charset="utf-8">
    <script src="/sys/webcomponentsjs/webcomponents.js"></script>
    <link rel="import" href="/sys/puppet-client/puppet-client.html">
    <style>
        body {
            display: flex;
            font-family: "Helvetica Neue","Helvetica","Arial","sans-serif";
        }

        input {
            font-size: 1em;
        }

        .col-left {
            flex: 200px 0 0;
        }

        .col-right {
            flex: 200px 10 0;
        }
    </style>
</head>

<body>
    <template id="root" bind>
        <ul class="col-left">
            <template repeat="{{Mails}}">
                <li><a href="{{Url}}">{{Title}}</a></li>
            </template>
        </ul>

        <div class="col-right">
            <template bind="{{Focused}}">
                <h1>{{Title}}</h1>
                <article>{{Content}}</article>
            </template>
        </div>
    </template>
    <puppet-client ref="root"></puppet-client>
</body>
</html>
```

Let's assume that the user first visits the URL `www.mysampleapp.com/emails` using a browser. The handler registered using `Handle.GET("/emails",...)` will be called and the html page master.html will be returned. If the user then clicks on a specific email, he will hit the handler for `www.mysampleapp.com/emails/123`. But as the master page is already cached, the browser will not leave the current page. Instead it will update the existing page by changing the content (the DOM) of its sub page named `Focused`.

### The best of both worlds

In the above example, the user could bookmark `www.mysampleapp.com/emails/123` and visit some other day or from another browser. The master page would not be present as the current content of the browsers and the call to `Self.GET("/emails")` would instead run the code that creates a new Master page.

### What is happening behind the scenes

Normally, when you visit a new page in a browser, the current page is dropped and a new page is parsed. This is built in to the browser and cannot be changed. So how does a SPA page load a new sub page? 

The answer is that it uses the JavaScript XHR support. Starcounter provides an implementation of loading partial data using the standard JSON-Patch format. So when you visit `www.mysampleapp.com/emails/123` from the browser address bar, the call will get the entire page and when you visit the same url in an existing session, a GET will be sent using the `Accept: application/json-patch+json` header via the JavaScript XHR and only the data needed to morph the current page into the new page is sent. 

You can read more on how the JSON-Patch protocol is used over at [PuppetJs spec](https://github.com/PuppetJs/PuppetJs/wiki/Server-communication).

> Get this sample app from <a class="fusion-button button-flat button-round button-xsmall button-default button-2" href="https://github.com/StarcounterSamples/MultiplePagesDemo"><i class="fa fa-github button-icon-left"></i><span class="fusion-button-text">GitHub</span></a>
