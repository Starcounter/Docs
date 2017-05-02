# Computed Properties

Starcounter allows you to use computed properties in your data model. Computing values on the fly is often as fast as accessing cached data and brings additional benefits. It allows you to save memory and always be sure that you get the current value.

Let us compute the `FullName` of a person from their `FirstName` and `LastName` and display it without any delay!

We start by simply adding the `FullName` property to our JSON.

<div class="code-name">PersonJson.json</a></div>

```json
"SaveTrigger$": 0,
"FullName": ""
```

Notice that we don't need to make `FullName` editable because we will modify it from the code-behind and not the view.

We now calculate the `FullName` by simply concatenating `FirstName` and `LastName`.

<div class="code-name">PersonJson.json.cs</div>

```cs
class PersonJson : Json
{
    public string FullName => FirstName + " " + LastName;
```

It´s that easy.

All that remains is to add `FullName` to the view. We do that using a Polymer binding to the JSON property `FullName`.

<div class="code-name">PersonJson.html</div>

{% raw %}
```html
<h1>Hey, {{model.FullName}}!</h1>
```
{% endraw %}

That was all for this part. Check out what you have achieved and we will move on to the next step.

<section class="see-yourself">
    <div>Try to type a new name into the input fields. Can you count how long it takes to update the view? I bet you cannot!</div>
</section>

![part 4 gif](/assets/part4resized.gif)

Our next step is to practice working on multiple object instances and relations by turning our app into a simple expense tracker.

If you get any errors, you can check your code against the [source code](https://github.com/StarcounterApps/HelloWorld/commit/502f3dbe9b1e76095aa2ec85ae49b5209c72da3f).
