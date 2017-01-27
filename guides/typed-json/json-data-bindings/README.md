# JSON data binding

<b style="color: red;">Note:</b> the dollar sign (`$`) in the JSON definition has been deprecated.

There are two exceptions:

- It is still and will be used to mark editable properties.
- Due to technical restrictions it is the only way to use the [Reuse](#Reuse-in-version-2.3.0.4343+) keyword.

Properties declared in JSON (the view-model) can be bound to either a property in the code-behind file or a CLR object that exposes one or more public properties. Properties that are bound will read and write the values directly to the underlying object with no need to manually transfer the values to the view-model.

### Default behaviour

By default each property will try to bind to a property in code-behind or a data object. If the binding fail, for example a property with the same name does not exist or no data object is specified, the property will be treated as unbound.

In the following example the binding will succeed since the property `Name` exists in the database object:

<div class="code-name">PersonJson.json</div>

```javascript
{
    "Name": "John"
}
```

<div class="code-name">Program.cs</div>

```cs
[Database]
public class Person
{
	public string Name;
}

public class Program
{
  public static void Main()
  {
    Handle.GET("/person/any", () =>
    {
      return new PersonJson()
      {
        Data = Db.SQL<Person>("SELECT p FROM Person p").First;
      });
    }
  }
}
```
Here, the property `Name` in the JSON `PersonJson` will be bound to the property `Name` in the database object `Person`  

Assuming there is one person in the database with the name "Christian", the resulting JSON from the request will be:

```javascript
{ "Name": "Christian" }
```

### Different type of bindings

There are different settings that can be specified on each property or JSON object. Auto, Bound, Unbound or Parent.

**Auto**
When the binding is specified as auto (which is the default setting) each property is matched against the code-behind and the data object (if any). If a code-property with the same name or with a name specified as binding is found the JSON property is treated as bound and getting and setting values to it will get and set the value on the underlying property.

If no matching property is found, the JSON property is treated as unbound and values will be stored in the JSON.

**Bound**
When a JSON property is specified as bound a compiler error or runtime error, depending on if the type to bind to is specified, is thrown. A JSON property declared as Bound **must** match a property in the code-behind or data object.

**Unbound**
An unbound JSON property will store the value in the JSON.

**Parent**
The same setting as specified on a parent is used, which will be one of the above (auto, bound, unbound).

### Setting type of binding for all children
JSON objects that can contain children (with a template of type `Starcounter.Templates.TObject`) can also specify how the bindings on the children will be treated.

By setting the property `BindChildren`, each child that don't specify it's own binding, i.e. use the `Parent` option will have the binding decided by the parent.

Setting the value in code-behind from `BindingStrategy` enumerable:

```cs
...
PersonJson.DefaultTemplate.BindChildren = BindingStrategy.Bound
...
```

The enumerable has the following values: "Auto", "Bound", and "Unbound".

**NOTE:** It is not allowed to set the value for this property to `BindingStrategy.UseParent`. An exception will be raised in this case.

### Specify data type

Denoting a specific type that is used as data-object is optional but comes with some benefits.

If the JSON object is static, that is all properties are known compile-time, you will get an error during compilation if bound properties are invalid, instead of getting a runtime error on first use. The Data property itself will also be typed to the correct type which means that you don't need to cast the data-object when using it.

The JSON code-behind class has to implement `IBound<T>` to set custom data type.

```cs
[PersonJson_json]
public partial class PersonJson : Json, IBound<MyNamespace.Person>
```

**Note:** the empty JSON objects like `{ }` do not have code-behind classes therefore it is not possible to declare custom data type.

### JSON property binding

All examples in this section use the same `Person` database class.

```cs
[Database]
public class Person
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public Person Father { get; set; }
}
```

#### Binding to a different property

If a property should be bound to a property that has a different name than the property in the JSON, a binding value can be set.

```js
{
  "Name": "John",
  "Surname": "Walker",
  "FirstName": "",
  "LastName": ""
}
```

```cs
public class PersonJson : Json, IBound<Person>
{
    static PersonJson()
    {
        PersonJson.DefaultTemplate.FirstName.Bind = "Name";
        PersonJson.DefaultTemplate.LastName.Bind = "Surname";
    }
}
```

#### Binding to a deep property

It is also possible to bind to deep properties by providing full path to the property.

```js
{
    "Name": "John",
    "Surname": "Walker",
    "FatherName": ""
}
```

```cs
DefaultTemplate.FatherName.Bind = "Father.Name";
```

#### Binding to a custom property in code-behind

Binding to custom properties in code-behind works the same way.

```js
{
    "Name": "John",
    "Surname": "Walker",
    "FullName": ""
}
```

```cs
public class PersonJson : Json, IBound<Person>
{
    static PersonJson()
    {
        PersonJson.DefaultTemplate.FullName.Bind = "FullNameBind";
    }

    public string FullNameBind
    {
        get { return string.Format("{0} - {1}", this.Name, this.Surname); }
    }
}
```

**NOTE**:

As a simplification when binding properties to code-behind it is also possible to use the same name of the member declared in Json-by-example and code-behind. Then the custom property in code-behind will be bound automatically (same behaviour as autobinding to a property in a data-object).

The following restrictions applies though:

- Due to restrictions in available type information when generating code for TypedJSON, this feature only works for properties that are declared on the same class. It does not work for properties declared in an inherited class.
- How it works underneath is that generating an accessor-property for getting and setting the JSON-property is suppressed when a property in code-behind already exists. This means that to access the JSON-property directly, either the template needs to be used, or the name (`json.GetValue(json.Template.Name)`, `json["Name"]`).

In the following example `FullName` in json will be automatically bound to the property `FullName` in code-behind:

```js
{
    "Name": "John",
    "Surname": "Walker",
    "FullName": ""
}
```

```cs
public class PersonJson : Json, IBound<Person>
{
    public string FullName
    {
        get { return string.Format("{0} - {1}", this.Name, this.Surname); }
    }
}
```



### Opt-out of binding

In some cases we want to make sure that a specific property is not bound. This can be achieved by either setting the value of `Bind` to `null` or specifying the property `BindingStrategy` on the specific template as `BindingStrategy.Unbound`

Using the same example as the section above but change `Street` to always be unbound would look like this

Code
```cs
...
PersonJson.DefaultTemplate.Street.Bind = null;
// or same behaviour setting BindingStrategy
PersonJson.DefaultTemplate.Street.BindingStrategy = BindingStrategy.Unbound;
...
```

### Reuse same JSON object

The `Reuse` keyword is used to reuse same JSON object multiple times. For example there is a page with list of entities and a page with an entity details. The entity itself is the same for both pages. It sounds reasonable to use same JSON object in both cases. The following code snippet demonstrates how to achieve that.

##### EntityJson.json

```json
{
    "Key": "",
    "Name": "",
    "Description": ""
}
```

##### ListPage.json

```json
{
    "Items": [{
        "$": { "Reuse": "AppNamespace.EntityJson" }
    }]
}
```

##### DetailsPage.json

```json
{
    "Entity": {
        "$": { "Reuse": "AppNamespace.EntityJson.cs" }
    }
}
```

**Note:** in case of `Reuse` you cannot specify custom code-behind class.

```cs
[ListPage_json.Items]
partial class ListPageItem : Json
{
    //This is wrong since the EntityJson.cs class already exists.
}
```

### Rules when bindings are created
1. If a code-behind file exists, a property is searched for there.
2. If a property was not found in the code-behind or no code-behind exists, a property in the data object is searched for.
3. If no property was found in steps 1 and 2 and the binding is set to `Auto`, the property will be unbound. If binding was set to `Bound` an exception will be raised.