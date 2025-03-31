using System;
using System.Collections.Generic;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Model;

namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.Results
{
    class DeployResult : Result
    {
        public IReadOnlyList<IRemoteConfigFile> Deployed { get; }

        public DeployResult(
            IReadOnlyList<RemoteConfigEntry> created,
            IReadOnlyList<RemoteConfigEntry> updated,
            IReadOnlyList<RemoteConfigEntry> deleted,
            IReadOnlyList<IRemoteConfigFile> deployed = null,
            IReadOnlyList<IRemoteConfigFile> failed = null) : base(created, updated, deleted, failed)
        {
            Deployed = deployed ?? Array.Empty<IRemoteConfigFile>();
        }
    }
}