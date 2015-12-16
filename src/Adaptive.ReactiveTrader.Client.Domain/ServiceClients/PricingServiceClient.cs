using Adaptive.ReactiveTrader.Client.Domain.Transport.Wamp;
using Adaptive.ReactiveTrader.Shared.DTO.Pricing;
using Adaptive.ReactiveTrader.Shared.Logging;
using System;
using System.Reactive.Linq;

namespace Adaptive.ReactiveTrader.Client.Domain.ServiceClients
{
    internal class PricingServiceClient : IPricingServiceClient
    {
        private readonly IWampConnection _connection;
        private readonly ILog _log;

        public PricingServiceClient(IWampConnection connection, ILoggerFactory loggerFactory)
        {
            _connection = connection;
            _log = loggerFactory.Create(typeof (PricingServiceClient));
        }

        public IObservable<PriceDto> GetSpotStream(string currencyPair)
        {
            if (string.IsNullOrEmpty(currencyPair)) throw new ArgumentException("currencyPair");

            var request = new GetSpotStreamRequestDto {symbol = currencyPair};

            return _connection.GetRequestStream<PriceDto>("pricing", "getPriceUpdates", request)
                  .Do(x => _log.Info($"Subscribed to prices for ccy pair {currencyPair}"));
        }
    }
}