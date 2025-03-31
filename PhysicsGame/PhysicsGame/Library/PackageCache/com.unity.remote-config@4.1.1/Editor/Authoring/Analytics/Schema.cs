using Newtonsoft.Json;

namespace Unity.Services.RemoteConfig.Authoring.Editor.Analytics
{
    class Schema
    {
        [JsonProperty("$schema")]
        public string Value;
    }
}
