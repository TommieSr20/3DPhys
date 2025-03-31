using Unity.Services.RemoteConfig.Authoring.Editor.Shared.EditorUtils;
using Unity.Services.RemoteConfig.Authoring.Editor.Shared.Logging;
using UnityEditor;

namespace Unity.Services.RemoteConfig.Authoring.Editor.Analytics
{
    class AnalyticsUtils
    {
        public static void RegisterEventDefault(string eventName, int version = 1)
        {
            Sync.RunNextUpdateOnMain(() =>
            {
                var result = EditorAnalytics.RegisterEventWithLimit(
                    eventName,
                    AnalyticsConstants.MaxEventPerHour,
                    AnalyticsConstants.MaxItems,
                    AnalyticsConstants.VendorKey,
                    version);

                Logger.LogVerbose($"Analytics: {eventName}.v{version} registered with result {result}");
            });
        }
    }
}
