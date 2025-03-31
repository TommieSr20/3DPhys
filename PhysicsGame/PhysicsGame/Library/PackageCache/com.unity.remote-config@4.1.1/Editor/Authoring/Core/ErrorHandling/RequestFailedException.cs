using System.Collections.Generic;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Model;

namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.ErrorHandling
{
    class RequestFailedException : RemoteConfigDeploymentException
    {
        long m_ErrorCode;
        string m_ErrorMessage;

        public override string Message => $"{StatusDescription} {StatusDetail}";
        public override string StatusDescription => $"Deployment Failed [{m_ErrorCode}].";
        public override string StatusDetail => m_ErrorMessage;
        public override StatusLevel Level => StatusLevel.Error;
        
        public RequestFailedException(long errorCode, string errorMessage)
        {
            m_ErrorCode = errorCode;
            m_ErrorMessage = errorMessage;
            AffectedFiles = new List<IRemoteConfigFile>();
        }
    }
}
