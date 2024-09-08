using Folder_Sync.Synchronizer;
using log4net;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Folder_Sync
{
    public partial class Worker : IHostedService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        private readonly FolderSynchronizer _folderSynchronizer;
        private readonly TimeSpan _syncInterval;

        public Worker(IOptions<WorkerSettings> options)
        {
            _folderSynchronizer = new FolderSynchronizer();
            if (TimeSpan.FromMinutes(options.Value.SyncIntervalInMinutes) > TimeSpan.Zero)
            {
                _syncInterval = TimeSpan.FromMinutes(options.Value.SyncIntervalInMinutes);
            }
            else
            {
                _syncInterval = TimeSpan.FromMinutes(10); // Sync Interval
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            log.Info("Folder Sync Service started.");

            string dirSource = Directory.GetCurrentDirectory();
            string refinedDirSource = new string(dirSource.Take(dirSource.Length - 21).ToArray());
            string dirReplica = Directory.GetCurrentDirectory();
            string refinedDirReplica = new string(dirReplica.Take(dirReplica.Length - 21).ToArray());

            ExecuteAsync(cancellationToken, Path.Combine(refinedDirSource, "SourceFolder"), Path.Combine(refinedDirReplica, "ReplicaFolder"), false);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            log.Info("Folder Sync Service stopped.");

            return Task.CompletedTask;
        }

        public async Task ExecuteAsync(CancellationToken stoppingToken, string sourceFolder, string replicaFolder, bool stopSync)
        {
            log.Info("Executing Folder Sync Service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                log.Info($"Running folder synchronization at {DateTimeOffset.Now}");

                try
                {
                    _folderSynchronizer.SyncFolders(sourceFolder, replicaFolder);
                    log.Info("Synchronization complete.");
                    if (stopSync)
                    {
                        log.Info("Folder Sync Service stopped.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    log.Error("An error occurred during folder synchronization.", ex);
                }

                await Task.Delay(_syncInterval, stoppingToken);
            }
        }
    }
}

