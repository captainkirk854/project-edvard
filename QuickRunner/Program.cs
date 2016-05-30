namespace QuickRunner
{
    using System;
    using Helpers;
    using System.Data;

    public class Program
    {
        public static void Main(string[] args)
        {
            // Initialise ..
            // Point to project sample (not a resource as such) data ..
            string cfgED = GetProjectDirectory() + "\\Sample" + "\\ED02.binds";
            string cfgVA = GetProjectDirectory() + "\\Sample" + "\\VA01.vap";

            // Path for serialised DataTable output ..
            const string KeyActionsFile = "EDVA_Binding_Actions.csv";
            const string KeyBindingsFile = "EDVA_Consolidated_KeyBindings.csv";
            string outputDirectory = Environment.ExpandEnvironmentVariables("%UserProfile%") + "\\Desktop";
            string keyActionsCSV = outputDirectory + "\\" + KeyActionsFile;
            string keyBindingsCSV = outputDirectory + "\\" + KeyBindingsFile;

            // DataTable receptacles ..
            DataTable keyActions = new DataTable();
            DataTable keyBindings = new DataTable();

            // Read EliteDangerous and Voice Attack configuration(s) to get key bindings ..
            try
            {
                keyActions = Game.BindingsReader.EliteDangerousBindings(cfgED);
                keyActions.Merge(Game.BindingsReader.VoiceAttackBindings(cfgVA));

                // Combine DataTables from both applications ..
                keyBindings = Game.BindingsReader.EliteDangerousKeyBindings(cfgED);
                keyBindings.Merge(Game.BindingsReader.VoiceAttackKeyBindings(cfgVA));
                Console.WriteLine("Config(s) Read");

                // Experimental: update ..
                string junkSetClause = "Context = EliteDangerous ,KeyValue = A";
                string junkWhereClause = "Context = EliteDangerous, KeyValue = A, KeyCode = 65";
                keyBindings.Update(junkSetClause, junkWhereClause);
                
                // Experimental: sort ..
                keyBindings = keyBindings.Sort(Enums.Column.Context.ToString() + " asc," + Enums.Column.KeyEnumerationCode.ToString() + " desc," + Enums.Column.ModifierKeyEnumerationCode.ToString() + " asc");
                
                // Write file(s) ..
                keyActions.CreateCSV(keyActionsCSV);
                keyBindings.CreateCSV(keyBindingsCSV);
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
