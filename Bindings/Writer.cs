namespace Bindings
{
    using Helpers;
    using System.Data;

    /// <summary>
    /// Write Elite Dangerous and Voice Attack Binding Configuration Files
    /// </summary>
    public static partial class Writer
    {
        public static DataTable Consolidate(DataTable keyBindingsVA, DataTable keyBindingsED, string cfgVA, string cfgED)
        {
            DataTable keyBindingsConsolidated = new DataTable();

            keyBindingsConsolidated = GameAction.Consolidate(keyBindingsVA, keyBindingsED);
            keyBindingsConsolidated.AddDefaultColumn(Enums.Column.VoiceAttackProfile.ToString(), cfgVA);
            keyBindingsConsolidated.AddDefaultColumn(Enums.Column.EliteDangerousBinds.ToString(), cfgED);
            keyBindingsConsolidated = keyBindingsConsolidated.Sort(Enums.Column.EliteDangerousAction.ToString() + " asc");

            return keyBindingsConsolidated;
        }
    }
}