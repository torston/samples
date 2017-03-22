using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ExchangeRateUpdater
{
    public class ExchangeRateProvider
    {
        private const string InvalidCurrencyCode = "422";
        private readonly ApiWrapper _apiWrapper = new ApiWrapper();

        public IEnumerable<ExchangeRate> GetExchangeRates(IEnumerable<Currency> currencies)
        {
            if (currencies == null)
            {
                throw new ArgumentException("Currencies cannot be null");
            }
            var currenciesList = new HashSet<Currency>(currencies);

            if (currenciesList.Count == 0)
            {
                return Enumerable.Empty<ExchangeRate>();
            }
            var exchangeRates = new List<ExchangeRate>();

            var targetCurrencies = string.Join(",", currenciesList.Select(c => c.Code));
            var tasks = currenciesList.Select(cur => _apiWrapper.GetRequest(cur.Code, targetCurrencies)).ToArray();

            Task.WaitAll(tasks);

            foreach (var t in tasks)
            {
                var response = t.Result;

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode.ToString() == InvalidCurrencyCode)
                    {
                        continue;
                    }
                    throw new Exception($"API Error {(int)response.StatusCode}");
                }

                var json = response.Content.ReadAsStringAsync().Result;

                var result = JsonConvert.DeserializeObject<ApiResponce>(json);
                exchangeRates
                    .AddRange(result.Rates.Select(k => new ExchangeRate(new Currency(result.Currency), new Currency(k.Key), k.Value)));
            }

            _apiWrapper.Dispose();

            return exchangeRates;
        }
    }
}
