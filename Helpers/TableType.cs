namespace Helpers
{
    using System.Data;

    public static class TableType
    {
        /// <summary>
        /// Define Bindable Actions DataTable Structure
        /// </summary>
        /// <returns></returns>
        public static DataTable BindableActions()
        {
            // New DataTable ..
            DataTable table = new DataTable();
            table.TableName = "BindableActions";

            // Define its structure ..
            table.Columns.Add(Enums.Column.Context.ToString(), typeof(string));
            table.Columns.Add(Enums.Column.KeyAction.ToString(), typeof(string));
            table.Columns.Add(Enums.Column.DevicePriority.ToString(), typeof(string));
            table.Columns.Add(Enums.Column.DeviceType.ToString(), typeof(string));

            return table;
        }

        /// <summary>
        /// Define Key Actions Bindings DataTable Structure
        /// </summary>
        /// <returns></returns>
        public static DataTable KeyActionBinder()
        {
            // New DataTable ..
            DataTable table = new DataTable();
            table.TableName = "ActionKeyBindings";

            // Define its structure ..
            table.Columns.Add(Enums.Column.Context.ToString(), typeof(string));
            table.Columns.Add(Enums.Column.KeyEnumeration.ToString(), typeof(string));
            table.Columns.Add(Enums.Column.KeyAction.ToString(), typeof(string));
            table.Columns.Add(Enums.Column.DevicePriority.ToString(), typeof(string));
            table.Columns.Add(Enums.Column.KeyGameValue.ToString(), typeof(string));
            table.Columns.Add(Enums.Column.KeyEnumerationValue.ToString(), typeof(string));
            table.Columns.Add(Enums.Column.KeyEnumerationCode.ToString(), typeof(int));
            table.Columns.Add(Enums.Column.KeyId.ToString(), typeof(string));
            table.Columns.Add(Enums.Column.ModifierKeyGameValue.ToString(), typeof(string));
            table.Columns.Add(Enums.Column.ModifierKeyEnumerationValue.ToString(), typeof(string));
            table.Columns.Add(Enums.Column.ModifierKeyEnumerationCode.ToString(), typeof(int));
            table.Columns.Add(Enums.Column.ModifierKeyId.ToString(), typeof(string));

            return table;
        }

        /// <summary>
        /// Define Consolidated Actions DataTable Structure
        /// </summary>
        /// <returns></returns>
        public static DataTable ConsolidatedActions()
        {
            // New DataTable ..
            DataTable table = new DataTable();
            table.TableName = "ConsolidatedActions";

            // Define its structure ..
            //--------------------------------------------------------------------------
            table.Columns.Add(Enums.Column.KeyEnumeration.ToString(), typeof(string));
            //--------------------------------------------------------------------------
            table.Columns.Add(Enums.Column.KeyUpdateRequired.ToString(), typeof(string));
            table.Columns.Add(Enums.Column.Rationale.ToString(), typeof(string));
            //--------------------------------------------------------------------------
            table.Columns.Add(Enums.Column.VoiceAttackAction.ToString(), typeof(string));
            table.Columns.Add(Enums.Column.EliteDangerousAction.ToString(), typeof(string));
            //--------------------------------------------------------------------------
            table.Columns.Add(Enums.Column.VoiceAttackKeyValue.ToString(), typeof(string));
            table.Columns.Add(Enums.Column.VoiceAttackKeyCode.ToString(), typeof(string));
            table.Columns.Add(Enums.Column.VoiceAttackKeyId.ToString(), typeof(string));
            //--------------------------------------------------------------------------
            table.Columns.Add(Enums.Column.EliteDangerousDevicePriority.ToString(), typeof(string)); 
            table.Columns.Add(Enums.Column.EliteDangerousKeyValue.ToString(), typeof(string));
            table.Columns.Add(Enums.Column.EliteDangerousKeyCode.ToString(), typeof(string));
            table.Columns.Add(Enums.Column.EliteDangerousKeyId.ToString(), typeof(string));
            table.Columns.Add(Enums.Column.EliteDangerousModifierKeyValue.ToString(), typeof(string));
            table.Columns.Add(Enums.Column.EliteDangerousModifierKeyCode.ToString(), typeof(string));
            table.Columns.Add(Enums.Column.EliteDangerousModifierKeyId.ToString(), typeof(string));
            //--------------------------------------------------------------------------
            table.Columns.Add(Enums.Column.VoiceAttackInternal.ToString(), typeof(string));
            table.Columns.Add(Enums.Column.VoiceAttackProfile.ToString(), typeof(string));
            table.Columns.Add(Enums.Column.EliteDangerousInternal.ToString(), typeof(string));           
            table.Columns.Add(Enums.Column.EliteDangerousBinds.ToString(), typeof(string));
            //--------------------------------------------------------------------------

            return table;
        }
    }
}
