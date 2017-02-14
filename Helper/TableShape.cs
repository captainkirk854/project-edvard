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
            table.Columns.Add(Edvard.Column.KeyEnumeration.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.KeyEnumerationValue.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.KeyEnumerationCode.ToString(), typeof(int));

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
            table.Columns.Add(Edvard.Column.Context.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.KeyAction.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.DevicePriority.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.DeviceType.ToString(), typeof(string));

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
            table.Columns.Add(Edvard.Column.Context.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.KeyEnumeration.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.KeyAction.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.DevicePriority.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.KeyGameValue.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.KeyEnumerationValue.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.KeyEnumerationCode.ToString(), typeof(int));
            table.Columns.Add(Edvard.Column.KeyId.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.ModifierKeyGameValue.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.ModifierKeyEnumerationValue.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.ModifierKeyEnumerationCode.ToString(), typeof(int));
            table.Columns.Add(Edvard.Column.ModifierKeyId.ToString(), typeof(string));

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
            table.Columns.Add(Edvard.Column.KeyEnumeration.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(Edvard.Column.KeyUpdateRequired.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.Rationale.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(Edvard.Column.VoiceAttackAction.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.EliteDangerousAction.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(Edvard.Column.VoiceAttackKeyValue.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.VoiceAttackKeyCode.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.VoiceAttackKeyId.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.VoiceAttackModifierKeyValue.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.VoiceAttackModifierKeyCode.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.VoiceAttackModifierKeyId.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(Edvard.Column.EliteDangerousDevicePriority.ToString(), typeof(string)); 
            table.Columns.Add(Edvard.Column.EliteDangerousKeyValue.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.EliteDangerousKeyCode.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.EliteDangerousKeyId.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.EliteDangerousModifierKeyValue.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.EliteDangerousModifierKeyCode.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.EliteDangerousModifierKeyId.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(Edvard.Column.VoiceAttackInternal.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.VoiceAttackProfile.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.EliteDangerousInternal.ToString(), typeof(string));           
            table.Columns.Add(Edvard.Column.EliteDangerousBinds.ToString(), typeof(string));
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
            table.Columns.Add(Edvard.Column.KeyEnumeration.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(Edvard.Column.EliteDangerousAction.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.VoiceAttackAction.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(Edvard.Column.VoiceAttackKeyValue.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.VoiceAttackKeyCode.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.EliteDangerousKeyValue.ToString(), typeof(string));

            table.Columns.Add(Edvard.Column.VoiceAttackModifierKeyValue.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.VoiceAttackModifierKeyCode.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.EliteDangerousModifierKeyValue.ToString(), typeof(string));
            ////--------------------------------------------------------------------------
            table.Columns.Add(Edvard.Column.VoiceAttackInternal.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.VoiceAttackProfile.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.EliteDangerousInternal.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.EliteDangerousBinds.ToString(), typeof(string));
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
            table.Columns.Add(Edvard.Column.VoiceAttackAction.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.EliteDangerousAction.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.VoiceAttackCommand.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.BindingSynchronisationStatus.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.VoiceAttackProfile.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.EliteDangerousBinds.ToString(), typeof(string));

            return table;
        }

        public static DataTable AllVoiceCommands()
        {
            // New DataTable ..
            DataTable table = new DataTable();
            table.TableName = "AllVoiceCommands";

            table.Columns.Add(Edvard.Column.VoiceAttackCategory.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.VoiceAttackCommand.ToString(), typeof(string));
            table.Columns.Add(Edvard.Column.VoiceAttackActionType.ToString(), typeof(string));

            return table;
        }
    }
}