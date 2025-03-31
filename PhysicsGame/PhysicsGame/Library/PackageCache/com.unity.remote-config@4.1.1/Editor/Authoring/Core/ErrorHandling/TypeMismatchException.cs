using System.Collections.Generic;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Model;

namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.ErrorHandling
{
    class TypeMismatchException : RemoteConfigDeploymentException
    {
        string m_TypeName;
        string m_Value;
        string m_EntryName;
        IRemoteConfigFile m_File;

        public override string StatusDescription => "Entries provided do not match the types specified";

        public override string StatusDetail =>
            $"Entry \"{m_EntryName}\" is expected to be of type \"{m_TypeName}\" but the value provided is \"{m_Value}\"";
        public override StatusLevel Level => StatusLevel.Error;

        public TypeMismatchException(IRemoteConfigFile file, string typeName, string value, string entryName)
        {
            m_File = file;
            m_TypeName = typeName;
            m_EntryName = entryName;
            m_Value = value;
            AffectedFiles = new List<IRemoteConfigFile>() { file };
        }
    }
}