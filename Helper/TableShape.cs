namespace Helper
{
    using System.Data;

    public static class TableShape
    {
        /// <summary>
        /// Define KeyMap DataTable Structure
        /// </summary>
        /// <returns></returns>
        public static DataTable DefineKeyMap()
        {
            // New DataTable ..
            DataTable table = new DataTable();
            table.TableName = "KeyMap";

            // Define its structure ..
            table.Columns.Add(EnumsEdVArd.Column.KeyEnumeration.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.KeyEnumerationValue.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.KeyEnumerationCode.ToString(), typeof(int));

            return table;
        }

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
            table.Columns.Add(EnumsEdVArd.Column.Context.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.KeyAction.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.DevicePriority.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.DeviceType.ToString(), typeof(string));

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
            table.Columns.Add(EnumsEdVArd.Column.Context.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.KeyEnumeration.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.KeyAction.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.DevicePriority.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.KeyGameValue.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.KeyEnumerationValue.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.KeyEnumerationCode.ToString(), typeof(int));
            table.Columns.Add(EnumsEdVArd.Column.KeyId.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.ModifierKeyGameValue.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.ModifierKeyEnumerationValue.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.ModifierKeyEnumerationCode.ToString(), typeof(int));
            table.Columns.Add(EnumsEdVArd.Column.ModifierKeyId.ToString(), typeof(string));

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
            ////--------------------------------------------------------------------------
            table.Columns.Add(EnumsEdVArd.Column.KeyEnumeration.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(EnumsEdVArd.Column.KeyUpdateRequired.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.Rationale.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(EnumsEdVArd.Column.VoiceAttackAction.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.EliteDangerousAction.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(EnumsEdVArd.Column.VoiceAttackKeyValue.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.VoiceAttackKeyCode.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.VoiceAttackKeyId.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.VoiceAttackModifierKeyValue.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.VoiceAttackModifierKeyCode.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.VoiceAttackModifierKeyId.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(EnumsEdVArd.Column.EliteDangerousDevicePriority.ToString(), typeof(string)); 
            table.Columns.Add(EnumsEdVArd.Column.EliteDangerousKeyValue.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.EliteDangerousKeyCode.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.EliteDangerousKeyId.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.EliteDangerousModifierKeyValue.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.EliteDangerousModifierKeyCode.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.EliteDangerousModifierKeyId.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(EnumsEdVArd.Column.VoiceAttackInternal.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.VoiceAttackProfile.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.EliteDangerousInternal.ToString(), typeof(string));           
            table.Columns.Add(EnumsEdVArd.Column.EliteDangerousBinds.ToString(), typeof(string));
            ////--------------------------------------------------------------------------

            return table;
        }

        /// <summary>
        /// Define Reverse Bindable Vacant Elite Dangerous Actions DataTable Structure
        /// </summary>
        /// <returns></returns>
        public static DataTable ReverseBindableVacantEliteDangerousActions()
        {
            // New DataTable ..
            DataTable table = new DataTable();
            table.TableName = "ReverseBindableVacantEliteDangerousActions";

            // Define its structure ..
            ////--------------------------------------------------------------------------
            table.Columns.Add(EnumsEdVArd.Column.KeyEnumeration.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(EnumsEdVArd.Column.EliteDangerousAction.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.VoiceAttackAction.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(EnumsEdVArd.Column.VoiceAttackKeyValue.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.VoiceAttackKeyCode.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.EliteDangerousKeyValue.ToString(), typeof(string));

            table.Columns.Add(EnumsEdVArd.Column.VoiceAttackModifierKeyValue.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.VoiceAttackModifierKeyCode.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.EliteDangerousModifierKeyValue.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(EnumsEdVArd.Column.VoiceAttackInternal.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.VoiceAttackProfile.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.EliteDangerousInternal.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.EliteDangerousBinds.ToString(), typeof(string));
            ////--------------------------------------------------------------------------

            return table;
        }

        /// <summary>
        /// Define Associated Commands DataTable Structure
        /// </summary>
        /// <returns></returns>
        public static DataTable AssociatedCommands()
        {
            // New DataTable ..
            DataTable table = new DataTable();
            table.TableName = "AssociatedCommands";

            // Define its structure ..
            table.Columns.Add(EnumsEdVArd.Column.VoiceAttackAction.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.EliteDangerousAction.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.VoiceAttackCommand.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.BindingSynchronisationStatus.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.VoiceAttackProfile.ToString(), typeof(string));
            table.Columns.Add(EnumsEdVArd.Column.EliteDangerousBinds.ToString(), typeof(string));

            return table;
        }
    }
}