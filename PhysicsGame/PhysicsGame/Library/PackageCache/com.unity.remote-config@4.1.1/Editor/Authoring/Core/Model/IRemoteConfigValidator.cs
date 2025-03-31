using System.Collections.Generic;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.ErrorHandling;

namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.Model
{
    interface IRemoteConfigValidator
    {
        IReadOnlyList<RemoteConfigEntry> FilterValidEntries(
            IReadOnlyList<IRemoteConfigFile> files,
            IReadOnlyList<RemoteConfigEntry> entries,
            ICollection<RemoteConfigDeploymentException> deploymentExceptions);
    }
}
