# Sharing data

## Introduction

Starcounter apps in the same code host share the same database. This means that if one app changes some data in the database, all other apps in the same code host will immediately see that change. This, together with Blending, makes it possible for two apps that have been developed separately to act as one.

To share data between the apps, they have to agree on a common database schema for the tables that they wish to share. This is done by defining a common data model in a separate assembly. 

## Creating a common data model

One of the templates provided with the Visual Studio extension can be used to create a starting point for a common data model. The template is called "Starcounter Class Library" and can be found under `Add New Project -> Visual C# -> Starcounter -> Starcounter Class Library`.

The database classes that will be shared across different apps can be added to this project.  
  
To conveniently share the common data model, it can be put in the same Visual Studio solution as the apps or uploaded to NuGet.

## Add to apps

To include this common data model in an application, add it as a reference. For it to properly work, the "Copy Local" property has to be set to false. This can be done in the "Properties" tab of the reference. This means that the common data model will be shared as a dynamic-link library \(DLL\) between the apps.  
  
When sharing a common data model across apps, the apps have to reference the same version of the DLL.

{% hint style="warning" %}
Don't extend the common data model or create database classes that reference the common data model in apps. Starcounter will then fail to read the data if one of the apps are not running.
{% endhint %}

{% hint style="warning" %}
When you run multiple apps with a common data model, restarting one app will not reload the common data model DLL. Thus, any changes in the common data model requires the database to be restarted.
{% endhint %}

## The Simplified data model

All the Starcounter [sample apps](https://github.com/search?q=topic%3Aapp+org%3AStarcounter&type=Repositories) use a preinstalled data model called "Simplified". This allows all these apps to share data. To utilize this data model and share data with the sample apps, load the data model into an app by referencing it and setting "Copy Local" to false.

## Packing apps with common data models

To create packages with `starpack`, the DLL must be in the package together together with the app executables. 

