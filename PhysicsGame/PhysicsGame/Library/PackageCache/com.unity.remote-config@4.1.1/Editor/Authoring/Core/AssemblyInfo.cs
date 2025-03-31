using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Unity.Services.RemoteConfig.Authoring.Editor")]
[assembly: InternalsVisibleTo("Unity.Services.Cli.RemoteConfig")]
[assembly: InternalsVisibleTo("Unity.Services.Cli.RemoteConfig.UnitTest")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

#if UNITY_INCLUDE_TESTS
[assembly: InternalsVisibleTo("Unity.Services.RemoteConfig.Tests.Editor.Authoring")]
#endif
