using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Model;

namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.Service
{
    interface IRemoteConfigClient
    {
        Task UpdateAsync(IReadOnlyList<RemoteConfigEntry> remoteConfigEntries);
        Task CreateAsync(IReadOnlyList<RemoteConfigEntry> remoteConfigEntries);
        Task<GetConfigsResult> GetAsync();
    }

    struct GetConfigsResult
    {
        public bool ConfigsExists { get; }
        public IReadOnlyList<RemoteConfigEntry> Configs { get; }

        public GetConfigsResult(bool configsExists, IReadOnlyList<RemoteConfigEntry> configs)
        {
            ConfigsExists = configsExists;
            Configs = configs;
        }
    }
}
