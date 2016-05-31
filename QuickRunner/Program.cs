namespace QuickRunner
{
    using Application;
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
            string outputDirectory = Environment.ExpandEnvironmentVariables("%UserProfile%") + "\\Desktop";
            string fileKeyActionsCSV = outputDirectory + "\\" + KeyActionsFile;
            string fileKeyBindingsCSV = outputDirectory + "\\" + KeyBindingsFile;
            string fileKeyConsolidatedBindingsCSV = outputDirectory + "\\" + KeyConsolidatedFile;

            // DataTable receptacles ..
            DataTable keyActions = new DataTable();
            DataTable keyBindingsED = new DataTable();
            DataTable keyBindingsVA = new DataTable();
            DataTable keyBindingsConsolidated = new DataTable();
            DataTable keyBindingsCSV = new DataTable();

            // Read EliteDangerous and Voice Attack configuration(s) to get key bindings ..
            try
            {
                keyActions = BindingsReader.EliteDangerousBindings(cfgED);
                keyActions.Merge(BindingsReader.VoiceAttackBindings(cfgVA));

                // Populate individual DataTables from both application bindings ..
                keyBindingsED = BindingsReader.EliteDangerousKeyBindings(cfgED);
                keyBindingsVA = BindingsReader.VoiceAttackKeyBindings(cfgVA);
                Console.WriteLine("Config(s) Read");

                // Consolidate Voice Attack action bindings with Elite Dangerous bindings ..
                keyBindingsConsolidated = ActionBinding.Consolidate(keyBindingsVA, keyBindingsED);
                keyBindingsConsolidated.AddDefaultColumn(Enums.Column.VoiceAttackProfile.ToString(), cfgVA);
                keyBindingsConsolidated.AddDefaultColumn(Enums.Column.EliteDangerousBinds.ToString(), cfgED);
                keyBindingsConsolidated = keyBindingsConsolidated.Sort(Enums.Column.EliteDangerousAction.ToString() + " asc");
                Console.WriteLine("Config(s) Consolidated");

                // Combine DataTables  ..
                keyBindingsCSV = keyBindingsED;
                keyBindingsCSV.Merge(keyBindingsVA);

                // Create CSV file(s) ..
                keyActions.CreateCSV(fileKeyActionsCSV);
                keyBindingsCSV.CreateCSV(fileKeyBindingsCSV);
                keyBindingsConsolidated.CreateCSV(fileKeyConsolidatedBindingsCSV);
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
