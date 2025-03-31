using System.Collections.Generic;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Formatting;

namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.Model
{
    class RemoteConfigParser : IRemoteConfigParser
    {
        IConfigTypeDeriver m_ConfigTypeDeriver;

        public RemoteConfigParser(IConfigTypeDeriver configTypeDeriver)
        {
            m_ConfigTypeDeriver = configTypeDeriver;
        }
        
        public IReadOnlyList<RemoteConfigEntry> ParseFile(RemoteConfigFileContent content, IRemoteConfigFile file)
        {
            var remoteConfigEntries = new List<RemoteConfigEntry>();
            foreach (var entry in content.entries)
            {
                var type = m_ConfigTypeDeriver.DeriveType(entry.Value);
                
                if (content.types != null && content.types.ContainsKey(entry.Key))
                {
                    type = content.types[entry.Key];
                }
                
                var value = CastToCorrectType(entry, type);

                var newEntry = new RemoteConfigEntry()
                {
                    Key = entry.Key,
                    Value = value,
                    File = file
                };
                
                remoteConfigEntries.Add(newEntry);
            }

            return remoteConfigEntries;
        }
        
        internal static object CastToCorrectType(KeyValuePair<string, object> entry, ConfigType type)
        {
            var value = entry.Value;

            switch (type)
            {
                case ConfigType.INT:
                    if (entry.Value is long longValue)
                    {
                        value = (int)longValue;
                    }
                    break;
                case ConfigType.FLOAT:
                    if (entry.Value is long longValue2)
                    {
                        value = (double)longValue2;
                    }
                    break;
            }

            return value;
        }
    }
}
