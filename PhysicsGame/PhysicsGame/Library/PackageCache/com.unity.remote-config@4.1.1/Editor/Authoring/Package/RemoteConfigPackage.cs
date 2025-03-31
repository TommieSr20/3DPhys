using System.IO;

namespace Unity.Services.RemoteConfig.Authoring.Editor.Package
{
    class RemoteConfigPackage
    {
        public const string Name = "com.unity.remote-config";
        public static readonly string RootPath = $"Packages/{Name}";
        public static readonly string EditorPath = Path.Join(RootPath, "Editor");
    }
}
