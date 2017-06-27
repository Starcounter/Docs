# Client-Side Blending

When merging responses from different apps with [server-side blending](server-side-blending), the views are stacked on top of each other. In most cases, that's not what we want. Instead, we would like to compose elements in the views to make the result look like one page. That's what client-side blending does:

![Blending comparison](/assets/BlendingComparison2.png)

## Composing Views
 
To solve this, we could edit the source code of the apps involved. This is impractical, and in some cases even impossible. The main reason is that the apps in a system, or application suite, are often created by different developers and it's not always possible to get access to their source code. A secondary reason is versatility. If an app is modified to work well with your application suite, it might not work well with other application suites.

Client-side blending lets you change the structure of blended apps without touching the source code. It does this by swapping the default composition of HTML that's sent to the client with an alternative composition. This requires a couple of things:
1. Alternative compositions should be defined with the default composition as a starting point. Otherwise, the HTML would have to be rewritten which would be inefficient, especially since alternative compositions are often similar to default compositions.
2. Alternative compositions have to be tied to a specific set of responses. For example, if you have four apps in your app suite and you swap out one of them, you wouldn't want the composition for the previous set of apps to still apply.
3. Alternative compositions have to be stored persistently. If an app is restarted, the defined compositions should still be applied.
4. Alternative compositions have to be applied when the different responses are merged and not on application start since server-side blending can change in runtime.

### 1. Defining Alternative Compositions

Alternative compositions are defined in the browser when the apps are running with the [CompositionEditor](https://github.com/StarcounterApps/CompositionProvider). It looks like this:

![Composition editor](/assets/CompositionEditor.gif)

The editor highlights the area that is changed and displays the changes in real time. There's no need to write the HTML from scratch, instead, the alternative composition is created from the default composition. 

### 2. Tying to Specific Responses

Every composition is tied to an identification. All identifications for the compositions in a page can be found by double clicking on the top input field in the CompositionEditor. They look like this:

```
[partial-id="/sc/htmlmerger?PetList=/PetList/views/PetListPage.html&MedicalRecordProvider=/MedicalRecordProvider/views/RecordSummary.html"]
[partial-id="/PetList/views/MasterPage.html"]
```

The first one is the indentification for the merged response of `PetListPage.html` from PetList and `RecordSummary.html` from MedicalRecordProvider. Keys of merged responses are always prefixed with `/sc/htmlmerger`.

The second is the composition for `MasterPage.html` from PetList. It has not been merged.

Starcounter automatically creates these identifications.    

### 3. Storing Compositions

### 4. Applying Compositions

## CompositionProvider

So what's the difference between an HTML document and a HTML composition?

HTML document is a HTML composition. The problem with "HTML Document" is that it is a formal, specific term: https://developer.mozilla.org/en-US/docs/Web/API/Document. Formally, HTML Composition is just markup of `HTMLDocumentFragment`. But this is quite to techy, specific, and not plain English to use in many contexts. Also, using just `HTMLDocumentFragment` may lead to more confusion as what I usually mean by HTML composition is "a piece of HTML markup that composes given elements in a desired layout" not an JS `object insatnceof HTMLDocumentFragment`

> Isn't a HTML composition most often composed of slot elements?

Probably it is, but it does not have to be. First of all, I think we/I use the phrase "HTML composition" to describe the piece of HTML in old-school apps that composes regular elements with all internals of those elements. Secondly, you can use `declarative-shadow-dom` or custom layout composition, with no slots at all, and it's completely fine. Also, the fact that HTML files usually contain `<div>` you don't say that "HTML is the way `<div>`s in a view are organized"

> If you start an app an open the CompositionEditor for a view, the result is always `<slot name="..."></slot>` etc.

Yes, because to blend the views together without breaking individual elements we use Shadow DOM, and let the suite authors/designers/composers/appdevs/ use all SD and HTML features, by providing HTML composition, we will put into shadow root of insertion point (the container of partial view).
I think in that case we could say we have "HTML composition to be used in Shadow Root" or "HTML Document Fragment to be used in Shadow DOM to compose apps' elements in desired layout". Therefore I might have shortened it to the phrase "Shadow DOM composition" or "layout composition" but I'm affrait of coning any specific "Terms and Definitions", as they would always be confusing,
because we would use the words extremely broad meaning that are widely used in web dev. Remember the problems with "Partial"? now we have "composition", "layout", "html", "shadow dom" which are even worse.

> if the view is not using declarative shadow DOM.

If it is using, you will most probably see some `slots` as well, but I don't think it does make any difference.


----

Maybe we could just a "piece of HTML" instead of "HTML composition" to  but this on other hand is too broad, me suggest that we mean "subset of HTML as a langauge",  or piece that is invalid on it's own `<div><p><span></span><script>` (not closed). It also does not deliver the idea that as we need to compose the visual elements on a screen, we just need to compose them accordingly in the code.
As in old-school  solutions, are are either rewriting entire HTML from scratch specifically for you needs, or, you are forced to write it in solution-specific language, or at least write it using solution specific elements and widgets. While in our solution, you are just (freely) composing (ANY) elements and document fragments together (delivered by the individual, separate  apps)