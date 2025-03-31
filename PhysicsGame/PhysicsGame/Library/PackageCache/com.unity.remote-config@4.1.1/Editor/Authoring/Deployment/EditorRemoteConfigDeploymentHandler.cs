using System.Collections.Generic;
using Unity.Services.DeploymentApi.Editor;
using Unity.Services.RemoteConfig.Authoring.Editor.Analytics;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Deployment;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Formatting;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.IO;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Json;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Model;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Service;

namespace Unity.Services.RemoteConfig.Authoring.Editor.Deployment
{
    class EditorRemoteConfigDeploymentHandler : RemoteConfigDeploymentHandler
    {
        IConfigAnalytics m_ConfigAnalytics;

        public EditorRemoteConfigDeploymentHandler(IRemoteConfigClient remoteConfigClient,
            IRemoteConfigParser remoteConfigParser,
            IRemoteConfigValidator remoteConfigValidator,
            IFormatValidator formatValidator,
            IConfigMerger configMerger,
            IJsonConverter jsonConverter,
            IFileSystem fileSystem,
            IConfigAnalytics configAnalytics)
            : base(remoteConfigClient,
                remoteConfigParser,
                remoteConfigValidator,
                formatValidator,
                configMerger,
                jsonConverter,
                fileSystem)
        {
            m_ConfigAnalytics = configAnalytics;
        }

        protected override void UpdateStatus(
            IRemoteConfigFile remoteConfigFile,
            string status,
            string detail,
            SeverityLevel severityLevel)
        {
            if (remoteConfigFile is DeploymentItem item)
            {
                switch (severityLevel)
                {
                    case SeverityLevel.Success: 
                        item.Status = DeploymentStatus.UpToDate;
                        break;
                    case SeverityLevel.Error:
                        item.Status = DeploymentStatus.FailedToDeploy;
                        break;
                    case SeverityLevel.None:
                    case SeverityLevel.Info:
                    case SeverityLevel.Warning:
                    default:
                        item.Status = new DeploymentStatus(status, detail, severityLevel);
                        break;
                        
                };
                
                item.SetStatusDetail(detail);
            }
        }

        protected override void UpdateProgress(IRemoteConfigFile remoteConfigFile, float progress)
        {
            if (remoteConfigFile is DeploymentItem item)
            {
                item.Progress = progress;
            }
        }

        protected override void OnFilesDeployed(int totalFilesRequested, IReadOnlyList<IRemoteConfigFile> filesDeployed)
        {
            m_ConfigAnalytics.SendDeployedEvent(totalFilesRequested, filesDeployed);
        }
    }
}
