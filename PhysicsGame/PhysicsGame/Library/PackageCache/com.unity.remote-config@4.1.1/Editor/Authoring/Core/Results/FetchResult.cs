using System;
using System.Collections.Generic;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Model;

namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.Results
{
    class FetchResult : Result
    {
        public IReadOnlyList<IRemoteConfigFile> Fetched { get; }

        public FetchResult(
            IReadOnlyList<RemoteConfigEntry> created,
            IReadOnlyList<RemoteConfigEntry> updated,
            IReadOnlyList<RemoteConfigEntry> deleted,
            IReadOnlyList<IRemoteConfigFile> fetched = null,
            IReadOnlyList<IRemoteConfigFile> failed = null) : base(created, updated,deleted, failed)
        {
            Fetched = fetched ?? Array.Empty<IRemoteConfigFile>();
        }
    }
}