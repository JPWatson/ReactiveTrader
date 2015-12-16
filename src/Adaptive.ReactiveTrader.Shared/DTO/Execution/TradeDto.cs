using System;

namespace Adaptive.ReactiveTrader.Shared.DTO.Execution
{
    public class TradeDto
    {
         public long TradeId { get; set; }
         public string TraderName { get; set; }
         public string CurrencyPair { get; set; }
         public long Notional { get; set; }
         public string DealtCurrency { get; set; }
         public DirectionDto Direction { get; set; }
         public decimal SpotRate { get; set; }
         public string TradeDate { get; set; }
         public string ValueDate { get; set; }
         public TradeStatusDto Status { get; set; }

        public override string ToString()
        {
            return
                $"TradeId: {TradeId}, TraderName: {TraderName}, CurrencyPair: {CurrencyPair}, Notional: {Notional}, Direction: {Direction}, SpotRate: {SpotRate}, TradeDate: {TradeDate}, ValueDate: {ValueDate}, Status: {Status}, DealtCurrency: {DealtCurrency}";
        }
    }
}