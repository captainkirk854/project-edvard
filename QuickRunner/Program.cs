namespace QuickRunner
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Helpers;
    using Game;
    using System.Data;

    class Program
    {
        static void Main(string[] args)
        {
            // Consts to point to test cfg files ..
            const string EDCfg = "C:\\Users\\Dad\\Desktop\\Elite Dangerous\\data\\ED\\GoodKeys.1.8.binds";
            const string VACfg = "C:\\Users\\Dad\\Desktop\\Elite Dangerous\\data\\VA\\My Elite Dangerous-Profile.vap";
            const int KeyBindingsColumnWidth = 20;

            // Initialise ..
            DataTable KeyBindingsTable = new DataTable();

            // Read Elite Dangerous and Voice Attack configuration(s) to get key bindings ..
            KeyBindingsTable = Game.ConfigRead.EliteDangerous(EDCfg);
            KeyBindingsTable.Merge(Game.ConfigRead.VoiceAttack(VACfg));
            Console.WriteLine("Config Read");
            Console.WriteLine("Press a key");
            Console.ReadKey();

            // Display DataTable Contents ..
            Data.DisplayDataTable(KeyBindingsTable, KeyBindingsColumnWidth, string.Empty);
            Console.WriteLine("Press a key");
            Console.ReadKey();

            Data.DisplayDataTable(KeyBindingsTable, KeyBindingsColumnWidth + 60, "KeyId");
            Console.WriteLine("Press a key");
            Console.ReadKey();

            Data.DisplayDataTable(KeyBindingsTable, KeyBindingsColumnWidth + 60, "ModifierKeyId");
            Console.WriteLine("Press a key");
            Console.ReadKey();
        }
    }
}
