using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json.Linq;

namespace ExchangeRateUpdater
{
    public class ExchangeRateProvider
    {
        private const string Service = "http://api.fixer.io/latest?base={0}&symbols={1}";
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
            var curr = currencies.ToList();
            var param = string.Join(",", curr.Select(c => c.Code));
            if (curr.Count == 0)
            {
                return Enumerable.Empty<ExchangeRate>();
            }

            var exchangeRates = new List<ExchangeRate>();
            using (var webClient = new HttpClient())
            {
                foreach (var cur in curr)
                {
                    var task = webClient.GetAsync(string.Format(Service, cur.Code, param)).Result;

                    if (!task.IsSuccessStatusCode)
                    {
                        // Base currency not exists in api
                        if (task.StatusCode.ToString() == "422")
                        {
                            continue;
                        }
                        throw new HttpException((int)task.StatusCode, "API Error");
                    }
                    var excRate = JObject.Parse(task.Content.ReadAsStringAsync().Result);
                    exchangeRates.AddRange(
                        excRate["rates"].Children()
                            .Select(rate => rate.ToString().Replace("\"", "").Split(':'))
                            .Select(rateArray => new ExchangeRate(cur, new Currency(rateArray[0]), decimal.Parse(rateArray[1]))));
                }
            }
            return exchangeRates;
        }
    }
}
