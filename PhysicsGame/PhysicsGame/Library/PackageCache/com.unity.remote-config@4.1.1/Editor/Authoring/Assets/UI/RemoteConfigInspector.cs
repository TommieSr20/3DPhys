using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Unity.Services.RemoteConfig.Authoring.Editor.Assets.UI
{
    [CustomEditor(typeof(RemoteConfigAsset))]
    [CanEditMultipleObjects]
    class CloudCodeScriptInspector : UnityEditor.Editor
    {
        const int k_MaxLines = 75;
        const string k_Template = "Packages/com.unity.remote-config/Editor/Authoring/Assets/UI/Assets/RemoteConfigInspector.uxml";

        public override VisualElement CreateInspectorGUI()
        {
            var myInspector = new VisualElement();
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_Template);
            visualTree.CloneTree(myInspector);

            ShowScriptBody(myInspector);

            return myInspector;
        }

        void ShowScriptBody(VisualElement myInspector)
        {
            var body = myInspector.Q<TextField>();
            if (targets.Length == 1)
            {
                body.visible = true;
                body.value = ReadScriptBody(targets[0]);
            }
            else
            {
                body.visible = false;
            }
        }

        static string ReadScriptBody(Object script)
        {
            var path = AssetDatabase.GetAssetPath(script);
            var lines = File.ReadLines(path).Take(k_MaxLines).ToList();
            if (lines.Count == k_MaxLines)
            {
                lines.Add("...");
            }
            return string.Join(Environment.NewLine, lines);
        }
    }
}
