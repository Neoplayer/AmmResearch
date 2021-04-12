using AmmResearch.Models;
using System;
using System.Numerics;
using System.Threading;

namespace AmmResearch
{
    public class SecretSecretBot
    {
        SecretCli secretcli;
        Token sscrt;
        Pair pair;
        string sscrtScrtPairAddress;
        BigInteger nativeBalance;
        BigInteger sscrtBalance;
        BigInteger prevNativeBalance = BigInteger.MinusOne;
        BigInteger prevSscrtBalance = BigInteger.MinusOne;
        bool isBalanceFresh = false;
        ulong dec = 1000000;
        decimal feeCoeff = 0.997M;

        public void Balancer()
        {
            while (true)
            {
                if (!isBalanceFresh)
                {
                    Console.WriteLine($"Checking balances...");
                    try
                    {
                        nativeBalance = secretcli.QueryBalance("secret170semenfmug96mkglt3kf4vuyn566lfx2uxzgg");
                        sscrtBalance = secretcli.QuerySnip20Balance(sscrt, "secret170semenfmug96mkglt3kf4vuyn566lfx2uxzgg", sscrt.ViewingKey);
                        if (sscrtBalance > BigInteger.MinusOne && nativeBalance > BigInteger.MinusOne)
                        {
                            if (prevNativeBalance != nativeBalance || prevSscrtBalance != sscrtBalance)
                            {
                                Console.WriteLine($"{nativeBalance / dec}.{nativeBalance % dec:000000} native scrt\n" +
                                    $"{sscrtBalance / dec}.{sscrtBalance % dec:000000} sscrt");
                                isBalanceFresh = true;
                                prevNativeBalance = nativeBalance;
                                prevSscrtBalance = sscrtBalance;
                                continue;
                            }
                            else
                            {
                                Console.WriteLine($"Waiting for balance to change...");
                                Thread.Sleep(TimeSpan.FromMilliseconds(300));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Data);
                        Console.WriteLine(e.Message);
                        Console.WriteLine(e.StackTrace);
                    }
                    Console.WriteLine("Balance seems old, check again in 5 sec...");
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                    continue;
                }
                Thread.Sleep(TimeSpan.FromMilliseconds(100));
                Console.Write(".");
            }
        }
        public void Init()
        {
            secretcli = new SecretCli("secretcli.exe", "secret170semenfmug96mkglt3kf4vuyn566lfx2uxzgg");
            sscrt = new Token
            {
                Address = "secret1k0jntykt7e4g3y88ltc60czgjuqdy4c9e8fzek",
                Symbol = "sscrt",
                Decimals = 6,
                ViewingKey = "api_key_fsXkkKs7LOcX2RtCwxAgSZ8fxrtnlIQlFIJ6Opdf2vw="
            };
            sscrtScrtPairAddress = "secret1rf4uqg4d2elmvp535ayhxwnrpdykmxan0nrwtg";
            pair = new Pair
            {
                Address = sscrtScrtPairAddress
            };
            Thread thrd = new Thread(Balancer);
            thrd.Start();
        }
        public void ApeIn(bool isPutSscrt, BigInteger amount)
        {
            if (isPutSscrt)
            {
                secretcli.SwapSnipToNative(sscrt, pair, amount, amount);
                isBalanceFresh = false;
                while (true)
                {
                    if (isBalanceFresh)
                    {
                        break;
                    }
                    Thread.Sleep(TimeSpan.FromMilliseconds(400));
                    Console.WriteLine("+");
                }
            }
            else
            {
                secretcli.SwapNativeToSnip(amount, amount);
                isBalanceFresh = false;
                while (true)
                {
                    if (isBalanceFresh)
                    {
                        break;
                    }
                    Thread.Sleep(TimeSpan.FromMilliseconds(400));
                    Console.WriteLine("+");
                }
            }
        }
        public void FuelUpWithScrt()
        {
            double totalBalance = (double)(nativeBalance + sscrtBalance);
            double sscrtShare = (double)sscrtBalance / totalBalance;
            if (totalBalance > 5000000)
            {
                if (sscrtShare < 0.7)
                {
                    Console.WriteLine("Converting scrt to sscrt");
                    double scrtToConvert = totalBalance * 0.8 - (double)sscrtBalance;
                    secretcli.ConvertScrtToSscrt(new BigInteger(scrtToConvert));
                    isBalanceFresh = false;
                    while (true)
                    {
                        if (isBalanceFresh)
                        {
                            break;
                        }
                        Thread.Sleep(TimeSpan.FromMilliseconds(400));
                        Console.Write("+");
                    }
                }
                if (nativeBalance < 5000000)
                {
                    Console.WriteLine("Converting sscrt to scrt");
                    double sscrtToConvert = totalBalance * 0.2 - (double)nativeBalance;
                    secretcli.ConvertSscrtToScrt(new BigInteger(sscrtToConvert));
                    isBalanceFresh = false;
                    while (true)
                    {
                        if (isBalanceFresh)
                        {
                            break;
                        }
                        Thread.Sleep(TimeSpan.FromMilliseconds(400));
                        Console.Write("-");
                    }
                }
            }
            else
            {
                Console.WriteLine("Total balance lower than 5scrt");
            }
        }
        public void Work()
        {
            decimal lastScrtLpBalance = 0M;
            while (true)
            {
                if (!isBalanceFresh)
                {
                    Thread.Sleep(300);
                    continue;
                }
                try
                {
                    FuelUpWithScrt();
                    decimal scrtLpBalance = (decimal)secretcli.QueryBalance(sscrtScrtPairAddress);
                    Console.WriteLine($"lp scrt balance = {scrtLpBalance / 1000000:0.00} {DateTime.Now.ToString("HH:mm:ss")}");
                    if (scrtLpBalance == lastScrtLpBalance)
                    {
                        Thread.Sleep(3000);
                        continue;
                    }
                    decimal sscrtLpBalance = (decimal)secretcli.QuerySnip20Balance(sscrt, sscrtScrtPairAddress, "SecretSwap");
                    double k = (double)scrtLpBalance * (double)sscrtLpBalance;
                    decimal avg = (decimal)Math.Sqrt(k);
                    Console.WriteLine($"avg = {new BigInteger(avg) / 1000000}");

                    decimal deficiency = avg - sscrtLpBalance;

                    if (deficiency >= 0)  //should be >= 0 
                    {
                        // too much scrt in the pool, we wanna add sscrt
                        // expected return = scrtBalance - avg; 
                        decimal expectedReturn = scrtLpBalance - avg;
                        decimal leftover = avg * (1 - feeCoeff) * 0.5M;
                        BigInteger amountPut = new BigInteger(expectedReturn - leftover);
                        if (amountPut <= 0)
                        {
                            Console.WriteLine("There is no arbitrage opportunity");
                            lastScrtLpBalance = scrtLpBalance;
                        }
                        else
                        {
                            Console.WriteLine($"Should put {amountPut / 1000000}.{amountPut % 1000000:000000} sscrt");
                            ApeIn(true, BigInteger.Min(amountPut, sscrtBalance));
                            continue;
                        }
                    }
                    else
                    {
                        decimal expectedReturn = sscrtLpBalance - avg;
                        decimal leftover = avg * (1 - feeCoeff) * 0.5M;
                        BigInteger amountPut = new BigInteger(expectedReturn - leftover);

                        if (amountPut <= 0)
                        {
                            Console.WriteLine("There is no arbitrage opportunity");
                            lastScrtLpBalance = scrtLpBalance;
                        }
                        else
                        {
                            Console.WriteLine($"Should put {amountPut / 1000000}.{amountPut % 1000000:000000} sscrt");
                            if (nativeBalance > 5000000)
                            {
                                ApeIn(false, BigInteger.Min(amountPut, nativeBalance - 2000000));
                            }
                            else
                            {
                                Console.WriteLine($"Insufficient funds. We have {nativeBalance} uscrt");
                            }
                        }


                        lastScrtLpBalance = scrtLpBalance;
                        // too much sscrt in the pool, we wanna add scrt
                        //decimal expectedReturn = sscrtLpBalance - avg;
                        //decimal leftover = avg * (1 - feeCoeff) * 0.5M;
                        //BigInteger amountPut = new BigInteger(expectedReturn - leftover);
                        //Console.WriteLine($"Should put {amountPut / 1000000}.{amountPut % 1000000:000000} scrt");
                        //ApeIn(false, amountPut);
                        //continue;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Data);
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
                Console.WriteLine("Sleep 5 sec.");
                Thread.Sleep(5000);
            }
        }
    }
}
