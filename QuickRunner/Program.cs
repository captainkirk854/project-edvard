namespace QuickRunner
{
    using Binding;
    using Helper;
    using System;
    using System.Data;

    public class Program
    {
        public static void Main(string[] args)
        {
            //////////////////////////////////////////////////////////////////
            // Initialise ..
            //////////////////////////////////////////////////////////////////

            // Point to project sample (not a resource as such) data ..
            string eliteDangerousBinds = GetProjectDirectory() + "\\Sample" + "\\ED01.binds";
            string voiceAttackProfile = GetProjectDirectory() + "\\Sample" + "\\VA02.vap";

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
                KeyReader.KeyType = Enums.InputKeyEnumType.WindowsForms; // [optional] sets key type enumeration to use
                KeyReaderEliteDangerous ed = new KeyReaderEliteDangerous(eliteDangerousBinds);
                KeyReaderVoiceAttack va = new KeyReaderVoiceAttack(voiceAttackProfile);
                Console.WriteLine("Configs read ..");

                // Update VoiceAttack Profile ..
                KeyWriterVoiceAttack newVoiceAttack = new KeyWriterVoiceAttack();
                Console.WriteLine("VoiceAttack Profile: {0}", newVoiceAttack.Update(GameActionAnalyser.VoiceAttack(va.GetBoundCommands(), ed.GetBoundCommands())) == true ? "updated" : "no update possible or required");

                PressIt();

                //////////////////////////////////////////////////////////////////
                //////// O P T I O N A L /////////////////////////////////////////
                //////////////////////////////////////////////////////////////////

                // Create CSV listing all possible actions ..
                var elitedangerousCommands = ed.GetBindableCommands();
                elitedangerousCommands.Merge(va.GetBindableCommands());
                elitedangerousCommands.CreateCSV(csvCommands);

                // Create CSV listing all bound actions ..
                elitedangerousCommands = ed.GetBoundCommands();
                elitedangerousCommands.Merge(va.GetBoundCommands());
                elitedangerousCommands.CreateCSV(csvBindings);

                // Create CSV listing all consolidated actions ..
                DataTable consolidatedBindings = GameActionAnalyser.VoiceAttack(va.GetBoundCommands(), ed.GetBoundCommands());
                consolidatedBindings = consolidatedBindings.Sort(Enums.Column.EliteDangerousAction.ToString() + " asc");
                consolidatedBindings.CreateCSV(csvConsolidatedBindings);

                //////////////////////////////////////////////////////////////////
                //////// O P T I O N A L /////////////////////////////////////////
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
