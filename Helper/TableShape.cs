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
            table.Columns.Add(EnumsInternal.Column.KeyEnumeration.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.KeyEnumerationValue.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.KeyEnumerationCode.ToString(), typeof(int));

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
            table.Columns.Add(EnumsInternal.Column.Context.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.KeyAction.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.DevicePriority.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.DeviceType.ToString(), typeof(string));

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
            table.Columns.Add(EnumsInternal.Column.Context.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.KeyEnumeration.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.KeyAction.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.DevicePriority.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.KeyGameValue.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.KeyEnumerationValue.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.KeyEnumerationCode.ToString(), typeof(int));
            table.Columns.Add(EnumsInternal.Column.KeyId.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.ModifierKeyGameValue.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.ModifierKeyEnumerationValue.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.ModifierKeyEnumerationCode.ToString(), typeof(int));
            table.Columns.Add(EnumsInternal.Column.ModifierKeyId.ToString(), typeof(string));

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
            table.Columns.Add(EnumsInternal.Column.KeyEnumeration.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(EnumsInternal.Column.KeyUpdateRequired.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.Rationale.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(EnumsInternal.Column.VoiceAttackAction.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.EliteDangerousAction.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(EnumsInternal.Column.VoiceAttackKeyValue.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.VoiceAttackKeyCode.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.VoiceAttackKeyId.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.VoiceAttackModifierKeyValue.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.VoiceAttackModifierKeyCode.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.VoiceAttackModifierKeyId.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(EnumsInternal.Column.EliteDangerousDevicePriority.ToString(), typeof(string)); 
            table.Columns.Add(EnumsInternal.Column.EliteDangerousKeyValue.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.EliteDangerousKeyCode.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.EliteDangerousKeyId.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.EliteDangerousModifierKeyValue.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.EliteDangerousModifierKeyCode.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.EliteDangerousModifierKeyId.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(EnumsInternal.Column.VoiceAttackInternal.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.VoiceAttackProfile.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.EliteDangerousInternal.ToString(), typeof(string));           
            table.Columns.Add(EnumsInternal.Column.EliteDangerousBinds.ToString(), typeof(string));
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
            table.Columns.Add(EnumsInternal.Column.KeyEnumeration.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(EnumsInternal.Column.EliteDangerousAction.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.VoiceAttackAction.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(EnumsInternal.Column.VoiceAttackKeyValue.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.VoiceAttackKeyCode.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.EliteDangerousKeyValue.ToString(), typeof(string));

            table.Columns.Add(EnumsInternal.Column.VoiceAttackModifierKeyValue.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.VoiceAttackModifierKeyCode.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.EliteDangerousModifierKeyValue.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(EnumsInternal.Column.VoiceAttackInternal.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.VoiceAttackProfile.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.EliteDangerousInternal.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.EliteDangerousBinds.ToString(), typeof(string));
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
            table.Columns.Add(EnumsInternal.Column.VoiceAttackAction.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.EliteDangerousAction.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.VoiceAttackCommand.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.BindingSynchronisationStatus.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.VoiceAttackProfile.ToString(), typeof(string));
            table.Columns.Add(EnumsInternal.Column.EliteDangerousBinds.ToString(), typeof(string));

            return table;
        }
    }
}