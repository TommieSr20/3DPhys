using UnityEditor;
using UnityEngine;

namespace Unity.Services.RemoteConfig.Authoring.Editor
{
    static class RemoteConfigResources
    {
        const string k_TexturePath = "DefaultAsset Icon";

        public static readonly Texture2D Icon = (Texture2D)EditorGUIUtility.IconContent(k_TexturePath).image;
    }
}
