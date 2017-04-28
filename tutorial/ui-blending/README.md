# UI Blending

This is where we tie everything together and get our final application.

First, download the `CompositionEditor` and `CompositionProvider` from the App Warehouse, the same way we did with the Images app in the previous step. 

You can now, if you haven't already, fire up the applications <code>HelloWorld</code>, <code>HelloWorldMapper</code>, <code>Images</code>, `CompositionEditor`, and `CompositionProvider`.

Open <code>HelloWorld</code> in the Starcounter Administrator and press <kbd>Ctrl</kbd> + <kbd>E</kbd>. Double click on the "css selector for partial to edit" input field and choose the alternative <code>[partial-id="/sc/htmlmerger?HelloWorld=/HelloWorld/ExpenseJson.html&Images=/Images/viewmodels/ConceptPage.html"]</code>.

What this does is that it uses htmlmerger, a class within Starcounter, to merge the files <code>ExpenseJson.html</code> and <code>ConceptPage.html</code> in HelloWorld and Images respectively.

Now, when we have described what files to merge we also want to describe how they should be merged. We do that in the input field below with the following code:
```html
<slot name='HelloWorld/description'></slot>
<slot name='HelloWorld/amount'></slot>
<div style="display: none">
    <slot name='Images/label'></slot>
</div>
<div style="width: 200px; height: 200px">
    <slot name='Images/control'></slot>
</div>
```

This code describes that we don't want to display the label for the images and that we adjust the width and height of them. It also orders the different elements in the way we want. Feel free to play around with these and see how it changes the layout on your page. 

<aside class="read-more">
   <a href="http://starcounter.io/guides/web/import-html-compositions/">Read about alternative HTML composition</a>
</aside>

Close the layout editor with <kbd>ctrl</kbd> + <kbd>E</kbd> should now see the following page:

![final tutorial image](/assets/Capture-2.png)

Well done! You have now created one of the most advanced "Hello World" applications and learned how to use Starcounter to build fast and modular applications. 

If you get any errors, you can check your code against the [source code](https://github.com/StarcounterApps/HelloWorld/commit/c85ff3123a80476fd65c35f9e949b79118e984d8).

<section class="hero"><strong>Disclaimer</strong>
The APIs used in this step are experimental. We consider blending of different application's user interfaces to be an essential feature. We are working hard to develop exceptional blending that might require the steps outlined here to be changed. Feel free to follow our <a href="http://starcounter.io/blog/">blog</a> to get information about these changes and more.</section>