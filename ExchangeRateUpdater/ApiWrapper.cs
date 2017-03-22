using System;
using System.Collections.Generic;
using System.Net.Http;
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

        public Dictionary<string, decimal> GetRequest(string baseCurrency, string targetCurrencies)
        {
            var response = _client.GetAsync(string.Format(Url, baseCurrency, targetCurrencies)).Result;
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode.ToString() == "422")
                {
                    return new Dictionary<string, decimal>();
                }
                throw new Exception($"API Error {(int) response.StatusCode}");
            }

            var json = response.Content.ReadAsStringAsync().Result;

            var result = JsonConvert.DeserializeObject<ApiResponce>(json);

            return result.Rates;
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
