using System.Collections.Generic;

namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.Model
{
    interface IRemoteConfigParser
    {
        IReadOnlyList<RemoteConfigEntry> ParseFile(RemoteConfigFileContent content, IRemoteConfigFile file);
    }
}
