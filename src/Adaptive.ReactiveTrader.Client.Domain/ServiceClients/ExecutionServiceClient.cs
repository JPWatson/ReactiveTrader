using Adaptive.ReactiveTrader.Client.Domain.Transport.Wamp;
using Adaptive.ReactiveTrader.Shared.DTO.Execution;
using System;
using System.Reactive.Linq;

namespace Adaptive.ReactiveTrader.Client.Domain.ServiceClients
{
    internal class ExecutionServiceClient : IExecutionServiceClient
    {
        private readonly IWampConnection _connection;

        public ExecutionServiceClient(IWampConnection connection)
        {
            _connection = connection;
        }

        public IObservable<TradeDto> ExecuteRequest(ExecuteTradeRequestDto executeTradeRequest)
        {
            return _connection.RequestResponse<ExecuteTradeResponseDto>("execution", "executeTrade", executeTradeRequest)
                              .Select(x => x.Trade)
                              .Where(x => x.Status != TradeStatusDto.Pending);
        }
    }
}
