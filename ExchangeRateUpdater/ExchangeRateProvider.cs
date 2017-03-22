using System;
using System.Collections.Generic;
using System.Linq;

namespace ExchangeRateUpdater
{
    public class ExchangeRateProvider
    {
        /// <summary>
        /// Should return exchange rates among the specified currencies that are defined by the source. But only those defined
        /// by the source, do not return calculated exchange rates. E.g. if the source contains "EUR/USD" but not "USD/EUR",
        /// do not return exchange rate "USD/EUR" with value calculated as 1 / "EUR/USD". If the source does not provide
        /// some of the currencies, ignore them.
        /// </summary>
        public IEnumerable<ExchangeRate> GetExchangeRates(IEnumerable<Currency> currencies)
        {
            if (currencies == null)
            {
                throw new ArgumentException("Currencies cannot be null");
            }
            var currenciesList = currencies.ToList();

            var targetCurrencies = string.Join(",", currenciesList.Select(c => c.Code));
            if (currenciesList.Count == 0)
            {
                return Enumerable.Empty<ExchangeRate>();
            }
            var exchangeRates = new List<ExchangeRate>();
            var api = new ApiWrapper();

            foreach (var cur in currenciesList)
            {
                var stringses = api.GetRequest(cur.Code, targetCurrencies);

                exchangeRates.AddRange(stringses.Select(k => new ExchangeRate(cur, new Currency(k.Key), k.Value)));
            }
            return exchangeRates;

        }
    }
}
