using System.Collections.Generic;

namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.Model
{
    interface IConfigMerger
    {
        IReadOnlyList<RemoteConfigEntry> MergeEntriesToDeploy(
            IReadOnlyList<RemoteConfigEntry> toCreate,
            IReadOnlyList<RemoteConfigEntry> toUpdate,
            IReadOnlyList<RemoteConfigEntry> toDelete,
            IReadOnlyList<RemoteConfigEntry> remoteConfigEntries);
    }
}
