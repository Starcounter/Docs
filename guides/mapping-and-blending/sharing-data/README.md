# Sharing Data

In order to make apps share data out of the box, a common data model is used.

## Creating a Common Data Model

One of the templates provided with the Visual Studio extension can be used to create a starting point for a common data model. The template is called "Starcounter Class Library" and can be found under `Add New Project -> Visual C# -> Starcounter -> Starcounter Class Library`. 

The database classes that will be shared across different apps can be added into this project.

To include this common data model in an application, add it as a reference. For it to properly work, the "Copy Local" property has to be set to false. This can be done in the "Properties" tab of the reference.

## The Simplified Data Model

All the Starcounter [sample apps](https://github.com/Starcounterapps) use a preinstalled data model called "Simplified". This allows all these apps to share data. To utilize this data model and share data with the sample apps, load the data model into an app by referencing it and setting "Copy Local" to false.