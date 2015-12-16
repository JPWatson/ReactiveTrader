using System;
using System.Globalization;
using Adaptive.ReactiveTrader.Shared.DTO.Execution;

namespace Adaptive.ReactiveTrader.Client.Domain.Models.Execution
{
    internal class TradeFactory : ITradeFactory
    {
        public ITrade Create(TradeDto trade)
        {
            var dateParseFormats = new[]
            {
                "dd/MM/yyyy",
                "MM/dd/yyyy",
                "u"
            };

            return new Trade(
                trade.CurrencyPair,
                trade.Direction == DirectionDto.Buy ? Direction.BUY : Direction.SELL,
                trade.Notional,
                trade.DealtCurrency,
                trade.SpotRate,
                trade.Status == TradeStatusDto.Done ? TradeStatus.Done : TradeStatus.Rejected,
                DateTime.ParseExact(trade.TradeDate, dateParseFormats, CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal),
                trade.TradeId,
                trade.TraderName,
                DateTime.Today); // The new server gives value date out in format SP. 16 Dec so just default to today trade.ValueDate);
        }
    }
}