namespace Helper
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Security.Permissions;
    using System.Threading;

    /// <summary>
    /// Method Collections
    /// </summary>
    public static class Stockpile
    {
        /// <summary>
        /// Validates File Path
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool ValidateFilepath(string filePath)
        {
            // Initialise permissions environment ..
            FileIOPermission permission = new FileIOPermission(PermissionState.None);
            permission.AllFiles = FileIOPermissionAccess.PathDiscovery;
            permission.Assert();
            
            // Test whether file path is valid ..
            try 
            { 
                Path.GetFullPath(filePath);
                Path.GetPathRoot(filePath);
                Path.GetFileName(filePath);

                // Reset permissions environment ..
                CodeAccessPermission.RevertAssert(); 
            }
            catch
            {
                 //pathname/filename could not be parsed.
                return false;
            }

            // Check read/write permission ..
            FileIOPermission checkFile = new FileIOPermission(FileIOPermissionAccess.AllAccess, filePath);
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
        public static string BackupFile(this string fileName, int maxNumberOfBackupsToKeep, int padSize)
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

        /// <summary>
        /// Get right-most part of string
        /// </summary>
        /// <param name="source"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Right(this string source, int length)
        {
            if (length >= source.Length)
            {
                return source;
            }
            else
            {
                return source.Substring(source.Length - length);
            }
        }

        /// <summary>
        /// Convert string to enumerated type
        /// </summary>
        /// <typeparam name="T">target: Enum Type</typeparam>
        /// <param name="enumTypeName"></param>
        /// <remarks>
        /// ref: http://stackoverflow.com/questions/16100/how-do-i-convert-a-string-to-an-enum-in-c
        /// </remarks>
        /// <returns></returns>
        public static T ParseStringToEnum<T>(string enumTypeName)
        {
            return (T)Enum.Parse(typeof(T), enumTypeName, true);
        }

        /// <summary>
        /// Simple File Printing facility
        /// </summary>
        /// <param name="filename"></param>
        public static void SendToPrinter(string filename)
        {
            // Define the print job ..
            ProcessStartInfo printjob = new ProcessStartInfo();
            printjob.Verb = "PRINT";
            printjob.FileName = @filename;
            printjob.CreateNoWindow = true;
            printjob.WindowStyle = ProcessWindowStyle.Hidden;

            // Start ..
            Process printProcess = new Process();
            printProcess.StartInfo = printjob;
            printProcess.Start();

            long ticks = -1;
            while (ticks != printProcess.TotalProcessorTime.Ticks)
            {
                ticks = printProcess.TotalProcessorTime.Ticks;
                Thread.Sleep(1000);
            }

            if (!printProcess.CloseMainWindow())
            {
                printProcess.Kill();
            }
        }
    }
}