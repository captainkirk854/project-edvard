namespace QuickRunner
{
    using Binding;
    using Helper;
    using KeyHelper;
    using System;
    using System.Data;

    public class Program
    {
        public static void Main(string[] args)
        {
            //////////////////////////////////////////////////////////////////
            // Initialise ..
            //////////////////////////////////////////////////////////////////
            string eliteDangerousBinds = string.Empty;
            string voiceAttackProfile = string.Empty;

            // Get user-input ..
            string defaultEDBindingsDirectory = Environment.ExpandEnvironmentVariables("%LOCALAPPDATA%") + "\\Frontier Developments\\Elite Dangerous\\Options\\Bindings";
            string defaultVAProfilesDirectory = Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%") + "\\VoiceAttack\\Sounds\\hcspack\\Profiles";

            Console.WriteLine("Fullpath of Elite Dangerous Binds (.binds) File ..");
            Console.WriteLine(" e.g. {0}", defaultEDBindingsDirectory);
            Console.WriteLine("> ..", defaultEDBindingsDirectory);
            eliteDangerousBinds = Console.ReadLine();
            
            Console.WriteLine("Fullpath of Voice Attack Profile (.vap) File ..");
            Console.WriteLine(" e.g. {0}", defaultVAProfilesDirectory);
            Console.WriteLine("> ..", defaultEDBindingsDirectory);
            voiceAttackProfile = Console.ReadLine();

            // Default if user-input is lazy ..
            if ((eliteDangerousBinds.Length == 0) || (voiceAttackProfile.Length == 0))
            {
                Console.WriteLine("Using sample data ..");

                // Point to project sample (not a resource as such) data ..
                eliteDangerousBinds = GetProjectDirectory() + "\\Sample" + "\\ED01.binds";
                voiceAttackProfile = GetProjectDirectory() + "\\Sample" + "\\VA02.vap";
            }
            else
            {
                // Perform backup ..
                Console.WriteLine("Backing up ..");
                eliteDangerousBinds.BackupFile(5, 3);
                voiceAttackProfile.BackupFile(5, 3);
            }

            // Path for serialised DataTable output ..
            const string Commands = "EDVA_Commands.csv";
            const string Bindings = "EDVA_Command_Bindings.csv";
            const string Consolidated = "EDVA_Consolidated_Bindings.csv";

            string csvOutputDirectory = Environment.ExpandEnvironmentVariables("%UserProfile%") + "\\Desktop";
            string csvCommands = csvOutputDirectory + "\\" + Commands;
            string csvBindings = csvOutputDirectory + "\\" + Bindings;
            string csvConsolidatedBindings = csvOutputDirectory + "\\" + Consolidated;

            //////////////////////////////////////////////////////////////////
            // Initialise ..
            //////////////////////////////////////////////////////////////////

            // Read EliteDangerous and VoiceAttack configuration(s) to get key bindings ..
            try
            {
                // Read Voice Attack Commands and Elite Dangerous Binds ..
                KeyReader.KeyType = KeyHelper.Enums.InputKeyEnumType.WindowsForms; // [optional] sets key type enumeration to use
                Console.WriteLine("Attempting update(s) ..");

                // Update VoiceAttack Profile (optional) ..
                KeyWriterVoiceAttack newVoiceAttack = new KeyWriterVoiceAttack();
                Console.WriteLine("Voice Attack Profile: {0}", newVoiceAttack.Update(GameActionAnalyser.VoiceAttack(eliteDangerousBinds, voiceAttackProfile), false) == true ? "updated" : "no update possible or required");

                // Reverse-synchronise any vacant Elite Dangerous Bindings (optional) ..
                KeyWriterEliteDangerous newEliteDangerous = new KeyWriterEliteDangerous();
                Console.WriteLine("Elite Dangerous Binds: {0}", newEliteDangerous.Update(GameActionAnalyser.EliteDangerous(eliteDangerousBinds, voiceAttackProfile), false) == true ? "updated" : "no update possible or required");

                PressIt();

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
                DataTable consolidatedBindings = GameActionAnalyser.VoiceAttack(eliteDangerousBinds, voiceAttackProfile);
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
