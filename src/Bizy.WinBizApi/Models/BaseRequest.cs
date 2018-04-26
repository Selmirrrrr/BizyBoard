namespace Bizy.WinBizApi.Models
{
    using Newtonsoft.Json;

    public class BaseRequest
    {
        [JsonProperty("Method", Order = 1)]
        public string Method { get; set; }

        [JsonProperty("Parameters", Order = 10)]
        public object[] Parameters { get; set; }

        public BaseRequest(string method, object[] parameters)
        {
            Method = method;
            Parameters = parameters;
        }
    }
}
