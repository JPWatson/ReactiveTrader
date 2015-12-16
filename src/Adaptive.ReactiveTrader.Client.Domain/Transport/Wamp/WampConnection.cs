using Adaptive.ReactiveTrader.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using WampClient;
using WampSharp.V2;
using WampSharp.V2.Client;
using WampSharp.V2.Core.Contracts;
using WampSharp.V2.Fluent;

namespace Adaptive.ReactiveTrader.Client.Domain.Transport.Wamp
{
    internal interface IWampConnection
    {
        IObservable<T> GetTopic<T>(string topic);
        IObservable<T> GetRequestStream<T>(string serviceType, string procedure, object payload = null);
        IObservable<T> RequestResponse<T>(string serviceType, string procedure, object payload = null);
    }

    internal class WampConnection : IWampConnection
    {
        private const string WampRealm = "com.weareadaptive.reactivetrader";
        private readonly string _userName;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IWampChannel _channel;
        private readonly Random _random = new Random();
        private readonly ReplaySubject<IDictionary<string, List<string>>> _currentServices = new ReplaySubject<IDictionary<string, List<string>>>(1);
        private readonly ILog _log;

        public WampConnection(string serverUri, string userName, ILoggerFactory loggerFactory)
        {
            _log = loggerFactory.Create(typeof(WampConnection));
            _userName = userName;
            _loggerFactory = loggerFactory;
            _channel = new WampChannelFactory().ConnectToRealm(WampRealm)
                                               .WebSocketTransport(serverUri)
                                               .JsonSerialization()
                                               .Build();

            // Hacky way of getting the current set of services until Keith's proper solution can be implemented here.
            // Just window the heartbeats every couple of seconds and see what is active at the time, then use those instances
            _channel.Open()
                    .ToObservable()
                    .Select(_ => GetTopic<HeartbeatDto>("status"))
                    .Merge()
                    .Window(TimeSpan.FromSeconds(2))
                    .Select(window => window.ToList())
                    .Concat()
                    .Select(GetServiceSummary)
                    .Subscribe(_currentServices);
        }

        private IDictionary<string, List<string>> GetServiceSummary(IList<HeartbeatDto> recentHeartbeats)
        {
            return recentHeartbeats.GroupBy(x => x.Type, x => x.Instance)
                                   .ToDictionary(x => x.Key, x => x.Distinct().ToList());
        }

        public async Task Connect()
        {
            try
            {
                await _channel.Open();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public IObservable<T> GetTopic<T>(string topic)
        {
            return _channel.RealmProxy.Services.GetSubject<T>(topic);
        }

        public IObservable<T> GetRequestStream<T>(string serviceType, string procedure, object payload = null)
        {
            return CallServiceInstance(serviceType, procedure, call => GetInnerRequestStream<T>(call, payload));
        }

        public IObservable<T> RequestResponse<T>(string serviceType, string procedure, object payload = null)
        {
            return CallServiceInstance(serviceType, procedure, call => DoRequestResponseCall<T>(call, payload));
        }

        private IObservable<T> CallServiceInstance<T>(string serviceType, string procedure, Func<string, IObservable<T>> serviceCall)
        {
            return _currentServices.Where(x => x != null && x.ContainsKey(serviceType))
                                   .Take(1)
                                   .Select(x =>
                                   {
                                       var instances = x[serviceType];
                                       return $"{instances[0]}.{procedure}";
                                   })
                                   .Select(serviceCall)
                                   .Merge();
        }

        private IObservable<T> GetInnerRequestStream<T>(string procedure, object payload)
        {
            var queueName = GetPrivateQueueName();

            _log.Info($"Subscribing to private queue {queueName} for procedure call {procedure}");
            var topicObservable = GetTopic<T>(queueName);

            _log.Info($"Invoking RPC call for procedure call {procedure}");
            _channel.RealmProxy.RpcCatalog.Invoke(new LoggingCallback<T>(procedure, _loggerFactory), new CallOptions(), procedure, new object[] {WrapMessage(payload, queueName)});

            return topicObservable;
        }

        private IObservable<T> DoRequestResponseCall<T>(string procedure, object payload)
        {
            var callBack = new ObservableCallback<T>(procedure, _loggerFactory);
            _channel.RealmProxy.RpcCatalog.Invoke(callBack, new CallOptions(), procedure, new object[] { WrapMessage(payload, string.Empty) });

            return callBack.ResponseObservable;
        }

        private MessageDto WrapMessage(object payload, string queueName)
        {
            return new MessageDto
            {
                Payload = payload ?? new NothingDto(),
                ReplyTo = queueName,
                Username = _userName
            };
        }

        private string GetPrivateQueueName()
        {
            return $"queue{_random.Next().ToString("x8")}";
        }
    }
}