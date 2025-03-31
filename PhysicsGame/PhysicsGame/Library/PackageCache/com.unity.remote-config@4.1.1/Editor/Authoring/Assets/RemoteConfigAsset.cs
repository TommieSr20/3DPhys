using Unity.Services.RemoteConfig.Authoring.Editor.Shared.Assets;
using Unity.Services.RemoteConfig.Authoring.Editor.Shared.EditorUtils;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Model;
using UnityEngine;

namespace Unity.Services.RemoteConfig.Authoring.Editor.Assets
{
    class RemoteConfigAsset : ScriptableObject, IPath, ISerializationCallbackReceiver
    {
        [SerializeField]
        // ReSharper disable once InconsistentNaming
        RemoteConfigFile m_Model;

        public RemoteConfigFile Model
        {
            get => m_Model;
            internal set => m_Model = value;
        }

        public string Path
        {
            get => m_Model.Path;
            set => m_Model.Path = value;
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            Sync.RunNextUpdateOnMain(() =>
            {
                if (m_Model.Path != null)
                {
                    m_Model.Name = System.IO.Path.GetFileName(Path);
                }
            });
        }
    }
}
