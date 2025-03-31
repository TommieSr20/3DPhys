using System.Collections.Generic;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.ErrorHandling;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Model;

namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.Formatting
{
    interface IFormatValidator
    {
        bool Validate(
            IRemoteConfigFile remoteConfigFile,
            RemoteConfigFileContent fileContent,
            ICollection<RemoteConfigDeploymentException> deploymentExceptions);
    }
}
