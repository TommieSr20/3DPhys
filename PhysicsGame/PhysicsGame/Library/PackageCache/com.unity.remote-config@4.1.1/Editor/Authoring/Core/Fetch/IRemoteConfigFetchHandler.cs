using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Model;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Results;

namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.Fetch
{
    interface IRemoteConfigFetchHandler
    {
        Task<FetchResult> FetchAsync(
            string rootDirectory,
            IReadOnlyList<IRemoteConfigFile> files,
            bool dryRun, 
            bool reconcile,
            CancellationToken token);
    }
}