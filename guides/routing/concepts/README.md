# Routing, Middleware and Context - Concepts

### Page

Page is a class that is responsible for handling a request. It's usually a Json view-model (note that `Json` class defines an implicit conversion to `Response`). It should be annotated with `UrlAttribute` to associate it with a specific URL:

```cs
[Url("/ChecklistDesigner/checklists/{?}")]
partial class ChecklistPage : IBound<Checklist>
```

### RoutingInfo

Object of `RoutingInfo` class contains all the necessary information about the current request:
* `SelectedPageType` (`Type`) - the type associated with this request. Middleware can use this to decide weather it should act on the request or not (e.g. check if the type is annotated with specific attribute). The pageCreator should use this to construct the actual Page
* `Request` (`Request`) - the original Starcounter request. Can be used to retrieve headers or raw request body
* `Arguments` (`string[]`) - the array of URL arguments (that go into "{?}" slots). Usually used by `ContextMiddleware` to create the Context
* `Context` (`object`) - it has separate section later. Remains null until some middleware (usually `ContextMiddleware`) sets it. 

### Router

Router is a mechanism (in form of a class `Router`) that is responsible for accepting HTTP requests, choosing appropriate Page type for them and feeding the requests into the Page to retrieve response. Usually router needs to be configured by its constructor:

```cs
public Router(Func<RoutingInfo, Response> pageCreator)
```

`pageCreator` is a function that should create a response and sets the Context in it if necessary. Usually it just instantiates an object of `RoutingInfo.SelectedPageType` and calls `PageContextSupport.HandleContext`. If you create the router via `Router.CreateDefault()` it will always create the page using the default constructor and call `PageContextSupport.HandleContext`. You can override this method to create the Pages differently, e.g. using Dependency Injection

### Middleware

Middleware is a mechanism to transform the request or the response before or after the page creation. Middleware has to implement `IPageMiddleware` interface and has to be registered with Router with `Router.AddMiddleware`. `IPageMiddleware` defines one method:
```cs
Response Run(RoutingInfo routingInfo, Func<Response> next);
```

It is run on every request handled by the Router. The `next` function runs pageCreator and all the middleware that were registered after this one and returns the resulting response. If you want to add behavior before handling the response, just add it before call to next. To add behavior after handling the response, add it after the call to next:

```cs
Response Run(RoutingInfo routingInfo, Func<Response> next)
{
  Logger.LogRequestStart(routingInfo);
  Response originalResponse = next();
  Logger.LogRequestEnd(routingInfo);
  return originalResponse;
}
```

If you want to prevent the original handling from happening, just don't call next and return an alternative response:

```cs
Response Run(RoutingInfo routingInfo, Func<Response> next)
{
  if(IsAuthorized(routingInfo)))
  {
    return next();
  }
  else
  {
    return Response.FromStatusCode(403);
  }
}
```

Middleware usually decide weather (and how) to act on the request basing on the `RoutingInfo.SelectedPageType` and its associated attributes.

### Context

Context is the data represented in URL arguments. Page `PersonPage` associated with URL `/people/person/{id}` would probably have Context of type `Person` (and would probably be `IBound<Person>`). If the type of the Page is `IBound<T>`, the Context type will be inferred to `T`. You can also explicitly specify the Context type by implementing the `IPageContext<T>` interface.

Context was introduced to let the middleware reason about the object(s) associated with the Page before the Page is constructed. If the Page is `IBound<T>` we can set its `Data` property to T and then act on it (e.g. decide weather the user is authorized to interact with the Page). But before the page is constructed (when majority of Middleware is run) the Context can't be retrieved from the Page object, since there is no Page object yet. That's why the `Context` property of the `RoutingInfo` has been introduced.

It's usually the best to register the `ContextMiddleware`, since it will automatically set the `RoutingInfo.Context` property if the Context type is a database type (which is almost always true). If you want to set the Context your way, just create and register your own Middleware that does it.

If you want to customise the the Context type, way that is created or handled for your page, follow this example:

```cs
[Url("/items/{0}/{1}")]
public partial class ItemPage: IPageContext<Tuple<Item, SubItem>>, IBound<SubItem>
//                             ^^-- this way you override the Context type, which would be inferred to SubItem
{
    private Item _item;
    
    [UriToContext]
    // this attribute marks the method that will be used to create the Context
    public static Tuple<Item, SubItem> UriToContext(string[] args)
    // its return type matches the Context type. It accepts arguments from URL
    {
        return Tuple.Create(Items.ById(args[0]), SubItems.ById(args[1]));
    }
    
    public void HandleContext(Tuple<Item, SubItem> context)
    // this method is defined in the IPageContext interface. It is invoked after the Page is created
    {
        this._item = context.Item;
        this.Data = context.SubItem;
    }
}
```

Middleware that are registered after the `ContextMiddleware` (or any other that sets the Context) can use Context to reason about the request. For example the `SecurityMiddleware` uses Context to check if the user is authorized to access the page.

If you specify you own pageCreator (the Func that you pass to `Router` constructor) you should also handle the Context somehow. This usually means setting `Data` property for `IBound` pages and if you don't need your own custom behavior you can just call `PageContextSupport.HandleContext` to do that for you.