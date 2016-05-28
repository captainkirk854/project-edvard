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
            DataTable keyBindingsTable = new DataTable();

           // const int KeyBindingsColumnWidth = 20;
            const string CSVFileName = "EDVA_Consolidated_KeyBindings.csv";

            // Point to project sample (not a resource as such) data ..
            string cfgED = GetProjectDirectory() + "\\Sample" + "\\ED01.binds";
            string cfgVA = GetProjectDirectory() + "\\Sample" + "\\VA01.vap";

            // Path for DataTable Debug Csv ..
            string dataTableDebugCSV = Environment.ExpandEnvironmentVariables("%UserProfile%");
            dataTableDebugCSV += "\\Desktop";
            dataTableDebugCSV += "\\" + CSVFileName;

            // Read EliteDangerous and Voice Attack configuration(s) to get key bindings ..
            try
            {
                // Combine DataTables from both applications ..
                keyBindingsTable = Game.KeyBindingsConfigReader.EliteDangerous(cfgED);
                keyBindingsTable.Merge(Game.KeyBindingsConfigReader.VoiceAttack(cfgVA));
                Console.WriteLine("Config(s) Read");

                // PressIt();

                // Debug DataTable Contents ..
                // KeyBindingsTable.Display(KeyBindingsColumnWidth, string.Empty);
                // PressIt();

                // Experimental: update ..
                string junkSetClause = "Context = EliteDangerous ,KeyValue = A";
                string junkWhereClause = "Context = EliteDangerous, KeyValue = A, KeyCode = 65";
                keyBindingsTable.Update(junkSetClause, junkWhereClause);
                
                // Experimental: sort ..
                keyBindingsTable = keyBindingsTable.Sort("KeyCode desc, ModifierKeyCode asc");
                keyBindingsTable.CreateCSV(dataTableDebugCSV);
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
