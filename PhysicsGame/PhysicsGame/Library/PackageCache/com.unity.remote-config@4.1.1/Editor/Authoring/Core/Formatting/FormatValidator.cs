using System;
using System.Collections.Generic;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.ErrorHandling;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Model;

namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.Formatting
{
    class FormatValidator : IFormatValidator
    {
        static List<ConfigType> s_NumericTypes = new List<ConfigType>() { ConfigType.INT, ConfigType.FLOAT, ConfigType.LONG };

        readonly IIllegalEntryDetector m_IllegalEntryDetector;
        readonly IConfigTypeDeriver m_ConfigTypeDeriver;
        
        public FormatValidator(IIllegalEntryDetector illegalEntryDetector, IConfigTypeDeriver typeDeriver)
        {
            m_IllegalEntryDetector = illegalEntryDetector;
            m_ConfigTypeDeriver = typeDeriver;
        }

        public bool Validate(
            IRemoteConfigFile remoteConfigFile,
            RemoteConfigFileContent remoteConfigFileContent,
            ICollection<RemoteConfigDeploymentException> deploymentExceptions)
        {
            if (remoteConfigFileContent == null)
            {
                deploymentExceptions.Add(new FileParseException(remoteConfigFile));
                return false;
            }

            if (m_IllegalEntryDetector.ContainsIllegalEntries(remoteConfigFile, deploymentExceptions))
            {
                return false;
            }
            
            if (!IsProperFormat(remoteConfigFile, remoteConfigFileContent, deploymentExceptions) 
                || !TypesCanBeMapped(remoteConfigFile, remoteConfigFileContent, deploymentExceptions))
            {
                return false;
            }

            return true;
        }

        static bool IsProperFormat(
            IRemoteConfigFile remoteConfigFile,
            RemoteConfigFileContent content,
            ICollection<RemoteConfigDeploymentException> deploymentExceptions)
        {
            var canBeMapped = true;

            if (content.entries == null
                || content.entries.Count == 0)
            {
                deploymentExceptions.Add(new NoEntriesException(remoteConfigFile));
                canBeMapped = false;
            }

            return canBeMapped;
        }

        bool TypesCanBeMapped(
            IRemoteConfigFile remoteConfigFile,
            RemoteConfigFileContent content,
            ICollection<RemoteConfigDeploymentException> deploymentExceptions)
        {
            if (content.types == null
                || content.types.Count == 0)
            {
                return true;
            }

            var canBeMapped = true;
            var typesAreValid = true;
            var typesMatch = true;

            foreach (var typeKvp in content.types)
            {
                var typeKey = typeKvp.Key;
                var typeValue = typeKvp.Value;
                object value = null;
                
                if (!content.entries.ContainsKey(typeKey))
                {
                    deploymentExceptions.Add(new MissingEntryForTypeException(typeKey, remoteConfigFile));
                    canBeMapped = false;
                }

                if (canBeMapped)
                {
                    value = content.entries[typeKey];
                }
                
                if (!IsTypeValid(typeValue))
                {
                    deploymentExceptions.Add(new InvalidTypeException(remoteConfigFile, typeKey));
                    typesAreValid = false;
                }

                if (typesAreValid && canBeMapped && !TypeMatches(typeValue, value))
                {
                    deploymentExceptions.Add(new TypeMismatchException(remoteConfigFile,
                        typeValue.ToString(),
                        value?.ToString(),
                        typeKey));
                    
                    typesMatch = false;
                }
            }

            return canBeMapped && typesAreValid && typesMatch;
        }

        static bool IsTypeValid(ConfigType type)
        {
            if (!Enum.IsDefined(typeof(ConfigType), type))
            {
                return false;
            }

            return true;
        }

        bool TypeMatches(ConfigType type, object value)
        {
            if (value == null)
            {
                return false;
            }

            var derivedType = m_ConfigTypeDeriver.DeriveType(value);

            if (s_NumericTypes.Contains(type))
            {
                return ValidateNumericTypes(derivedType, type);
            }

            return derivedType == type;
        }

        bool ValidateNumericTypes(ConfigType derivedType, ConfigType typeToValidate)
        {
            if (derivedType == ConfigType.FLOAT)
            {
                return typeToValidate == derivedType;
            }

            return s_NumericTypes.Contains(derivedType) && s_NumericTypes.Contains(typeToValidate);
        }
    }
}
