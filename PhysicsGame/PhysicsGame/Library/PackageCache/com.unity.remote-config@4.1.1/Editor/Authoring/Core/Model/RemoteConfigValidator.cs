using System.Collections.Generic;
using System.Linq;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.ErrorHandling;

namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.Model
{
    class RemoteConfigValidator : IRemoteConfigValidator
    {
        public IReadOnlyList<RemoteConfigEntry> FilterValidEntries(
            IReadOnlyList<IRemoteConfigFile> files,
            IReadOnlyList<RemoteConfigEntry> entries,
            ICollection<RemoteConfigDeploymentException> deploymentExceptions)
        {
            var validEntries = new List<RemoteConfigEntry>();
            foreach (var entry in entries)
            {
                var containingFiles = files
                    .Where(file => file.Entries.Any(fileEntry => fileEntry.Key == entry.Key))
                    .ToList();

                if (containingFiles.Count > 1)
                {
                    deploymentExceptions.Add(
                        new DuplicateKeysInMultipleFilesException(
                            entry.Key,
                            containingFiles));
                    continue;
                }
                validEntries.Add(entry);
            }

            return validEntries;
        }
    }
}
