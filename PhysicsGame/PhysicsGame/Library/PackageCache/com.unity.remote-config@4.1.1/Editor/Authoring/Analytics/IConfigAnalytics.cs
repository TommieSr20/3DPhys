using System.Collections.Generic;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Model;

namespace Unity.Services.RemoteConfig.Authoring.Editor.Analytics
{
    interface IConfigAnalytics
    {
        void SendCreatedEvent();
        void SendDeployedEvent(int totalFilesRequested, IEnumerable<IRemoteConfigFile> validFiles);
    }
}
