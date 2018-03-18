# How to migrate from \[SynonymousTo\]

## Goal

The `[SynonymousTo]` attribute was deprecated in 2.3.2 and removed in Starcounter 2.4. This page explains how to migrate from  `[SynonymousTo]` to standard C\# features.

{% hint style="info" %}
We only use properties in this guide since that's what recommended as described in [Database classes](../topic-guides/database/database-classes.md#properties-and-fields).
{% endhint %}

## Migrate synonymous fields with same type

{% code-tabs %}
{% code-tabs-item title="Before.cs" %}
```csharp
[Database] 
public class Foo 
{
    public string Bar;
    
    [SynonymousTo("Bar")]
    public string Fubar;
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

{% code-tabs %}
{% code-tabs-item title="After.cs" %}
```csharp
[Database]
public class Foo
{
    public string Bar { get; set; }
    
    public string Fubar
    {
        get => Bar;
        set => Bar = value;
    }
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

## Migrate synonymous fields with narrowed type

{% code-tabs %}
{% code-tabs-item title="Before.cs" %}
```csharp
[Database] 
public class Foo 
{
    public Foo Bar;
    
    [SynonymousTo("Bar")]
    public Narrowed Fubar;
}

[Database] 
public class Narrowed : Foo {}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

{% code-tabs %}
{% code-tabs-item title="After.cs" %}
```csharp
[Database] 
public class Foo 
{
    public Foo Bar { get; set; } 
    
    public Narrowed Fubar 
    {
        get => (Narrowed)Bar;
        set => Bar = value; 
    }
}

[Database] 
public class Narrowed : Foo {}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

## Migrate read-only synonym pattern

{% code-tabs %}
{% code-tabs-item title="Before.cs" %}
```csharp
[Database] 
public class Foo 
{
    public string Bar;
    
    [SynonymousTo("Bar")]
    public readonly string Fubar;
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

{% code-tabs %}
{% code-tabs-item title="After.cs" %}
```csharp
[Database] 
public class Foo 
{
  public string Bar { get; set; }
  public string Fubar => Bar;
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

## Summary

After these changes, the database schema will still be compatible, so no unload/reload is needed. We hope that these changes makes the code easier to read and more familiar to C\# developers.

