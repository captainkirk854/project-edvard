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
        private const string Commands = "EDVArd_Commands.csv";
        private const string Bindings = "EDVArd_Command_Bindings.csv";
        private const string Consolidated = "EDVArd_Consolidated_Bindings.csv";
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
            backup,
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

            // mandatory ..
            string argFilePathBinds = commands.Parse(ArgOption.binds.ToString(), true);
            string argFilePathVap = commands.Parse(ArgOption.vap.ToString(), true);
            string argModeSync = commands.Parse(ArgOption.sync.ToString());

            // optional ..
            string argDirectoryPathBackup = commands.Parse(ArgOption.backup.ToString(), true);
            string argDirectoryPathAnalysis = commands.Parse(ArgOption.analysis.ToString(), true);
            bool argCreateReferenceTag = Convert.ToBoolean(commands.Parse(ArgOption.tag.ToString()));
            string argFilePathDictionaryWrite = commands.Parse(ArgOption.write.ToString(), true);
            string argFilePathDictionaryRead = commands.Parse(ArgOption.read.ToString(), true);
            string argSample = commands.Parse(ArgOption.sample.ToString());

            // specials ..
            if ((argDirectoryPathBackup != null) && (argDirectoryPathBackup.ToLower() == "desktop")) { argDirectoryPathBackup = userDesktop; }
            if ((argDirectoryPathAnalysis != null) && (argDirectoryPathAnalysis.ToLower() == "desktop")) { argDirectoryPathAnalysis = userDesktop; }
            if ((argFilePathDictionaryWrite != null) && (argFilePathDictionaryWrite.ToLower() == "desktop")) { argFilePathDictionaryWrite = userDesktop; }
            if ((argFilePathDictionaryRead != null) && (argFilePathDictionaryRead.ToLower() == "desktop")) { argFilePathDictionaryRead = userDesktop; } 

            // Determine file-type (user/sample) to be processed ..
            if (argSample == null)
            {
                if (File.Exists(argFilePathBinds))
                {
                    eliteDangerousBinds = argFilePathBinds;
                }
                else
                {
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
                Console.WriteLine("One or more required file(s) is missing!");
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
            GameActionExchanger actionExchange = new GameActionExchanger();

            // Optional arg: Dictionary export
            if (GenericIO.ValidateFilepath(argFilePathDictionaryWrite) && GenericIO.CreateDirectory(argFilePathDictionaryWrite, true))
            {
                actionExchange.Export(argFilePathDictionaryWrite);
            }
            else
            {
                Console.WriteLine("option /{0}: bypassed", ArgOption.write.ToString());
            }

            // Optional arg: Dictionary import ..
            if (File.Exists(argFilePathDictionaryRead))
            {
                actionExchange.Import(argFilePathDictionaryRead);
            }
            else
            {
                Console.WriteLine("option /{0}: bypassed", ArgOption.read.ToString());
            }

            if (!argCreateReferenceTag)
            {
                Console.WriteLine("option /{0}: bypassed", ArgOption.tag.ToString());
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
                        Console.WriteLine("option /{0}: bypassed", ArgOption.backup.ToString());
                    }

                    // Attempt update ..
                    KeyWriterVoiceAttack newVoiceAttack = new KeyWriterVoiceAttack();
                    Console.WriteLine("Voice Attack Profile: {0}", newVoiceAttack.Update(GameActionAnalyser.VoiceAttack(eliteDangerousBinds, voiceAttackProfile, actionExchange), argCreateReferenceTag) == true ? "updated" : "no update possible or required");
                }
                else
                {
                    Console.WriteLine("VoiceAttack Profile update: bypassed");
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
                        Console.WriteLine("option /{0}: bypassed", ArgOption.backup.ToString());
                    }

                    // Attempt update ..
                    KeyWriterEliteDangerous newEliteDangerous = new KeyWriterEliteDangerous();
                    Console.WriteLine("Elite Dangerous Binds: {0}", newEliteDangerous.Update(GameActionAnalyser.EliteDangerous(eliteDangerousBinds, voiceAttackProfile, actionExchange), argCreateReferenceTag) == true ? "updated" : "no update possible or required");
                }
                else
                {
                    Console.WriteLine("Elite Dangerous Binds update: bypassed");
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
                    string csvCommands = Path.Combine(argDirectoryPathAnalysis, Commands);
                    string csvBindings = Path.Combine(argDirectoryPathAnalysis, Bindings);
                    string csvConsolidatedBindings = Path.Combine(argDirectoryPathAnalysis, Consolidated);

                    KeyReaderEliteDangerous ed = new KeyReaderEliteDangerous(eliteDangerousBinds);
                    KeyReaderVoiceAttack va = new KeyReaderVoiceAttack(voiceAttackProfile);

                    // Create CSV listing all possible actions ..
                    DataTable elitedangerousCommands = ed.GetBindableCommands();
                    elitedangerousCommands.Merge(va.GetBindableCommands());
                    elitedangerousCommands.CreateCSV(csvCommands);

                    // Create CSV listing all bound actions ..
                    elitedangerousCommands = ed.GetBoundCommands();
                    elitedangerousCommands.Merge(va.GetBoundCommands());
                    elitedangerousCommands.CreateCSV(csvBindings);

                    // Create CSV listing all consolidated actions ..
                    DataTable consolidatedBindings = GameActionAnalyser.VoiceAttack(eliteDangerousBinds, voiceAttackProfile, actionExchange);
                    consolidatedBindings = consolidatedBindings.Sort(Helper.Enums.Column.EliteDangerousAction.ToString() + " asc");
                    consolidatedBindings.CreateCSV(csvConsolidatedBindings);
                }
                else
                {
                    Console.WriteLine("option /{0}: bypassed", ArgOption.analysis.ToString());
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
            string usageInformation =
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
                                 "  /" + ArgOption.backup.ToString() + System.Environment.NewLine +
                                 "           Directory path for backup file(s)" + System.Environment.NewLine +
                                 "  /" + ArgOption.analysis.ToString() + System.Environment.NewLine +
                                 "           Directory path for operational analysis file(s)" + System.Environment.NewLine +
                                 "  /" + ArgOption.tag.ToString() + System.Environment.NewLine +
                                 "           Create reference tag in affected file(s)" + System.Environment.NewLine +
                                 "  /" + ArgOption.write.ToString() + System.Environment.NewLine +
                                 "           File path to export action dictionary" + System.Environment.NewLine +
                                 "  /" + ArgOption.read.ToString() + System.Environment.NewLine +
                                 "           File path to import action dictionary" + System.Environment.NewLine;

            Console.WriteLine(usageInformation);
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
