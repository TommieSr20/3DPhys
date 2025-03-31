using System.Collections.Generic;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Model;

namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.ErrorHandling
{
    class FileParseException : RemoteConfigDeploymentException
    {
        const string k_FormatExample = "{'entries': {}, 'types': {}}";
        IRemoteConfigFile m_File;

        public override string Message => $"{StatusDescription} {StatusDetail}";
    

        public override string StatusDescription => "Unable To Parse";

        public override string StatusDetail => $"The file {m_File.Name} is not of proper format {k_FormatExample} where each type can be successfully mapped to an entry. See schema at 'https://ugs-config-schemas.unity3d.com/v1/remote-config.schema.json'";
        public override StatusLevel Level => StatusLevel.Error;

        public FileParseException(IRemoteConfigFile file)
        {
            m_File = file;
            AffectedFiles = new List<IRemoteConfigFile> {file};
        }
    }
}
