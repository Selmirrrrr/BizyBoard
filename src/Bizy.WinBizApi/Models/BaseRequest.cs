namespace Bizy.WinBizApi.Models
{
    using Newtonsoft.Json;

    public class BaseRequest
    {
        [JsonProperty("Method")]
        public string Method { get; set; }

        [JsonProperty("Parameters")]
        public BaseRequestParams Parameters { get; set; }

        public BaseRequest(string method, BaseRequestParams parameters)
        {
            Method = method;
            Parameters = parameters;
        }
    }
}
