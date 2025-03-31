using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.DeploymentApi.Editor;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.ErrorHandling;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Formatting;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.IO;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Json;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Model;
using UnityEditor;

namespace Unity.Services.RemoteConfig.Authoring.Editor.Deployment
{
    class ValidateCommand : Command<RemoteConfigFile>
    {
        public override string Name => L10n.Tr("Validate");

        readonly IRemoteConfigValidator m_Validator;
        readonly IFormatValidator m_FormatValidator;
        readonly IRemoteConfigParser m_RemoteConfigParser;
        readonly IFileSystem m_FileSystem;
        readonly IJsonConverter m_JsonConverter;

        public ValidateCommand(IRemoteConfigValidator validator, IFormatValidator formatValidator, IRemoteConfigParser configParser, IFileSystem fileSystem, IJsonConverter jsonConverter)
        {
            m_Validator = validator;
            m_FormatValidator = formatValidator;
            m_RemoteConfigParser = configParser;
            m_FileSystem = fileSystem;
            m_JsonConverter = jsonConverter;
        }
        
        public override async Task ExecuteAsync(IEnumerable<RemoteConfigFile> items, CancellationToken cancellationToken = default)
        {
            var rcFileList = items.ToList();
            rcFileList.ForEach(i => i.States.Clear());

            var validationErrors = new List<RemoteConfigDeploymentException>();

            var filesContent = await DeserializeFiles(rcFileList);

            var validFiles = rcFileList
                .Where((t, i) => m_FormatValidator.Validate(t, filesContent[i], validationErrors))
                .ToList();
            
            var entries = validFiles.SelectMany(file => file.Entries).ToList();

            m_Validator.FilterValidEntries(validFiles, entries, validationErrors);
            
            SetValidationStates(validationErrors);
        }

        async Task<IReadOnlyList<RemoteConfigFileContent>> DeserializeFiles(IReadOnlyList<IRemoteConfigFile> configFiles)
        {
           var result = await Task.WhenAll(configFiles.Select(SetContent));

           return result;
        }

        async Task<RemoteConfigFileContent> SetContent(IRemoteConfigFile configFile)
        {
            var text = await m_FileSystem.ReadAllText(configFile.Path);
            var content = m_JsonConverter.DeserializeObject<RemoteConfigFileContent>(text, true);
            configFile.Entries = content?.ToRemoteConfigEntries(configFile, m_RemoteConfigParser);

            return content;
        }
        
        static void SetValidationStates(List<RemoteConfigDeploymentException> validationErrors)
        {
            foreach (var validationError in validationErrors)
            {
                var assetState = ToAssetState(validationError);
                
                foreach (var item in validationError.AffectedFiles.Cast<DeploymentItem>())
                {
                    if (item.States.All(state => state.Description != assetState.Description))
                    {
                        item.States.Add(assetState);
                    }
                }
            }
        }

        static AssetState ToAssetState(RemoteConfigDeploymentException source)
        {
            SeverityLevel level;
            switch (source.Level)
            {
                case RemoteConfigDeploymentException.StatusLevel.Error:
                    level = SeverityLevel.Error;
                    break;
                case RemoteConfigDeploymentException.StatusLevel.Warning:
                    level = SeverityLevel.Warning;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            return new AssetState(source.StatusDescription, source.StatusDetail, level);
        }
    }
}
