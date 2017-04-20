# Avoiding CSS conflicts

When your system is composed of HTML responses from multiple apps, there's always a risk of naming conflicts and cascading side effects in your CSS. A solution to this is to use a convention that isolates styles coming from different apps.

We recommend BEM, a well-known convention that solves this problem.

## What is BEM?

In BEM naming convention, you **only use classes** in your stylesheets.  There are three kinds of classes at your disposal: Blocks, Elements, and Modifiers.

A **Block** is a basic class that represents a logical area of your app's UI (*a menu, login form, a search form*).

An **Element** is a smaller fragment of a Block that performs a particular function (*a link in the menu, a password input in the login form, a search icon*). The Element part of a class starts after `__`.

A **Modifier** is a variation of a Block or of an Element (*an expanded menu, an active menu item, a password input with an invalid value, a disabled search button*). The Modifier part of a class starts after `--`.

Possible combinations of Blocks, Elements and Modifiers are the following:

```css
.block {}
.block__element {}
.block--modifier {}
.block__element--modifier {}
```

When applied to an HTML structure, the above CSS class names are used in the following way:

```html
<div class="block">
  <div class="block__element"> ... </div>
  <div class="block__element--modifier"> ... </div>
</div>

<div class="block--modifier">
  <div class="block__element"> ... </div>
  <div class="block__element--modifier"> ... </div>
</div>
```

Note here that **everything at the root level must be a Block**. A Block can have multiple Element and Modifier sections and every Element and Modifer has to belong to a Block.

## Example

Consider the [SignIn app](https://github.com/starcounterapps/signin):

![BEM example](../../../assets/BEM-example.PNG)

Here, the Block is marked in red and the Element sections in blue.

From this, these BEM classes can be derived:

```css
.signin-form {}
.signin-form--expanded {}
.signin-form__text-input {}
.signin-form__labeled-checkbox {}
.signin-form__labeled-checkbox--checked {}
```

Check out the source code of [People](https://github.com/StarcounterApps/People) or [KitchenSink](https://github.com/StarcounterApps/KitchenSink). These sample apps show how to apply BEM in practice.

## BEM in Starcounter apps

We recommend the following rules when using BEM selectors in Starcounter apps.

* __Only use BEM class selectors in your stylesheets__. Do not use element selectors, id selectors, or inline styles for the purpose of styling.

* __Give meaningful names__ to the Block, Element and Modifier sections. For example, `.chatter-avatar` is much more descriptive than `.chatter-img`.

* __Use resusable names__ for the Block, Element, and Modifier sections. As seen in the example above, `.signin-form__text-input` is preferred over `.signin-form__firstname-input` since `text-input` is more resusable than `firstname-input`.

* __Prefix Block sections with the app name__ to isolate your classes from other apps. For example, the class for a menu block in the "Chatter" app should be `.chatter-menu`.

* __Use lowercase classes__. `.Chatter-Menu` is wrong, `.chatter-menu` is right.

* __Separate words with a hyphen__ when there are multiple words in a Block, Element or Modifier section. For example: `.chatter-chat-message__message-text`.

* __Block and Element must be in the same HTML template__. Otherwise, implicit couplings are created between templates which might break when the partial mapping changes. If you want to define `.chatter-menu` in a parent partial and the menu items in a nested partials, these menu items will become new blocks (`.chatter-menu-item`, not `.chatter-menu__item`).

* __Modifier classes should extend base classes__.

 When you set a `.chatter-menu__item--active` class on an element, it should not be needed to add the `.chatter-menu__item` base class.

 In your stylesheet, the definition for the base class should include all the modifiers, like this:

 ```css
.chatter-menu__item,
.chatter-menu__item--active {
    font-size: 11px;
}

 .chatter-menu__item--active {
    font-weight: bold;
}
```

* __Never nest Blocks inside Blocks and Elements inside Elements.__

 If there’s need for more nesting, it means there’s too much complexity and the elements should be stripped down into smaller Blocks.

* __Mixing BEM with Bootstrap__

 Starcounter sample apps use the CSS framework Bootstrap to create a unified look and feel.

 Even though Bootstrap does not follow BEM, there are no issues with mixing Bootstrap and BEM in a single project because there are no collisions. In fact, by just looking at the class, you can immediately tell if that class is shared with other apps (Bootstrap) or if it's private to this particular app (BEM).

 It is **not** fine to override Bootstrap classes in your app's stylesheet. The only proper way to extend style is to with a BEM selector, for example:

 {% raw %}
 ```html
 <ul class="chatter-autocomplete">
    <template is="dom-repeat" items="{{model.FoundAttachment}}">
        <li class="chatter-autocomplete__item">
            <button type="button" class="btn btn-sm btn-link chatter-autocomplete__choose" onmousedown="++this.value;" value="{{item.ChooseTrigger$::click}}">{{item.NameAndType}}</button>
        </li>
    </template>
 </ul>
 ```
 {% endraw %}

 For reference, the available Bootstrap classes can be found in [bootstrap.css](https://github.com/Starcounter/Starcounter/blob/develop/src/BuildSystem/ClientFiles/StaticFiles/sys/bootswatch/paper/bootstrap.css).

## Further reading

- [BEM: Key concepts (bem.info)](https://en.bem.info/method/key-concepts/)
- [Naming convention (bem.info)](https://en.bem.info/method/naming-convention/)
- [BEM-like Naming (cssguidelin.es)](http://cssguidelin.es/#bem-like-naming)
- [MindBEMding – getting your head ’round BEM syntax (csswizardry.com)](http://csswizardry.com/2013/01/mindbemding-getting-your-head-round-bem-syntax/)
- [An Introduction to the BEM Methodology (tutsplus.com)](http://webdesign.tutsplus.com/articles/an-introduction-to-the-bem-methodology--cms-19403)

## Future standards way: CSS Scoping

There might be a web standard in future that solves the problem of defining stylesheet for only a part of an HTML document.

[CSS Scoping](https://drafts.csswg.org/css-scoping/) proposal adds a new `scoped` attribute to the `<style>` element. When a stylesheet is provided inside of a `<style scoped>` element, it will only be applied to the current parent element and its children.

So far, this proposal was not approved by Google and Microsoft. [Caniuse.com](http://caniuse.com/#feat=style-scoped) says that the only web browser that implements it as of 2016 is Mozilla Firefox.
