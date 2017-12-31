namespace Helper
{
    using System.Data;
    using Items;

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
            table.Columns.Add(EDVArd.Column.KeyEnumeration.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.KeyEnumerationValue.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.KeyEnumerationCode.ToString(), typeof(int));

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
            table.Columns.Add(EDVArd.Column.Context.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.KeyAction.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.KeyActionType.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.DevicePriority.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.DeviceType.ToString(), typeof(string));

            return table;
        }

        /// <summary>
        /// Define Key Actions Definition DataTable Structure
        /// </summary>
        /// <returns></returns>
        public static DataTable KeyActionDefinition()
        {
            // New DataTable ..
            DataTable table = new DataTable();
            table.TableName = "KeyActionDefinition";

            // Define its structure ..
            table.Columns.Add(EDVArd.Column.Context.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.KeyEnumeration.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.KeyAction.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.DevicePriority.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.KeyGameValue.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.KeyEnumerationValue.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.KeyEnumerationCode.ToString(), typeof(int));
            table.Columns.Add(EDVArd.Column.KeyActionType.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.KeyId.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.ModifierKeyGameValue.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.ModifierKeyEnumerationValue.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.ModifierKeyEnumerationCode.ToString(), typeof(int));
            table.Columns.Add(EDVArd.Column.ModifierKeyId.ToString(), typeof(string));

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
            table.Columns.Add(EDVArd.Column.KeyEnumeration.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(EDVArd.Column.KeyUpdateRequired.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.Rationale.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(EDVArd.Column.VoiceAttackAction.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.EliteDangerousAction.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(EDVArd.Column.VoiceAttackKeyValue.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.VoiceAttackKeyCode.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.VoiceAttackKeyId.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.VoiceAttackModifierKeyValue.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.VoiceAttackModifierKeyCode.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.VoiceAttackModifierKeyId.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(EDVArd.Column.EliteDangerousDevicePriority.ToString(), typeof(string)); 
            table.Columns.Add(EDVArd.Column.EliteDangerousKeyValue.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.EliteDangerousKeyCode.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.EliteDangerousKeyId.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.EliteDangerousModifierKeyValue.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.EliteDangerousModifierKeyCode.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.EliteDangerousModifierKeyId.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(EDVArd.Column.VoiceAttackInternal.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.VoiceAttackProfile.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.EliteDangerousInternal.ToString(), typeof(string));           
            table.Columns.Add(EDVArd.Column.EliteDangerousBinds.ToString(), typeof(string));
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
            table.Columns.Add(EDVArd.Column.KeyEnumeration.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(EDVArd.Column.EliteDangerousAction.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.VoiceAttackAction.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(EDVArd.Column.VoiceAttackKeyValue.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.VoiceAttackKeyCode.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.EliteDangerousKeyValue.ToString(), typeof(string));

            table.Columns.Add(EDVArd.Column.VoiceAttackModifierKeyValue.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.VoiceAttackModifierKeyCode.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.EliteDangerousModifierKeyValue.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(EDVArd.Column.VoiceAttackInternal.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.VoiceAttackProfile.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.EliteDangerousInternal.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.EliteDangerousBinds.ToString(), typeof(string));
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
            table.Columns.Add(EDVArd.Column.VoiceAttackAction.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.EliteDangerousAction.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.VoiceAttackCommand.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.BindingSynchronisationStatus.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.VoiceAttackProfile.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.EliteDangerousBinds.ToString(), typeof(string));

            return table;
        }

        public static DataTable AllVoiceCommands()
        {
            // New DataTable ..
            DataTable table = new DataTable();
            table.TableName = "AllVoiceCommands";

            table.Columns.Add(EDVArd.Column.VoiceAttackCategory.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.VoiceAttackCommand.ToString(), typeof(string));
            table.Columns.Add(EDVArd.Column.VoiceAttackActionType.ToString(), typeof(string));

            return table;
        }
    }
}