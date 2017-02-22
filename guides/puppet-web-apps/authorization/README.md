# Authorization Library

This library helps the developer to prevent the user from accessing / acting on data he doesn't have privileges to.

## Foundation

### Permissions

Permission is a core concept in Authorization. A Permission represents specific action that we can allow the user to perform. This could be "listing all users" or "displaying details of user Joozek" or "changing the details of user Joozek". The granularity of Permissions (you could have just one "changing the details of user X" or specific Permission for each property of the user) is up to the developer and it's recommended to limit the scope of permissions to an object (so "change user X" is probably better then "change any user" and "change last name of user X").

Whenever a user will perform an action (open the page, click a button) a Permission will be checked to verify if he is allowed to do it. Grouping Permissions together ("show list of users or companies") is discouraged, because it's easy to group them later in the configuration (AuthorizationRules).

Let's define some example database classes that will be useful in this guide:

```cs
[Database]
public class Invoice : Something
{
    public bool IsSettled;
}

[Database]
public class InvoiceRow : Something
{
    public string Product;
    public decimal Price;
}

[Database]
public class InvoiceInvoiceRow : Relation
{
    [SynonymousTo(nameof(From))]
    public Invoice Invoice;
    [SynonymousTo(nameof(To))]
    public InvoiceRow InvoiceRow;
}
```

And now, let's define permissions that we'll use to describe the authorization rules later:

```cs
public class ListInvoices: Permission
{
}

public class DisplayInvoice: Permission
{
    public Invoice Invoice {get; private set;}
    public DisplayInvoice(Invoice invoice)
{

this.Invoice = invoice;
}
}
```

Permissions can be anything as long as they derive from `Permission`, but some parts of this library work best if their constructors accept only serializables and/or database types.

### Define authorization rules

Now let's define rules that will decide weather the user will be granted the permissions or not.

```cs
var rules = new AuthorizationRules();
rules.AddRule(new ClaimRule<ListInvoices, SystemUserClaim>((claim, permission) => claim.SystemUser.Name == "admin"));
```

Rules implement `IAuthorizationRule<TPermission>` interface, which defines one method:

```cs
public bool Evaluate(
IEnumerable<Claim> claims, // each Claim represents a fact about the current user. Most popular ones are
// SystemUserClaim and PersonClaim
IAuthorizationEnforcement authorizationEnforcement, // this can be used to check if the current user has other permissions
TPermission permission) // the actual permission to check
```

`SystemUserClaim` checks weather a claim of specified type (SystemUserClaim) exists and matches the provided predicate. Here, we effectively check if current user has a name "admin".

Claims are a way obtain information about the current user without direct dependency on authentication system. There are two built in claims:
* SystemUserClaim - if present it means that a current user is signed in as a specific SystemUser
* PersonClaim - if present it means that a current user represents specific person. It is preferred mean of establishing user's identity, since it's usually more convenient to manage People than Users

There are other built-in rules in `Starcounter.Authorization.Core.Rules` namespace. Feel free to use them or create your own.

### Check the rules

`AuthorizationEnforcement` is a class responsible for checking the if the current user has a specific permission with regard to a rule set. Let's create it using the rules we defined in previous steps:

```cs
var enforcement = AuthorizationEnforcement(rules, new SystemUserAuthentication());
```

The second argument passed is a authentication backend (a something that obtains the claims about a current user). It's usually enough to use the built in `SystemUserAuthentication`. It will provide `SystemUserClaim` and `PersonClaim` for singed in users.

We can now use our new `IAuthorizationEnforcement` to check if we have a permission:

```cs
bool canWe = enforcement.CheckPermission(new ListInvoices()); // note that the outcome depends on weather we are currently signed in as "admin"
```

## Automatic checking of permissions - SecurityMiddleware

Manual checking of permissions is powerful, but usually we need something that will check the permissions for us. That's what SecurityMiddleware is doing - it's preventing users from accessing pages they don't have access to and from making interactions they are not allowed to do.

### Prerequisite: Router

