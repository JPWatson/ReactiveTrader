using Adaptive.ReactiveTrader.Client.Domain.Transport.Wamp;
using Adaptive.ReactiveTrader.Shared.DTO.Execution;
using Adaptive.ReactiveTrader.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace Adaptive.ReactiveTrader.Client.Domain.ServiceClients
{
    internal class BlotterServiceClient : IBlotterServiceClient
    {
        private readonly IWampConnection _connection;
        private readonly ILog _log;

        public BlotterServiceClient(IWampConnection connection, ILoggerFactory loggerFactory)
        {
            _connection = connection;
            _log = loggerFactory.Create(typeof (BlotterServiceClient));
        }

        public IObservable<IEnumerable<TradeDto>> GetTradesStream()
        {
            return _connection.GetRequestStream<TradesDto>("blotter", "getTradesStream")
                  .Select(x => x.Trades)
                  .Do(x => _log.InfoFormat("Subscribed to trades and received {0} trades.", x.Count));
        }
    }
}