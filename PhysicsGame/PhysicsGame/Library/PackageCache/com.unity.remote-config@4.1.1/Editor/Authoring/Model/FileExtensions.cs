namespace Unity.Services.RemoteConfig.Authoring.Editor.Model
{
    class FileExtensions
    {
        #if UNITY_2022_1_OR_NEWER
        public static string[] SupportedExtensions => new string[] {".rc"};
        #endif        

        public static string GetExtension()
        {
            #if UNITY_2022_1_OR_NEWER
            return ".rc";
            #else
            return ".rc";
            #endif
        } 
    }
}
