namespace Bizy.WinBizApi.Models
{
    using Newtonsoft.Json;

    public class BaseResponse
    {
        [JsonProperty("ErrorsCount")]
        public int ErrorsCount { get; set; }

        [JsonProperty("ErrorLast")]
        public string ErrorLast { get; set; }

        [JsonProperty("ErrorsMsg")]
        public string ErrorsMsg { get; set; }
    }
}