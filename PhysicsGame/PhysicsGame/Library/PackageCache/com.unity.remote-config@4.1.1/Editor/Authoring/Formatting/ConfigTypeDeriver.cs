using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Formatting;

namespace Unity.Services.RemoteConfig.Authoring.Editor.Formatting
{
    class ConfigTypeDeriver : IConfigTypeDeriver
    {
        static Dictionary<Type, ConfigType> s_TypeToConfigType = new Dictionary<Type, ConfigType>()
        {
            { typeof(string), ConfigType.STRING },
            { typeof(int), ConfigType.INT },
            { typeof(bool), ConfigType.BOOL },
            { typeof(float), ConfigType.FLOAT },
            { typeof(double), ConfigType.FLOAT },
            { typeof(long), ConfigType.LONG },
            { typeof(JArray), ConfigType.JSON},    
            { typeof(JObject), ConfigType.JSON}
        };

        public ConfigType DeriveType(object obj)
        {
            return s_TypeToConfigType[obj.GetType()];
        }
    }
}