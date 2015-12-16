using System;
using System.Collections.Generic;
using Adaptive.ReactiveTrader.Shared.Logging;
using WampSharp.Core.Serialization;
using WampSharp.V2.Core.Contracts;
using WampSharp.V2.Rpc;

namespace Adaptive.ReactiveTrader.Client.Domain.Transport.Wamp
{
    internal class LoggingCallback<T> : IWampRawRpcOperationClientCallback
    {
        private readonly string _procedureName;
        private readonly ILog _log;

        public LoggingCallback(string procedureName, ILoggerFactory loggerFactory)
        {
            _log = loggerFactory.Create(typeof (LoggingCallback<>));
            _procedureName = procedureName;
        }

        public void Result<TMessage>(IWampFormatter<TMessage> formatter, ResultDetails details)
        {
            _log.Info($"Got callback for procedure {_procedureName} with no arguments");
            OnResult(default(T));
        }

        public void Result<TMessage>(IWampFormatter<TMessage> formatter, ResultDetails details, TMessage[] arguments)
        {
            _log.Info($"Got callback for procedure {_procedureName}");
            OnResult(formatter.Deserialize<T>(arguments[0]));
        }

        public void Result<TMessage>(IWampFormatter<TMessage> formatter,
                                     ResultDetails details,
                                     TMessage[] arguments,
                                     IDictionary<string, TMessage> argumentsKeywords)
        {
            _log.Info($"Got callback for procedure {_procedureName}");
            OnResult(formatter.Deserialize<T>(arguments[0]));
        }

        protected virtual void OnResult(T response)
        {
        }

        public void Error<TMessage>(IWampFormatter<TMessage> formatter, TMessage details, string error)
        {
            _log.Error($"Error response {error} for procedure {_procedureName}");
            throw new InvalidOperationException(error);
        }

        public void Error<TMessage>(IWampFormatter<TMessage> formatter, TMessage details, string error, TMessage[] arguments)
        {
            _log.Error($"Error response {error} for procedure {_procedureName}");
            throw new InvalidOperationException(error);
        }

        public void Error<TMessage>(IWampFormatter<TMessage> formatter, TMessage details, string error, TMessage[] arguments,
                                    TMessage argumentsKeywords)
        {
            _log.Error($"Error response {error} for procedure {_procedureName}");
            throw new InvalidOperationException(error);
        }
    }
}