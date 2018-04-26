namespace Bizy.WinBizApi.Models
{
    using Helpers;
    using Newtonsoft.Json;

    [JsonConverter(typeof(RequestJsonConverter))]
    public class BaseRequestParams
    {
        public string Param1 { get; set; }
        public int Param2 { get; set; }
        public string Param3 { get; set; }
    }
}