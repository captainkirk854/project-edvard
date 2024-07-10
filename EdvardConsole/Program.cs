namespace EdvardConsole
{
    using GameKey.Adapter;
    using GameKey.Binding.Analysis;
    using GameKey.Binding.Readers;
    using GameKey.Binding.Writers;
    using Items;
    using System;
    using System.Data;
    using System.IO;
    using Utility;

    public class Program
    {
        private const string VersionNumber = "1.001";
        private const string DesktopKeyword = "desktop";
        private const int BackupCycle = 50;
        private const int BackupFilenameLeftPadSize = 4;

        private static readonly string DefaultEliteDangerousBindingsDirectory = Environment.ExpandEnvironmentVariables("%LOCALAPPDATA%") + "\\Frontier Developments\\Elite Dangerous\\Options\\Bindings";
        private static readonly string DefaultVoiceAttackProfilesDirectory = Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%") + "\\VoiceAttack\\Sounds\\hcspack\\Profiles";
        private static readonly string UserDesktop = Environment.ExpandEnvironmentVariables("%UserProfile%") + "\\Desktop";

        #region [Internal Test Settings]
        // Support for crude test harness ..
        private static readonly string InternalTestRootDirectory = AppRuntime.SolutionDirectory + "\\Tests\\TestData\\Integration";
        private static readonly string InternalTestSetAnalysisInputDirectory = "in";
        private static readonly string InternalTestSetAnalysisOutputDirectory = "out";
        #endregion

        public static void Main(string[] args)
        {
            #region [Command-Line Argument Initialisation]
            string filepathEliteDangerousBinds = string.Empty;
            string filepathVoiceAttackProfile = string.Empty;

            // Parse Command Line arguments ..
            CommandLine commands = new CommandLine(args);

            // Mandatory argument(s) ..
            string argFilePathBinds = commands.Parse(EDVArd.ArgOption.binds.ToString(), true);
            string argFilePathVap = commands.Parse(EDVArd.ArgOption.vap.ToString(), true);
            string argModeSync = commands.Parse(EDVArd.ArgOption.sync.ToString());

            // Optional argument(s)..
            string argDirectoryPathBackup = commands.Parse(EDVArd.ArgOption.backup.ToString(), true);
            string argDirectoryPathAnalysis = commands.Parse(EDVArd.ArgOption.analysis.ToString(), true);
            string argAnalysisFileFormat = commands.Parse(EDVArd.ArgOption.format.ToString());
            string argFilePathDictionaryWrite = commands.Parse(EDVArd.ArgOption.export.ToString(), true);
            string argFilePathDictionaryRead = commands.Parse(EDVArd.ArgOption.import.ToString(), true);
            string argTestSet = commands.Parse(EDVArd.ArgOption.test.ToString());
            bool argCreateReferenceTag = Convert.ToBoolean(commands.Parse(EDVArd.ArgOption.tag.ToString()));

            // Specials for arguments containing file paths ..
            if (argDirectoryPathBackup == "true") { argDirectoryPathBackup = null; }
            if (argDirectoryPathAnalysis == "true") { argDirectoryPathAnalysis = null; }
            if (argFilePathDictionaryWrite == "true") { argFilePathDictionaryWrite = null; }
            if (argFilePathDictionaryRead == "true") { argFilePathDictionaryRead = null; }
            try
            {
                if (argDirectoryPathBackup.ToLower() == DesktopKeyword) { argDirectoryPathBackup = UserDesktop; }
                if (argDirectoryPathAnalysis.ToLower() == DesktopKeyword) { argDirectoryPathAnalysis = UserDesktop; }
                if (argFilePathDictionaryWrite.ToLower() == DesktopKeyword) { argFilePathDictionaryWrite = UserDesktop; }
                if (argFilePathDictionaryRead.ToLower() == DesktopKeyword) { argFilePathDictionaryRead = UserDesktop; }
            }
            catch
            { 
            }

            // Default to CSV format if analysis format not defined ..
            argAnalysisFileFormat = argAnalysisFileFormat == null ? EDVArd.ArgSubOption.csv.ToString() : argAnalysisFileFormat;
            #endregion

            #region [Command-Line Argument Validation]

            // Help Message ..
            if (Convert.ToBoolean(commands.Parse(EDVArd.ArgOption.help.ToString())))
            {
                ConsistentExit();
            }

            // Processing mode ..
            if (argModeSync == null || argModeSync == "true")
            {
                Console.WriteLine();
                Console.WriteLine("A valid synchronisation mode must be selected!" + System.Environment.NewLine);
                Console.WriteLine(" e.g.");
                Console.WriteLine("     /{0} {1}", EDVArd.ArgOption.sync.ToString(), EDVArd.ArgSubOption.oneway_to_binds.ToString());
                Console.WriteLine("     /{0} {1}", EDVArd.ArgOption.sync.ToString(), EDVArd.ArgSubOption.twoway.ToString());
                Console.WriteLine();
                ConsistentExit();
            }

            // Determine file-type (user/test) to be processed (e.g. /test Set01 /sync:twoway) ..
            if (argTestSet == null)
            {
                if (File.Exists(argFilePathBinds))
                {
                    filepathEliteDangerousBinds = argFilePathBinds;
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("Path to Elite Dangerous Binds (.binds) File must be valid!" + System.Environment.NewLine);
                    Console.WriteLine(" e.g. /{0} {1}", EDVArd.ArgOption.binds.ToString(), Path.Combine(DefaultEliteDangerousBindingsDirectory, "Custom.binds"));
                    Console.WriteLine();
                    ConsistentExit();
                }

                if (File.Exists(argFilePathVap))
                {
                    filepathVoiceAttackProfile = argFilePathVap;
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("Path to Voice Attack Profile (.vap) File must be valid!" + System.Environment.NewLine);
                    Console.WriteLine(" e.g. /{0} {1}", EDVArd.ArgOption.vap.ToString(), Path.Combine(DefaultVoiceAttackProfilesDirectory, "Custom.vap"));
                    Console.WriteLine();
                    ConsistentExit();
                }
            }
            else
            {
                // internal development test is active ..
                Console.WriteLine("Using internal test data (test-set: {0}) ..", argTestSet);
                Console.WriteLine();

                // Select first file of each type as test files to use ..
                var internalTestDirectory = HandleIO.GetCaseSensitiveDirectoryPath(Path.Combine(InternalTestRootDirectory, argTestSet));
                var internalTestInputDataDirectory = HandleIO.GetCaseSensitiveDirectoryPath(Path.Combine(internalTestDirectory, InternalTestSetAnalysisInputDirectory));
                filepathEliteDangerousBinds = Directory.GetFiles(internalTestInputDataDirectory, "*.binds")[0];
                filepathVoiceAttackProfile = Directory.GetFiles(internalTestInputDataDirectory, "*.vap")[0];

                // Force redirect of analysis result(s) to internal test area ..
                argDirectoryPathAnalysis = Path.Combine(internalTestDirectory, InternalTestSetAnalysisOutputDirectory);
                argAnalysisFileFormat = EDVArd.ArgSubOption.csv.ToString();
            }
            
            // Final Check ..
            if (!(File.Exists(filepathEliteDangerousBinds) && File.Exists(filepathVoiceAttackProfile)))
            {
                Console.WriteLine("Required file(s) are missing!");
                PressIt();
                Environment.Exit(0);
            }
            #endregion

            #region [Initialision]

            // Set key type enumeration type to use ..
            KeyBindingReader.KeyType = KeyEnum.Type.WindowsForms;

            // Initialise lookup dictionary for inter-game action references ..
            GameKeyAndCommandBindingsAdapter bindingsAdapter = null;
            try
            {
                bindingsAdapter = new GameKeyAndCommandBindingsAdapter();
            }
            catch
            {
                Console.WriteLine("Action Exchange Dictionary is invalid");
                PressIt();
                Environment.Exit(0);
            }

            // Optional arg: Backup
            if (argDirectoryPathBackup == null)
            {
                Console.WriteLine("unused option: /" + EDVArd.ArgOption.backup.ToString());
            }

            // Optional arg: Dictionary export
            if (HandleIO.ValidateFilepath(argFilePathDictionaryWrite) && HandleIO.CreateDirectory(argFilePathDictionaryWrite, true))
            {
                bindingsAdapter.Export(argFilePathDictionaryWrite);
            }
            else
            {
                Console.WriteLine("unused option: /{0}", EDVArd.ArgOption.export.ToString());
            }

            // Optional arg: Dictionary import ..
            if (File.Exists(argFilePathDictionaryRead))
            {
                bindingsAdapter.Import(argFilePathDictionaryRead);
            }
            else
            {
                Console.WriteLine("unused option: /{0}", EDVArd.ArgOption.import.ToString());
            }

            if (!argCreateReferenceTag)
            {
                Console.WriteLine("unused option: /{0}", EDVArd.ArgOption.tag.ToString());
            }
            #endregion

            #region [File Processing]
            try
            {
                #region [Read and update VoiceAttack Configuration File]
                // Update VoiceAttack Profile (optional) ..
                if ((argModeSync == EDVArd.ArgSubOption.twoway.ToString()) || (argModeSync == EDVArd.ArgSubOption.oneway_to_vap.ToString()))
                {
                    // Intro ..
                    Console.WriteLine(System.Environment.NewLine);
                    Console.WriteLine("Attempting VoiceAttack Profile update ..");

                    // Backup (optional) ..
                    Console.WriteLine("backup: {0}", HandleIO.SequentialFileBackup(argDirectoryPathBackup, filepathVoiceAttackProfile, BackupCycle, BackupFilenameLeftPadSize).ToString());

                    // Attempt synchronisation update ..
                    KeyBindingWriterVoiceAttack newVoiceAttack = new KeyBindingWriterVoiceAttack();
                    Console.WriteLine("Voice Attack Profile: {0}", newVoiceAttack.Update(KeyBindingAnalyser.VoiceAttack(filepathEliteDangerousBinds, filepathVoiceAttackProfile, bindingsAdapter), argCreateReferenceTag) == true ? "updated" : "no update possible or required");
                }
                else
                {
                    Console.WriteLine("VoiceAttack Profile update: not selected");
                }
                #endregion

                #region [Read and update EliteDangerous Configuration File]
                // Reverse-synchronise any vacant Elite Dangerous Bindings (optional) ..
                if ((argModeSync == EDVArd.ArgSubOption.twoway.ToString()) || (argModeSync == EDVArd.ArgSubOption.oneway_to_binds.ToString()))
                {
                    // Intro ..
                    Console.WriteLine(System.Environment.NewLine);
                    Console.WriteLine("Attempting Elite Dangerous Binds update ..");

                    // Backup (optional) ..
                    Console.WriteLine("backup: {0}", HandleIO.SequentialFileBackup(argDirectoryPathBackup, filepathEliteDangerousBinds, BackupCycle, BackupFilenameLeftPadSize).ToString());

                    // Attempt synchronisation update ..
                    KeyBindingWriterEliteDangerous newEliteDangerous = new KeyBindingWriterEliteDangerous();
                    Console.WriteLine("Elite Dangerous Binds: {0}", newEliteDangerous.Update(KeyBindingAnalyser.EliteDangerous(filepathEliteDangerousBinds, filepathVoiceAttackProfile, bindingsAdapter), argCreateReferenceTag) == true ? "updated" : "no update possible or required");
                }
                else
                {
                    Console.WriteLine("Elite Dangerous Binds update: not selected");
                }
                #endregion

                #region [Test-specific copy-processed-files-to-target]
                if (argTestSet != null)
                {
                    Console.WriteLine();
                    Console.WriteLine("Copying processed test file(s) to {0}", argDirectoryPathAnalysis);
                    HandleIO.CopyFile(filepathEliteDangerousBinds, argDirectoryPathAnalysis);
                    HandleIO.CopyFile(filepathVoiceAttackProfile, argDirectoryPathAnalysis);
                }
                #endregion

                #region [Analysis]
                Console.WriteLine(System.Environment.NewLine);

                // Re-read Voice Attack Commands and Elite Dangerous Binds for analysis Information ..
                if (HandleIO.ValidateFilepath(argDirectoryPathAnalysis) && HandleIO.CreateDirectory(argDirectoryPathAnalysis, false))
                {
                    Console.WriteLine("Preparing analysis data ..");

                    // Read (updated) file(s) ..
                    KeyBindingReaderEliteDangerous eliteDangerous = new KeyBindingReaderEliteDangerous(filepathEliteDangerousBinds);
                    KeyBindingReaderVoiceAttack voiceAttack = new KeyBindingReaderVoiceAttack(filepathVoiceAttackProfile);

                    string sortOrder = string.Empty;

                    // Elite Dangerous ..
                    sortOrder = Items.EDVArd.Column.KeyAction.ToString() + ',' + Items.EDVArd.Column.DevicePriority.ToString();
                    DataTable bindableEliteDangerous = eliteDangerous.GetBindableCommands().Sort(sortOrder);
                    DataTable keyBoundEliteDangerous = eliteDangerous.GetBoundCommands().Sort(sortOrder);

                    // Voice Attack ..
                    sortOrder = Items.EDVArd.Column.KeyAction.ToString();
                    DataTable bindableVoiceAttack = voiceAttack.GetBindableCommands().Sort(sortOrder);
                    DataTable keyBoundVoiceAttack = voiceAttack.GetBoundCommands().Sort(sortOrder);

                    sortOrder = Items.EDVArd.Column.VoiceAttackCategory.ToString() + ',' + Items.EDVArd.Column.VoiceAttackCommand.ToString();
                    DataTable allVoiceAttackCommands = voiceAttack.GetCommandStringsForAllCategories().Sort(sortOrder);

                    // Consolidated ..
                    sortOrder = Items.EDVArd.Column.EliteDangerousAction.ToString() + ',' + Items.EDVArd.Column.VoiceAttackAction.ToString();
                    DataTable consolidatedBoundActions = KeyBindingAnalyser.VoiceAttack(filepathEliteDangerousBinds, filepathVoiceAttackProfile, bindingsAdapter).Sort(sortOrder);
                    DataTable relatedVoiceAttackCommands = voiceAttack.GetRelatedCommandStrings(consolidatedBoundActions).Sort(sortOrder);

                    // Create appropriate type of analysis file ..
                    try
                    {
                        Console.WriteLine("Creating '{0}' analysis file(s) in {1}", argAnalysisFileFormat, argDirectoryPathAnalysis);
                        const char Dlm = '-';

                        switch (HandleStrings.ParseStringToEnum<EDVArd.ArgSubOption>(argAnalysisFileFormat))
                        {
                            case EDVArd.ArgSubOption.csv:
                                bindableEliteDangerous.CreateCSV(argDirectoryPathAnalysis, Application.Name.EliteDangerous.ToString() + Dlm + EDVArd.AnalysisFile.KeyBindableActions.ToString());
                                bindableVoiceAttack.CreateCSV(argDirectoryPathAnalysis, Application.Name.VoiceAttack.ToString() + Dlm + EDVArd.AnalysisFile.KeyBindableActions.ToString());
                                keyBoundEliteDangerous.CreateCSV(argDirectoryPathAnalysis, Application.Name.EliteDangerous.ToString() + Dlm + EDVArd.AnalysisFile.KeyBoundActions.ToString());
                                keyBoundVoiceAttack.CreateCSV(argDirectoryPathAnalysis, Application.Name.VoiceAttack.ToString() + Dlm + EDVArd.AnalysisFile.KeyBoundActions.ToString());
                                consolidatedBoundActions.CreateCSV(argDirectoryPathAnalysis, Application.Name.Edvard.ToString() + Dlm + EDVArd.AnalysisFile.ConsolidatedKeyBoundActions.ToString());
                                relatedVoiceAttackCommands.CreateCSV(argDirectoryPathAnalysis, Application.Name.VoiceAttack.ToString() + Dlm + EDVArd.AnalysisFile.RelatedCommands.ToString());
                                allVoiceAttackCommands.CreateCSV(argDirectoryPathAnalysis, Application.Name.VoiceAttack.ToString() + Dlm + EDVArd.AnalysisFile.AllCommands.ToString());
                                break;

                            case EDVArd.ArgSubOption.htm:
                                bindableEliteDangerous.CreateHTM(argDirectoryPathAnalysis, Application.Name.EliteDangerous.ToString() + Dlm + EDVArd.AnalysisFile.KeyBindableActions.ToString());
                                bindableVoiceAttack.CreateHTM(argDirectoryPathAnalysis, Application.Name.VoiceAttack.ToString() + Dlm + EDVArd.AnalysisFile.KeyBindableActions.ToString());
                                keyBoundEliteDangerous.CreateHTM(argDirectoryPathAnalysis, Application.Name.EliteDangerous.ToString() + Dlm + EDVArd.AnalysisFile.KeyBoundActions.ToString());
                                keyBoundVoiceAttack.CreateHTM(argDirectoryPathAnalysis, Application.Name.VoiceAttack.ToString() + Dlm + EDVArd.AnalysisFile.KeyBoundActions.ToString());
                                consolidatedBoundActions.CreateHTM(argDirectoryPathAnalysis, Application.Name.Edvard.ToString() + Dlm + EDVArd.AnalysisFile.ConsolidatedKeyBoundActions.ToString());
                                relatedVoiceAttackCommands.CreateHTM(argDirectoryPathAnalysis, Application.Name.VoiceAttack.ToString() + Dlm + EDVArd.AnalysisFile.RelatedCommands.ToString());
                                allVoiceAttackCommands.CreateHTM(argDirectoryPathAnalysis, Application.Name.VoiceAttack.ToString() + Dlm + EDVArd.AnalysisFile.AllCommands.ToString());
                                break;
                        }
                    }
                    catch
                    {
                        Console.WriteLine("unknown analysis format: {0}", argAnalysisFileFormat);
                    }
                }
                else
                {
                    Console.WriteLine("unused option: /{0}", EDVArd.ArgOption.analysis.ToString());
                }
                #endregion

                PressIt();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong ... we cry real tears ...");
                Console.WriteLine(" > Exception Message [{0}]", ex.Message);
                PressIt();
                throw;
            }
            #endregion
        }

        #region UsageInfo
        /// <summary>
        /// Command Line Usage Information
        /// </summary>
        private static void ShowUsage()
        {
            string description = "EDVArd [Elite Dangerous/Voice Attack reader] " +
                                 System.Environment.NewLine +
                                 "                                            v." + VersionNumber +
                                 System.Environment.NewLine +
                                 "                                            (c)2016 MarMaSoPHt854 " +
                                 System.Environment.NewLine;

            string helpInformation =
                                 description +
                                 System.Environment.NewLine +
                                 "Key " +
                                 System.Environment.NewLine +
                                 System.Environment.NewLine +
                                 "  [mandatory]" +
                                 System.Environment.NewLine +
                                 "  /" + EDVArd.ArgOption.binds.ToString() + System.Environment.NewLine +
                                 "           File path to Elite Dangerous .binds" + System.Environment.NewLine +
                                 "  /" + EDVArd.ArgOption.vap.ToString() + System.Environment.NewLine +
                                 "           File path to Voice Attack .vap" + System.Environment.NewLine +
                                 "  /" + EDVArd.ArgOption.sync.ToString() + System.Environment.NewLine +
                                 "           Synchronisation Mode" + System.Environment.NewLine +
                                 "            :" + EDVArd.ArgSubOption.twoway.ToString() + System.Environment.NewLine +
                                 "            :" + EDVArd.ArgSubOption.oneway_to_vap.ToString() + System.Environment.NewLine +
                                 "            :" + EDVArd.ArgSubOption.oneway_to_binds.ToString() + System.Environment.NewLine +
                                 "            :" + EDVArd.ArgSubOption.none.ToString() + System.Environment.NewLine +
                                 System.Environment.NewLine +
                                 "  [optional]" +
                                 System.Environment.NewLine +
                                 "  /" + EDVArd.ArgOption.help.ToString() + System.Environment.NewLine +
                                 "           This help" + System.Environment.NewLine +
                                 "  /" + EDVArd.ArgOption.backup.ToString() + System.Environment.NewLine +
                                 "           Directory path for backup file(s)" + System.Environment.NewLine +
                                 "  /" + EDVArd.ArgOption.analysis.ToString() + System.Environment.NewLine +
                                 "           Directory path for operational analysis file(s)" + System.Environment.NewLine +
                                 "  /" + EDVArd.ArgOption.format.ToString() + System.Environment.NewLine +
                                 "           File format for operational analysis file(s) (csv[default], htm)" + System.Environment.NewLine +
                                 "  /" + EDVArd.ArgOption.tag.ToString() + System.Environment.NewLine +
                                 "           Create reference tag in affected file(s)" + System.Environment.NewLine +
                                 "  /" + EDVArd.ArgOption.export.ToString() + System.Environment.NewLine +
                                 "           File path to export action dictionary" + System.Environment.NewLine +
                                 "  /" + EDVArd.ArgOption.import.ToString() + System.Environment.NewLine +
                                 "           File path to import action dictionary";

            string usageExamples =
                                 "Sample Usage" +
                                 System.Environment.NewLine +
                                 System.Environment.NewLine +
                                 "/binds \"C:\\Elite Dangerous\\My.binds\" /vap C:\\HCSVoicePack\\My.vap /sync:twoway /analysis desktop /tag" +
                                 System.Environment.NewLine +
                                 "           Attempts bidirectional synchronisation, will tag affected file(s) and Analysis File(s) written to user desktop" +
                                 System.Environment.NewLine +
                                 "            note: in all cases, the Elite Dangerous key binds are master. Only unbound actions(s) in the .binds file can be updated by this utility" +
                                 System.Environment.NewLine +
                                 System.Environment.NewLine +
                                 "/binds \"C:\\Elite Dangerous\\My.binds\" /vap C:\\HCSVoicePack\\My.vap /sync:oneway_to_vap /backup \"C:\\My Backups\"" +
                                 System.Environment.NewLine +
                                 "           Attempts update of Voice Attack Profile with backup of affected file" + 
                                 System.Environment.NewLine +
                                 System.Environment.NewLine +
                                 "/binds \"C:\\Elite Dangerous\\My.binds\" /vap C:\\HCSVoicePack\\My.vap /sync:none /export \"C:\\My Actions\\Action001.xml\"" +
                                 System.Environment.NewLine +
                                 "           No update of either file type, but will export Action Dictionary as .xml file" +
                                 System.Environment.NewLine +
                                 System.Environment.NewLine +
                                 "/binds \"C:\\Elite Dangerous\\My.binds\" /vap \"C:\\HCSVoicePack\\My.vap\" /sync:twoway /import \"C:\\My Actions\\Action001_modified.xml\" /tag" +
                                 System.Environment.NewLine +
                                 "           Attempts bidirectional synchronisation using a modified Action Dictionary to override internal dictionary, and will internally tag updated file(s)";

            string disclaimer =
                                 System.Environment.NewLine +
                                 System.Environment.NewLine +
                                 "Disclaimer:" +
                                 System.Environment.NewLine +
                                 System.Environment.NewLine + 
                                 "Software downloaded is provided 'as is' without warranty of any kind, either express or implied, including, but not limited to, " + System.Environment.NewLine +
                                 "the implied warranties of fitness for a purpose, or the warranty of non-infringement." + System.Environment.NewLine +
                                 System.Environment.NewLine + 
                                 "Without limiting the foregoing, there is no warranty that: " + System.Environment.NewLine +
                                    "  i.the software will meet your requirements" + System.Environment.NewLine +
                                    " ii.the software will be uninterrupted, timely, secure or error-free" + System.Environment.NewLine +
                                    "iii.the results that may be obtained from the use of the software will be effective, accurate or reliable" + System.Environment.NewLine +
                                    " iv.the quality of the software will meet your expectations" + System.Environment.NewLine +
                                    "  v.any errors in the software obtained will be corrected.";

            // Display to user ..
            Console.WriteLine(System.Environment.NewLine);
            Console.WriteLine(helpInformation);
            Console.WriteLine(System.Environment.NewLine);
            Console.WriteLine(usageExamples);
            Console.WriteLine(System.Environment.NewLine);
            Console.WriteLine(disclaimer);
            Console.WriteLine(System.Environment.NewLine);
        }
        #endregion

        /// <summary>
        /// Consistent Exit
        /// </summary>
        private static void ConsistentExit()
        {
            ShowUsage();
            PressIt();
            Environment.Exit(0);
        }

        /// <summary>
        /// We laughed, we cried ..
        /// </summary>
        private static void PressIt()
        {
            Console.WriteLine();
            Console.WriteLine("Press a key to continue ..");
            Console.ReadKey();
        }
    }
}