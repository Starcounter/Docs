# Network

Starcounter communicates with standard web protocols such as HTTP and WebSocket, TCP and UDP. Starcounter is very efficient at handling requests using standard high level web standards such as REST and JSON.

While REST is a standard option for web apps, Starcounter has also built-in support for creating stateful web apps using JSON-Patch.

Starcounter apps can make `Self` requests, which can be used for internal signalling or composing responses from smaller bits and pieces.

<div class="part-box">
  <h2 class="toc-headline">Articles in the {{ page. title}} section</h2>
  {% for item in summary.parts[0].articles[2].articles[6].articles %}
    <a href="../../{{ item.path}}"><p class="toc-text">{{ item.title }}</p></a>
  {% endfor %}
</div>
