# Callback methods of Starcounter JS

Typically there are two kind of events, happening in typed JSON files.
There might be events coming from the client side and events, happening on the server side of the app.
In this article we will deepen into the methods of Starcounter JS triggered by the actions on the server side.

<h2>OnData</h2>

Typed JSON objects have properties, which exist as a playground for your actions. 
Therefore, once you update a <code>DataType</code> mandatory property internally on Typed JSON object their own <code>OnData</code> method will be triggered, indicating property initialization / update. Moreover when a new data object is set, <code>OnData</code> method can implement the update for other linked properties in the <a href="http://starcounter.io/guides/json/code-behind/">code-behind file</a> of the view-model.

The result of <code>OnData</code> functionality is that after connecting a database object to the view-model method will refresh the view every time you set the new property. No more copying and setting values through added functionality. Use case of the method can be studied from our explicit Tutorial.

<h2>HasChanged</h2>

And the same goes for <code>HasChanged</code> - that method is always called when there is a change on a server side. But this time it indicates a change of a value in JSON root class.

<blockquote>To capture a change in a child subtree, it is efficient to define another partial class and use HasChanged method there.</blockquote>

This method implemented in the same way as <code>OnData</code> - all the declaration is happening in the <a href="http://starcounter.io/guides/json/code-behind/">code-behind file</a>. 
Unlike <code>OnData</code> the method <code>HasChanged</code> is not that commonly used but only when there is a need for auto-committed database transactions every time data updates.
There is a quick example on <code>HasChanged</code> usage:

<pre><code class="cs">using Starcounter;
using Starcounter.Templates;

namespace ModelChangeEventTestProject
{
    partial class Page2 : Page
    {
        protected override void HasChanged(TValue property)
        {
            base.HasChanged(property);
        }
    }

    [Page2_json.Property2]
    partial class Page2Property2 : Page {
        protected override void HasChanged(TValue property) {
            base.HasChanged(property);
        }
    }
}  
</code></pre>

Just to sum up methods purposes:

<ul>
<li><code>OnData</code> - triggered when the data property is changed. </li>
<li><code>Haschaned</code> - triggered when the value is changed. </li>
</ul>