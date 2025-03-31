using Unity.Services.DeploymentApi.Editor;
using Unity.Services.RemoteConfig.Authoring.Editor.Analytics;
using Unity.Services.RemoteConfig.Authoring.Editor.Deployment;
using Unity.Services.RemoteConfig.Authoring.Editor.Formatting;
using Unity.Services.RemoteConfig.Authoring.Editor.IO;
using Unity.Services.RemoteConfig.Authoring.Editor.Json;
using Unity.Services.RemoteConfig.Authoring.Editor.Networking;
using Unity.Services.RemoteConfig.Authoring.Editor.Shared.DependencyInversion;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Deployment;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Formatting;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.IO;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Json;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Model;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Service;
using UnityEditor;

namespace Unity.Services.RemoteConfig.Authoring.Editor
{
    class RemoteConfigServices : AbstractRuntimeServices<RemoteConfigServices>
    {
        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            Instance.Initialize(new ServiceCollection());
            var deploymentProvider = Instance.GetService<DeploymentProvider>();
            Deployments.Instance.DeploymentProviders.Add(deploymentProvider);
        }

        protected override void Register(ServiceCollection collection)
        {
            collection.RegisterStartupSingleton(Factories.Default<DeploymentProvider, RemoteConfigDeploymentProvider>);
            collection.Register(Factories.Default<Command<RemoteConfigFile>, DeployCommand>);
            collection.Register(Factories.Default<IFormatValidator, FormatValidator>);
            collection.Register(Factories.Default<IRemoteConfigDeploymentHandler, EditorRemoteConfigDeploymentHandler>);
            collection.Register(Factories.Default<IWebApiClient, RcWebApiClient>);
            collection.Register(Factories.Default<IDeploymentInfo, DeploymentInfo>);
            collection.Register(Factories.Default<IRemoteConfigParser, RemoteConfigParser>);
            collection.Register(Factories.Default<IRemoteConfigValidator, RemoteConfigValidator>);
            collection.Register(Factories.Default<IConfigTypeDeriver, ConfigTypeDeriver>);
            collection.Register(Factories.Default<IAnalyticsWrapper, AnalyticsWrapper>);
            collection.Register(Factories.Default<IConfigAnalytics, ConfigAnalytics>);
            collection.Register(Factories.Default<IJsonConverter, JsonConverter>);
            collection.Register(Factories.Default<IFileSystem, FileSystem>);
            collection.Register(Factories.Default<IConfigMerger, ConfigMerger>);
            collection.Register(Factories.Default<IIllegalEntryDetector, IllegalEntryDetector>);
            collection.Register(Factories.Default<IRemoteConfigClient, RemoteConfigClient>);
            collection.Register(Factories.Default<ValidateCommand>);
        }
    }
}
