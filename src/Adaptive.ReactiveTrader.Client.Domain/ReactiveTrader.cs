using Adaptive.ReactiveTrader.Client.Domain.Concurrency;
using Adaptive.ReactiveTrader.Client.Domain.Instrumentation;
using Adaptive.ReactiveTrader.Client.Domain.Models.Execution;
using Adaptive.ReactiveTrader.Client.Domain.Models.Pricing;
using Adaptive.ReactiveTrader.Client.Domain.Models.ReferenceData;
using Adaptive.ReactiveTrader.Client.Domain.Repositories;
using Adaptive.ReactiveTrader.Client.Domain.ServiceClients;
using Adaptive.ReactiveTrader.Client.Domain.Transport.Wamp;
using Adaptive.ReactiveTrader.Shared.Logging;
using System;
using System.Reactive.Linq;

namespace Adaptive.ReactiveTrader.Client.Domain
{
    public class ReactiveTrader : IReactiveTrader, IDisposable
    {
        //private ConnectionProvider _connectionProvider;
        private ILoggerFactory _loggerFactory;
        private ILog _log;
        private IControlRepository _controlRepository;

        public void Initialize(string username, string[] servers, ILoggerFactory loggerFactory = null, string authToken = null) 
        {
            _loggerFactory = loggerFactory ?? new DebugLoggerFactory();
            _log = _loggerFactory.Create(typeof(ReactiveTrader));
            //_connectionProvider = new ConnectionProvider(username, servers, _loggerFactory);
            var wampConnection = new WampConnection(servers[0], username, _loggerFactory);

            var referenceDataServiceClient = new ReferenceDataServiceClient(wampConnection, _loggerFactory);
            var executionServiceClient = new ExecutionServiceClient(wampConnection);
            var blotterServiceClient = new BlotterServiceClient(wampConnection, _loggerFactory);
            var pricingServiceClient = new PricingServiceClient(wampConnection, _loggerFactory);

            if (authToken != null)
            {
                //var controlServiceClient = new ControlServiceClient(new AuthTokenProvider(authToken), _connectionProvider, _loggerFactory);
                //_controlRepository = new ControlRepository(controlServiceClient);
            }

            PriceLatencyRecorder = new PriceLatencyRecorder();
            var concurrencyService = new ConcurrencyService();

            var tradeFactory = new TradeFactory();
            var executionRepository = new ExecutionRepository(executionServiceClient, tradeFactory, concurrencyService);
            var priceFactory = new PriceFactory(executionRepository, PriceLatencyRecorder);
            var priceRepository = new PriceRepository(pricingServiceClient, priceFactory, _loggerFactory);
            var currencyPairUpdateFactory = new CurrencyPairUpdateFactory(priceRepository);
            TradeRepository = new TradeRepository(blotterServiceClient, tradeFactory);
            ReferenceData = new ReferenceDataRepository(referenceDataServiceClient, currencyPairUpdateFactory);
        }

        public IReferenceDataRepository ReferenceData { get; private set; }
        public ITradeRepository TradeRepository { get; private set; }
        public IPriceLatencyRecorder PriceLatencyRecorder { get; private set; }

        public IControlRepository Control
        {
            get
            {
                if (_controlRepository == null)
                    throw new InvalidOperationException("You must supply an authentication token when initializing to use the control API.");
                return _controlRepository;
            }
        }

        public IObservable<ConnectionInfo> ConnectionStatusStream
        {
            get
            {
                return Observable.Empty<ConnectionInfo>();
                //return _connectionProvider.GetActiveConnection()
                //    .Do(_ => _log.Info("New connection created by connection provider"))
                //    .Select(c => c.StatusStream)
                //    .Switch()
                //    .Publish()
                //    .RefCount();
            }
        }

        public void Dispose()
        {
            //_connectionProvider.Dispose();
        }
    }
}
