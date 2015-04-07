# Rabbit Operations

Operations support for RabbitMQ applications with support for popular
message bus libraries.  Right now it only supports NServiceBus.  Next
planned is Rebus.

The general idea is to read messages from audit and error queues into
RavenDB, a document database with excellent indexing capabilities built
on Lucene.Net. Message content is automatically indexed for searching. The built-in web front end supports
searching, tailing. Shortly, monitoring, heart beating and integration with New Relic and PagerDuty will be added.

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
there is room for one or two more core contributors. Next interesting
challenges are replaying errors for NServiceBus and adding support
for Rebus.
