using System.Collections.Generic;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Model;

namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.ErrorHandling
{
    class NoEntriesException : RemoteConfigDeploymentException
    {
        IRemoteConfigFile m_File;

        public override string Message => $"{StatusDescription} {StatusDetail}";

        public override string StatusDescription => "Invalid Format.";
        public override string StatusDetail => $"The file {m_File.Name} does not have a key 'entries'.";
        public override StatusLevel Level => StatusLevel.Error;

        public NoEntriesException(IRemoteConfigFile file)
        {
            m_File = file;
            AffectedFiles = new List<IRemoteConfigFile> { file };
        }
    }
}
