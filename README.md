# Rabbit Operations

Operations support for RabbitMQ applications including integrations with popular
message bus libraries.  Right now it only supports NServiceBus.  We are also planning to support Rebus and MassTransit.

The application reads messages from audit and error queues into
RavenDB, a document database with excellent indexing capabilities built
on Lucene.Net. Message content is automatically indexed for searching. The built-in web front end supports
searching and monitors message rates. It also links to the RabbiMQ management console. Next up is support for MassTransit and Rebus. Future plans include support for trend analysis, tailing, monitoring, heart beating and integration with New Relic and PagerDuty.

Check out our [Trello board](https://trello.com/b/m0ZLn5d7/rabbitoperations) to see what is in development and what is being planned.

![Screen](/docs/images/screenshot.png?raw=true "Screenshot")

## Prerequisites

Your application should send successfully processed messages to the audit queue and any message that fails processing to the error queue. Popular .NET message bus libraries, like NServiceBus, MassTransit and Rebus either do this by default or can be configured to do so.

## Getting Started For Developers

Install Git LFS support and track .zip files.

_You must have Visual Studio 2015 or Microsoft Build Tools 2015 installed to build the application._

Install node.js on your machine. You can get it from the [node.js site](http://nodejs.org). You also need to tell Visual Studio to use the version of node you installed
by following [these instructions](http://ryanhayes.net/synchronize-node-js-install-version-with-visual-studio-2015/)


 After that, install the necessary node
modules using npm:

````
npm install
````

Install psake on your machine.  You can get it from the [psake repository](https://github.com/psake/psake) or install it via [Choclatey](https://chocolatey.org/packages/psake):

````
choco install psake
````

Open a Powershell window and issue the command to build and run tests.  If you installed psake from the repository:

````
invoke-psake
````

Or, if you installed it via Choclatey:

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

You can start the application either by running RabbitOperations.Collector from Visual Studio or from a Powershell command prompt via psake.  If you installed psake via the repository, use:

````
invoke-psake startCollector
````
Or, if you installed it from Choclatey:

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

## Contributing

Pull requests are welcome. If you want to get involved in the project,
there is room for one or two more core contributors. There are many interesting challenges remaining.
Please contact tom@cabanski.com for more information.
