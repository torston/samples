using System.Collections.Generic;
using Newtonsoft.Json;

namespace ExchangeRateUpdater
{
    public class ApiResponce
    {
        [JsonProperty("base")]
        public string Currency;
        [JsonProperty("rates")]
        public Dictionary<string, decimal> Rates;
    }
}
