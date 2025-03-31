using System;
using Newtonsoft.Json.Linq;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Formatting;

namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.Model
{
    class RemoteConfigEntry
    {
        public string Key;
        public object Value;
        public IRemoteConfigFile File;
        
        public ConfigType GetEntryConfigType()
        {
            switch (Value)
            {
                case string:
                    return ConfigType.STRING;
                case int:
                    return ConfigType.INT;
                case bool:
                    return ConfigType.BOOL;
                case float:
                case double:
                    return ConfigType.FLOAT;
                case long:
                    return ConfigType.LONG;
                case JArray:
                case JObject:
                    return ConfigType.JSON;
                default:
                    throw new ArgumentNullException(nameof(Value));
            }
        }
    }
}