using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ExchangeRateUpdater
{
    internal class ApiWrapper : IDisposable
    {
        private readonly HttpClient _client;

        private const string Url = "http://api.fixer.io/latest?base={0}&symbols={1}";

        public ApiWrapper()
        {
            _client = new HttpClient();
        }

        public Task<HttpResponseMessage> GetRequest(string baseCurrency, string targetCurrencies)
        {
            return _client.GetAsync(string.Format(Url, baseCurrency, targetCurrencies));
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
