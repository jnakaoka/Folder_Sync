using Folder_Sync.Synchronizer;
using log4net.Core;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Folder_Sync.Test
{
    [TestFixture]
    public class FolderSynchronizerTests
    {
        private Mock<ILogger> _mockLogger;
        private FolderSynchronizer _folderSynchronizer;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger>();
            _folderSynchronizer = new FolderSynchronizer();
        }

        [Test]
        public void SyncFolders_ShouldCopyNewFilesAndUpdateModifiedFiles()
        {
            var sourcePath = Path.Combine(Directory.GetCurrentDirectory(), "source");
            var replicaPath = Path.Combine(Directory.GetCurrentDirectory(), "replica");
            var sourceFile1 = Path.Combine(sourcePath, "file1.txt");
            var sourceFile2 = Path.Combine(sourcePath, "file2.txt");
            var replicaFile1 = Path.Combine(replicaPath, "file1.txt");
            var replicaFile2 = Path.Combine(replicaPath, "file2.txt");
            Directory.CreateDirectory(sourcePath);
            Directory.CreateDirectory(replicaPath);
            File.WriteAllText(sourceFile1, "source file 1");
            File.WriteAllText(sourceFile2, "source file 2");
            File.WriteAllText(replicaFile1, "replica file 1");
            File.WriteAllText(replicaFile2, "replica file 2");
            File.SetLastWriteTime(sourceFile1, DateTime.Now.AddHours(-1));
            File.SetLastWriteTime(sourceFile2, DateTime.Now.AddHours(-2));
            File.SetLastWriteTime(replicaFile1, DateTime.Now.AddHours(-3));
            File.SetLastWriteTime(replicaFile2, DateTime.Now.AddHours(-4));

            _folderSynchronizer.SyncFolders(sourcePath, replicaPath);

            // Assert
            Assert.That(File.ReadAllText(replicaFile1), Is.EqualTo("source file 1"));
            Assert.That(File.ReadAllText(replicaFile2), Is.EqualTo("source file 2"));
        }

        [Test]
        public void SyncFolders_ShouldDeleteFilesThatNoLongerExistInSource()
        {
            var sourcePath = "source";
            var replicaPath = "replica";
            var sourceFile1 = Path.Combine(sourcePath, "file1.txt");
            var sourceFile2 = Path.Combine(sourcePath, "file2.txt");
            var replicaFile1 = Path.Combine(replicaPath, "file1.txt");
            var replicaFile2 = Path.Combine(replicaPath, "file2.txt");
            File.WriteAllText(sourceFile1, "source file 1");
            File.WriteAllText(sourceFile2, "source file 2");
            File.WriteAllText(replicaFile1, "replica file 1");
            File.WriteAllText(replicaFile2, "replica file 2");
            File.Delete(sourceFile2);

            _folderSynchronizer.SyncFolders(sourcePath, replicaPath);

            // Assert
            Assert.That(File.Exists(replicaFile2), Is.False);
        }

        [Test]
        public void SyncFolders_ShouldRecursivelySyncSubdirectories()
        {
            var sourcePath = "source";
            var replicaPath = "replica";
            var sourceSubdir1 = Path.Combine(sourcePath, "subdir1");
            var sourceSubdir2 = Path.Combine(sourcePath, "subdir2");
            var replicaSubdir1 = Path.Combine(replicaPath, "subdir1");
            var replicaSubdir2 = Path.Combine(replicaPath, "subdir2");
            Directory.CreateDirectory(sourceSubdir1);
            Directory.CreateDirectory(sourceSubdir2);
            Directory.CreateDirectory(replicaSubdir1);
            Directory.CreateDirectory(replicaSubdir2);
            var sourceFile1 = Path.Combine(sourceSubdir1, "file1.txt");
            var sourceFile2 = Path.Combine(sourceSubdir2, "file2.txt");
            var replicaFile1 = Path.Combine(replicaSubdir1, "file1.txt");
            var replicaFile2 = Path.Combine(replicaSubdir2, "file2.txt");
            File.WriteAllText(sourceFile1, "source file 1");
            File.WriteAllText(sourceFile2, "source file 2");
            File.WriteAllText(replicaFile1, "replica file 1");
            File.WriteAllText(replicaFile2, "replica file 2");
            File.SetLastWriteTime(sourceFile1, DateTime.Now.AddHours(-1));
            File.SetLastWriteTime(sourceFile2, DateTime.Now.AddHours(-2));
            File.SetLastWriteTime(replicaFile1, DateTime.Now.AddHours(-3));
            File.SetLastWriteTime(replicaFile2, DateTime.Now.AddHours(-4));

            _folderSynchronizer.SyncFolders(sourcePath, replicaPath);

            // Assert
            Assert.That(File.ReadAllText(replicaFile1), Is.EqualTo("source file 1"));
            Assert.That(File.ReadAllText(replicaFile2), Is.EqualTo("source file 2"));
        }
    } 
}