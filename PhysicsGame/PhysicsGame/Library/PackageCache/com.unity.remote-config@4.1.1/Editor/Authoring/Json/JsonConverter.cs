using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Unity.Services.RemoteConfig.Authoring.Editor.Shared.Serialization;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Json;

namespace Unity.Services.RemoteConfig.Authoring.Editor.Json
{
    class JsonConverter : IJsonConverter
    {
        public T DeserializeObject<T>(string value, bool matchCamelCaseFieldName = false)
        {
            var contractResolver = matchCamelCaseFieldName ? new CamelCasePropertyNamesContractResolver() : new DefaultContractResolver();

            var settings = new JsonSerializerSettings()
            {
                ContractResolver = contractResolver
            };

            try
            {
                return IsolatedJsonConvert.DeserializeObject<T>(value, settings);
            }
            catch
            {
                return default;
            }
        }

        public string SerializeObject<T>(T obj)
        {
            var stringEnumConverter = new StringEnumConverter();
            return IsolatedJsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented, stringEnumConverter);
        }
    }
}
