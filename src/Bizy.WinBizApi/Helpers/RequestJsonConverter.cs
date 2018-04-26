namespace Bizy.WinBizApi.Helpers
{
    using System;
    using Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class RequestJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(BaseRequestParams);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) => throw new NotImplementedException();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var req = (BaseRequestParams)value;
            var ja = new JArray
            {
                req.Param1,
                req.Param2,
                req.Param3
            };
            ja.WriteTo(writer);
        }
    }
}
