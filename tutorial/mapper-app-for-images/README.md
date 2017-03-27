# Mapper App for Images

To display our images, we will use a prefabricated application called "Images" that we can get from the Starcounter App Warehouse. This application will be on the same shared screen as the application we've built. To accomplish this, we will do something called [mapping](/guides/mapping-and-blending/sharing-screen/). Fortunately, the Images [README.md](https://github.com/starcounterprefabs/images) provides us with a mapping sample that we can use in our code.

To implement this we need to do some setup.

1. Create a new project and call it <code>HelloWorldMapper</code>.
2. Add <code>Simplified.Data.Model</code> to this project the same way we did with the <code>HelloWorld</code> project.
3. Create a reference to the <code>HelloWorld</code>. You can find this in the <code>Projects</code> tab of the <code>Reference Manager</code>.
4. Change the <code>Copy Local</code> property to False for the <code>Simplified.Data.Model</code>.

Now we can start getting some code in there. As mentioned earlier, we simply copy the mapping code from [here](https://github.com/StarcounterPrefabs/Images/blob/master/README.md) into the <code>Main</code> method of <code>Program.cs</code> in <code>HelloWorldMapper</code> and do some small adjustments. In addition, you have to add <code>using Starcounter.Internal</code> to the beginning of the same file in order to use <code>StarcounterEnvironment.RunWithinApplication</code>.

<div class="code-name">Program.cs</div>

{% raw %}
```cs
UriMapping.OntologyMap<HelloWorld.Expense>("/HelloWorld/partial/expense/{?}");

StarcounterEnvironment.RunWithinApplication("Images", () => {
    Handle.GET("/images/partials/concept-expense/{?}", (string objectId) => {
        return Self.GET("/images/partials/concept/" + objectId);
    });

    UriMapping.OntologyMap<HelloWorld.Expense>("/images/partials/concept-expense/{?}");
});
```
{% endraw %}

To get the applications up and running correctly, start them in this order:

1. HelloWorld
2. Images
3. HelloWorldMapper

It is advisable to first start HelloWorld, then go to `http://localhost:8181/#/databases/default/appstore`, find Images under Starcounter Prefabs Store and click `Download`. It will then be possible to go to `http://localhost:8181/#/databases/default` and simply press `Start`. At last, start HelloWorldMapper from Visual Studio or the command line.

Open up <code>HelloWorld</code> in the <code>Starcounter Administrator</code> and you should see a screen that looks like this after you have added your images:

![part 8 GIF](/assets/part8-resize.gif)

That's cool, but we have to do some work to make them look nice together. Let's fix that in the last step!

If you get any errors, you can check your code against the [source code](https://github.com/StarcounterApps/HelloWorld/commit/0477165d371b37c8a65daf489de5fb34d70c70f4).

<section class="hero"><strong>Disclaimer</strong>
The APIs used in this step are experimental. We consider blending of different application's user interfaces to be an essential feature. We are working hard to develop exceptional blending that might require the steps outlined here to be changed. Feel free to follow our <a href="http://starcounter.io/blog/">blog</a> to get information about these changes and more.</section>
