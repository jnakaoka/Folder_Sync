using log4net;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Folder_Sync.Synchronizer
{
    public class FolderSynchronizer : IFolderSynchronizer
    {
        private readonly ILog _logger;

        public FolderSynchronizer()
        {
            _logger = LogManager.GetLogger(typeof(FolderSynchronizer));
        }

        public void SyncFolders(string sourcePath, string replicaPath)
        {
            if (!Directory.Exists(sourcePath))
            {
                var message = $"Source folder '{sourcePath}' not found.";
                _logger.Error(message);
                throw new DirectoryNotFoundException(message);
            }

            if (!Directory.Exists(replicaPath))
            {
                Directory.CreateDirectory(replicaPath);
                _logger.Info($"Replica folder created: {replicaPath}");
            }

            // Copy new files or make the update if necessary
            foreach (var file in Directory.GetFiles(sourcePath))
            {
                var destFile = Path.Combine(replicaPath, Path.GetFileName(file));
                
                if (!File.Exists(destFile) || File.GetLastWriteTime(file) > File.GetLastWriteTime(destFile))
                {
                    File.Copy(file, destFile, true);
                    _logger.Info($"File copied: {file} -> {destFile}");
                }
            }

            // remove deleted files on source
            foreach (var file in Directory.GetFiles(replicaPath))
            {
                var sourceFile = Path.Combine(sourcePath, Path.GetFileName(file));
                if (!File.Exists(sourceFile))
                {
                    File.Delete(file);
                    _logger.Info($"File deleted: {file}");
                }
            }

            // Recursively sync subfolders
            foreach (var directory in Directory.GetDirectories(sourcePath))
            {
                var destDir = Path.Combine(replicaPath, Path.GetFileName(directory));
                SyncFolders(directory, destDir);
            }

            // Remove subfolders that no longer exist in source
            foreach (var directory in Directory.GetDirectories(replicaPath))
            {
                var sourceDir = Path.Combine(sourcePath, Path.GetFileName(directory));
                if (!Directory.Exists(sourceDir))
                {
                    Directory.Delete(directory, true);
                    _logger.Info($"Directory deleted: {directory}");
                }
            }
        }
    }
}
