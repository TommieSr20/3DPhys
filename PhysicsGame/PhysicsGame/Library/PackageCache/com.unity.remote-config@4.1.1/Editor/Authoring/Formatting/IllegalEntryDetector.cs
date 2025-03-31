using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.ErrorHandling;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Formatting;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Model;

namespace Unity.Services.RemoteConfig.Authoring.Editor.Formatting
{
    class IllegalEntryDetector : IIllegalEntryDetector
    {
        static readonly string[] s_ValidEntries = new string[] {"$schema", "entries", "types"};

        public bool ContainsIllegalEntries(
            IRemoteConfigFile remoteConfigFile, 
            ICollection<RemoteConfigDeploymentException> deploymentExceptions)
        {
            using var file = File.OpenText(remoteConfigFile.Path);
            using var jsonReader = new JsonTextReader(file);
            var json = JToken.ReadFrom(jsonReader) as JObject;

            foreach (var entry in json)
            {
                if (!s_ValidEntries.Contains(entry.Key))
                {
                    deploymentExceptions.Add(new FileParseException(remoteConfigFile));
                    return true;
                }
            }

            return false;
        }
    }
}
