using Unity.Services.RemoteConfig.Authoring.Editor.Model;
using UnityEditor.AssetImporters;

namespace Unity.Services.RemoteConfig.Authoring.Editor.Assets
{
    class RemoteConfigImporterAttribute : ScriptedImporterAttribute
    {
        public RemoteConfigImporterAttribute()
#if UNITY_2022_1_OR_NEWER
            : base(1, FileExtensions.SupportedExtensions)
#else
            : base(1, FileExtensions.GetExtension())
#endif
        {
        }
    }
}
