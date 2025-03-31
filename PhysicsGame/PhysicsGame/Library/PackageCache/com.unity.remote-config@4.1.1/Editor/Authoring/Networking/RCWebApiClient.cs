using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Unity.Services.RemoteConfig.Authoring.Editor.Networking
{
    /// <summary>
    /// This class provides an async functionality for making requests to the Remote Config
    /// Admin API. It is a wrapper for RemoteConfigWebApiClient, necessary as we do not have
    /// an async Remote Config API client
    ///
    /// Limitation: can only make on request of each type at a time.
    /// </summary>
    class RcWebApiClient : IWebApiClient, IDisposable
    {
        List<RcWebApiClientRequest> m_ActiveRequests = new List<RcWebApiClientRequest>();

        public async Task<FetchResult> Fetch(string cloudProjectId, string environmentId)
        {
            var request = new RcWebApiClientRequest();
            m_ActiveRequests.Add(request);
            var result = await request.Fetch(cloudProjectId, environmentId);
            m_ActiveRequests.Remove(request);

            return new Unity.Services.RemoteConfig.Authoring.Editor.Networking.FetchResult(result as JObject);
        }

        public async Task<string> Post(string cloudProjectId, string environmentId, object configs)
        {
            var request = new RcWebApiClientRequest();
            m_ActiveRequests.Add(request);
            var result = await request.Post(cloudProjectId, environmentId, JArray.FromObject(configs));
            m_ActiveRequests.Remove(request);
            return result as string;
        }

        public async Task Put(string cloudProjectId, string environmentId, string configId, object configs)
        {
            var request = new RcWebApiClientRequest();
            m_ActiveRequests.Add(request);
            var result = await request.Put(cloudProjectId, environmentId, configId, JArray.FromObject(configs));
            m_ActiveRequests.Remove(request);
        }
        
        public void Dispose()
        {
            m_ActiveRequests.ForEach(request => request.Dispose());
            m_ActiveRequests = null;
        }
    }
}
