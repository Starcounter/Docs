# Visual Studio Extension

Starcounter provides a Visual Studio Extension to simplify the development of Starcounter applications. It provides templates to build on and the ability to start applications from Visual Studio. 

## Templates

There are currently two project templates and three item templates. These are:

**Project templates**:

* Starcounter Application
* Starcounter Class Library

**Item templates**:

* Starcounter HTML template with dom-bind
* Starcounter Typed JSON
* Starcounter Typed JSON with code-behind

### Starcounter Application Template

The Starcounter application template is the starting point to creating applications with Starcounter. It contains four references: `Starcounter`, `Starcounter.Internal`, `Starcounter.Logging`, and `Starcounter.XSON`. Additionally, it comes with a boilerplate `Program.cs` file that looks like this:

```cs
using System;
using Starcounter;

namespace MyApp
{
    class Program
    {
        static void Main()
        {

        }
    }
}
```

### Starcounter Class Library

The Starcounter class library template is the starting point for creating a shared data model to use across applications. For example, the [Simplified](https://github.com/StarcounterApps/Simplified) DLL that is used to provide a shared data model to the Starcounter [sample apps](https://github.com/StarcounterApps) is built with this template. It contains the same references as the Starcounter application template. This is how the boilerplate `Program.cs` file looks:

```cs
using System;
using Starcounter;

namespace MyClassLibrary
{

    [Database]
    public class Entity1
    {
        public string Field1;
    }
}
```

### Starcounter HTML template with dom-bind

This item templates gives a starting point for creating HTML view definitions with Polymer. 

It contains the following code:

```html
<link rel="import" href="/sys/polymer/polymer.html">

<template>
    <template is="dom-bind">
        
    </template>
</template>
```

### Starcounter Typed JSON

This template is the starting point for creating a view-model definition using JSON-by-example. It is simply an empty `.json` file containing an empty JSON object:

```json
{
}
```

### Starcounter Typed JSON with code-behind

This template is the same as the Starcounter Typed JSON file, except that it also provides a code-behind file. Thus, two files are created with this template, `.json` and `.json.cs`. 

The `.json` file is identical to the Starcounter Type JSON file. The `.json.cs` file contains the following code:

```cs
using Starcounter;

namespace MyApp
{
    partial class MyPage : Json
    {
    }
}
```

## Starting Applications From Visual Studio

With the Visual Studio Extension, apps can be started directly from the development environment. This is done the same way any other application would be started from Visual Studio, by clicking the `Start` butoon or <kbd>f5</kbd>.

Further instructions on this can be found in [Starting and Stopping Apps](/guides/working-with-starcounter/starting-and-stopping-apps/). There it is also described how it is possible to set particular arguments on application start from `Debug` -> `MyApp Properties` -> `Debug`.
