# Avoiding CSS conflicts (BEM naming convention)

A complex web app is composed out of HTML responses from multiple apps. This means that the merged HTML, CSS and JavaScript runs in the global namespace.

How to make sure that your CSS does not have side effects in other apps?

## Future standards way: CSS Scoping

There might be a web standard in future that solves the problem of defining stylesheet for only a part of an HTML document.

[CSS Scoping](https://drafts.csswg.org/css-scoping/) proposal adds a new `scoped` attribute to the `<style>` element. When a stylesheet is provided inside of a `<style scoped>` element, it will only be applied to the current parent element and its children.

So far, this proposal was not approved by Google and Microsoft. [Caniuse.com](http://caniuse.com/#feat=style-scoped) says that the only web browser that implements it as of 2016 is Mozilla Firefox.

## Current best practices way: BEM

Another approach is to use a naming convention that works in every web browser. The naming convention that Starcounter advises is called BEM (Block Element Modifier).

The basic principle in BEM is to **only use classes in your stylesheets**.

Possible combinations of Blocks, Elements and Modifiers are the following:

```css
.block {}
.block__element {}
.block--modifier {}
.block__element--modifier {}
```

A **Block** represents an logical area of your app's UI. For example:

- a menu
- a login form
- a search form

An **Element** is a fragment of the Block that performs a particular function. For example:

- a menu item
- a password input in the login form
- a search button

An Element only makes sense in the context of its Block. It cannot appear outside of the Block.

**Modifiers** represent variations of Blocks and Elements. For example:

- an expanded menu
- an active menu item
- a password input with an invalid value
- a disabled search button

Considering a sample "Chatter" app that has a menu, the BEM class names for that menu could be the following:

```css
.chatter-menu {}
.chatter-menu__item {}
.chatter-menu__item--active {}
.chatter-menu--expanded {}
```

## BEM in Starcounter apps

We advise the following rules to be applied when using BEM selectors in Starcoutner apps.

### 1. Use just BEM

Only use BEM class selectors in your stylesheets. Do not use element selectors, or id selectors for the purpose of styling.

Do not use inline styles.

### 2. Give meaningful names

Try to give meaningful names to your Blocks, Elements and Modifiers. For example, `.chatter-avatar` tells you much more about the purpose of a Block than `.chatter-img`.

### 3. Block should be prefixed with the app name

To isolate BEM used in your apps from BEM used in the other apps, the Block should be prefixed with the app name. For example, the class name for a menu block in the "Chatter" app should be `.chatter-menu`.

### 4. Use lowercase names, separate words with hyphen

We advise to always use lowercase.

When you have to use multiple words in Block, Element or Modifier section, separate them with a hyphen (`-`), for example: `.chatter-chat-message__message-text`.

### 5. Element should be defined after double underscore

Double underscore (`__`) separates the Block part from the Element part in the class name.

### 6. Modifier should be defined after double hyphen

Double hyphen (`--`) separates the Block or Element part from the Modifier part in the class name. There are only "boolean" style modifiers.

For example: `.chatter-menu__item--active`

This is called ["Harry Roberts' style" modifiers](https://en.bem.info/method/naming-convention/) on BEM.info.

### 7. Block and Element must be in the same HTML template

It is not correct to use an Element in a HTML template, without the Block declared in the same template. Otherwise you create a implicit coupling of these two templates, which might break when the partial mapping changes.

If you want to define `.chatter-menu` in a parent partial and the menu items in a nested partials, these menu items will become new blocks (`.chatter-menu-item`, not `.chatter-menu__item`).

### 8. Modifier classes should extend base classes

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

### 9. Mixing BEM with Bootstrap

You might have noticed that Starcounter sample apps use Bootstrap CSS framework to achieve common look and feel.

Even though Bootstrap does not follow BEM, it is totally fine to mix Bootstrap and BEM in a single project, because there is no collision. In fact, by just looking at the CSS class name, you can immediately tell if that class name is shared with other apps (Bootstrap) or is it private to this particular app (BEM).

It is **not** fine to override Bootstrap classes in your app's stylesheet. The only proper way to extend style is to with a BEM selector, for example:

{% raw %}
```html
<ul class="chatter-autocomplete">
    <template is="dom-repeat" items="{{model.FoundAttachment}}">
        <li class="chatter-autocomplete__item">
            <button type="button" class="btn btn-sm btn-link chatter-autocomplete__choose" onmousedown="++this.value;" value="{{item.Choose$::click}}">{{item.NameAndType}}</button>
        </li>
    </template>
</ul>
```
{% endraw %}

For reference, the available Bootstrap CSS class names can be found in [bootstrap.css](https://github.com/Starcounter/Starcounter/blob/develop/src/BuildSystem/ClientFiles/StaticFiles/sys/bootswatch/paper/bootstrap.css).

## Why we advise BEM

We have found BEM to be excellent mix of simplicity and effectiveness to deal with CSS in micro apps, for a number of reasons outlined below.

### Prevents your styles from affecting other apps

Using your Starcounter app name as the prefix in the Block part of the BEM selector makes sure that the stylesheet will affect only your app.

### Prevents accidental styling of nested elements

Adding a `chatter-menu__item` class to a `<li>` element in a menu makes sure that the style is applied only at that specific level, and will not affect nested `<li>` elements.

You might think you are in good control of such cases. The reality shows that when multiple people work on a project, it becomes unclear when it is desired to apply the style to nested elements and when it is an unwanted side effect.

### Encapsulates code for reuse

BEM encourages to define logical parts of the UI that are easily movable within the project.

When you look at the source code, it is immediately obvious what are the HTML elements needed for that logical part and what is the CSS code that styles them.

### Applicable to projects of any size

BEM was designed to make it easier to work on big projects. Yet, even a smallest web app might use CSS that needs to be easily understandable by the team and work without side effects.

By using BEM, you identify that a CSS class definition comes from your app and is used only there.

## Sample apps

Check out the source code of [KitchenSink](https://github.com/StarcounterSamples/KitchenSink). This sample app that shows how to apply BEM in practice.

## Further reading

- [BEM: Key concepts (bem.info)](https://en.bem.info/method/key-concepts/)
- [Naming convention (bem.info)](https://en.bem.info/method/naming-convention/)
- [BEM-like Naming (cssguidelin.es)](http://cssguidelin.es/#bem-like-naming)
- [MindBEMding – getting your head ’round BEM syntax (csswizardry.com)](http://csswizardry.com/2013/01/mindbemding-getting-your-head-round-bem-syntax/)
- [An Introduction to the BEM Methodology (tutsplus.com)](http://webdesign.tutsplus.com/articles/an-introduction-to-the-bem-methodology--cms-19403)
