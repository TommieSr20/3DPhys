using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Unity.RemoteConfig.Editor;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.ErrorHandling;

namespace Unity.Services.RemoteConfig.Authoring.Editor.Networking
{
    class RcWebApiClientRequest : IDisposable
    {
        TaskCompletionSource<object> m_TaskCompletionSource;

        public RcWebApiClientRequest()
        {
            m_TaskCompletionSource = new TaskCompletionSource<object>();
            RemoteConfigWebApiClient.postConfigRequestFinished += OnPostRequestFinished;
            RemoteConfigWebApiClient.fetchConfigsFinished += OnFetchRequestFinished;
            RemoteConfigWebApiClient.settingsRequestFinished += OnPutRequestFinished;
            RemoteConfigWebApiClient.rcRequestFailed += OnRequestFailed;
        }
        
        public Task<object> Fetch(string cloudProjectId, string environmentId)
        {
            RemoteConfigWebApiClient.FetchConfigs(cloudProjectId, environmentId);
            return m_TaskCompletionSource.Task;
        }

        public Task<object> Post(string cloudProjectId, string environmentId, JArray configs)
        {
            RemoteConfigWebApiClient.PostConfig(cloudProjectId, environmentId, configs);
            return m_TaskCompletionSource.Task;
        }

        public Task<object> Put(string cloudProjectId, string environmentId, string configId, JArray configs)
        {
            RemoteConfigWebApiClient.PutConfig(cloudProjectId, environmentId, configId, configs);
            return m_TaskCompletionSource.Task;
        }
        
        void OnPutRequestFinished()
        {
            m_TaskCompletionSource.TrySetResult(null);
        }

        void OnFetchRequestFinished(JObject config)
        {
            m_TaskCompletionSource.TrySetResult(config);
        }

        void OnPostRequestFinished(string configId)
        {
            m_TaskCompletionSource.TrySetResult(configId);
        }

        void OnRequestFailed(long errorCode, string errorMessage)
        {
            m_TaskCompletionSource.TrySetException(new RequestFailedException(errorCode, errorMessage));
        }
        
        public void Dispose()
        {
            m_TaskCompletionSource = null;
            RemoteConfigWebApiClient.postConfigRequestFinished -= OnPostRequestFinished;
            RemoteConfigWebApiClient.fetchConfigsFinished -= OnFetchRequestFinished;
            RemoteConfigWebApiClient.settingsRequestFinished -= OnPutRequestFinished;
            RemoteConfigWebApiClient.rcRequestFailed -= OnRequestFailed;
        }
    }
}
