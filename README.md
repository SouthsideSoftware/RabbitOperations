# Rabbit Operations

Operations support for RabbitMQ applications with support for popular
message bus libraries.  Right now it only supports NServiceBus.  Next
planned is Rebus.

The general idea is to read messages from audit and error queues into
RavenDB, a document database with excellent indexing capabilities built
on Lucene.Net. Eventually, you will be able to configure rules to parse business
keys out of the body so messages can be related to one another and searched
by logical business keys and keywords. There will be a web front end
to support configuration and searching.

## Documentation

Very limited documentation is available in the [wiki](https://github.com/SouthsideSoftware/RabbitOperations/wiki)

## License

[GPL v3](http://www.gnu.org/licenses/gpl-3.0.txt)

## Use of RavenDB

RavenDB is embedded by default. Ayende has graciously agreed to provide a license for this purpose. You can also
use an external RavenDB if you like. If you use RavenDB externally, you will have to obtain an appropriate license
from their [website](http://www.ravendb.org).

## Current State
* Windows service to read messages is completed. It currently parses
NServiceBus Headers and capture the raw message body. It then writes
a document with the data to RavenDB.

## Next Steps
* Check out our [Trello board](https://trello.com/b/m0ZLn5d7/rabbitoperations) that shows what we're up to and what
we're planning.

## Contributing

Pull requests welcome. If you want to get involved in the project,
there is room for one or two more core contributors. Next interesting
challenges are replaying errors for NServiceBus and adding support
for Rebus.
