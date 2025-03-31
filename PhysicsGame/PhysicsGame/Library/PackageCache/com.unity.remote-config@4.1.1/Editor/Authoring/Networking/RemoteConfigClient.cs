using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Deployment;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Model;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Service;

namespace Unity.Services.RemoteConfig.Authoring.Editor.Networking
{
    class RemoteConfigClient : IRemoteConfigClient
    {
        IWebApiClient m_WebApiClient;
        IDeploymentInfo m_DeploymentInfo;

        string ConfigId { get; set; }

        public RemoteConfigClient(IWebApiClient webApiClient, IDeploymentInfo deploymentInfo)
        {
            m_WebApiClient = webApiClient;
            m_DeploymentInfo = deploymentInfo;
        }
        
        public async Task UpdateAsync(IReadOnlyList<RemoteConfigEntry> remoteConfigEntries)
        { 
            await m_WebApiClient.Put(
                m_DeploymentInfo.CloudProjectId, 
                m_DeploymentInfo.EnvironmentId, 
                ConfigId, 
                ToDto(remoteConfigEntries));
        }

        public async Task CreateAsync(IReadOnlyList<RemoteConfigEntry> remoteConfigEntries)
        {
            var configId = await m_WebApiClient.Post(
                m_DeploymentInfo.CloudProjectId, 
                m_DeploymentInfo.EnvironmentId, 
                ToDto(remoteConfigEntries));
            ConfigId = configId;
        }

        public async Task<GetConfigsResult> GetAsync()
        {
            var fetchResult = await m_WebApiClient.Fetch(m_DeploymentInfo.CloudProjectId, m_DeploymentInfo.EnvironmentId);

            IReadOnlyList<RemoteConfigEntry> configs = null;
            if (fetchResult.ConfigsExist)
            {
                var entries = JArray.FromObject(fetchResult.ConfigsValue);
                configs = ToRemoteConfigEntry(entries.ToObject<IReadOnlyList<RemoteConfigEntryDTO>>());
                ConfigId = fetchResult.ConfigId;
            }

            return new GetConfigsResult(fetchResult.ConfigsExist, configs);
        }
        
        static IReadOnlyList<RemoteConfigEntryDTO> ToDto(IReadOnlyList<RemoteConfigEntry> entries)
        {
            return entries.Select(entry => new RemoteConfigEntryDTO()
            {
                key = entry.Key,
                type = entry.GetEntryConfigType().ToString().ToLower(),
                value = entry.Value
            }).ToList();
        }
        
        static IReadOnlyList<RemoteConfigEntry> ToRemoteConfigEntry(IReadOnlyList<RemoteConfigEntryDTO> entryDTOs)
        {
            return entryDTOs.Select(dto => dto.ToRemoteConfigEntry()).ToList();
        }
    }
}
