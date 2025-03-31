using System.Collections.Generic;
using System.Linq;

namespace Unity.Services.RemoteConfig.Editor.Authoring.Core.Model
{
    static class RemoteConfigFileExtension
    {
        public static RemoteConfigFileContent ToFileContent(this IRemoteConfigFile file)
        {
            return new RemoteConfigFileContent(file.Entries);
        }

        public static void RemoveEntries(this IRemoteConfigFile file, IReadOnlyList<RemoteConfigEntry> entryToRemove)
        {
            file.Entries.RemoveAll(entry => entryToRemove.Any(remove => remove.Key == entry.Key));
        }
        
        public static void UpdateEntries(this IRemoteConfigFile file, IReadOnlyList<RemoteConfigEntry> entriesToUpdate)
        {
            foreach (var entryToUpdate in entriesToUpdate)
            {
               var entry = file.Entries.Find(entry => entry.Key == entryToUpdate.Key);
               
               if (entry != null)
               {
                   entry.Value = entryToUpdate.Value;
               }
            }
        }

        public static void UpdateOrCreateEntries(this IRemoteConfigFile file, IReadOnlyList<RemoteConfigEntry> entriesToUpdate)
        {
            foreach (var entryToUpdate in entriesToUpdate)
            {
                var entry = file.Entries.Find(entry => entry.Key == entryToUpdate.Key);
               
                if (entry != null)
                {
                    entry.Value = entryToUpdate.Value;
                }
                else
                {
                    file.Entries.Add(entryToUpdate);
                    entryToUpdate.File = file;
                }
            }
        }
    }
}