using Adaptive.ReactiveTrader.Client.Domain.Transport.Wamp;
using Adaptive.ReactiveTrader.Shared.DTO.ReferenceData;
using Adaptive.ReactiveTrader.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace Adaptive.ReactiveTrader.Client.Domain.ServiceClients
{
    class ReferenceDataServiceClient : IReferenceDataServiceClient
    {
        private readonly IWampConnection _connection;
        private readonly ILog _log;

        public ReferenceDataServiceClient(IWampConnection connection, ILoggerFactory loggerFactory)
        {
            _connection = connection;
            _log = loggerFactory.Create(typeof (ReferenceDataServiceClient));
        }

        public IObservable<IEnumerable<CurrencyPairUpdateDto>> GetCurrencyPairUpdatesStream()
        {
            return _connection.GetRequestStream<CurrencyPairUpdatesDto>("reference", "getCurrencyPairUpdatesStream")
                              .Select(x => x.Updates)
                              .Do(x => _log.InfoFormat("Subscribed to currency pairs and received {0} currency pairs.", x.Count));
        }
    }
}