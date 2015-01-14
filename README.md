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

## License

[GPL v3](http://www.gnu.org/licenses/gpl-3.0.txt)

## Use of RavenDB

Currently, you must install RavenDB separately. We're thinking about adding
an embedded option in the future. RavenDB's licensing should
allow this as this project is released under GPL v3.

## Current State
* Windows service to read messages is completed. It currently parses
NServiceBus Headers and capture the raw message body. It then writes
a document with the data to RavenDB.

## Next Steps
* Web interface with basic message search and configuration screens
* Ability to replay errors for NServiceBus
* Ability to parse out and index business keys for search
* Business keys used to tie together related messages
* Support for Rebus

## Contributing

Pull requests welcome. If you want to get involved in the project,
there is room for one or two more core contributors. Next interesting
challenges are replaying errors for NServiceBus and adding support
for Rebus.
