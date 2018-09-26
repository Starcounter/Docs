# Localizing Starcounter Apps

Real world applications often need to support a variety of languages. This article describes how to add such support to your application. It doesn't describe any practices for translation of texts or internals of how localization support works in Starcounter.

## Getting started

1. Create a resource file with the strings to localize.
   1. In Visual Studio, open "Add new item" menu and choose "Resources File".
   1. Select any name for your file. We'll use `Strings.resx` in this example.
   1. Add as many resource files as many languages you want to support. Use the following format when naming them: `name.ll-cc.resx` where `ll` is the language code and `cc` is the country code. To continue our example, you would add `Strings.sv-se.resx` to support Swedish language as used in Sweden.
1. Populate your resource files with texts you want to localize.
   1. The `name` column should contain a valid C# indentifier, like `SignIn`. Value should be the relevant text in language chosen for this file. For example, it could be "Logga in" in file `Strings.sv-se.resx`. The comments column can be used freely.
1. Visual Studio will generate a class from your resource file. In our example, it will generate `Strings` class with a string member `SignIn`.
   1. The value of this property will depend on currently selected culture. You don't have to worry about setting it in your application, it's done by a special language selection application.
   1. In your application, instead of using hard-coded strings use members of this class. For example, here is an example of view-model using a generated resource class:

   ```c#
   public partial class SignInViewModel: Json
   {
       public string Message {get; set;}
       public string Username {get; set;}
       public string Password {get; set;}

       public void Handle(Input.SignInTrigger input)
       {
           if(!_signInService.SignIn(Username, Password))
           {
               Message = Strings.WrongUsernameOrPassword;
               return;
           }
           // ...
       }
   }
   ```

## Using placeholders

Often you need to put some data in the texts you are localizing. For example you want to localize the text that includes the current user's name: 
```c#
Message = $"Hello, {user.FirstName}";
```

To do this, include placeholders (`{0}`, `{1}`, etc.) in your localized texts. Continuing our example, you would put the following value in `Strings.resx` for name `HelloUser`: `Hello, {0}`. To use it, use `string.Format` method:

```c#
Message = string.Format(Strings.HelloUser, user.FirstName);
```

You can introduce a simple class that wraps this formatting to make your code more readable:

```c#
public class StringsFormatted
{
    public static string HelloUser(string userName) => string.Format(Strings.HelloUser, userName);
}
```
```c#
Message = StringsFormatted.HelloUser(user.FirstName);
```

## Localizing client-side strings

To localize texts that are currently hard-coded into HTML views, you have to move them to the view-model. For example, imagine you want to localize the following view-model:

```json
{
    "Html":"/MyApplication/terms",
    "AgreeTrigger$":0
} 
```
```html
<template>
    <dom-bind>
        <template is="dom-bind">
            <p>To use our application you need to agree to the terms and conditions. Click the button below to do so</p>
            <button value="{{model.AgreeTrigger$::click}}" onmousedown="++this.value">I agree</button>
        </template>
    </dom-bind>
</template>
```

First, you need to define new properties in your view-model and use them in the view:
```json
{
    "Html":"/MyApplication/terms",
    "AgreeTrigger$":0,
    "YouNeedToAgree":"",
    "IAgree":""
} 
```
```html
<template>
    <dom-bind>
        <template is="dom-bind">
            <p>{{model.YouNeedToAgree}}</p>
            <button value="{{model.AgreeTrigger$::click}}" onmousedown="++this.value">{{model.IAgree}}</button>
        </template>
    </dom-bind>
</template>
```

Then, you need to add `YouNeedToAgree` and `IAgree` to your `Strings.resx` file and all of its versions.
Lastly, you need to pass the values to the client side in your code-behind:

```c#
public partial class TermsViewModel: Json
{
    public string YouNeedToAgree => Strings.YouNeedToAgree;
    public string IAgree => Strings.IAgree;

    // ...
}
```

Now, your view-model is localized.
