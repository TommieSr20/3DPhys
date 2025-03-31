using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Model;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Results;

namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.Deployment
{
    interface IRemoteConfigDeploymentHandler
    {
        Task<DeployResult> DeployAsync(IReadOnlyList<IRemoteConfigFile> configFiles, bool reconcile, bool dryRun);
    }
}
