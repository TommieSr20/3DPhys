using System.Collections.Generic;

namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.Model
{
    interface IRemoteConfigFile
    {
        string Name { get; }
        string Path { get; set; }
        List<RemoteConfigEntry> Entries { get; set; }
    }
}
