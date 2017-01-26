# Accessing, and invoking, external methods from objects within nested view-models

When working with objects located inside nested view-models, one can only work within that specific scope. As one might suspect, this makes it difficult to invoke methods from anywhere but this scope.

One solution to this is to use <code>Action</code>. <code>Action</code> is a public delegate void, and like any delegate, it can be used to pass methods as arguments to other methods. By using <code>Action</code>, one can pass a method from outside of the objects scope as an argument, and have the method be invoked as a result of being called from within the nested view-model. The following example will explain how this can be used.

In order to use <code>Action</code>, one must first declare it.

<div class="code-name">CustomElementRelationPage.json.cs</div>

```cs
using System;
using Starcounter;
using Simplified.Ring3;

namespace People
{
    [CustomElementRelationPage_json]
    partial class CustomElementRelationPage : Json, IBound<CustomElementRelation>
    {
        protected ContactInfoProvider contactInfoProvider = new ContactInfoProvider();
        public Action NeedRefresh { get; set; } // Declares the Action delegate
...
```

<code>Action</code> will be containing the method we want to pass as an argument. By using the <code>get</code> accessor we can access its contents, and by using the <code>set</code> accessor we can set our chosen method as the content.

The other thing that must be done is to actually pass a method to the <code>Action</code> delegate. This can be done outside the scope of our object.

<div class="code-name">PersonPage.json.cs</div>

```cs
public void RefreshCustomElements()
{
    this.CustomElements.Clear();

    foreach (CustomElementRelation row in contactInfoProvider.SelectCustomElementRelations(this.Data))
    {
        CustomElementRelationPage page = Self.GET<CustomElementRelationPage>("/people/partials/custom-element-relations/" + row.Key);

        // This is where we pass a method to our Action delegate
        // Here, the content gets set to "RefreshCustomElements();"
        page.NeedRefresh = () =>
        {
            this.RefreshCustomElements();
        };

        this.CustomElements.Add(page);
    }
    RefreshElementTypes();
    }
...
```

As can be seen in the code above, our <code>Action</code> delegate is given a path.
In this example, the path points to <code>this.RefreshCustomElements();</code>, which is used to refresh all of the custom elements. By using the <code>Action</code> delegate, we pass this method to all instances of <code>CustomElementRelationPage</code>.

After declaring our <code>Action</code> delegate, and assigning it content (in this case <code>this.RefreshCustomElements();</code>) we must invoke <code>NeedRefresh</code>

<div class="code-name">CustomElementRelationPage.json.cs</div>
```cs
void AddNewItem(Input.SelectedType input)
{
    string inputValue = input.Value;
    if (inputValue != null && inputValue != "")
    {
        if(CheckAvailability(input.Value))
        {
            Db.Transact(delegate
            {
                CustomElementRelationType type = new CustomElementRelationType();
                type.Name = inputValue;
            });
            NeedRefresh(); // Calls the Action delegate
        }
    }
}
...
```

By invoking the <code>Action</code> delegate, it redirects the invocation to the content we assigned to it in the previous code snippet.

The example above shows how the <code>CustomElementRelationPage.json.cs</code> can invoke <code>RefreshCustomElements();</code> from within <code>PersonPage.json.cs</code> by using the <code>Action</code> delegate. This invocation would otherwise be inaccessible from the scope of <code>CustomElementRelationPage.json.cs</code>


## Read more

[MSDN Delegates](https://msdn.microsoft.com/en-us/library/ms173171.aspx)
[MSDN Action Delegate](https://msdn.microsoft.com/en-us/library/system.action(v=vs.110).aspx)


## See the full example

[github.com/StarcounterPrefabs/People](https://github.com/StarcounterSamples/People)
