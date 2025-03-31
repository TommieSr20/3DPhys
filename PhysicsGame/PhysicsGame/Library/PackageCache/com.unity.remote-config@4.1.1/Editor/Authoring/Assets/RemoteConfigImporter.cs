using System.IO;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Model;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Unity.Services.RemoteConfig.Authoring.Editor.Assets
{
    [RemoteConfigImporter]
    class RemoteConfigImporter : ScriptedImporter
    {
        const string k_RemoteConfigAssetIdentifier = "RemoteConfig";

        public override void OnImportAsset(AssetImportContext ctx)
        {
            var remoteConfigAsset = ScriptableObject.CreateInstance<RemoteConfigAsset>();
            remoteConfigAsset.Model = new RemoteConfigFile()
            {
                Path = Path.Join(
                    Application.dataPath.Replace("Assets", ""),
                    ctx.assetPath)
            };
            
            ctx.AddObjectToAsset(k_RemoteConfigAssetIdentifier, remoteConfigAsset, RemoteConfigResources.Icon);
            ctx.SetMainObject(remoteConfigAsset);
        }
    }
}
