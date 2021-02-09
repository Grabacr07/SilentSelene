using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ResinTimer.Serialization
{
    public static class JsonUtil
    {
        private static readonly JsonSerializerSettings _commonSettings
            = new JsonSerializerSettings()
            {
                ContractResolver = new WritableOnlyResolver(),
                Converters = new List<JsonConverter>()
                {
                    new StringEnumConverter(),
                },
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            };

        public static string Serialize<T>(T value)
            => JsonConvert.SerializeObject(value, _commonSettings);

        public static T Deserialize<T>(string json)
            => JsonConvert.DeserializeObject<T>(json, _commonSettings) ?? throw new ArgumentException("Invalid json.", nameof(json));

        public static T DeepCopy<T>(T value)
            => Deserialize<T>(Serialize(value));
    }
}
