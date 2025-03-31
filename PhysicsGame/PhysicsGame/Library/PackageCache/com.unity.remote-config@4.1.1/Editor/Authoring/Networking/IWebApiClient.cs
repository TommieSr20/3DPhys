using System.Threading.Tasks;

namespace Unity.Services.RemoteConfig.Authoring.Editor.Networking
{
    interface IWebApiClient
    {
        Task<FetchResult> Fetch(string cloudProjectId, string environmentId);
        Task<string> Post(string cloudProjectId, string environmentId, object configs);
        Task Put(string cloudProjectId, string environmentId, string configId, object configs);
    }
}
