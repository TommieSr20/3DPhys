using Newtonsoft.Json.Linq;

namespace Unity.Services.RemoteConfig.Authoring.Editor.Networking
{
    class FetchResult
    {
        const string k_ConfigType = "settings";
        
        public bool ConfigsExist { get; }
        public string ConfigId { get; }
        public object ConfigsValue { get; }
        
        public FetchResult(JObject config)
        {
            if (config != null)
            {
                if (config["type"]?.Value<string>() == k_ConfigType)
                {
                    ConfigsExist = true;
                    ConfigId = config["id"].Value<string>();;
                }
                    
                ConfigsValue = config["value"]?.Value<JArray>();
            }
        }

        public override string ToString()
        {
            return $"ConfigsExist: {ConfigsExist}, ConfigId: {ConfigId}, ConfigsValue: {ConfigsValue}";
        }
    }
}
