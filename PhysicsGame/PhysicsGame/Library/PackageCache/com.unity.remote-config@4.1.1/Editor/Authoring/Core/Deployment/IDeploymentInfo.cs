namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.Deployment
{
    interface IDeploymentInfo
    {
        public string EnvironmentId { get; }
        public string CloudProjectId { get; }
    }
}
