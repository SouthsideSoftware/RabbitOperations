# Rabbit Operations

Operations support for RabbitMQ applications with support for popular
message bus libraries.  Right now it only supports NServiceBus.  Next
planned is Rebus.

The general idea is to read messages from audit and error queues into
RavenDB, a document database with excellent indexing capabilities built
on Lucene.Net. Message content is automatically indexed for searching. The built-in web front end supports
searching, tailing. Shortly, monitoring, heart beating and integration with New Relic and PagerDuty will be added.

![Screen](/docs/images/screenshot.png?raw=true "Screenshot")

## Getting Started For Developers

Install psake on your machine.  You can get it from the [psake repository](https://github.com/psake/psake) or install it via [Choclatey](https://chocolatey.org/packages/psake).  Then open a Powershell window and issue the command:

````
psake
````

If you want to build from Visual Studio, you will need to add the following to your nuget.config file:

````
<packageSources>
   <add key="nuget.org" value="https://www.myget.org/F/southside/" />
</packageSources>
````

Check out the [nuget docs](https://docs.nuget.org/consume/nuget-config-file) for more information on editing the nuget.config file.

You can start the application either by running RabbitOperations.Collector from Visual Studio or from a Powershell command prompt via psake:

````
psake startCollector
````

## Documentation and Release Notes

Documentation is available in the [wiki](https://github.com/SouthsideSoftware/RabbitOperations/wiki)

## License

[GPL v3](http://www.gnu.org/licenses/gpl-3.0.txt)

## Use of RavenDB

RavenDB is embedded by default. Ayende has graciously agreed to provide a license for this purpose. You can also
use an external RavenDB if you like. If you use RavenDB externally, you will have to obtain an appropriate license
from their [website](http://www.ravendb.org).

## Next Steps
Check out our [Trello board](https://trello.com/b/m0ZLn5d7/rabbitoperations) that shows what we're up to and what
we're planning.

## Contributing

Pull requests welcome. If you want to get involved in the project,
there is room for one or two more core contributors. Many interesting challenges
remain including adding support for Rebus and MassTransit.
