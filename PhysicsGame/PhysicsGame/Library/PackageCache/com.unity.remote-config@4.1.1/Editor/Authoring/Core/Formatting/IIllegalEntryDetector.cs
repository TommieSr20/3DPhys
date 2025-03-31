using System.Collections.Generic;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.ErrorHandling;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Model;

namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.Formatting
{
    interface IIllegalEntryDetector
    {
        bool ContainsIllegalEntries(IRemoteConfigFile remoteConfigFile,
            ICollection<RemoteConfigDeploymentException> deploymentExceptions);
    }
}