`SecurityMiddleware` is a feature that depends on Router. If you don't know what it is - go ahead and check it out // `TODO` LINK

To enable `SecurityMiddleware` add it to a Router:

```cs
router.AddMiddleware(new SecurityMiddleware(
enforcement,
info => Response.FromStatusCode(403), // a function returning a Response to give instead of a restricted Page
PageSecurity.CreateThrowingDeniedHandler<Exception>())); // defines the behavior in case of forbidden interaction
```
`TODO` describe checkDeniedHandler

### Restricting access to a page

Primary mean of managing access to pages will be the `RequiredPermissionAttribute`. Let's use it to mark secure our example page:

```cs
[Url("/invoices/invoices")]
[RequirePermission(typeof(ListInvoices))]
public partial class InvoicesPage : Json
{
// ...
}
```

After adding this attribute, every time a user will open the URL "/invoices/invoices", the Authorization library will check weather he has is granted a ListInvoices Permission. If he is not, then he'll be presented with 403 Forbidden page (or any other response configured in `SecurityMiddleware` constructor). What's more, every time a user will try to change a property on this page or run a handler on this page, the same permission will be checked. In case of failure, an Exception will be thrown (this also can be customised using the `SecurityMiddleware` constructor).

If you want to check a different permission before allowing a user to execute a handler, you can add the `RequiredPermissionAttribute` to the handler itself:

```cs
[Url("/invoices/invoices")]
[RequirePermission(typeof(ListInvoices))]
public partial class InvoicesPage : Json
{
    [RequirePermission(typeof(AddInvoice))]
    // this will override the required permission
    public void Handle(Input.AddInvoice)
{
// ...
}
}
```
This also works with subpages:
```cs
[Url("/invoices/invoices")]
[RequirePermission(typeof(ListInvoices))]
public partial class InvoicesPage : Json
{
    [RequirePermission(typeof(AddInvoice))]
    // this will override the required permission
    [InvoicesPage_json.SomeDialog]
    public partial class DialogItem : Json
{
// ...
}
}
```

If you want to disable permission checking in some part (subpage / handler) of your Page you can just mark it as `[RequirePermission(null)]`

```cs
[Url("/invoices/invoices")]
[RequirePermission(typeof(ListInvoices))]
public partial class InvoicesPage : Json
{
    [RequirePermission(null)]
    // this will override the required permission
    public void Handle(Input.Back)
{
// ...
}
}
```

### Contextual permissions

So far, we've only checked permissions that accept no arguments in their constructors. That's easy - the library creates the permission objects using their default constructors. Another popular case is when the permission pertains to a specific object - like this one:

```cs
public class DisplayInvoice: Permission
{
    public Invoice Invoice {get; private set;}
    public DisplayInvoice(Invoice invoice)
{
this.Invoice = invoice;
}
}
```

To check that kind of Permission automatically, the Page's Context type would need to match the Permission's constructor parameter type (`Invoice`). This usually just means that the page is `IBound<Invoice>`:

```cs
[Url("/invoices/invoices/{0}")]
[RequirePermission(typeof(DisplayInvoice))]
public partial class InvoicePage : Json, IBound<Invoice>
{
}
```

In above example, upon requesting "/invoices/invoices/Abc", the following would happen:

* Context would be retrieved - in this case an Invoice object with id "Abc"
* In case no such object exists - 404 error
* A Permission would be constructed and checked - in this case `DisplayInvoice` with context passed as a constructor argument
* In case the permission is refused - 403 error

note: Remember, that in order to have the Context retrieved automatically you need the `ContextMiddleware`

The permission to be checked could also be directly specified using the `CustomCheckClassAttribute` (name subject to discussion and change)):

```cs
[Url("/invoices/invoices/{0}")]
public partial class InvoicePage : Json, IBound<Invoice>
{
    [CustomCheckClass]
    public static Permission CreatePermissionToCheck(Invoice context) => new DisplayInvoice(context);
}
```

Method marked with this attribute should be public and static, return `Permission` and accept context as its argument. Whatever permission it returns is then checked to see if the user can access the page.
