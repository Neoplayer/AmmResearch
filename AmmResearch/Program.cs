using System;
using System.Collections.Generic;

namespace AmmResearch
{
    class Program
    {
        static void Main()
        {
            AmmPair pair = new AmmPair("SCRT", "USDT", 41171.48M, 131611.94M);
            Console.WriteLine($"{pair.Ticker}={pair.Price}");
            pair.CalculateIoForPrice(3.40M, out decimal pooledBaseDiff, out decimal pooledQuoteDiff);
            Console.WriteLine($"diff {pair.BaseName} = {pooledBaseDiff} diff {pair.QuoteName} ={pooledQuoteDiff}");
            decimal amountAdd = pooledQuoteDiff;
            Console.WriteLine($"Added {amountAdd:0.000} {pair.QuoteName} into {pair.Ticker} pool");
            decimal baseReturn = pair.AddQuote(amountAdd);
            Console.WriteLine($"{pair.Ticker}={pair.Price}  return={baseReturn} {pair.BaseName}");
        }
    }
    public class AmmPair
    {
        private decimal FeeRate { get; set; }
        public string BaseName { get; set; }
        public string QuoteName { get; set; }
        public string Ticker => BaseName + QuoteName;
        public decimal AmountBase { get; set; }
        public decimal AmountQuote { get; set; }
        public decimal K { get; set; }
        public decimal Price => AmountQuote / AmountBase;
        public AmmPair(string baseName, string quoteName, decimal amountBase, decimal amountQuote)
        {
            BaseName = baseName;
            QuoteName = quoteName;
            AmountBase = amountBase;
            AmountQuote = amountQuote;
            K = AmountBase * AmountQuote;
            FeeRate = 0.003M;
        }
        public decimal AddBase(decimal addedBaseAmount)
        {
            if (addedBaseAmount < 0M)
                if (Math.Abs(addedBaseAmount) >= AmountBase)
                    throw new ArgumentOutOfRangeException();
            AmountBase += addedBaseAmount;
            decimal newAmountQuote = K / AmountBase;
            decimal wipedQuoteAmount = AmountQuote - newAmountQuote;
            AmountQuote = newAmountQuote;
            return wipedQuoteAmount / (1M + FeeRate);
        }
        public decimal AddQuote(decimal addedQuoteAmount)
        {
            if (addedQuoteAmount < 0M)
                if (Math.Abs(addedQuoteAmount) > AmountQuote)
                    throw new ArgumentOutOfRangeException();
            AmountQuote += addedQuoteAmount;
            decimal newAmountBase = K / AmountQuote;
            decimal wipedBaseAmount = AmountBase - newAmountBase;
            AmountBase = newAmountBase;
            return wipedBaseAmount / (1M + FeeRate);
        }
        public void CalculateIoForPrice(decimal price, out decimal diffBase, out decimal diffQuote)
        {
            if (price <= 0M)
                throw new FormatException();
            decimal newAmountQuote = (decimal)Math.Sqrt((double)(K * price));
            decimal newAmountBase = (decimal)Math.Sqrt((double)(K / price));
            diffBase = newAmountBase - AmountBase;
            diffQuote = newAmountQuote - AmountQuote;
        }
    }
}