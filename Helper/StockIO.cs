namespace Helper
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.AccessControl;
    using System.Security.Permissions;

    /// <summary>
    /// IO-related Helper Methods
    /// </summary>
    public static class StockIO
    {
        /// <summary>
        /// Validates File Path
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool ValidateFilepath(string filePath)
        {              
            // Test whether file path is valid ..
            try 
            { 
                Path.GetFullPath(filePath);
                Path.GetPathRoot(filePath);
                Path.GetFileName(filePath);
            }
            catch
            {
                // pathname/filename could not be parsed ..
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks File-Write write-access for Directory
        /// </summary>
        /// <remarks>
        ///  Not wholly accurate
        /// </remarks>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool CheckFileWriteAccessForDirectoryUsingFileIOPermission(string filePath)
        {
            // Check read/write permission ..
            FileIOPermission checkFile = new FileIOPermission(FileIOPermissionAccess.Write, filePath);
            try
            {
                checkFile.Demand();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks File-Write write-access for Directory
        /// </summary>
        /// <remarks>
        ///  Not wholly accurate
        /// </remarks>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static bool CheckFileWriteAccessForDirectoryUsingFileSystemRights(string directoryPath)
        {
            var accessControlList = Directory.GetAccessControl(directoryPath);
            var accessRules = accessControlList.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
            if (accessControlList == null || accessRules == null)
            {
                return false;
            }

            var writeAllow = false;
            var writeDeny = false;

            // Loop through layers of access rules ..
            foreach (FileSystemAccessRule rule in accessRules)
            {
                if ((FileSystemRights.Write & rule.FileSystemRights) != FileSystemRights.Write)
                {
                    continue;
                }

                if (rule.AccessControlType == AccessControlType.Allow)
                {
                    writeAllow = true;
                }
                else if (rule.AccessControlType == AccessControlType.Deny)
                {
                    writeDeny = true;
                }
            }

            return writeAllow && !writeDeny;
        }

        /// <summary>
        /// Checks File-Write write-access for Directory
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static bool CheckFileWriteAccessForDirectoryUsingFile(string directory)
        {
            // Initialise ..
            string junkFile = new Guid().ToString() + ".junkFile.tmp";
            bool writeAllow = false;

            // Test write in directory ...
            if (Directory.Exists(directory))
            {
                string fullPath = Path.Combine(directory, junkFile);

                try
                {
                    using (FileStream fs = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write))
                    {
                        fs.WriteByte(0xff);
                    }

                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                        writeAllow = true;
                    }
                }
                catch (Exception)
                {
                    writeAllow = false;
                }
            }

            return writeAllow;
        }

        /// <summary>
        /// Create Directory if needed ..
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isPathToFile"></param>
        /// <returns></returns>
        public static bool CreateDirectory(string path, bool isPathToFile)
        {
            // Initialise ..
            string targetDirectory = path;

            if (isPathToFile)
            {
                targetDirectory = Path.GetDirectoryName(path);
            }

            try
            {
                // Create target directory if needed ..
                if (!System.IO.Directory.Exists(targetDirectory))
                {
                    System.IO.Directory.CreateDirectory(targetDirectory);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Copy File
        /// </summary>
        /// <param name="sourceFullpath"></param>
        /// <param name="targetDirectory"></param>
        /// <returns></returns>
        public static string CopyFile(string sourceFullpath, string targetDirectory)
        {
            // Extract file name ..
            string targetFile = Path.GetFileName(sourceFullpath);
            string targetFullPath = Path.Combine(targetDirectory, targetFile);

            try
            {
                // Create target directory if needed ..
                CreateDirectory(targetDirectory, false);

                // .. prior to file copy ..
                System.IO.File.Copy(sourceFullpath, targetFullPath, true);
            }
            catch
            {
                return string.Empty;
            }

            return targetFullPath;
        }

        /// <summary>
        /// Make numbered backup copy of file 
        /// </summary>
        /// <remarks>
        ///  Rework of:
        ///     ref: http://www.rajapet.com/2014/03/a-file-versioning-helper-class-in-c-to-make-a-backup-copy-of-a-file-and-keep-the-last-n-copies-of-that-file.html/amp
        ///  Backup files have the name filename.exe.###
        ///   ### = zero justified sequence number starting at 1
        ///  Can get unexpected results (not fatal) when files exceed format limits
        /// </remarks>
        /// <param name="fileName"></param>
        /// <param name="maxNumberOfBackupsToKeep"></param>
        /// <param name="padSize"></param>
        /// <returns></returns>
        public static string BackupFile(string fileName, int maxNumberOfBackupsToKeep, int padSize)
        {
            // Initialise ..
            const char PadChar = '0';
            const char FileBackupSeparator = '.';
            string formatstring = new string(PadChar, padSize);
            var latestBackupFileName = string.Empty;
            int backupSequenceNumber = 1;
            padSize = padSize < 2 ? padSize = 2 : padSize;

            // Test for existing file of same name ..
            if (File.Exists(fileName))
            {
                // Get list of any previous file backup(s) ordered by their creation time ..
                var backupFiles = new DirectoryInfo(Path.GetDirectoryName(fileName)).GetFiles()
                                                                                    .Where(f => f.Name.Contains(Path.GetFileName(fileName) + FileBackupSeparator))
                                                                                    .OrderBy(f => f.CreationTimeUtc)
                                                                                    .ToList();
                try
                {
                    // Get name of last backup ... 
                    var lastBackupFilename = backupFiles.LastOrDefault().ToString();

                    // If at least one previous backup copy exists ..
                    if (lastBackupFilename != null)
                    {
                        // Derive its sequence number and add 1 ...
                        if (int.TryParse(Path.GetExtension(lastBackupFilename).Right(padSize), out backupSequenceNumber))
                        {
                            backupSequenceNumber++;

                            // Reset if sequence number exceeds that allowed by string format ..
                            if (backupSequenceNumber == int.Parse('1' + formatstring))
                            {
                                backupSequenceNumber = 1;
                            }
                        }

                        // Count existing backups ..
                        if (backupFiles.Count() >= maxNumberOfBackupsToKeep)
                        {
                            // Find file(s) for delete from top of list ..
                            var expiredFiles = backupFiles.Take(backupFiles.Count() - maxNumberOfBackupsToKeep + 1);

                            // Terminate expired file(s) ..
                            foreach (var expiredFile in expiredFiles)
                            {
                                File.Delete(Path.Combine(Path.GetDirectoryName(fileName), expiredFile.ToString()));
                            }
                        }
                    }
                }
                catch
                {
                    // No backup file(s) exist (yet) ..
                }

                // Create formatted backup file name for latest file ..
                latestBackupFileName = string.Format("{0}" + FileBackupSeparator + "{1:" + formatstring + "}", fileName, backupSequenceNumber);

                // Copy current file to new backup name (overwrite any existing file) ..
                File.Copy(fileName, latestBackupFileName, true);
            }

            // return ..
            return latestBackupFileName;
        }
    }
}