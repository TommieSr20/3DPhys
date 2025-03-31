using System.Collections.Generic;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Model;

namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.ErrorHandling
{
    class InvalidTypeException : RemoteConfigDeploymentException
    {
        string m_TypeName;
        IRemoteConfigFile m_File;

        public override string Message => $"{StatusDescription} {StatusDetail}";

        public override string StatusDescription => "Invalid type specified";

        public override string StatusDetail =>
            $"{m_TypeName} specifies an invalid type. A type must be one of 'STRING`, `INT`, `BOOL`, `FLOAT`, `LONG`, `JSON`";
        public override StatusLevel Level => StatusLevel.Error;

        public InvalidTypeException(IRemoteConfigFile file, string typeName)
        {
            m_File = file;
            m_TypeName = typeName;
            AffectedFiles = new List<IRemoteConfigFile> {file};
        }
    }
}
