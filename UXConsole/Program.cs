namespace UXConsole
{
    using Binding;
    using Helper;
    using KeyHelper;
    using System;
    using System.Data;
    using System.IO;

    public class Program
    {
        private const string VersionNumber = "1.000";
        private const string Commands = "edvCommands";
        private const string Bindings = "edvCommand_Bindings";
        private const string Consolidated = "edvConsolidated_Bindings";
        private const string CSV = "csv";
        private const string HTM = "html";
        private const int NumberOfBackupsToKeep = 50;

        /// <summary>
        /// Enumeration of Arguments
        /// </summary>
        private enum ArgOption
        {
            binds,
            vap,
            sync,
            read,
            write,
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
            //////////////////////////////////////////////////////////////////
            // C O M M A N D  L I N E ..
            //////////////////////////////////////////////////////////////////

            // Binds and Profile ..
            string defaultEDBindingsDirectory = Environment.ExpandEnvironmentVariables("%LOCALAPPDATA%") + "\\Frontier Developments\\Elite Dangerous\\Options\\Bindings";
            string defaultVAProfilesDirectory = Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%") + "\\VoiceAttack\\Sounds\\hcspack\\Profiles";
            string userDesktop = Environment.ExpandEnvironmentVariables("%UserProfile%") + "\\Desktop";

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
            string argFilePathDictionaryWrite = commands.Parse(ArgOption.write.ToString(), true);
            string argFilePathDictionaryRead = commands.Parse(ArgOption.read.ToString(), true);
            string argSample = commands.Parse(ArgOption.sample.ToString());

            // Help Message ..
            if (Convert.ToBoolean(commands.Parse(ArgOption.help.ToString())))
            {
                ShowUsage();
                PressIt();
                Environment.Exit(0);
            }

            // Specials 
            if ((argDirectoryPathBackup != null) && (argDirectoryPathBackup.ToLower() == "desktop")) { argDirectoryPathBackup = userDesktop; }
            if ((argDirectoryPathAnalysis != null) && (argDirectoryPathAnalysis.ToLower() == "desktop")) { argDirectoryPathAnalysis = userDesktop; }
            if ((argFilePathDictionaryWrite != null) && (argFilePathDictionaryWrite.ToLower() == "desktop")) { argFilePathDictionaryWrite = userDesktop; }
            if ((argFilePathDictionaryRead != null) && (argFilePathDictionaryRead.ToLower() == "desktop")) { argFilePathDictionaryRead = userDesktop; }
            argAnalysisFileFormat = argAnalysisFileFormat == null ? ArgSubOption.csv.ToString() : argAnalysisFileFormat;

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
                    Console.WriteLine(" e.g. /binds {0}", Path.Combine(defaultEDBindingsDirectory, "Custom.binds"));
                    Console.WriteLine();
                    ShowUsage();
                    PressIt();
                    Environment.Exit(0);
                }

                if (File.Exists(argFilePathVap))
                {
                    voiceAttackProfile = argFilePathVap;
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("Path to Voice Attack Profile (.vap) File must be valid!" + System.Environment.NewLine);
                    Console.WriteLine(" e.g. /vap {0}", Path.Combine(defaultVAProfilesDirectory, "Custom.vap"));
                    Console.WriteLine();
                    ShowUsage();
                    PressIt();
                    Environment.Exit(0);
                }
            }
            else
            {
                Console.WriteLine("Using sample data ..");

                // Point to project sample (not a resource as such) data ..
                eliteDangerousBinds = GetProjectDirectory() + "\\Sample" + "\\ED01.binds";
                voiceAttackProfile = GetProjectDirectory() + "\\Sample" + "\\VA02.vap";
            }
            
            // Final Check ..
            if (!(File.Exists(eliteDangerousBinds) && File.Exists(voiceAttackProfile)))
            {
                Console.WriteLine("Required file(s) are missing!");
                PressIt();
                Environment.Exit(0);
            }

            //////////////////////////////////////////////////////////////////
            // C O M M A N D  L I N E ..
            //////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////
            // I N I T I A L I S E ..
            //////////////////////////////////////////////////////////////////

            // Initialise key enum type to use ..
            KeyReader.KeyType = KeyHelper.Enums.InputKeyEnumType.WindowsForms; // [optional] sets key type enumeration to use

            // Initialise lookup dictionary for inter-game action references ..
            GameActionExchanger actionExchange = null;
            try
            {
                actionExchange = new GameActionExchanger();
            }
            catch
            {
                Console.WriteLine("Action Exchange Dictionary is invalid");
                PressIt();
                Environment.Exit(0);
            }

            // Optional arg: Dictionary export
            if (GenericIO.ValidateFilepath(argFilePathDictionaryWrite) && GenericIO.CreateDirectory(argFilePathDictionaryWrite, true))
            {
                actionExchange.Export(argFilePathDictionaryWrite);
            }
            else
            {
                Console.WriteLine("unused option: /{0}", ArgOption.write.ToString());
            }

            // Optional arg: Dictionary import ..
            if (File.Exists(argFilePathDictionaryRead))
            {
                actionExchange.Import(argFilePathDictionaryRead);
            }
            else
            {
                Console.WriteLine("unused option: /{0}", ArgOption.read.ToString());
            }

            if (!argCreateReferenceTag)
            {
                Console.WriteLine("unused option: /{0}", ArgOption.tag.ToString());
            }

            //////////////////////////////////////////////////////////////////
            // I N I T I A L I S E ..
            //////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////
            // U P D A T E ..
            //////////////////////////////////////////////////////////////////
            // Read and update EliteDangerous and VoiceAttack configuration(s) ..
            try
            {
                // Update VoiceAttack Profile (optional) ..
                if ((argModeSync == ArgSubOption.twoway.ToString()) || (argModeSync == ArgSubOption.oneway_to_vap.ToString()))
                {
                    // Intro ..
                    Console.WriteLine(System.Environment.NewLine);
                    Console.WriteLine("Attempting VoiceAttack Profile update ..");

                    // Backup ..
                    if (GenericIO.ValidateFilepath(argDirectoryPathBackup))
                    {
                        if (GenericIO.BackupFile(GenericIO.CopyFile(voiceAttackProfile, argDirectoryPathBackup), NumberOfBackupsToKeep, 3) == string.Empty)
                        {
                            Console.WriteLine("Backup attempt: failed");
                        }
                    }
                    else
                    {
                        Console.WriteLine("unused option: /{0}", ArgOption.backup.ToString());
                    }

                    // Attempt update ..
                    KeyWriterVoiceAttack newVoiceAttack = new KeyWriterVoiceAttack();
                    Console.WriteLine("Voice Attack Profile: {0}", newVoiceAttack.Update(GameActionAnalyser.VoiceAttack(eliteDangerousBinds, voiceAttackProfile, actionExchange), argCreateReferenceTag) == true ? "updated" : "no update possible or required");
                }
                else
                {
                    Console.WriteLine("VoiceAttack Profile update: not selected");
                }

                // Reverse-synchronise any vacant Elite Dangerous Bindings (optional) ..
                if ((argModeSync == ArgSubOption.twoway.ToString()) || (argModeSync == ArgSubOption.oneway_to_binds.ToString()))
                {
                    // Intro ..
                    Console.WriteLine(System.Environment.NewLine);
                    Console.WriteLine("Attempting Elite Dangerous Binds update ..");

                    // Backup ..
                    if (GenericIO.ValidateFilepath(argDirectoryPathBackup))
                    {
                        if (GenericIO.BackupFile(GenericIO.CopyFile(eliteDangerousBinds, argDirectoryPathBackup), NumberOfBackupsToKeep, 3) == string.Empty)
                        {
                            Console.WriteLine("Backup attempt: failed");
                        }
                    }
                    else
                    {
                        Console.WriteLine("unused option: /{0}", ArgOption.backup.ToString());
                    }

                    // Attempt update ..
                    KeyWriterEliteDangerous newEliteDangerous = new KeyWriterEliteDangerous();
                    Console.WriteLine("Elite Dangerous Binds: {0}", newEliteDangerous.Update(GameActionAnalyser.EliteDangerous(eliteDangerousBinds, voiceAttackProfile, actionExchange), argCreateReferenceTag) == true ? "updated" : "no update possible or required");
                }
                else
                {
                    Console.WriteLine("Elite Dangerous Binds update: not selected");
                }

                //////////////////////////////////////////////////////////////////
                // U P D A T E ..
                //////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////
                //////// O P T I O N A L /////FINAL PROFILE AND BINDS STATUS//////
                //////////////////////////////////////////////////////////////////

                // Re-read Voice Attack Commands and Elite Dangerous Binds for analysis information ..
                Console.WriteLine(System.Environment.NewLine);
                if (GenericIO.ValidateFilepath(argDirectoryPathAnalysis) && GenericIO.CreateDirectory(argDirectoryPathAnalysis, false))
                {
                    // Intro ..
                    Console.WriteLine("Creating Analysis File(s) in {0}", argDirectoryPathAnalysis);

                    string csvCommands = Path.Combine(argDirectoryPathAnalysis, Commands + "." + CSV);
                    string csvBindings = Path.Combine(argDirectoryPathAnalysis, Bindings + "." + CSV);
                    string csvConsolidatedBindings = Path.Combine(argDirectoryPathAnalysis, Consolidated + "." + CSV);

                    string htmCommands = Path.Combine(argDirectoryPathAnalysis, Commands + "." + HTM);
                    string htmBindings = Path.Combine(argDirectoryPathAnalysis, Bindings + "." + HTM);
                    string htmConsolidatedBindings = Path.Combine(argDirectoryPathAnalysis, Consolidated + "." + HTM);

                    KeyReaderEliteDangerous ed = new KeyReaderEliteDangerous(eliteDangerousBinds);
                    KeyReaderVoiceAttack va = new KeyReaderVoiceAttack(voiceAttackProfile);

                    // Create table of all possible actions ..
                    DataTable elitedangerousCommands = ed.GetBindableCommands();
                    elitedangerousCommands.Merge(va.GetBindableCommands());

                    // Create table of all bound actions ..
                    elitedangerousCommands = ed.GetBoundCommands();
                    elitedangerousCommands.Merge(va.GetBoundCommands());
                    
                    // Create table of all consolidated actions ..
                    DataTable consolidatedBindings = GameActionAnalyser.VoiceAttack(eliteDangerousBinds, voiceAttackProfile, actionExchange);
                    consolidatedBindings = consolidatedBindings.Sort(Helper.Enums.Column.EliteDangerousAction.ToString() + " asc");

                    // Create appropriate type of analysis file ..
                    try
                    {
                        switch (StockPile.ParseStringToEnum<ArgSubOption>(argAnalysisFileFormat))
                        {
                            case ArgSubOption.csv:
                                elitedangerousCommands.CreateCSV(csvCommands);
                                elitedangerousCommands.CreateCSV(csvBindings);
                                consolidatedBindings.CreateCSV(csvConsolidatedBindings);
                                break;

                            case ArgSubOption.htm:
                                elitedangerousCommands.CreateHTML(htmCommands, Commands);
                                elitedangerousCommands.CreateHTML(htmBindings, Bindings);
                                consolidatedBindings.CreateHTML(htmConsolidatedBindings, Consolidated);
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

                PressIt();
                //////////////////////////////////////////////////////////////////
                //////// O P T I O N A L /////FINAL PROFILE AND BINDS STATUS//////
                //////////////////////////////////////////////////////////////////
            }
            catch
            {
                Console.WriteLine("Something went wrong ... we cry real tears ...");
                PressIt();
                throw;
            }
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
                                 "  /" + ArgOption.write.ToString() + System.Environment.NewLine +
                                 "           File path to export action dictionary" + System.Environment.NewLine +
                                 "  /" + ArgOption.read.ToString() + System.Environment.NewLine +
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
                                 "/binds \"C:\\Elite Dangerous\\My.binds\" /vap C:\\HCSVoicePack\\My.vap /sync:none /write \"C:\\My Actions\\Action001.xml\"" +
                                 System.Environment.NewLine +
                                 "           No update of either file type, but will export Action Dictionary as .xml file" +
                                 System.Environment.NewLine +
                                 System.Environment.NewLine +
                                 "/binds \"C:\\Elite Dangerous\\My.binds\" /vap \"C:\\HCSVoicePack\\My.vap\" /sync:twoway /read \"C:\\My Actions\\Action001_modified.xml\" /tag" +
                                 System.Environment.NewLine +
                                 "           Attempts bidirectional synchronisation using a modified Action Dictionary to override internal and will tag affected file(s)";

            string disclaimer =
                                 System.Environment.NewLine +
                                 System.Environment.NewLine +
                                 "Legalese" +
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

            Console.WriteLine(System.Environment.NewLine);
            Console.WriteLine(helpInformation);
            Console.WriteLine(System.Environment.NewLine);
            Console.WriteLine(usageExamples);
            Console.WriteLine(System.Environment.NewLine);
            Console.WriteLine(disclaimer);
            Console.WriteLine(System.Environment.NewLine);
        }

        /// <summary>
        /// Crude way of getting current Visual Studio project directory
        /// </summary>
        /// <returns></returns>
        private static string GetProjectDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory.Replace("Debug", string.Empty).Replace("bin", string.Empty).Replace("\\\\\\", string.Empty);
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
