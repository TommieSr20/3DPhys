namespace Unity.Services.RemoteConfig.Authoring.Editor.Analytics
{
    interface IAnalyticsWrapper
    {
        void Register(string eventName, int version = 1);
        void Send(string eventName, object parameters = null, int version = 1);
    }
}
