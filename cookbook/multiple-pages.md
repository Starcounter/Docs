# Multiple pages

Typically, whenever you move from one page to another page in a browser, the browser loses all state apart from the set cookies. This means that all JavaScript code, any DOM elements and ongoing animations are lost. This makes moving between pages slower than it could be.

This is solved in single page applications \(SPA\) using [`history.pushState`](https://developer.mozilla.org/en-US/docs/Web/Guide/API/DOM/Manipulating_the_browser_history). For instance, GMail allows the user to bookmark emails and use the navigation buttons \(back and forward\) while, technically, everything is going on on a single page. In this way, the user gets the best of two worlds, the speed of SPA and the expected behaviour of a multipage application. Starcounter has built in support for creating SPA applications that supports multiple pages.

## Example

Let's assume that we have an email application. On the left is a list of all emails and on the right is a single focused email. You can use an URL to navigate to any particular email \(i.e. `/MultiplePagesDemo/emails/123`\). In this way, each particular email works as a separate web page. However, when you step between the emails in the list on the left, you don't want the browser to load a completely new page. This would make the email application slow and any DOM that is outside of the email page \(i.e. the page inside the main page\) would be reset.

![Multiple pages in action](../.gitbook/assets/multiplepages%20%281%29.gif)

In Starcounter, you can create master pages and sub pages that are handled on the browser side. In this way, the part of the master page that does not change between pages is actually the _same_ DOM when you move in-between pages. The JavaScript instance is the same and any ongoing animations work fluently. Above all, performance is stellar.

## Example source code

### View-model

MailsPage.json

```javascript
{
  "Html": "/MultiplePagesDemo/views/MailsPage.html",
  "Mails": [
    {
      "Title": "",
      "Url": ""
    }
  ],
  "Focused": {}
}
```

The `Mails` element is used to display a list with links to their corresponding email. The `Focused` element is filled with a single `Focused.json`.

Focused.json

```javascript
{
  "Html": "/MultiplePagesDemo/views/Focused.html",
  "Title": "",
  "Content": ""
}
```

The `Focused.json` has its own Html file which will be inserted into the `MailsPage` in a SPA manner.

### Application code

Program.cs

```csharp
using Starcounter;
using System.Linq;

namespace MultiplePagesDemo
{
    [Database]
    public class Mail
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Url => "/MultiplePagesDemo/mails/" + this.GetObjectID();
    }

    public class Program
    {
        static void Main()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Handle.GET("/MultiplePagesDemo/mails", () =>
            {
                if (Session.Current != null)
                {
                    return Session.Current.Data;
                }
                Session.Current = new Session(SessionOptions.PatchVersioning);

                var mailPage = new MailsPage()
                {
                    Session = Session.Current,
                    Mails = Db.SQL<Mail>("SELECT m FROM MultiplePagesDemo.Mail m")
                };

                Focused foc = new Focused();
                foc.Data = mailPage.Mails.FirstOrDefault().Data;
                mailPage.Focused = foc;

                return mailPage;
            });

            Handle.GET("/multiplepagesdemo/mails/{?}", (string id) =>
            {
                Mail mail = Db.SQL<Mail>("SELECT m FROM multiplepagesdemo.mail m WHERE objectid=?", id).FirstOrDefault();
                MailsPage mailsPage = Self.GET<MailsPage>("/multiplepagesdemo/mails");
                mailsPage.Focused.Data = mail;
                return mailsPage;
            });

            Db.Transact(() =>
            {
                bool emptyMailbox = Db.SQL<long>("SELECT COUNT(m) FROM multiplepagesdemo.mail m").FirstOrDefault() == 0;
                if (emptyMailbox)
                {
                    new Mail()
                    {
                        Title = "Hello Mail",
                        Content = "This is my first email!"
                    };

                    new Mail()
                    {
                        Title = "Greetings",
                        Content = "How are you? Regards jack"
                    };
                }
            });
        }
    }
}
```

The program defines two [`Handle.GET`](https://docs.starcounter.io/guides/network/handling-http-requests/) operations. One to retrieve the master, MailsPage, and the other to retrieve specific emails.

The first, `Handle.GET("/MultiplePagesDemo/mails"...`, returns the master json MailsPage. It checks if there is a current session and if so uses it instead of creating a new. It uses the first mail, if any, as the focused mail.

The second, `Handle.GET("/multiplepagesdemo/mails/{?}"...`, returns a MailsPage as well. It does however specify which email should be focused. By relying on [`Self.GET`](https://docs.starcounter.io/guides/network/internal-self-calls/) we call the first handler which includes the functionality to check for exisiting sessions.

### View

`MailsPage.html` contains the full page to display. It lists all the mails as linked text. The most important part is the [`<starcounter-include partial=...`](https://docs.starcounter.io/guides/web-apps/html-views/) which loads the Focused element of `MailsPage.json` with its own html file.

## How it works

When a client visits `/MultiplePagesDemo/mails` the system will create a session and return the MailsPage.json. When a user then clicks on a specific mail the system will retrieve the MailsPage.json once again but as the client already has it cached there is no need to send it again and the page will not reload. The only differing element, `Focused`, will just be replaced.

This works just as well if a client is directly accessing a specific mail using it's URL. This means that a client can bookmark specific mails without any problems as if it were a multiple pages application.

