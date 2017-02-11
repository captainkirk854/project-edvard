namespace UXConsole
{
    using Binding;
    using Helper;
    using Items;
    using KeyHelper;
    using System;
    using System.Data;
    using System.IO;

    public class Program
    {
        private const string VersionNumber = "1.001";
        private const string DesktopKeyword = "desktop";
        private const string CSV = "csv";
        private const string HTM = "html";
        private const int BackupCycle = 50;
        private const int BackupFilenameLeftPadSize = 4;
        private const string Commands = "edvCommands";
        private const string Bindings = "edvCommand_Bindings";
        private const string Consolidated = "edvConsolidated_Bindings";
        private const string Associated = "edvAssociated_Commands";
        private const string AllCommands = "edvAll_Commands";
        private static readonly string DefaultEliteDangerousBindingsDirectory = Environment.ExpandEnvironmentVariables("%LOCALAPPDATA%") + "\\Frontier Developments\\Elite Dangerous\\Options\\Bindings";
        private static readonly string DefaultVoiceAttackProfilesDirectory = Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%") + "\\VoiceAttack\\Sounds\\hcspack\\Profiles";
        private static readonly string UserDesktop = Environment.ExpandEnvironmentVariables("%UserProfile%") + "\\Desktop";

        /// <summary>
        /// Enumeration of Arguments
        /// </summary>
        private enum ArgOption
        {
            binds,
            vap,
            sync,
            import,
            export,
            tag,
            analysis,
            format,
            backup,
            help,
            sample
        }

        /// <summary>
        /// Enumeration of Argument Sub Options
        /// </summary>
        private enum ArgSubOption
        {
            twoway,
            oneway_to_binds,
            oneway_to_vap,
            csv,
            htm,
            none
        }

        public static void Main(string[] args)
        {
            #region [Command-Line Argument Initialisation]
            string eliteDangerousBinds = string.Empty;
            string voiceAttackProfile = string.Empty;

            // Parse Command Line arguments ..
            CommandLine commands = new CommandLine(args);

            // Mandatory argument(s) ..
            string argFilePathBinds = commands.Parse(ArgOption.binds.ToString(), true);
            string argFilePathVap = commands.Parse(ArgOption.vap.ToString(), true);
            string argModeSync = commands.Parse(ArgOption.sync.ToString());

            // Optional argument(s)..
            string argDirectoryPathBackup = commands.Parse(ArgOption.backup.ToString(), true);
            string argDirectoryPathAnalysis = commands.Parse(ArgOption.analysis.ToString(), true);
            string argAnalysisFileFormat = commands.Parse(ArgOption.format.ToString());
            bool argCreateReferenceTag = Convert.ToBoolean(commands.Parse(ArgOption.tag.ToString()));
            string argFilePathDictionaryWrite = commands.Parse(ArgOption.export.ToString(), true);
            string argFilePathDictionaryRead = commands.Parse(ArgOption.import.ToString(), true);
            string argSample = commands.Parse(ArgOption.sample.ToString());

            // Specials 
            if ((argDirectoryPathBackup != null) && (argDirectoryPathBackup.ToLower() == DesktopKeyword)) { argDirectoryPathBackup = UserDesktop; }
            if ((argDirectoryPathAnalysis != null) && (argDirectoryPathAnalysis.ToLower() == DesktopKeyword)) { argDirectoryPathAnalysis = UserDesktop; }
            if ((argFilePathDictionaryWrite != null) && (argFilePathDictionaryWrite.ToLower() == DesktopKeyword)) { argFilePathDictionaryWrite = UserDesktop; }
            if ((argFilePathDictionaryRead != null) && (argFilePathDictionaryRead.ToLower() == DesktopKeyword)) { argFilePathDictionaryRead = UserDesktop; }
            argAnalysisFileFormat = argAnalysisFileFormat == null ? ArgSubOption.csv.ToString() : argAnalysisFileFormat;
            #endregion

            #region [Command-Line Argument Validation]

            // Help Message ..
            if (Convert.ToBoolean(commands.Parse(ArgOption.help.ToString())))
            {
                ConsistentExit();
            }

            // Processing mode ..
            if (argModeSync == null || argModeSync == "true")
            {
                Console.WriteLine();
                Console.WriteLine("A valid synchronisation mode must be selected!" + System.Environment.NewLine);
                Console.WriteLine(" e.g.");
                Console.WriteLine("     /{0} {1}", ArgOption.sync.ToString(), ArgSubOption.oneway_to_binds.ToString());
                Console.WriteLine("     /{0} {1}", ArgOption.sync.ToString(), ArgSubOption.twoway.ToString());
                Console.WriteLine();
                ConsistentExit();
            }

            // Determine file-type (user/sample) to be processed ..
            if (argSample == null)
            {
                if (File.Exists(argFilePathBinds))
                {
                    eliteDangerousBinds = argFilePathBinds;
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("Path to Elite Dangerous Binds (.binds) File must be valid!" + System.Environment.NewLine);
                    Console.WriteLine(" e.g. /{0} {1}", ArgOption.binds.ToString(), Path.Combine(DefaultEliteDangerousBindingsDirectory, "Custom.binds"));
                    Console.WriteLine();
                    ConsistentExit();
                }

                if (File.Exists(argFilePathVap))
                {
                    voiceAttackProfile = argFilePathVap;
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("Path to Voice Attack Profile (.vap) File must be valid!" + System.Environment.NewLine);
                    Console.WriteLine(" e.g. /{0} {1}", ArgOption.vap.ToString(), Path.Combine(DefaultVoiceAttackProfilesDirectory, "Custom.vap"));
                    Console.WriteLine();
                    ConsistentExit();
                }
            }
            else
            {
                Console.WriteLine("Using internal test data ..");

                // Point to project sample (not a resource as such) data ..
                eliteDangerousBinds = GetVisualStudioProjectBaseDirectory() + "\\Sample" + "\\ED01.binds";
                voiceAttackProfile = GetVisualStudioProjectBaseDirectory() + "\\Sample" + "\\VA03.vap";
            }
            
            // Final Check ..
            if (!(File.Exists(eliteDangerousBinds) && File.Exists(voiceAttackProfile)))
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
            KeyBindingAndCommandConnector keyLookup = null;
            try
            {
                keyLookup = new KeyBindingAndCommandConnector();
            }
            catch
            {
                Console.WriteLine("Action Exchange Dictionary is invalid");
                PressIt();
                Environment.Exit(0);
            }

            // Optional arg: Dictionary export
            if (HandleIO.ValidateFilepath(argFilePathDictionaryWrite) && HandleIO.CreateDirectory(argFilePathDictionaryWrite, true))
            {
                keyLookup.Export(argFilePathDictionaryWrite);
            }
            else
            {
                Console.WriteLine("unused option: /{0}", ArgOption.export.ToString());
            }

            // Optional arg: Dictionary import ..
            if (File.Exists(argFilePathDictionaryRead))
            {
                keyLookup.Import(argFilePathDictionaryRead);
            }
            else
            {
                Console.WriteLine("unused option: /{0}", ArgOption.import.ToString());
            }

            if (!argCreateReferenceTag)
            {
                Console.WriteLine("unused option: /{0}", ArgOption.tag.ToString());
            }
            #endregion

            #region [File Processing]
            try
            {
                #region [Read and update VoiceAttack Configuration File]
                // Update VoiceAttack Profile (optional) ..
                if ((argModeSync == ArgSubOption.twoway.ToString()) || (argModeSync == ArgSubOption.oneway_to_vap.ToString()))
                {
                    // Intro ..
                    Console.WriteLine(System.Environment.NewLine);
                    Console.WriteLine("Attempting VoiceAttack Profile update ..");

                    // Backup (optional) ..
                    SequentialFileBackup(argDirectoryPathBackup, voiceAttackProfile);

                    // Attempt synchronisation update ..
                    KeyBindingWriterVoiceAttack newVoiceAttack = new KeyBindingWriterVoiceAttack();
                    Console.WriteLine("Voice Attack Profile: {0}", newVoiceAttack.Update(KeyBindingAnalyser.VoiceAttack(eliteDangerousBinds, voiceAttackProfile, keyLookup), argCreateReferenceTag) == true ? "updated" : "no update possible or required");
                }
                else
                {
                    Console.WriteLine("VoiceAttack Profile update: not selected");
                }
                #endregion

                #region [Read and update EliteDangerous Configuration File]
                // Reverse-synchronise any vacant Elite Dangerous Bindings (optional) ..
                if ((argModeSync == ArgSubOption.twoway.ToString()) || (argModeSync == ArgSubOption.oneway_to_binds.ToString()))
                {
                    // Intro ..
                    Console.WriteLine(System.Environment.NewLine);
                    Console.WriteLine("Attempting Elite Dangerous Binds update ..");

                    // Backup (optional) ..
                    SequentialFileBackup(argDirectoryPathBackup, eliteDangerousBinds);

                    // Attempt synchronisation update ..
                    KeyBindingWriterEliteDangerous newEliteDangerous = new KeyBindingWriterEliteDangerous();
                    Console.WriteLine("Elite Dangerous Binds: {0}", newEliteDangerous.Update(KeyBindingAnalyser.EliteDangerous(eliteDangerousBinds, voiceAttackProfile, keyLookup), argCreateReferenceTag) == true ? "updated" : "no update possible or required");
                }
                else
                {
                    Console.WriteLine("Elite Dangerous Binds update: not selected");
                }
                #endregion

                #region [Analysis]
                // Re-read Voice Attack Commands and Elite Dangerous Binds for analysis information ..
                Console.WriteLine(System.Environment.NewLine);
                if (HandleIO.ValidateFilepath(argDirectoryPathAnalysis) && HandleIO.CreateDirectory(argDirectoryPathAnalysis, false))
                {
                    // Intro ..
                    Console.WriteLine("Creating Analysis File(s) in {0}", argDirectoryPathAnalysis);

                    // Construct File paths ..
                    string csvCommands = Path.Combine(argDirectoryPathAnalysis, Commands + "." + CSV);
                    string csvBindings = Path.Combine(argDirectoryPathAnalysis, Bindings + "." + CSV);
                    string csvConsolidatedBindings = Path.Combine(argDirectoryPathAnalysis, Consolidated + "." + CSV);
                    string csvAssociatedCommandStrings = Path.Combine(argDirectoryPathAnalysis, Associated + "." + CSV);
                    string csvAllCommandStrings = Path.Combine(argDirectoryPathAnalysis, AllCommands + "." + CSV);

                    string htmCommands = Path.Combine(argDirectoryPathAnalysis, Commands + "." + HTM);
                    string htmBindings = Path.Combine(argDirectoryPathAnalysis, Bindings + "." + HTM);
                    string htmConsolidatedBindings = Path.Combine(argDirectoryPathAnalysis, Consolidated + "." + HTM);
                    string htmAssociatedCommandStrings = Path.Combine(argDirectoryPathAnalysis, Associated + "." + HTM);
                    string htmAllCommandStrings = Path.Combine(argDirectoryPathAnalysis, AllCommands + "." + HTM);

                    // Read (updated) files ..
                    KeyBindingReaderEliteDangerous ed = new KeyBindingReaderEliteDangerous(eliteDangerousBinds);
                    KeyBindingReaderVoiceAttack va = new KeyBindingReaderVoiceAttack(voiceAttackProfile);

                    // Create table of all possible actions ..
                    DataTable elitedangerousAllCommands = ed.GetBindableCommands();
                    elitedangerousAllCommands.Merge(va.GetBindableCommands());

                    // Create table of all bound actions ..
                    DataTable elitedangerousBoundCommands = ed.GetBoundCommands();
                    elitedangerousBoundCommands.Merge(va.GetBoundCommands());
                    
                    // Create table of all consolidated actions ..
                    DataTable consolidatedBoundCommands = KeyBindingAnalyser.VoiceAttack(eliteDangerousBinds, voiceAttackProfile, keyLookup);
                    consolidatedBoundCommands = consolidatedBoundCommands.Sort(Items.Edvard.Column.EliteDangerousAction.ToString() + " asc");

                    // Create table of related Command Strings ..
                    DataTable associatedCommands = va.GetAssociatedCommandStrings(consolidatedBoundCommands);

                    // Create table of all Command Strings ..
                    DataTable allCommands = va.GetCommandStringsForAllCategories();

                    // Create appropriate type of analysis file ..
                    try
                    {
                        switch (HandleStrings.ParseStringToEnum<ArgSubOption>(argAnalysisFileFormat))
                        {
                            case ArgSubOption.csv:
                                elitedangerousAllCommands.CreateCSV(csvCommands);
                                elitedangerousBoundCommands.CreateCSV(csvBindings);
                                consolidatedBoundCommands.CreateCSV(csvConsolidatedBindings);
                                associatedCommands.CreateCSV(csvAssociatedCommandStrings);
                                allCommands.CreateCSV(csvAllCommandStrings);
                                break;

                            case ArgSubOption.htm:
                                elitedangerousAllCommands.CreateHTML(htmCommands, Commands);
                                elitedangerousBoundCommands.CreateHTML(htmBindings, Bindings);
                                consolidatedBoundCommands.CreateHTML(htmConsolidatedBindings, Consolidated);
                                associatedCommands.CreateHTML(htmAssociatedCommandStrings);
                                allCommands.CreateHTML(htmAllCommandStrings);
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
                    Console.WriteLine("unused option: /{0}", ArgOption.analysis.ToString());
                }
                #endregion

                PressIt();
            }
            catch
            {
                Console.WriteLine("Something went wrong ... we cry real tears ...");
                PressIt();
                throw;
            }
            #endregion
        }

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
                                 "  /" + ArgOption.binds.ToString() + System.Environment.NewLine +
                                 "           File path to Elite Dangerous .binds" + System.Environment.NewLine +
                                 "  /" + ArgOption.vap.ToString() + System.Environment.NewLine +
                                 "           File path to Voice Attack .vap" + System.Environment.NewLine +
                                 "  /" + ArgOption.sync.ToString() + System.Environment.NewLine +
                                 "           Synchronisation Mode" + System.Environment.NewLine +
                                 "            :" + ArgSubOption.twoway.ToString() + System.Environment.NewLine +
                                 "            :" + ArgSubOption.oneway_to_vap.ToString() + System.Environment.NewLine +
                                 "            :" + ArgSubOption.oneway_to_binds.ToString() + System.Environment.NewLine +
                                 "            :" + ArgSubOption.none.ToString() + System.Environment.NewLine +
                                 System.Environment.NewLine +
                                 "  [optional]" +
                                 System.Environment.NewLine +
                                 "  /" + ArgOption.help.ToString() + System.Environment.NewLine +
                                 "           This help" + System.Environment.NewLine +
                                 "  /" + ArgOption.backup.ToString() + System.Environment.NewLine +
                                 "           Directory path for backup file(s)" + System.Environment.NewLine +
                                 "  /" + ArgOption.analysis.ToString() + System.Environment.NewLine +
                                 "           Directory path for operational analysis file(s)" + System.Environment.NewLine +
                                 "  /" + ArgOption.format.ToString() + System.Environment.NewLine +
                                 "           File format for operational analysis file(s) (csv[default], htm)" + System.Environment.NewLine +
                                 "  /" + ArgOption.tag.ToString() + System.Environment.NewLine +
                                 "           Create reference tag in affected file(s)" + System.Environment.NewLine +
                                 "  /" + ArgOption.export.ToString() + System.Environment.NewLine +
                                 "           File path to export action dictionary" + System.Environment.NewLine +
                                 "  /" + ArgOption.import.ToString() + System.Environment.NewLine +
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

        /// <summary>
        /// File Backup
        /// </summary>
        /// <param name="backupDirectory"></param>
        /// <param name="filepath"></param>
        private static void SequentialFileBackup(string backupDirectory, string filepath)
        {
            // Validate ..
            if (HandleIO.ValidateFilepath(backupDirectory))
            {
                // Backup ..
                if (HandleIO.BackupFile(HandleIO.CopyFile(filepath, backupDirectory), BackupCycle, BackupFilenameLeftPadSize) == string.Empty)
                {
                    Console.WriteLine("Backup attempt: failed");
                }
            }
            else
            {
                Console.WriteLine("unused option: /{0}", ArgOption.backup.ToString());
            }
        }

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
        /// Crude way of getting current Visual Studio project base directory
        /// </summary>
        /// <returns></returns>
        private static string GetVisualStudioProjectBaseDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory.Replace("Debug", string.Empty).Replace("Release", string.Empty).Replace("bin", string.Empty).Replace("\\\\\\", string.Empty);
        }

        /// <summary>
        /// We laughed, we cried ..
        /// </summary>
        private static void PressIt()
        {
            Console.WriteLine("Press a key");
            Console.ReadKey();
        }
    }
}
