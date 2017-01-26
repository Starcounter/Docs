# Ontology mapping

The main purpose of ontology mapping is to provide a mechanism to combine work produced by several independent applications without direct interaction between those applications. Certain URI handlers can be associated with concepts in the ontology hierarchy (class inheritance), for example, a handler from People application "/people/partials/persons/{?}" represents a certain person.

Ontology mapping allows mapping URI handlers of independent applications to shared classes in the given class hierarchy. As with UriMapping, each application sees only its own responses. Combined result from several applications is composed from merged responses from all applications during serialization to the client.



Here is the description of the `UriMapping.OntologyMap` function:

```cs
void OntologyMap(
    String appProcessedUri,
    String mappedClassInfo,
    Func<String, String> converterToClass,
    Func<String, String> converterFromClass)

```
where
* `appProcessedUri` - a URI to a GET handler that ends with a string parameter, for example, `/people/partials/organizations/{?}`
* `mappedClassInfo` - fully namespaced class name to which `appProcessedUri` handler is supposed to be mapped.
* `converterToClass` - `null` or converter to the mapped class object.
* `converterToClass` - `null` or converter from the mapped class object.

(NOTE: Curly braces syntax in mapping is available in Starcounter starting from version `2.1.878`.)

Here is an example of mapping People application's handler `/people/partials/persons/{?}` to class Person in a given class hierarchy to which this class Person belongs:

```cs
UriMapping.OntologyMap("/people/partials/persons/{?}", typeof(Person).FullName, null, null);
```

and here is how the mapped handler looks like:

```cs
Handle.GET<string>("/people/partials/persons/{?}", (string id) =>
{
  return Db.Scope<PersonPage>(() =>
  {
    PersonPage page = new PersonPage()
    {
      Html = "/People/viewmodels/PersonPage.html"
    };
    page.RefreshPerson(id);
    return page;
  });
});
```

### Traversing the ontology tree

By mapping a handler to a class in a given class hierarchy we create an association between them. When a URI handler mapped to ontology is called, the ontology tree (class hierarchy based on inheritance) is traversed up, from child to parent, starting from the node on which the mapped handler was called. On each level of the tree, mapped URI handlers are called and results from that calls are combined. The execution up goes until the root of the ontology tree is reached (i.e. class that has no parents).

### Example

For example, we might have the following abstract ontology tree of classes: Something -> Animal -> Dog. Where "Dog" inherits from "Animal", and "Animal" inherits from "Something". "Something" is a root of ontology tree in this example. If a handler mapped on "Dog" class is called, then all handlers mapped to this class are also called. Then all handlers mapped on class "Animal" and eventually, on class "Something". As with ordinary URI mapping, each application will see the responses from its own handlers only. Internally responses are combined as well from other applications, and they are merged and serialized when the response is returned to the client.
