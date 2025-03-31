using System.IO;
using Unity.Services.RemoteConfig.Authoring.Editor.Analytics;
using Unity.Services.RemoteConfig.Authoring.Editor.Package;
using UnityEditor;

namespace Unity.Services.RemoteConfig.Authoring.Editor.Model
{
    class CreateRemoteConfigFile
    {
        const string k_TemplatePath = "Authoring/Model/Template/new_remote_config.rc.txt";
        const string k_DefaultFileName = "new_remote_config";

        [MenuItem("Assets/Create/Services/Remote Config", false, 81)]
        public static void CreateConfig()
        {
            var templatePath = Path.Combine(RemoteConfigPackage.EditorPath, k_TemplatePath);
            var fileName = k_DefaultFileName + FileExtensions.GetExtension();
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, fileName);
            
            RemoteConfigServices.Instance.GetService<IConfigAnalytics>().SendCreatedEvent();
        }
    }
}
