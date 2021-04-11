using AmmResearch.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Numerics;

namespace AmmResearch
{
    public class SecretCli
    {
        Process p;
        public string Address { get; set; }
        public string objectLock = "lockme";
        public SecretCli(string filename, string address)
        {
            p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = filename;
            Address = address;
        }
        private string ExecuteCmd(string args)
        {
            string output = string.Empty;
            lock (objectLock)
            {
                p.StartInfo.Arguments = args;
                p.Start();
                output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
            }
            return output;
        }
        public void ConvertScrtToSscrt(BigInteger amount)
        {
            string args = $"tx snip20 deposit secret1k0jntykt7e4g3y88ltc60czgjuqdy4c9e8fzek --amount {amount}uscrt --from {Address} -y";
            Console.WriteLine(ExecuteCmd(args));
        }
        public void ConvertSscrtToScrt(BigInteger amount)
        {
            string args = $"tx snip20 redeem secret1k0jntykt7e4g3y88ltc60czgjuqdy4c9e8fzek {amount} --from {Address} -y";
            Console.WriteLine(ExecuteCmd(args));
        }
        public BigInteger QueryBalance(string address)
        {
            string args = $"query account {address}";
            string output = ExecuteCmd(args);
            JToken jt = JToken.Parse(output);
            string foobar = jt["value"]["coins"].First["amount"].ToString();
            bool parsingResult = BigInteger.TryParse(foobar, out BigInteger balance);
            return balance;
        }
        public BigInteger QuerySnip20Balance(Token token, string address, string viewingKey)
        {
            string args = $"query snip20 balance {token.Address} {address} {viewingKey}";
            string output = ExecuteCmd(args);
            JToken jt = JToken.Parse(output);
            string foobar = jt["balance"]["amount"].ToString();
            bool parsingResult = BigInteger.TryParse(foobar, out BigInteger balance);
            return balance;
        }
        public void SwapSnip2Snip(Token token, Pair pair, BigInteger input, BigInteger minOutput)
        {
            string msg = $"{{\"swap\":{{\"expected_return\":\"{1000}\"}}}}";
            var plainBytes = System.Text.Encoding.UTF8.GetBytes(msg);
            string base64msg = Convert.ToBase64String(plainBytes);
            string args = $"tx compute execute {token.Address} \"" +
                $"{{\\\"send\\\": {{\\\"recipient\\\": \\\"{pair.Address}\\\"," +
                $"\\\"amount\\\": \\\"{input}\\\",\\\"msg\\\": \\\"{base64msg}\\\"}}}}\" " +
                $"--from ban --gas 360000 --gas-prices 0.25uscrt -y";
            Console.WriteLine(ExecuteCmd(args));
        }
        public void SwapSnipToNative(Token token, Pair pair, BigInteger input, BigInteger minOutput)
        {
            string msg = $"{{\"swap\":{{\"expected_return\":\"{minOutput}\"}}}}";
            var plainBytes = System.Text.Encoding.UTF8.GetBytes(msg);
            string base64msg = Convert.ToBase64String(plainBytes);
            string args = $"tx compute execute {token.Address} \"" +
                $"{{\\\"send\\\": {{\\\"recipient\\\": \\\"{pair.Address}\\\"," +
                $"\\\"amount\\\": \\\"{input}\\\",\\\"msg\\\": \\\"{base64msg}\\\"}}}}\" " +
                $"--from ban --gas 260000 --gas-prices 0.25uscrt -y";
            Console.WriteLine(ExecuteCmd(args));
        }
        public void SwapNativeToSnip(BigInteger input, BigInteger minOutput)
        {
            string contractAddr = "secret1rf4uqg4d2elmvp535ayhxwnrpdykmxan0nrwtg";
            string msg = $"{{\"swap\":{{\"offer_asset\":{{\"info\":{{\"native_token\":{{\"denom\":\"uscrt\"}}}}," +
                $"\"amount\":\"{input}\"}},\"expected_return\":\"{minOutput}\"}}}}";
            var plainBytes = System.Text.Encoding.UTF8.GetBytes(msg);
            string base64msg = Convert.ToBase64String(plainBytes);
            string args = $""; // TODO
        }
    }
}