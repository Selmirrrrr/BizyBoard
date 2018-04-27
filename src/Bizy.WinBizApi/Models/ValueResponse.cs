namespace Bizy.WinBizApi.Models
{
    using Newtonsoft.Json;

    public class ValueResponse : BaseResponse
    {
        [JsonProperty("Value")]
        public string Value { get; set; }
    }
}