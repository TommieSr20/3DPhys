using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.DeploymentApi.Editor;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.ErrorHandling;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Formatting;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.IO;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Json;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Model;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Results;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Service;

namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.Deployment
{
    class RemoteConfigDeploymentHandler : IRemoteConfigDeploymentHandler
    {
        readonly IRemoteConfigClient m_RemoteConfigClient;
        readonly IRemoteConfigParser m_RemoteConfigParser;
        readonly IRemoteConfigValidator m_RemoteConfigValidator;
        readonly IFormatValidator m_FormatValidator;
        readonly IConfigMerger m_ConfigMerger;
        readonly IJsonConverter m_JsonConverter;
        readonly IFileSystem m_FileSystem;

        public RemoteConfigDeploymentHandler(
            IRemoteConfigClient remoteConfigClient,
            IRemoteConfigParser remoteConfigParser,
            IRemoteConfigValidator remoteConfigValidator,
            IFormatValidator formatValidator,
            IConfigMerger configMerger,
            IJsonConverter jsonConverter,
            IFileSystem fileSystem)
        {
            m_RemoteConfigClient = remoteConfigClient;
            m_RemoteConfigParser = remoteConfigParser;
            m_RemoteConfigValidator = remoteConfigValidator;
            m_FormatValidator = formatValidator;
            m_ConfigMerger = configMerger;
            m_JsonConverter = jsonConverter;
            m_FileSystem = fileSystem;
        }
        
        public async Task<DeployResult> DeployAsync(
            IReadOnlyList<IRemoteConfigFile> configFiles, 
            bool reconcile = false, 
            bool dryRun = false)
        {
            if (!dryRun)
            {
                SetStartDeployingStatus(configFiles);
            }
            
            var deploymentExceptions = new List<RemoteConfigDeploymentException>();
            var filesContent = await DeserializeFiles(configFiles);
            
            var validFiles = configFiles
                .Where((t, i) => m_FormatValidator.Validate(t, filesContent[i], deploymentExceptions))
                .ToList();
            
            var allLocalEntries = validFiles.SelectMany(file=>file.Entries).ToList();

           
            var validLocalEntries = m_RemoteConfigValidator.FilterValidEntries(validFiles, allLocalEntries, deploymentExceptions);
            
            var serverRemoteConfigResult = await GetServerRemoteConfig(configFiles, dryRun);
            var remoteEntries = serverRemoteConfigResult.ConfigsExists
                ? serverRemoteConfigResult.Configs
                : Array.Empty<RemoteConfigEntry>();

            var toUpdate = FindEntriesToUpdate(remoteEntries, validLocalEntries);
            var toDelete = FindEntriesToDelete(remoteEntries, validLocalEntries, reconcile);
            var toCreate = FindEntriesToCreate(remoteEntries, validLocalEntries);
            var toDeploy =  m_ConfigMerger.MergeEntriesToDeploy(toCreate, toUpdate, toDelete, remoteEntries);
            
            var failedFiles = FindFailedFiles(deploymentExceptions);
            var filesToDeploy = FindFilesToDeploy(toDeploy).Except(failedFiles).ToList();
            

            Exception exceptionToThrow = null;

            if (!dryRun)
            {
                exceptionToThrow = await DeployEntriesAndUpdateStatus(
                    serverRemoteConfigResult,
                    toDeploy,
                    failedFiles,
                    filesToDeploy,
                    deploymentExceptions,
                    configFiles);
            }

            HandleDeploymentException(deploymentExceptions, exceptionToThrow);

            return new DeployResult(
                toCreate, 
                toUpdate, 
                toDelete, 
                filesToDeploy.Except(failedFiles).ToList(), 
                failedFiles);
        }
        
        async Task<Exception> DeployEntriesAndUpdateStatus(
            GetConfigsResult serverRemoteConfigResult,
            IReadOnlyList<RemoteConfigEntry> toDeploy,
            IReadOnlyList<IRemoteConfigFile> failedFiles,
            List<IRemoteConfigFile> filesToDeploy,
            List<RemoteConfigDeploymentException>deploymentExceptions,
            IReadOnlyList<IRemoteConfigFile> configFiles)
        {
            Exception exceptionToThrow = null;
            try
            {
                if (serverRemoteConfigResult.ConfigsExists)
                {
                    await m_RemoteConfigClient.UpdateAsync(toDeploy);
                }
                else
                {
                    await m_RemoteConfigClient.CreateAsync(toDeploy);
                }

                SetDeployedStatus(filesToDeploy);
            }
            catch (RemoteConfigDeploymentException e)
            {
                deploymentExceptions.Add(e);
                e.AffectedFiles.AddRange(filesToDeploy);
                failedFiles = FindFailedFiles(deploymentExceptions);
                filesToDeploy = FindFilesToDeploy(toDeploy).Except(failedFiles).ToList();
            }
            catch (Exception e)
            {
                SetFailedStatus(filesToDeploy, e.Message);

                exceptionToThrow = e;
            }

            OnFilesDeployed(configFiles.Count, filesToDeploy);

            return exceptionToThrow;
        }

        async Task<GetConfigsResult> GetServerRemoteConfig(
            IReadOnlyList<IRemoteConfigFile> configFiles,
            bool dryRun)
        {
            try
            {
                return await m_RemoteConfigClient.GetAsync();
            }
            catch (Exception e)
            {
                if (!dryRun)
                    SetFailedStatus(configFiles, detail: e.Message);
                throw;
            }
        }
        
        protected virtual void UpdateStatus(
            IRemoteConfigFile remoteConfigFile,
            string status,
            string detail,
            SeverityLevel severityLevel) {}

        protected virtual void UpdateProgress(
            IRemoteConfigFile remoteConfigFile,
            float progress) {}
        
        protected virtual void OnFilesDeployed(int totalFilesRequested, IReadOnlyList<IRemoteConfigFile> filesDeployed) {}

        void HandleDeploymentException(
            ICollection<RemoteConfigDeploymentException> deploymentExceptions, 
            Exception exceptionToThrow)
        {
            if (!deploymentExceptions.Any())
            {
                return;
            }

            foreach (var deploymentException in deploymentExceptions)
            {
                SetFailedStatus(
                    deploymentException.AffectedFiles, 
                    deploymentException.StatusDescription,
                    deploymentException.StatusDetail);
            }

            if (exceptionToThrow != null)
            {
                throw exceptionToThrow;
            }
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

        void SetStatusAndProgress(
            IReadOnlyList<IRemoteConfigFile> files, 
            string status, 
            string detail, 
            SeverityLevel severityLevel, 
            float progress)
        {
            foreach (var file in files)
            {
                UpdateStatus(file, status, detail, severityLevel);
                UpdateProgress(file, progress);
            }
        }
        
        void SetStartDeployingStatus(IReadOnlyList<IRemoteConfigFile> files)
        {
            SetStatusAndProgress(files, 
                null, 
                null, 
                SeverityLevel.None, 
                0f);
        }
        
        void SetDeployedStatus(IReadOnlyList<IRemoteConfigFile> files)
        {
            SetStatusAndProgress(files, 
                "Deployed", 
                "Deployed Successfully", 
                SeverityLevel.Success, 
                100f);
        }
        
        void SetFailedStatus(IReadOnlyList<IRemoteConfigFile> files, string status = null, string detail = null)
        {
            SetStatusAndProgress(files, 
                status ?? "Failed to deploy", 
                detail ?? " Unknown Error", 
                SeverityLevel.Error, 
                0f);
        }

        static IReadOnlyList<RemoteConfigEntry> FindEntriesToUpdate(
            IReadOnlyList<RemoteConfigEntry> remote,
            IReadOnlyList<RemoteConfigEntry> localKeys)
        {
            var toUpdate = localKeys
                .Where(l => remote.Any(r => r.Key == l.Key))
                .ToList();

            return toUpdate;
        }

        static IReadOnlyList<RemoteConfigEntry> FindEntriesToDelete(
            IReadOnlyList<RemoteConfigEntry> remote,
            IReadOnlyList<RemoteConfigEntry> local,
            bool reconcile)
        {
            if (!reconcile)
            {
                return new List<RemoteConfigEntry>();
            }

            var toDelete = remote
                .Where(l => local.All(r => r.Key != l.Key))
                .ToList();

            return toDelete;
        }

        static IReadOnlyList<RemoteConfigEntry> FindEntriesToCreate(
            IReadOnlyList<RemoteConfigEntry> remote,
            IReadOnlyList<RemoteConfigEntry> local)
        {
            var remoteSet = remote.Select(l => l.Key).ToHashSet();
            return local
                .Where(k => !remoteSet.Contains(k.Key))
                .ToList();
        }

        static IReadOnlyList<IRemoteConfigFile> FindFilesToDeploy(
            IReadOnlyList<RemoteConfigEntry> toDeploy)
        {
            return toDeploy
                .Where(entry => entry.File != null)
                .Select(entry => entry.File)
                .ToList();
        }
        
        static IReadOnlyList<IRemoteConfigFile> FindFailedFiles(
            IReadOnlyList<RemoteConfigDeploymentException> deploymentExceptions)
        {
            var failed = new List<IRemoteConfigFile>();

            foreach (var exception in deploymentExceptions)
            {
                failed.AddRange(exception.AffectedFiles);
            }

            return failed.Distinct().ToList();
        }
    }
}
