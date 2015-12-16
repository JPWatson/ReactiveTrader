using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WampSharp.Core.Serialization;
using WampSharp.V2.Core.Contracts;
using WampSharp.V2.Rpc;

namespace Adaptive.ReactiveTrader.Client.Domain.Transport.Wamp
{
    internal class ObservableCallback<T> : IWampRawRpcOperationClientCallback
    {
        private readonly ReplaySubject<T> _responseSubject = new ReplaySubject<T>(1);

        public IObservable<T> ResponseObservable => _responseSubject.AsObservable();

        public void Result<TMessage>(IWampFormatter<TMessage> formatter, ResultDetails details)
        {
            
        }

        public void Result<TMessage>(IWampFormatter<TMessage> formatter, ResultDetails details, TMessage[] arguments)
        {
            PublishResult(formatter, arguments);
        }

        public void Result<TMessage>(IWampFormatter<TMessage> formatter,
                                     ResultDetails details,
                                     TMessage[] arguments,
                                     IDictionary<string, TMessage> argumentsKeywords)
        {

            PublishResult(formatter, arguments);
        }

        private void PublishResult<TMessage>(IWampFormatter<TMessage> formatter, TMessage[] arguments)
        {
            var result = formatter.Deserialize<T>(arguments[0]);
            _responseSubject.OnNext(result);
        }

        public void Error<TMessage>(IWampFormatter<TMessage> formatter, TMessage details, string error)
        {
            throw new InvalidOperationException(error);
        }

        public void Error<TMessage>(IWampFormatter<TMessage> formatter, TMessage details, string error, TMessage[] arguments)
        {
            throw new InvalidOperationException(error);
        }

        public void Error<TMessage>(IWampFormatter<TMessage> formatter, TMessage details, string error, TMessage[] arguments,
                                    TMessage argumentsKeywords)
        {
            throw new InvalidOperationException(error);
        }
    }
}