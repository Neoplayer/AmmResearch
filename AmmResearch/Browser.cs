using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AmmResearch
{
    class Browser : IDisposable
    {
        internal const byte MaxTries = 5;
        private const byte MaxConnections = 10; // Defines maximum number of connections per ServicePoint. Be careful, as it also defines maximum number of sockets in CLOSE_WAIT state
        private const byte MaxIdleTime = 15; // Defines in seconds, how long socket is allowed to stay in CLOSE_WAIT state after there are no connections to it
        internal readonly CookieContainer CookieContainer = new CookieContainer();
        private HttpClient HttpClient;
        internal Browser()
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = true, // This must be false if we want to handle custom redirection schemes such as "steammobile"
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                CookieContainer = CookieContainer,
                MaxConnectionsPerServer = MaxConnections,
                UseProxy = false
            };
            HttpClient = new HttpClient(httpClientHandler) { Timeout = TimeSpan.FromSeconds(60) };
            HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Sutox/0.1");
        }
        public void Dispose() => HttpClient.Dispose();
        internal Task<HttpResponseMessage> UrlPostToHttp(string requestUri, IReadOnlyCollection<KeyValuePair<string, string>> data = null, string referer = null)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            if (data != null)
            {
                try
                {
                    request.Content = new FormUrlEncodedContent(data);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            if (!string.IsNullOrEmpty(referer))
            {
                request.Headers.Referrer = new Uri(referer);
            }
            string json = JsonConvert.SerializeObject(request, Formatting.Indented);
            var result = HttpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);

            return result;
        }
        internal Task<string> UrlPostToString(string requestUri, IReadOnlyCollection<KeyValuePair<string, string>> data = null, string referer = null)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            if (data != null)
            {
                try
                {
                    request.Content = new FormUrlEncodedContent(data);
                }
                catch (UriFormatException e)
                {
                    Console.WriteLine($"{ToString()} {e.StackTrace}");
                }
            }
            if (!string.IsNullOrEmpty(referer))
            {
                request.Headers.Referrer = new Uri(referer);
            }
            return HttpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead).Result.Content.ReadAsStringAsync();
        }
        internal Task<HttpResponseMessage> UrlGetToHttp(string request, string referer = null)
        {
            return Task.Run(() =>
            {
                try
                {
                    HttpRequestMessage requestMsg = new HttpRequestMessage(HttpMethod.Get, request);
                    if (!string.IsNullOrEmpty(referer))
                    {
                        requestMsg.Headers.Referrer = new Uri(referer);
                    }
                    HttpResponseMessage httpRespMsg = HttpClient.SendAsync(requestMsg, HttpCompletionOption.ResponseContentRead).Result;
                    return httpRespMsg;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
            });
        }
        internal Task<string> UrlGetToString(string request, string referer = null)
        {
            return Task.Run(() =>
            {
                try
                {
                    HttpRequestMessage requestMsg = new HttpRequestMessage(HttpMethod.Get, request);
                    if (!string.IsNullOrEmpty(referer))
                    {
                        requestMsg.Headers.Referrer = new Uri(referer);
                    }
                    var httpRespMsg = HttpClient.SendAsync(requestMsg, HttpCompletionOption.ResponseContentRead).Result;
                    string content = httpRespMsg.Content.ReadAsStringAsync().Result;
                    return content;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                    return null;
                }
            });
        }
    }
}
