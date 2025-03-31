using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.DeploymentApi.Editor;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Deployment;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Model;
using UnityEditor;
using Logger = Unity.Services.RemoteConfig.Authoring.Editor.Shared.Logging.Logger;

namespace Unity.Services.RemoteConfig.Authoring.Editor.Deployment
{
    class DeployCommand : Command<RemoteConfigFile>
    {
        bool m_IsBusy;
        bool m_Reconcile;
        bool m_DryRun;
        public override string Name => L10n.Tr("Deploy");

        IRemoteConfigDeploymentHandler m_RemoteConfigDeploymentHandler;

        public DeployCommand(IRemoteConfigDeploymentHandler remoteConfigDeploymentHandler)
        {
            m_RemoteConfigDeploymentHandler = remoteConfigDeploymentHandler;
            m_Reconcile = false;
            m_DryRun = false;
        }

        public override async Task ExecuteAsync(IEnumerable<RemoteConfigFile> items, CancellationToken cancellationToken = default)
        {
            if (m_IsBusy) return;

            try
            {
                m_IsBusy = true;
                var configFiles = items.ToList();
                var remoteConfigFiles = items as IReadOnlyList<RemoteConfigFile> ?? configFiles.AsReadOnly();
                Logger.LogVerbose($"Deployment triggered: {string.Join(", ", remoteConfigFiles.Select(item => item.Name))}");
                await m_RemoteConfigDeploymentHandler.DeployAsync(remoteConfigFiles, m_Reconcile, m_DryRun);
            }
            finally
            {
                m_IsBusy = false;
            }
        }
    }
}
