namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.Json
{
    interface IJsonConverter
    {
        T DeserializeObject<T>(string value, bool matchCamelCaseFieldName = false);
        string SerializeObject<T>(T obj);
    }
}
