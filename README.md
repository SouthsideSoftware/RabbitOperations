# Rabbit Operations

Operations support for RabbitMQ applications including integration with popular
message bus libraries.  Right now it only supports NServiceBus.  We are also planning to support Rebus and MassTransit.

The application reads messages from audit and error queues into
RavenDB, a document database with excellent indexing capabilities built
on Lucene.Net. Message content is automatically indexed for searching. The built-in web front end supports
searching and monitors message rates. It also links to the RabbiMQ management console. Future plans include support for replaying errors, tailing, monitoring, heart beating and integration with New Relic and PagerDuty. Check out our [Trello board](https://trello.com/b/m0ZLn5d7/rabbitoperations) that shows what is in development and what is being planned.

![Screen](/docs/images/screenshot.png?raw=true "Screenshot")

## Prerequisites

Your application should send successfully processed messages to the audit queue and any message that fails processing to the error queue. Popular .NET message bus libraries, like NServiceBus, MassTransit and Rebus either do this by default or can be configured to do so.

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
there is room for one or two more core contributors. Next interesting
challenges are replaying errors for NServiceBus and adding support
for Rebus.
