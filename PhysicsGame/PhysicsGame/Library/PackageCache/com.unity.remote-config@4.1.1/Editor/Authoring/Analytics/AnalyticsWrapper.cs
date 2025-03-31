using UnityEditor;

namespace Unity.Services.RemoteConfig.Authoring.Editor.Analytics
{
    class AnalyticsWrapper : IAnalyticsWrapper
    {
        public void Register(string eventName, int version = 1)
        {
            AnalyticsUtils.RegisterEventDefault(eventName, version);
        }

        public void Send(string eventName, object parameters = null, int version = 1)
        {
            EditorAnalytics.SendEventWithLimit(eventName, parameters, version);
        }
    }
}
