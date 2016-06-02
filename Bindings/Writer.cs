namespace Bindings
{
    using Helpers;
    using System.Data;

    /// <summary>
    /// Write Elite Dangerous and Voice Attack Binding Configuration Files
    /// </summary>
    public static partial class Writer
    {
        public static DataTable Consolidate(DataTable keyBindingsVA, DataTable keyBindingsED)
        {
            DataTable keyBindingsConsolidated = GameAction.Consolidate(keyBindingsVA, keyBindingsED);
            keyBindingsConsolidated = keyBindingsConsolidated.Sort(Enums.Column.EliteDangerousAction.ToString() + " asc");

            return keyBindingsConsolidated;
        }
    }
}