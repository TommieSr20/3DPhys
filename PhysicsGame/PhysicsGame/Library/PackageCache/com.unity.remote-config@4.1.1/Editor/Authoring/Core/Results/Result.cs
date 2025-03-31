using System;
using System.Collections.Generic;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Model;

namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.Results
{
    class Result
    {
        public IReadOnlyList<RemoteConfigEntry> Created { get; }
        public IReadOnlyList<RemoteConfigEntry> Updated { get; }
        public IReadOnlyList<RemoteConfigEntry> Deleted { get; }
        
        public IReadOnlyList<IRemoteConfigFile> Failed { get; }

        public Result(
            IReadOnlyList<RemoteConfigEntry> created,
            IReadOnlyList<RemoteConfigEntry> updated,
            IReadOnlyList<RemoteConfigEntry> deleted,
            IReadOnlyList<IRemoteConfigFile> failed = null)
        {
            Created = created;
            Updated = updated;
            Deleted = deleted;
            Failed = failed?? Array.Empty<IRemoteConfigFile>();
        }
    }
}