using System;
using System.Collections.Generic;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Model;

namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.ErrorHandling
{
    abstract class RemoteConfigDeploymentException : Exception
    {
        public List<IRemoteConfigFile> AffectedFiles { get; protected set; }
        public abstract string StatusDescription { get; }
        public abstract string StatusDetail { get; }
        public abstract StatusLevel Level { get; }

        public enum StatusLevel
        {
            Error,
            Warning
        }
    }
}
