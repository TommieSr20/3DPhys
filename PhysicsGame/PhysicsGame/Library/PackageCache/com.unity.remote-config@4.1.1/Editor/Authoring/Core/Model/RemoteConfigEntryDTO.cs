// ReSharper disable InconsistentNaming - Used in serialization

using System;

namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.Model
{
    class RemoteConfigEntryDTO
    {
        public string key;
        public object value;
        public string type;

        public RemoteConfigEntry ToRemoteConfigEntry()
        {
            return new RemoteConfigEntry()
            {
                File = null,
                Key = key,
                Value = ToValue(value, type)
            };
        }

        public static object ToValue(
            object val,
            string type)
        {
            switch (type)
            {
                case "string":
                case "json":
                case "bool":
                case "long":
                    return val;
                case "int":
                    return (int)(long)val;
                case "float":
                    return NumericToFloat(val);
            }

            throw new NotSupportedException($"Type '{type}' is not supported, value: '{val}', value type: '{val.GetType().Name}' !");
        }

        static object NumericToFloat(object val)
        {
            if (val is double)
                return val;
            //the second "cast" is a type-conversion not a type-cast
            if (val is float f)
                return (double)f;
            if (val is int i)
                return (double)i;
            return (double)(long)val;
        }
    }

}
