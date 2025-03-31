using System.Collections.Generic;
using System.Linq;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Formatting;

namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.Model
{
    class RemoteConfigFileContent
    {
        // ReSharper disable InconsistentNaming - Used in serialization
        public Dictionary<string, object> entries;
        public Dictionary<string, ConfigType> types;
        // ReSharper restore InconsistentNaming

        public RemoteConfigFileContent()
        {
            entries = new Dictionary<string, object>();
            types = new Dictionary<string, ConfigType>();
        }
        
        public RemoteConfigFileContent(IReadOnlyList<RemoteConfigEntry> remoteConfigEntries)
        {
            entries = new Dictionary<string, object>();
            types = new Dictionary<string, ConfigType>();

            foreach (var entry in remoteConfigEntries)
            {
                UpdateEntry(entry);
            }
        }

        public void UpdateEntry(RemoteConfigEntry entry)
        {
            entries[entry.Key] = entry.Value;
            var type = entry.GetEntryConfigType();
            if (ShouldAddType(type))
            {
                types[entry.Key] = type;
            }
            else
            {
                types.Remove(entry.Key);
            }
        }

        static bool ShouldAddType(ConfigType type)
        {
            return type != ConfigType.BOOL 
                && type != ConfigType.JSON 
                && type != ConfigType.STRING;
        }
        
        public List<RemoteConfigEntry> ToRemoteConfigEntries(IRemoteConfigFile file, IRemoteConfigParser parser)
        {
            return parser.ParseFile(this, file).ToList();
        }
    }
}
