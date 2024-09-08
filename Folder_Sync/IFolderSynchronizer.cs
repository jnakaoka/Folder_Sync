using System.IO;

namespace Folder_Sync
{
    public interface IFolderSynchronizer
    {
        void SyncFolders(string sourceFolder, string replicaFolder);
    }
}
