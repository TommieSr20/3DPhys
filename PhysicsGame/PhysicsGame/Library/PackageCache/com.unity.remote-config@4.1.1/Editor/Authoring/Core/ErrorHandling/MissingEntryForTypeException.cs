using System.Collections.Generic;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Model;

namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.ErrorHandling
{
    class MissingEntryForTypeException : RemoteConfigDeploymentException
    {
        IRemoteConfigFile m_File;
        string m_Key;

        public override string Message => $"{StatusDescription} {StatusDetail}";
            
        public override string StatusDescription => "Invalid Format.";
        public override string StatusDetail =>
            $"The key '{m_Key}' in the file '{m_File.Name}' was not found in the entries but exists in the types.";
        public override StatusLevel Level => StatusLevel.Error;

        public MissingEntryForTypeException(string key, IRemoteConfigFile file)
        {
            m_Key = key;
            m_File = file;
            AffectedFiles = new List<IRemoteConfigFile> { file };
        }
    }
}
