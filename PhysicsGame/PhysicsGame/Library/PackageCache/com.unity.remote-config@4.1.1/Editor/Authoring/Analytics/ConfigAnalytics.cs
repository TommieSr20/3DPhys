using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.RemoteConfig.Authoring.Editor.Shared.Crypto;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.IO;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Json;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Model;

namespace Unity.Services.RemoteConfig.Authoring.Editor.Analytics
{
    class ConfigAnalytics : IConfigAnalytics
    {
        internal const string EventNameCreate = "remoteconfig_configcreated";
        internal const string EventNameDeployed = "remoteconfig_configdeployed";
        internal const string EventNameDeployedGroup = "remoteconfig_configgroupdeployed";
        const int k_VersionCreate = 2;
        const int k_VersionDeployed = 2;
        const int k_VersionDeployedGroup = 1;

        IAnalyticsWrapper m_AnalyticsWrapper;
        IJsonConverter m_JsonConverter;
        IFileSystem m_FileSystem;
        
        public ConfigAnalytics(IAnalyticsWrapper analyticsWrapper, IJsonConverter jsonConverter, IFileSystem fileSystem)
        {
            m_AnalyticsWrapper = analyticsWrapper;
            m_JsonConverter = jsonConverter;
            m_FileSystem = fileSystem;
            
            m_AnalyticsWrapper.Register(EventNameCreate, k_VersionCreate);
            m_AnalyticsWrapper.Register(EventNameDeployed, k_VersionDeployed);
            m_AnalyticsWrapper.Register(EventNameDeployedGroup, k_VersionDeployedGroup);
        }

        public void SendCreatedEvent()
        {
            m_AnalyticsWrapper.Send(EventNameCreate, null, k_VersionCreate);
        }

        public void SendDeployedEvent(int totalConfigsRequested, IEnumerable<IRemoteConfigFile> validConfigs)
        {
            var remoteConfigFiles = validConfigs.ToList();
            if (totalConfigsRequested == 0
                || remoteConfigFiles.Count == 0)
            {
                return;
            }
            
            foreach (var configFile in remoteConfigFiles)
            {
                var txt = m_FileSystem.ReadAllText(configFile.Path);
                var schema = m_JsonConverter.DeserializeObject<Schema>(txt.Result);
                
                var singleConfigParams = new SingleConfigParams()
                {
                    hashedConfigName = Hash.SHA1(configFile.Name),
                    hasSchema = schema.Value != null,
                };
                m_AnalyticsWrapper.Send(EventNameDeployed, singleConfigParams, k_VersionDeployed);
            }

            var configGroupParams = new ConfigGroupParams()
            {
                nbConfigRequested = totalConfigsRequested,
                nbConfigSuccessful = remoteConfigFiles.Count,
            };
            m_AnalyticsWrapper.Send(EventNameDeployedGroup, configGroupParams, k_VersionDeployedGroup);
        }
    }

    // variable names are lowercase to match naming in schema
    [Serializable]
    struct SingleConfigParams
    {
        public string hashedConfigName;
        public bool hasSchema;
    }

    [Serializable]
    struct ConfigGroupParams
    {
        public int nbConfigRequested;
        public int nbConfigSuccessful;
    }
}
