namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.Formatting
{
    // ReSharper disable InconsistentNaming - Used in serialization
    enum ConfigType
    {
        STRING,
        INT,
        BOOL,
        FLOAT,
        LONG,
        JSON
    }
    
    interface IConfigTypeDeriver
    {
        ConfigType DeriveType(object obj);
    }
}
