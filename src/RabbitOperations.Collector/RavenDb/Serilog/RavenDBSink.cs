﻿// Copyright 2014 Serilog Contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Raven.Client;
using Serilog.Sinks.PeriodicBatching;
using LogEvent = Serilog.Sinks.RavenDB.Data.LogEvent;
using Raven.Json.Linq;
using Serilog.Events;

namespace Serilog.Sinks.RavenDB
{
    /// <summary>
    /// Writes log events as documents to a RavenDB database.
    /// </summary>
    public class RavenDBSink : PeriodicBatchingSink
    {   
        readonly IFormatProvider _formatProvider;
        readonly IDocumentStore _documentStore;
        readonly string _defaultDatabase;
        readonly TimeSpan? _expirationTimeSpan;
        private readonly TimeSpan? _errorExpirationTimeSpan;

        /// <summary>
        /// A reasonable default for the number of events posted in
        /// each batch.
        /// </summary>
        public const int DefaultBatchPostingLimit = 50;

        /// <summary>
        /// A reasonable default time to wait between checking for event batches.
        /// </summary>
        public static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(2);

        /// <summary>
        /// Construct a sink posting to the specified database.
        /// </summary>
        /// <param name="documentStore">A documentstore for a RavenDB database.</param>
        /// <param name="batchPostingLimit">The maximum number of events to post in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="defaultDatabase">Optional name of default database</param>
        /// <param name="expirationTimeSpan">Optional time before a logged message will be expired assuming the expiration bundle is installed. Zero (00:00:00) means no expiration. If this is not provided but errorExpirationTimeSpan is, errorExpirationTimeSpan will be used for non-errors too.</param>
        /// <param name="errorExpirationTimeSpan">Optional time before a logged error message will be expired assuming the expiration bundle is installed.  Zero (00:00:00) means no expiration. If this is not provided but expirationTimeSpan is, expirationTimeSpan will be used for errors too.</param>
        public RavenDBSink(IDocumentStore documentStore, int batchPostingLimit, TimeSpan period, IFormatProvider formatProvider, string defaultDatabase = null, TimeSpan? expirationTimeSpan = null, TimeSpan? errorExpirationTimeSpan = null)
            : base(batchPostingLimit, period)
        {
            if (documentStore == null) throw new ArgumentNullException("documentStore");
            _formatProvider = formatProvider;
            _documentStore = documentStore;
            _defaultDatabase = defaultDatabase;
            _expirationTimeSpan = expirationTimeSpan;
            _errorExpirationTimeSpan = errorExpirationTimeSpan;
            if (_errorExpirationTimeSpan == null)
            {
                _errorExpirationTimeSpan = _expirationTimeSpan;
            }
            if (_expirationTimeSpan == null)
            {
                _expirationTimeSpan = _errorExpirationTimeSpan;
            }
        }

        /// <summary>
        /// Emit a batch of log events, running asynchronously.
        /// </summary>
        /// <param name="events">The events to emit.</param>
        /// <remarks>Override either <see cref="PeriodicBatchingSink.EmitBatch"/> or <see cref="PeriodicBatchingSink.EmitBatchAsync"/>,
        /// not both.</remarks>
        protected override async Task EmitBatchAsync(IEnumerable<global::Serilog.Events.LogEvent> events)
        {
            using (var session = string.IsNullOrWhiteSpace(_defaultDatabase) ? _documentStore.OpenAsyncSession() : _documentStore.OpenAsyncSession(_defaultDatabase))
            {
                foreach (var logEvent in events)
                {
                    var logEventDoc = new LogEvent(logEvent, logEvent.RenderMessage(_formatProvider));
                    await session.StoreAsync(logEventDoc);
                    if (_expirationTimeSpan != null || _errorExpirationTimeSpan != null)
                    {
                        if (logEvent.Level == LogEventLevel.Error || logEvent.Level == LogEventLevel.Fatal)
                        {
                            if (_expirationTimeSpan > TimeSpan.Zero)
                            {
                                var metaData = await session.Advanced.GetMetadataForAsync(logEventDoc);
                                metaData["Raven-Expiration-Date"] =
                                    new RavenJValue(DateTime.UtcNow.Add(_expirationTimeSpan.Value));
                            }
                        }
                        else
                        {
                            if (_expirationTimeSpan > TimeSpan.Zero)
                            {
                                var metaData = await session.Advanced.GetMetadataForAsync(logEventDoc);
                                metaData["Raven-Expiration-Date"] =
                                    new RavenJValue(DateTime.UtcNow.Add(_expirationTimeSpan.Value));
                            }
                        }
                    }
                }
                await session.SaveChangesAsync();
            }
        }
    }
}
