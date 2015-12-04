using System.Collections.Generic;

namespace RabbitOperations.Domain.Configuration
{
    public class MessageTypeHandling
    {
        /// <summary>
        /// Gets and sets the message types supported
        /// </summary>
        public IList<string> MessageTypes { get; set; }
        /// <summary>
        /// Gets and sets the paths navigated to find keys.  For example, Order.CorrelationId would look for the CorrelationId of the order.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Keys are used to identify a message and join related messages. Search by key is supported.
        /// </para>
        /// <para>
        /// If any of the properties references a collection, all members of the collection will be traversed.
        /// </para>
        /// </remarks>
        public IList<JsonPath> KeyPaths { get; set; }
        /// <summary>
        /// Gets and sets the paths navigated to find keywords. For example, Order.SiteName would look for the SiteName of the order.
        /// </summary>
        /// <para>
        /// Search by keyword is supported. Unlike keys, keywords do not imply a relationship between multiple messages. For example, a keyword
        /// on site name could be used to find all messages having to do with a site but would not allow you to traverse a set of messages
        /// by the site name.
        /// </para>
        /// <para>
        /// If any of the properties references a collection, all members of the collection will be traversed.
        /// </para>
        public IList<JsonPath> KeywordPaths { get; set; }
        /// <summary>
        /// Additional keywords to be applied to messages of this type. See the dicussion of KeyPaths for more information
        /// on keywords.
        /// </summary>
        public IList<string> Keywords { get; set; } 

    }
}