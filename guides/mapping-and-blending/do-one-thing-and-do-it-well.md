# Do one thing and do it well

> Write programs that do one thing and do it well.

*Doug McIlroy (2003). [The Art of Unix Programming: Basics of the Unix Philosophy](http://www.catb.org/esr/writings/taoup/html/ch01s06.html)*

Even though Starcounter applications use C# and web technologies, Starcounter is conceptually closer to Unix than  the Microsoft stack because of Starcounter's adherence to the principle that every single application should have a limited responsibility, as stated in the quote above.

<!--For the previous years, many of us have built programs with monolithic frameworks. We have seen these solutions grown significantly over time in answer to multiple business cases presented by our customers. As the software grows, it becomes less and less maintainable. But what is the alternative?-->

When your app is super easy to integrate with other apps (that share data on a shared screen - see the following pages!), then it can focus on solving exactly the problems that it's intended to do.

> For example, a product management app should only do product management. It should not deal with user authentication. This is a responsibility of a user authentication app (and we already have that one).

An app is an application small enough yet functional enough to deliver its function, standalone or in interplay with other apps. Apps don't know of each other and interoperate out of the box to create new solutions. The smallest apps, sometimes referred to as micro-apps, might not be that useful running on their own, but they still can.

The logic runs entirely on the server. Server side code (clean C#) is used to express the business logic and application logic through the data model and the view model.

There is no client-side logic, yet the apps feel responsive and lightweight thanks to Puppet. The client side templates (clean HTML, CSS, minimum JavaScript) to reflect the view model in a web browser.

All apps are represented by an icon and a title and can be installed and uninstalled.

If you liked the Unix philosophy reference, [The Art of Unix Programming](http://www.catb.org/esr/writings/taoup/html/ch01s06.html) has a whole chapter elaborating on its principles.
