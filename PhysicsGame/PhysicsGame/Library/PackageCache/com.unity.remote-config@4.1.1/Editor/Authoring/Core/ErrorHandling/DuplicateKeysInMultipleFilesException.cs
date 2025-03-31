using System.Collections.Generic;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Model;

namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.ErrorHandling
{
    class DuplicateKeysInMultipleFilesException : RemoteConfigDeploymentException
    {
        string m_Key;
        
        public override string Message => $"{StatusDescription} {StatusDetail}";
        
        public override string StatusDescription => "Duplicate keys in files.";
        public override StatusLevel Level => StatusLevel.Warning;

        public override string StatusDetail
        {
            get
            {
                 var detail = $"Key '{m_Key}' was found in multiple files: ";
                 
                 foreach (var file in AffectedFiles)
                 {
                     detail += $" '{file.Path}'";
                 }

                 return detail;
            }     
        }
        

        public DuplicateKeysInMultipleFilesException(string key, IEnumerable<IRemoteConfigFile> files)
        {
            m_Key = key;

            AffectedFiles = new List<IRemoteConfigFile>(files);
        }
    }
}
