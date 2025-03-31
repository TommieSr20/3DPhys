using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.DeploymentApi.Editor;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.ErrorHandling;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.IO;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Json;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Model;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Results;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Service;

namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.Fetch
{
    abstract class RemoteConfigFetchHandler : IRemoteConfigFetchHandler
    {
        const string k_FetchResultName = "fetched_keys.rc";
        readonly IRemoteConfigClient m_Client;
        readonly IFileSystem m_FileSystem;
        readonly IJsonConverter m_JsonConverter;
        readonly IRemoteConfigValidator m_RemoteConfigValidator;
        readonly IRemoteConfigParser m_RemoteConfigParser;

        public RemoteConfigFetchHandler(
            IRemoteConfigClient client,
            IFileSystem fileSystem,
            IJsonConverter jsonConverter,
            IRemoteConfigValidator remoteConfigValidator,
            IRemoteConfigParser remoteConfigParser)
        {
            m_Client = client;
            m_FileSystem = fileSystem;
            m_JsonConverter = jsonConverter;
            m_RemoteConfigValidator = remoteConfigValidator;
            m_RemoteConfigParser = remoteConfigParser;
        }
        
        public async Task<FetchResult> FetchAsync(
            string rootDirectory,
            IReadOnlyList<IRemoteConfigFile> files,
            bool dryRun = false,
            bool reconcile = false,
            CancellationToken token = default)
        {
            var serverRemoteConfigResult = await m_Client.GetAsync();

            var fetchExceptions = new List<RemoteConfigDeploymentException>();
            
            var remote = serverRemoteConfigResult.ConfigsExists 
                ? serverRemoteConfigResult.Configs 
                : Array.Empty<RemoteConfigEntry>();

            await DeserializeFiles(files);
            
            var allLocal = GetLocalRemoteConfigEntries(files);
            var validLocal = m_RemoteConfigValidator.FilterValidEntries(files, allLocal, fetchExceptions);

            var failedFiles = FindFailedFiles(fetchExceptions);

            var toUpdate = FindEntriesToUpdate(remote, validLocal);
            var toDelete = FindEntriesToDelete(remote, validLocal);
            var toCreate = FindEntriesToCreate(remote, validLocal, reconcile);

            UpdateLocal(remote, toUpdate);
            DeleteLocal(files, toDelete);

            var filesToWrite = GetFilesToWrite(toUpdate, toDelete);
            var fetchedFiles = new List<IRemoteConfigFile>(){};

            if (!dryRun)
            {
                fetchedFiles = await WriteOrDeleteFiles(filesToWrite);
            }
            
            if (reconcile)
            {
                fetchedFiles = fetchedFiles
                    .Union(await CreateOrUpdateDefaultFile(rootDirectory, filesToWrite, toCreate, dryRun))
                    .ToList();
            }
            
            HandleDeploymentException(fetchExceptions);
            
            return new FetchResult(
                toCreate, 
                toUpdate, 
                toDelete, 
                fetchedFiles, 
                failedFiles);
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
            configFile.Entries = content.ToRemoteConfigEntries(configFile, m_RemoteConfigParser);
        
            return content;
        }

        protected abstract IRemoteConfigFile ConstructRemoteConfigFile(
            string path);
        
        async Task<List<IRemoteConfigFile>> CreateOrUpdateDefaultFile(string rootDirectory,
            IReadOnlyList<IRemoteConfigFile> filesToWrite,
            IReadOnlyList<RemoteConfigEntry> toCreate,
            bool dryRun)
        {
            var defaultFile = filesToWrite.FirstOrDefault(f => f.Name == k_FetchResultName);
            if (defaultFile == null)
            {
                defaultFile = await CreateNewRemoteConfigFile(rootDirectory, toCreate, dryRun);
            }
            else
            {
                defaultFile.UpdateOrCreateEntries(toCreate);

                await WriteOrDeleteFiles(new[] { defaultFile });
            }

            return defaultFile is null ? new List<IRemoteConfigFile>(){} : new List<IRemoteConfigFile>() { defaultFile };
        }

        static IReadOnlyList<RemoteConfigEntry> FindEntriesToUpdate(
            IReadOnlyList<RemoteConfigEntry> remote,
            IReadOnlyList<RemoteConfigEntry> local)
        {
            var toUpdate = local
                .Where(l => remote.Any(r => r.Key == l.Key))
                .ToList();

            return toUpdate;
        }

        static IReadOnlyList<RemoteConfigEntry> FindEntriesToDelete(
            IReadOnlyList<RemoteConfigEntry> remote,
            IReadOnlyList<RemoteConfigEntry> local)
        {
            var toDelete = local
                .Where(l => remote.All(r => r.Key != l.Key))
                .ToList();

            return toDelete;
        }

        static IReadOnlyList<RemoteConfigEntry> FindEntriesToCreate(
            IReadOnlyList<RemoteConfigEntry> remote,
            IReadOnlyList<RemoteConfigEntry> local,
            bool reconcile)
        {
            if (!reconcile)
                return new List<RemoteConfigEntry>();
            
            var localSet = local.Select(l => l.Key).ToHashSet();
            return remote
                .Where(k => !localSet.Contains(k.Key))
                .ToList();
        }

        static void UpdateLocal(
            IReadOnlyList<RemoteConfigEntry> remote,
            IReadOnlyList<RemoteConfigEntry> toUpdate)
        {
            foreach (var entry in toUpdate)
            {
                entry.File.UpdateEntries(remote);
            }
        }
        
        static void DeleteLocal(
            IReadOnlyList<IRemoteConfigFile> files,
            IReadOnlyList<RemoteConfigEntry> toDelete)
        {
            foreach (var file in files.Where(file=> file.Entries != null))
            {
                file.RemoveEntries(toDelete);
            }
        }

        static IReadOnlyList<IRemoteConfigFile> GetFilesToWrite(
            IReadOnlyList<RemoteConfigEntry> toUpdate,
            IReadOnlyList<RemoteConfigEntry> toDelete)
        {
            return toUpdate
                .Concat(toDelete)
                .Select(entry=> entry.File)
                .Distinct()
                .ToList();
        }

        async Task<List<IRemoteConfigFile>> WriteOrDeleteFiles(
            IReadOnlyList<IRemoteConfigFile> files)
        {
            var tasks = new List<Task>(files.Count);

            var outFiles = new List<IRemoteConfigFile>() { };

            foreach (var file in files)
            {
                if (file.Entries.Any())
                {
                    var content = new RemoteConfigFileContent(file.Entries);

                    var text = m_JsonConverter.SerializeObject(content);

                    tasks.Add(m_FileSystem.WriteAllText(file.Path, text));
                }
                else
                {
                    tasks.Add(m_FileSystem.Delete(file.Path));
                }
                // we assume everything will go well with IO ops
                outFiles.Add(file);
            }

            await Task.WhenAll(tasks);
            return outFiles;
        }

        async Task<IRemoteConfigFile> CreateNewRemoteConfigFile(
            string rootDirectory,
            IReadOnlyList<RemoteConfigEntry> toCreate,
            bool dryRun)
        {
            if (toCreate.Count == 0)
            {
                return null;
            }
            
            var filePath = Path.Combine(rootDirectory, k_FetchResultName);
            var file = ConstructRemoteConfigFile(filePath);

            foreach (var entry in toCreate)
            {
                entry.File = file;
            }

            if (dryRun)
            {
                return null;
            }
            
            var content = new RemoteConfigFileContent(toCreate);
            
            var serializeObject = m_JsonConverter.SerializeObject(content);
            
            await m_FileSystem.WriteAllText(filePath, serializeObject);
            return file;
        }

        static List<RemoteConfigEntry> GetLocalRemoteConfigEntries(IReadOnlyList<IRemoteConfigFile> files)
        {
            return files
                .Where(file => file.Entries != null)
                .SelectMany(file => file.Entries)
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
        
        void HandleDeploymentException(ICollection<RemoteConfigDeploymentException> deploymentExceptions)
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
        }
        
        void SetFailedStatus(IReadOnlyList<IRemoteConfigFile> files, string status = null, string detail = null)
        {
            SetStatusAndProgress(files, 
                status ?? "Failed to deploy", 
                detail ?? " Unknown Error", 
                SeverityLevel.Error, 
                0f);
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
        
        protected virtual void UpdateStatus(
            IRemoteConfigFile remoteConfigFile,
            string status,
            string detail,
            SeverityLevel severityLevel) {}

        protected virtual void UpdateProgress(
            IRemoteConfigFile remoteConfigFile,
            float progress) {}
    }
}