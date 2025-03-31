using System.Collections.Generic;
using System.Linq;

namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.Model
{
    class ConfigMerger : IConfigMerger
    {
        public IReadOnlyList<RemoteConfigEntry> MergeEntriesToDeploy(
            IReadOnlyList<RemoteConfigEntry> toCreate,
            IReadOnlyList<RemoteConfigEntry> toUpdate,
            IReadOnlyList<RemoteConfigEntry> toDelete,
            IReadOnlyList<RemoteConfigEntry> remoteConfigEntries)
        {
            var localEntries = toCreate.Concat(toUpdate).ToList();
            var remoteEntries = remoteConfigEntries.Except(toDelete).ToList();
            
            return MergeConfigs(localEntries, remoteEntries);
        }
        
        static IReadOnlyList<RemoteConfigEntry> MergeConfigs(
            IReadOnlyList<RemoteConfigEntry> clientConfigs, 
            IReadOnlyList<RemoteConfigEntry> configsFromRemote)
        {
            var clientConfigsList = clientConfigs.ToList();

            var clientKeys = clientConfigsList.Select(config => config.Key).ToList();
            var remoteKeys = configsFromRemote.Select(config => config.Key).ToList();

            var conflicts = clientKeys.Intersect(remoteKeys);
            var cleanedUpConfigsFromRemote = 
                configsFromRemote.Where(token => !conflicts.Contains(token.Key));

            var finalConfigs = new List<RemoteConfigEntry>();
            
            finalConfigs.AddRange(clientConfigsList);
            finalConfigs.AddRange(cleanedUpConfigsFromRemote);

            return finalConfigs;
        }
    }
}
