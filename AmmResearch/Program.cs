using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace AmmResearch
{

    public class Program
    {
        //static List<DateTime> calls = new List<DateTime>();
        //public class Matrix
        //{
        //    HashSet<Token> tokens = new HashSet<Token>();
        //    HashSet<Pair> pairs = new HashSet<Pair>();

        //    public void Init()
        //    {
        //        string json = File.ReadAllText("pairs.json");               
        //        SecretSwapPairsResponse pairs = JsonConvert.DeserializeObject<SecretSwapPairsResponse>(json);
        //        foreach (var pair in pairs.Pairs)
        //        {
        //            tokens.Add();
        //            //tokens.Add(pair.AssetInfos.Last());
        //        }
        //    }
        //    public void Loop()
        //    {
        //        SecretCli secretcli = new SecretCli("secretcli.exe", "secret170semenfmug96mkglt3kf4vuyn566lfx2uxzgg");
        //        while (true)
        //        {
        //            foreach (Pair pair in pairs)
        //            {
        //                try
        //                {
        //                    BigInteger vol0 = secretcli.QuerySnip20Balance(pair.Token0, pair.Address);
        //                    BigInteger vol1 = secretcli.QuerySnip20Balance(pair.Token1, pair.Address);
        //                    decimal vol0dec = decimal.Parse(vol0.ToString()) / (decimal)Math.Pow(10, pair.Token0.Decimals);
        //                    decimal vol1dec = decimal.Parse(vol1.ToString()) / (decimal)Math.Pow(10, pair.Token1.Decimals);
        //                    decimal rate = vol1dec / vol0dec;
        //                    decimal inverse = 1 / rate;
        //                    Console.Write($"|{pair.Token0.Symbol}{pair.Token1.Symbol}={rate:0.000000000}({inverse:0.000000000})| ");
        //                }
        //                catch (Exception e)
        //                {
        //                    Console.WriteLine(e.Data);
        //                    Console.WriteLine(e.Message);
        //                    Console.WriteLine(e.StackTrace);
        //                }
        //            }
        //            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss"));
        //            Thread.Sleep(10000);
        //        }
        //    }
        //}
        //static void Call()
        //{
        //    Console.WriteLine("Calling...");

        //    Dictionary<string, string> data = new Dictionary<string, string>
        //    {
        //        { "time", "1603337755" },
        //        { "n", "+74959999999" }
        //    };
        //    try
        //    {
        //        calls.Add(DateTime.Now);
        //        Browser browser = new Browser();
        //        string response = browser.UrlPostToHttp("https://cb.zadarma.com/4f1ee08c0c/", data).Result.Content.ReadAsStringAsync().Result;
        //        Console.WriteLine(response);
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine($"Exception while trying to call.");
        //        Console.WriteLine(e);
        //    }
        //}
        static void Main()
        {

            SecretSecretBot bot = new SecretSecretBot();
            bot.Init();
            bot.Work();

            //Matrix matrix = new Matrix();
            //matrix.Init();
            //matrix.Loop();

            // Call();
            //Token sscrt = new Token
            //{
            //    Address = "secret1k0jntykt7e4g3y88ltc60czgjuqdy4c9e8fzek",
            //    Symbol = "sscrt",
            //    Decimals = 6
            //};
            //string sscrtScrtPairAddress = "secret1rf4uqg4d2elmvp535ayhxwnrpdykmxan0nrwtg";
            //SecretCli secretcli = new SecretCli("secretcli.exe");
            //while (true)
            //{
            //    try
            //    {
            //        BigInteger snip20Output = secretcli.QuerySnip20Balance(sscrt, sscrtScrtPairAddress);
            //        BigInteger nativeOutput = secretcli.QueryBalance(sscrtScrtPairAddress);
            //        AmmPair pair = new AmmPair("sscrt", "scrt", (decimal)snip20Output / 1000000, (decimal)nativeOutput / 1000000);

            //        pair.CalculateIoForPrice(1M, out decimal pooledBaseDiff, out decimal pooledQuoteDiff);
            //        string substring = string.Empty;
            //        if (pair.AmountBase <= pair.AmountQuote)
            //        {
            //            substring = $"{pooledBaseDiff} {pair.BaseName}";
            //        }
            //        else
            //        {
            //            substring = $"{pooledQuoteDiff} {pair.QuoteName}";
            //        }
            //        if (pooledBaseDiff > 800 || pooledQuoteDiff > 800)
            //        {
            //            Console.BackgroundColor = ConsoleColor.Red;
            //        }
            //        Console.Write($"[{pair.AmountBase:0} sscrt {pair.AmountQuote:0} scrt] price = {pair.Price:0.0000}  {substring}  {DateTime.Now:[HH:mm:ss]}           ");
            //        Console.ResetColor();
            //        Thread.Sleep(TimeSpan.FromSeconds(15));
            //        Console.WriteLine();
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(e.Data);
            //        Console.WriteLine(e.Message);
            //        Console.WriteLine(e.StackTrace);
            //    }
            //}

            //AmmPair pair = new AmmPair("scrt", "susdt", 363076M, 1193905M);
            //Console.WriteLine($"{pair.Ticker}={pair.Price}");
            //pair.CalculateIoForPrice(3.42M, out decimal pooledBaseDiff, out decimal pooledQuoteDiff);
            //Console.WriteLine($"diff {pair.BaseName} = {pooledBaseDiff} diff {pair.QuoteName} ={pooledQuoteDiff}");

            //decimal amountAdd = pooledBaseDiff;
            //decimal baseReturn = pair.AddBase(amountAdd);
            //Console.WriteLine(baseReturn);
            //Console.WriteLine($"Added {amountAdd:0.000} {pair.QuoteName} into {pair.Ticker} pool");
            //Console.WriteLine($"{pair.Ticker}={pair.Price}  return={baseReturn} {pair.BaseName}");
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
                decimal wipedQuoteAmount = (AmountQuote - newAmountQuote) * (1M - FeeRate);
                AmountQuote -= wipedQuoteAmount;
                return wipedQuoteAmount;
            }
            public decimal AddQuote(decimal addedQuoteAmount)
            {
                if (addedQuoteAmount < 0M)
                    if (Math.Abs(addedQuoteAmount) > AmountQuote)
                        throw new ArgumentOutOfRangeException();
                AmountQuote += addedQuoteAmount;
                decimal newAmountBase = K / AmountQuote;
                decimal wipedBaseAmount = (AmountBase - newAmountBase) * (1M - FeeRate);
                AmountBase -= wipedBaseAmount;
                return wipedBaseAmount;
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
            public decimal AddQuoteVirtual(decimal addedQuoteAmount)
            {
                if (addedQuoteAmount < 0M)
                    if (Math.Abs(addedQuoteAmount) > AmountQuote)
                        throw new ArgumentOutOfRangeException();

                decimal newAmountBase = K / (AmountQuote + addedQuoteAmount);
                decimal wipedBaseAmount = (AmountBase - newAmountBase) * (1M - FeeRate);
                AmountBase -= wipedBaseAmount;
                return wipedBaseAmount;
            }
        }
       
    }
}