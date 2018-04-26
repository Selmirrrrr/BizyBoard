namespace Bizy.WinBizApi.Models
{
    using System;
    using System.Linq;
    using Extensions;
    using Newtonsoft.Json;

    public class BaseRequest
    {
        [JsonProperty("Method", Order = 1)]
        public string Method { get; set; }
    }

    public class GetStockParams : BaseRequest
    {
        [JsonProperty("Parameters", Order = 10)]
        public object[] Parameters { get; set; }

        public GetStockParams(string cMethod, int nItem, DateTime? dDateEnd = null, DateTime? dDateStart = null, int? nWarehouse = null, DateTime? dExpiryEnd = null, DateTime? dExpiryStart = null)
        {
            Method = "Stock";
            Parameters = new object[] { cMethod, nItem, dDateEnd?.ToWinBizString(), dDateStart?.ToWinBizString(), nWarehouse?.ToString(), dExpiryEnd?.ToWinBizString(), dExpiryStart?.ToWinBizString() };
            Parameters = Parameters.AsEnumerable().Where(p => p != null).ToArray();
        }
    }
}
