using Adaptive.ReactiveTrader.Shared.Logging;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Adaptive.ReactiveTrader.Client.Domain.Transport.Wamp
{
    internal class ObservableCallback<T> : LoggingCallback<T>
    {
        private readonly ReplaySubject<T> _responseSubject = new ReplaySubject<T>(1);

        public ObservableCallback(string procedureName, ILoggerFactory loggerFactory) : base(procedureName, loggerFactory)
        {
        }

        public IObservable<T> ResponseObservable => _responseSubject.AsObservable();

        protected override void OnResult(T result)
        {
            _responseSubject.OnNext(result);
            _responseSubject.OnCompleted();
        }
    }
}