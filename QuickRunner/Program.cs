namespace QuickRunner
{
    using System;
    using Helpers;
    using System.Data;

    class Program
    {
        static void Main(string[] args)
        {
            // Initialise ..
            DataTable KeyBindingsTable = new DataTable();

            const int KeyBindingsColumnWidth = 20;
            const string CSVFileName = "EDVA_Consolidated_KeyBindings.csv";

            // Point to project sample (not a resource as such) data ..
            string EDCfg = GetProjectDirectory() + "\\Sample" + "\\ED01.binds";
            string VACfg = GetProjectDirectory() + "\\Sample" + "\\VA01.vap";

            // Path for DataTable Debug Csv ..
            string DataTableDebugCSV = Environment.ExpandEnvironmentVariables("%UserProfile%");
            DataTableDebugCSV += "\\Desktop";
            DataTableDebugCSV += "\\" + CSVFileName;

            // Read EliteDangerous and Voice Attack configuration(s) to get key bindings ..
            try
            {
                // Combine DataTables from both applications ..
                KeyBindingsTable = Game.KeyBindingsConfigReader.EliteDangerous(EDCfg);
                KeyBindingsTable.Merge(Game.KeyBindingsConfigReader.VoiceAttack(VACfg));
                Console.WriteLine("Config(s) Read");
                PressIt();

                // Debug DataTable Contents ..
                KeyBindingsTable.Display(KeyBindingsColumnWidth, string.Empty);
                KeyBindingsTable.CreateCSV(DataTableDebugCSV);
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
            return (AppDomain.CurrentDomain.BaseDirectory).Replace("Debug", string.Empty).Replace("bin", string.Empty).Replace("\\\\\\", string.Empty);
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
