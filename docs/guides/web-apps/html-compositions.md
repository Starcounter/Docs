# HTML Compositions

## Introduction

When you want to deploy your custom HTML composition, you have two options:

* Use the REST API for HTML compositions
* Use the [CompositionEditor](https://github.com/Starcounter/compositioneditor)

## REST API for HTML Compositions

{% hint style="warning" %}
The HTML composition REST API is deprecated from Starcounter 2.3.1. Compositions can still be accessed through the class `HTMLComposition`
{% endhint %}

The REST API allows importing compositions for sets of blended HTML partials. The CompositionEditor has to run for this API to work.

{% api-method method="get" host="http://localhost:8080" path="/sc/partial/composition?key={?}&ver={?}" %}
{% api-method-summary %}
Composition
{% endapi-method-summary %}

{% api-method-description %}
Returns the HTML of a specific composition
{% endapi-method-description %}

{% api-method-spec %}
{% api-method-request %}
{% api-method-query-parameters %}
{% api-method-parameter name="key" type="string" required=true %}
The key of the composition.
{% endapi-method-parameter %}

{% api-method-parameter name="ver" type="string" required=false %}
The version of the composition.
{% endapi-method-parameter %}
{% endapi-method-query-parameters %}
{% endapi-method-request %}

{% api-method-response %}
{% api-method-response-example httpCode=200 %}
{% api-method-response-example-description %}
The HTML of the requested composition
{% endapi-method-response-example-description %}

```javascript
<style>:host{display:block}</style>
<style>
    .kitchensink-layout {
        display: flex;
    }

    .kitchensink-layout__column-left {
        flex: 0 0 160px;
        margin-right: 20px;
    }

    .kitchensink-layout__column-right {
        flex: 0 1 600px;
    }
</style>
<div class="kitchensink-layout">
    <nav class="kitchensink-layout__column-left">
        <slot name="kitchensink/nav"></slot>
    </nav>
    <div class="kitchensink-layout__column-right">
        <slot name="kitchensink/current"></slot>
    </div>
</div>
```
{% endapi-method-response-example %}
{% endapi-method-response %}
{% endapi-method-spec %}
{% endapi-method %}

{% api-method method="post" host="http://localhost:8080" path="/sc/partial/composition?key={?}&ver={?}" %}
{% api-method-summary %}
Composition
{% endapi-method-summary %}

{% api-method-description %}
Creates a new composition at the specified key if there's no composition there. The body consists of the composition HTML
{% endapi-method-description %}

{% api-method-spec %}
{% api-method-request %}
{% api-method-query-parameters %}
{% api-method-parameter name="key" type="string" required=true %}
The key of the composition.
{% endapi-method-parameter %}

{% api-method-parameter name="ver" type="string" required=false %}
The version of the composition.
{% endapi-method-parameter %}
{% endapi-method-query-parameters %}
{% endapi-method-request %}

{% api-method-response %}
{% api-method-response-example httpCode=204 %}
{% api-method-response-example-description %}

{% endapi-method-response-example-description %}

```javascript

```
{% endapi-method-response-example %}
{% endapi-method-response %}
{% endapi-method-spec %}
{% endapi-method %}

{% api-method method="delete" host="http://localhost:8080" path="/sc/partial/composition?key={?}&ver={?}" %}
{% api-method-summary %}
Composition
{% endapi-method-summary %}

{% api-method-description %}
Deletes the composition with the specific key and version
{% endapi-method-description %}

{% api-method-spec %}
{% api-method-request %}
{% api-method-query-parameters %}
{% api-method-parameter name="key" type="string" required=true %}
The key of the composition. If the key is "all", all composition are deleted.
{% endapi-method-parameter %}

{% api-method-parameter name="ver" type="string" required=false %}
The version of the composition.
{% endapi-method-parameter %}
{% endapi-method-query-parameters %}
{% endapi-method-request %}

{% api-method-response %}
{% api-method-response-example httpCode=204 %}
{% api-method-response-example-description %}

{% endapi-method-response-example-description %}

```javascript

```
{% endapi-method-response-example %}
{% endapi-method-response %}
{% endapi-method-spec %}
{% endapi-method %}

