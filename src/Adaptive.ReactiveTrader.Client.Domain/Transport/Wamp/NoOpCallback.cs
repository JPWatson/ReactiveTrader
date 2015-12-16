using System;
using System.Collections.Generic;
using WampSharp.Core.Serialization;
using WampSharp.V2.Core.Contracts;
using WampSharp.V2.Rpc;

namespace Adaptive.ReactiveTrader.Client.Domain.Transport.Wamp
{
    internal class NoOpCallback : IWampRawRpcOperationClientCallback
    {

        public void Result<T>(IWampFormatter<T> formatter, ResultDetails details)
        {
        }

        public void Result<T>(IWampFormatter<T> formatter, ResultDetails details, T[] arguments)
        {
        }

        public void Result<TMessage>(IWampFormatter<TMessage> formatter,
                                     ResultDetails details,
                                     TMessage[] arguments,
                                     IDictionary<string, TMessage> argumentsKeywords)
        {
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