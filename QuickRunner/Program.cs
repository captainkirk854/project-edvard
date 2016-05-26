namespace QuickRunner
{
    using System;
    using Helpers;
    using System.Data;

    class Program
    {
        static void Main(string[] args)
        {
            // Point to project sample (not a resource as such) data ..
            string EDCfg = GetProjectDirectory() + "\\Sample" + "\\ED01.binds";
            string VACfg = GetProjectDirectory() + "\\Sample" + "\\VA01.vap";
            const int KeyBindingsColumnWidth = 20;

            // Initialise ..
            DataTable KeyBindingsTable = new DataTable();

            // Read Elite Dangerous and Voice Attack configuration(s) to get key bindings ..
            try
            {
                KeyBindingsTable = Game.KeyBindingsConfigReader.EliteDangerous(EDCfg);
                KeyBindingsTable.Merge(Game.KeyBindingsConfigReader.VoiceAttack(VACfg));
                Console.WriteLine("Config(s) Read");
                PressIt();

                // Display DataTable Contents ..
                Data.DisplayDataTable(KeyBindingsTable, KeyBindingsColumnWidth, string.Empty);
                PressIt();

                Data.DisplayDataTable(KeyBindingsTable, KeyBindingsColumnWidth + 60, "KeyId");
                PressIt();

                Data.DisplayDataTable(KeyBindingsTable, KeyBindingsColumnWidth + 60, "ModifierKeyId");
                PressIt();
            }
            catch
            {
                Console.WriteLine("Something went wrong ... we cry");
                PressIt();
            }
        }

        /// <summary>
        /// Crude way of getting current project directory
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
