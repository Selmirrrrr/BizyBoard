namespace Bizy.WinBizApi.Models
{
    using Newtonsoft.Json;

    public class ValueResponse : BaseResponse
    {
        [JsonProperty("Value")]
        public decimal Value { get; set; }
    }
}