namespace QuickRunner
{
    using Binding;
    using Helper;
    using KeyHelper;
    using System;
    using System.Data;
    using System.IO;

    public class Program
    {
        public static void Main(string[] args)
        {
            //////////////////////////////////////////////////////////////////
            // I N I T I A L I S E ..
            //////////////////////////////////////////////////////////////////

            // Binds and Profile ..
            string defaultEDBindingsDirectory = Environment.ExpandEnvironmentVariables("%LOCALAPPDATA%") + "\\Frontier Developments\\Elite Dangerous\\Options\\Bindings";
            string defaultVAProfilesDirectory = Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%") + "\\VoiceAttack\\Sounds\\hcspack\\Profiles";
            string eliteDangerousBinds = string.Empty;
            string voiceAttackProfile = string.Empty;

            // Path(s) for various serialised DataTable output ..
            const string Commands = "EDVA_Commands.csv";
            const string Bindings = "EDVA_Command_Bindings.csv";
            const string Consolidated = "EDVA_Consolidated_Bindings.csv";
            string csvOutputDirectory = Environment.ExpandEnvironmentVariables("%UserProfile%") + "\\Desktop";
            string csvCommands = csvOutputDirectory + "\\" + Commands;
            string csvBindings = csvOutputDirectory + "\\" + Bindings;
            string csvConsolidatedBindings = csvOutputDirectory + "\\" + Consolidated;

            bool tagInternalReferenceOnUpdate = false;

            // Command Line Argument(s) ..
            CommandLineParser commandLine = new CommandLineParser(args);

            // Parse Command Line arguments ..
            // mandatory ..
            string argBinds = commandLine["binds"];
            string argVap = commandLine["vap"];

            // optional ..
            string argSample = commandLine["sample"];
            string argexport = commandLine["export"];
            string argimport = commandLine["import"];
            string argTag = commandLine["tag"];

            string sampleUsage = string.Format(" -binds {0} -vap {1} -export C:\\TEMP\\EDVARD_ACTIONS_DICTIONARY.xml -import C:\\TEMP\\EDVARD_ACTIONS_DICTIONARY.xml -tag",
                                               defaultEDBindingsDirectory + "\\Custom.binds",
                                               defaultVAProfilesDirectory + "\\Custom.vap");

            // Determine which type (user/sample) of file(s) are to be processed ..
            if (argSample == null)
            {
                if (File.Exists(argBinds))
                {
                    eliteDangerousBinds = argBinds;
                }
                else
                {
                    Console.WriteLine("Path to Elite Dangerous Binds (.binds) File must be valid!");
                    Console.WriteLine(" e.g. -binds {0}", defaultEDBindingsDirectory + "\\Custom.binds");
                    Console.WriteLine();
                    Console.WriteLine(sampleUsage);
                    PressIt();
                    Environment.Exit(0);
                }

                if (File.Exists(argVap))
                {
                    voiceAttackProfile = argVap;
                }
                else
                {
                    Console.WriteLine("Path to Voice Attack Profile (.vap) File must be valid!");
                    Console.WriteLine(" e.g. -vap {0}", defaultVAProfilesDirectory + "\\Custom.vap");
                    Console.WriteLine();
                    Console.WriteLine(sampleUsage);
                    PressIt();
                    Environment.Exit(0);
                }                 

                // Perform backup ..
                Console.WriteLine("Backing up ..");
                eliteDangerousBinds.BackupFile(5, 3);
                voiceAttackProfile.BackupFile(5, 3);
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
                Console.WriteLine("No file(s) to process !!");
                PressIt();
                Environment.Exit(0);
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
                // Initialise key enum type to use ..
                KeyReader.KeyType = KeyHelper.Enums.InputKeyEnumType.WindowsForms; // [optional] sets key type enumeration to use
                
                // Initialise lookup dictionary for inter-game action references ..
                GameActionExchanger actionExchange = new GameActionExchanger();

                // Optional command line argument - export ..  
                if (Stockpile.ValidateFilepath(argexport))
                {
                    actionExchange.Export(argexport);
                }

                // Optional command line argument - import ..
                if (File.Exists(argimport))
                {
                    actionExchange.Import(argimport);
                }

                // Attempt update(s) ..
                Console.WriteLine("Attempting update(s) ..");

                // Tag file(s) internally if updated ..
                if (argTag == "true")
                {
                    tagInternalReferenceOnUpdate = true;
                }

                // Update VoiceAttack Profile (optional) ..
                KeyWriterVoiceAttack newVoiceAttack = new KeyWriterVoiceAttack();
                Console.WriteLine("Voice Attack Profile: {0}", newVoiceAttack.Update(GameActionAnalyser.VoiceAttack(eliteDangerousBinds, voiceAttackProfile, actionExchange), tagInternalReferenceOnUpdate) == true ? "updated" : "no update possible or required");

                // Reverse-synchronise any vacant Elite Dangerous Bindings (optional) ..
                KeyWriterEliteDangerous newEliteDangerous = new KeyWriterEliteDangerous();
                Console.WriteLine("Elite Dangerous Binds: {0}", newEliteDangerous.Update(GameActionAnalyser.EliteDangerous(eliteDangerousBinds, voiceAttackProfile, actionExchange), tagInternalReferenceOnUpdate) == true ? "updated" : "no update possible or required");

                PressIt();
                //////////////////////////////////////////////////////////////////
                // U P D A T E ..
                //////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////
                //////// O P T I O N A L /////FINAL PROFILE AND BINDS STATUS//////
                //////////////////////////////////////////////////////////////////

                // Re-read Voice Attack Commands and Elite Dangerous Binds ..
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
