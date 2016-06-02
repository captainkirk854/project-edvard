namespace QuickRunner
{
    using Bindings;
    using Helpers;
    using System;
    using System.Data;

    public class Program
    {
        public static void Main(string[] args)
        {
            // Initialise ..
            // Point to project sample (not a resource as such) data ..
            string cfgED = GetProjectDirectory() + "\\Sample" + "\\ED01.binds";
            string cfgVA = GetProjectDirectory() + "\\Sample" + "\\VA02.vap";

            // Path for serialised DataTable output ..
            const string KeyActionsFile = "EDVA_Binding_Actions.csv";
            const string KeyBindingsFile = "EDVA_Combined_KeyBindings.csv";
            const string KeyConsolidatedFile = "EDVA_Consolidated_KeyBindings.csv";

            string csvOutputDirectory = Environment.ExpandEnvironmentVariables("%UserProfile%") + "\\Desktop";
            string csvKeyActions = csvOutputDirectory + "\\" + KeyActionsFile;
            string csvKeyBindings = csvOutputDirectory + "\\" + KeyBindingsFile;
            string csvKeyBindingsConsolidated = csvOutputDirectory + "\\" + KeyConsolidatedFile;

            // Read EliteDangerous and VoiceAttack configuration(s) to get key bindings ..
            try
            {
                Console.WriteLine("Reading Config(s)");

                // Create DataTable listing all possible actions ..
                DataTable keyActions = Reader.EliteDangerousBindings(cfgED);
                keyActions.Merge(Reader.VoiceAttackBindings(cfgVA));

                // Populate individual DataTables from both application bindings ..
                DataTable keyBindingsED = Reader.EliteDangerousKeyBindings(cfgED);
                DataTable keyBindingsVA = Reader.VoiceAttackKeyBindings(cfgVA);
                Console.WriteLine("Config(s) read");

                // Consolidate Voice Attack action bindings with Elite Dangerous bindings ..
                DataTable keyBindingsConsolidated = Writer.Consolidate(keyBindingsVA, keyBindingsED);
                Console.WriteLine("Config(s) consolidated");

                // Update VoiceAttack Profile ..
                Writer.UpdateVoiceAttackProfile(keyBindingsConsolidated);
                Console.WriteLine("VoiceAttack updated");

                // Create informational CSV file(s) ..
                DataTable keyBindingsCSV = keyBindingsED;
                keyBindingsCSV.Merge(keyBindingsVA);
                keyActions.CreateCSV(csvKeyActions);
                keyBindingsCSV.CreateCSV(csvKeyBindings);
                keyBindingsConsolidated.CreateCSV(csvKeyBindingsConsolidated);
                PressIt();
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
