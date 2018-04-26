namespace Bizy.WinBizApi.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class ListResponse<T> : BaseResponse
    {
        [JsonProperty("Value")]

        public List<T> Values { get; set; }
    }
}