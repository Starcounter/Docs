# Blending with Context

There are situations when you need to blend the same token with multiple URIs but you want only one of them to be invoked depending on the context. For example a network support person should have access to an employee record but it needs to be scoped to only relative information with no edit rights while a system admin can have a full editable view of employee. Now there are two different roles trying to access the same entity but in different ways, this is where Context comes into play. 

Contexts is an array of string that helps filter out the blending one step further. Two blending matches if both have the same token and context. In the previous example you can blend Employee with two different contexts:
```
Blender.MapUri<Employee>("/Employees/view/{?}", string[] { "View" });
Blender.MapUri<Employee>("/Employees/edit/{?}", string[] { "Writable" });
```
All our prefab apps use Contexts with blending. Some apps require a full view of Person from the [People](https://github.com/StarcounterApps/People) app while other only need very basic info e.g. information that can be displyed in the search result. So we have two different views mapped to the same entity but different contexts, the mapping can be achieved like:
```
Blender.MapUri<Person>("/people/partials/persons/{?}", new string[] { "page" });
Blender.MapUri<Person>("/people/partials/persons/rows/{?}", new string[] { "searchitem" });
```
Inside [Search](https://github.com/StarcounterApps/Search) app Person is blended with `searchitem` context, this will make sure that only `/people/partials/persons/rows/{?}` URI will get invoked whenever the source is Search app.
```
Blender.MapUri<Person>("/search/partials/person/{?}", new string[] { "searchitem" })
```
Blending Contexts is an optional parameter. If the value of Contexts is `null` or the parameter is not provided, only tokens will be matched. So a blending without context will pass every matching rule for blenders with the same token irrespective of their context.

### Recommendations

Blending token can be a string or a class. It is recommended to use `Type` blending when possible as opposed to string tokens because this makes it more semantic. For example `Blender.MapUri<Product>(URI)` is more well-designed approach than `Blender.MapUri(URI, "product")`.

String literals used inside contexts array can be anything but the recommended approach is to use generic names. Below are some examples:

#### Icon

Can be used to display launch icons or smaller icons in other scenarios

#### SeachItem

Item that is restricted in height. Used when user is typing search information to make a real time display of search result.

#### Page

Show me everything you've got. I'm either a small phone or a large desktop screen.

#### Banner

Pages that are very restricted in height. Will often be used in repeated sequences. I.e. a list of contact persons or a list of time-line items.
