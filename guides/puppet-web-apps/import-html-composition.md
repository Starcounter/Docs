# Import HTML composition

When you want to deploy your custom HTML composition, you have two options:

<ol>
<li>Use the GUI in Launcher (explained in the tutorial)</li>
<li>Use the REST API for importing HTML compositions</li>
</ol>

<h1>REST API for importing HTML compositions</h1>

The REST API allows importing compositions for sets of blended HTML partials.

<h2>Setup</h2>

To enable the API, run a Starcounter app that calls <code>Starcounter.HTMLComposition.Register();</code>. Launcher calls it, so you might as well start Launcher.

<h2>Importing composition</h2>

Place your HTML composition in a file like this one of Hello World:

<div class="code-name"><a href="https://github.com/StarcounterSamples/HelloWorld/blob/tutorial-20161018/src/HelloWorldMapper/ExpenseJsonComposition.html">ExpenseJsonComposition.html</a></div>

<pre><code class="html">&lt;content select="[slot='HelloWorld/description']"&gt;&lt;/content&gt;
&lt;content select="[slot='HelloWorld/amount']"&gt;&lt;/content&gt;
&lt;div style="display: none"&gt;
&lt;content select="[slot='Images/label']"&gt;&lt;/content&gt;
&lt;/div&gt;
&lt;div style="width: 200px; height: 200px"&gt;
&lt;content select="[slot='Images/control']"&gt;&lt;/content&gt;
&lt;/div&gt;
</code></pre>

This code merges the different elements from <code>Images</code> and <code>HelloWorld</code>. It also resizes the images and hides the image labels to make it align better with the rest of the page. This can obviously be edited by you to create the exact look that you want.

Go to the command prompt where you installed Git Bash, navigate to your <code>HelloWorld</code> solution and run:

<pre><code class="txt">curl -XPOST --data-binary '@src/HelloWorldMapper/ExpenseJsonComposition.html' 'http://localhost:8080/sc/partial/composition?key=%2Fsc%2Fhtmlmerger%3FHelloWorld%3D%2FHelloWorld%2FExpenseJson.html
%26Images%3D%2FImages%2Fviewmodels%2FConceptPage.html&amp;ver='
</code></pre>

If you get a 404 Not Found error, it means that the API was not initialized. See setup instructions on the top of the page.

<h3>GET and DELETE HTML Compositions</h3>

In addition to using POST, we can also use the GET and DELETE methods contained in the REST API.

To GET the HTML composition that's posted we can use cURL like we did with our POST command. Here's how a cURL command to get the HTML composition above would look:

<pre><code class="txt">curl 'http://localhost:8080/sc/partial/composition?key=%2Fsc%2Fhtmlmerger%3FHelloWorld%3D%2FHelloWorld%2FExpenseJson.html
%26Images%3D%2FImages%2Fviewmodels%2FConceptPage.html&amp;ver='
</code></pre>

If we decode the command above, we get the following result:

<pre><code class="txt">curl ‘http://localhost:8080/sc/partial/composition?key=/sc/htmlmerger?HelloWorld=/HelloWorld/ExpenseJson.html&amp;Images=/Images/viewmodels/ConceptPage.html&amp;ver='
</code></pre>

Here we see clearly what it does. There's a key that marks what we will do (htmlmerger) and then it directs to the two files that we will merge. In this case the <code>ExpenseJson.html</code> and the <code>ConceptPage.html</code> file. From this, you can build your own keys to use in your programs. If you want to know more about how to practically do this, you can take a look in the Launcher provided <a href="https://github.com/StarcounterPrefabs/Launcher">here</a> where we use the graphical interface explained in the Hello World tutorial to compose the HTML of two different applications.

And here's the output:

![curl output](/assets/getCurl.png)

If the key and/or the version is invalid, then you would get a 404 back. Otherwise, the response status will be 200.

To delete, we only need to slightly modify the command above by inserting <code>-X DELETE</code> before the URI. It should look something like this:

<pre><code class="txt">curl -X DELETE 'http://localhost:8080/sc/partial/composition?key=%2Fsc%2Fhtmlmerger%3FHelloWorld%3D%2FHelloWorld%2FExpenseJson.html
%26Images%3D%2FImages%2Fviewmodels%2FConceptPage.html&amp;ver='
</code></pre>

This method will always return the status code 204. To delete all your HTML compositions you can simply insert <code>all</code> for the key.

You can check whether these work as you intend by looking in the database through the Starcounter Administrator where you can access all you HTML compositions by running the SQL command <code>SELECT i FROM Starcounter.HTMLComposition i</code>.