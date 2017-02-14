# Sharing data

There is no benefit of running multiple apps simultaneously, if they are siloed.

How can I share data between apps without any serialization, microservices, REST, etc?

Multiple apps running in the same database will share data out of the box in two cases:

- they use the same data model
- they are mapped to a common data model

## Sharing data through using the same data model

The easiest way to get started on sharing data between apps is to use the same data model.

Starcounter comes preinstalled with a data model called `Simplified.dll`. If you load it as a reference in your app, you can use the data of our [Developer Samples](https://github.com/StarcounterSamples). We are accepting pull requests to [Simplified](https://github.com/StarcounterSamples/Simplified), if you would like to propose extending this data model.

If you have a separate data model that you would like to share between multiple apps, you can do this by mimicking how Simplified works. Just create the data model in a separate project and reference to it. All of you apps will load it as a DLL.

## Sharing data through database mapping

Starcounter is running a pilot database mapping program with selected partners. This allows to share data through mapping to a common data model, rather than sharing the same data model. Please contact us for details.
