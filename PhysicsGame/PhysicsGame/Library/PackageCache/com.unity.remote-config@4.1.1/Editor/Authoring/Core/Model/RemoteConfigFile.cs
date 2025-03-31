using System;
using System.Collections.Generic;
using Unity.Services.DeploymentApi.Editor;

namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.Model
{
    [Serializable]
    class RemoteConfigFile : DeploymentItem, IRemoteConfigFile
    {
        public sealed override string Path
        {
            get => base.Path;
            set
            {
                base.Path = value;
                Name = System.IO.Path.GetFileName(value);
            }
        }
        public List<RemoteConfigEntry> Entries { get; set; }
    }
}
