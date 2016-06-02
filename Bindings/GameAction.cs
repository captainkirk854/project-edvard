namespace Bindings
{
    using System;
    using System.Data;
    using Helpers;

    public static class GameAction
    {
        private const string NA = "n/a";

        /// <summary>
        /// Consolidate Action bindings between VoiceAttack and Elite Dangerous ..
        /// </summary>
        /// <param name="voiceAttack"></param>
        /// <param name="eliteDangerous"></param>
        /// <returns></returns>
        public static DataTable Consolidate(DataTable voiceAttack, DataTable eliteDangerous)
        {
            // Initialise lookup dictionary ..
            ActionExchange actions = new ActionExchange();
            actions.Initialise();

            // Datatable to hold tabulated contents ..
            DataTable consolidatedaction = DefineConsolidatedActions();

            // Search through all defined Voice Attack bindings ..
            var voiceattackBindings = from va in voiceAttack.AsEnumerable()
                                    select
                                       new
                                         {
                                            KeyEnumeration = va.Field<string>(Enums.Column.KeyEnumeration.ToString()),
                                            EliteDangerousAction = actions.GetED(va.Field<string>(Enums.Column.KeyAction.ToString())),
                                            Action = va.Field<string>(Enums.Column.KeyAction.ToString()),
                                            KeyValue = va.Field<string>(Enums.Column.KeyEnumerationValue.ToString()),
                                            KeyCode = va.Field<int>(Enums.Column.KeyEnumerationCode.ToString()),
                                            KeyID = va.Field<string>(Enums.Column.KeyId.ToString()),
                                            FilePath = va.Field<string>(Enums.Column.FilePath.ToString())
                                         };

            // .. and compare with what has been defined in the Elite Dangerous bindings ..
            foreach (var voiceattackBinding in voiceattackBindings)
            {
                bool definedInED = false;
                string remapRequired = "unknown";
                string rationale = "unknown";

                var elitedangerousBindings = from ed in eliteDangerous.AsEnumerable()
                                            where ed.Field<string>(Enums.Column.KeyAction.ToString()) == voiceattackBinding.EliteDangerousAction
                                           select
                                              new
                                                {
                                                    Action = ed.Field<string>(Enums.Column.KeyAction.ToString()),
                                                    KeyPriority = ed.Field<string>(Enums.Column.DevicePriority.ToString()),
                                                    KeyGameValue = ed.Field<string>(Enums.Column.KeyGameValue.ToString()),
                                                    KeyEnumerationValue = ed.Field<string>(Enums.Column.KeyEnumerationValue.ToString()),
                                                    KeyEnumerationCode = ed.Field<int>(Enums.Column.KeyEnumerationCode.ToString()),
                                                    FilePath = ed.Field<string>(Enums.Column.FilePath.ToString())
                                                };

                // Compare matching action bindings with their assigned key value/code ..
                foreach (var elitedangerousBinding in elitedangerousBindings)
                {
                    definedInED = true;
                    if (elitedangerousBinding.KeyEnumerationCode == voiceattackBinding.KeyCode)
                    {
                        remapRequired = Enums.ReMapRequired.NO.ToString();
                        rationale = "Key codes are aligned";
                    }
                    else
                    {
                        if (elitedangerousBinding.KeyEnumerationCode > 0)
                        {
                            remapRequired = Enums.ReMapRequired.YES.ToString();
                            rationale = string.Format("Misaligned key codes: Voice Attack Profile requires change in key code from [{0}] to [{1}]", voiceattackBinding.KeyCode, elitedangerousBinding.KeyEnumerationCode);
                        }
                        else
                        {
                            remapRequired = Enums.ReMapRequired.NO.ToString();
                            rationale = string.Format("Unresolvable key code for: [{0}]", elitedangerousBinding.KeyGameValue);
                        }
                    }

                    // Append to DataTable ..
                    consolidatedaction.LoadDataRow(new object[] 
                                                {
                                                 voiceattackBinding.Action, //VoiceAttackAction
                                                 voiceattackBinding.EliteDangerousAction, //EliteDangerousAction
                                                 elitedangerousBinding.KeyPriority, //DevicePriority
                                                 voiceattackBinding.KeyEnumeration, //KeyEnumeration
                                                 voiceattackBinding.KeyValue, //VoiceAttackKeyValue
                                                 elitedangerousBinding.KeyGameValue, //EliteDangerousKeyValue
                                                 voiceattackBinding.KeyCode, //VoiceAttackKeyCode
                                                 elitedangerousBinding.KeyEnumerationCode, //EliteDangerousKeyCode
                                                 voiceattackBinding.KeyID, //VoiceAttackKeyId
                                                 remapRequired, //ReMapRequired
                                                 rationale, //Rationale
                                                 voiceattackBinding.FilePath, //VoiceAttackProfile
                                                 elitedangerousBinding.FilePath//EliteDangerousBinds
                                                },
                                                false);
                }

                // If not defined in Elite Dangerous binding file ..
                if (!definedInED)
                {
                    // Append to DataTable
                    remapRequired = Enums.ReMapRequired.NO.ToString();
                    rationale = string.Format("[{0}] has not been bound to a key", voiceattackBinding.EliteDangerousAction);
                    consolidatedaction.LoadDataRow(new object[] 
                                                {
                                                 voiceattackBinding.Action, //VoiceAttackAction
                                                 voiceattackBinding.EliteDangerousAction, //EliteDangerousAction
                                                 NA, //DevicePriority
                                                 voiceattackBinding.KeyEnumeration, //KeyEnumeration
                                                 voiceattackBinding.KeyValue, //VoiceAttackKeyValue
                                                 NA, //EliteDangerousKeyValue
                                                 voiceattackBinding.KeyCode, //VoiceAttackKeyCode
                                                 NA, //EliteDangerousKeyCode
                                                 voiceattackBinding.KeyID, //VoiceAttackKeyId
                                                 remapRequired, //ReMapRequired
                                                 rationale, //Rationale
                                                 voiceattackBinding.FilePath, //VoiceAttackProfile
                                                 NA //EliteDangerousBinds
                                                },
                                                false);
                }
            }

            return consolidatedaction;
        }

        /// <summary>
        /// Define Binding Actions DataTable Structure
        /// </summary>
        /// <returns></returns>
        private static DataTable DefineConsolidatedActions()
        {
            // New DataTable ..
            DataTable consolidatedActions = new DataTable();
            consolidatedActions.TableName = "ConsolidatedActions";

            // Define its structure ..
            consolidatedActions.Columns.Add(Enums.Column.VoiceAttackAction.ToString(), typeof(string));
            consolidatedActions.Columns.Add(Enums.Column.EliteDangerousAction.ToString(), typeof(string));
            consolidatedActions.Columns.Add(Enums.Column.DevicePriority.ToString(), typeof(string));
            consolidatedActions.Columns.Add(Enums.Column.KeyEnumeration.ToString(), typeof(string));
            consolidatedActions.Columns.Add(Enums.Column.VoiceAttackKeyValue.ToString(), typeof(string));
            consolidatedActions.Columns.Add(Enums.Column.EliteDangerousKeyValue.ToString(), typeof(string));
            consolidatedActions.Columns.Add(Enums.Column.VoiceAttackKeyCode.ToString(), typeof(string));
            consolidatedActions.Columns.Add(Enums.Column.EliteDangerousKeyCode.ToString(), typeof(string));
            consolidatedActions.Columns.Add(Enums.Column.VoiceAttackKeyId.ToString(), typeof(string));
            consolidatedActions.Columns.Add(Enums.Column.ReMapRequired.ToString(), typeof(string));
            consolidatedActions.Columns.Add(Enums.Column.Rationale.ToString(), typeof(string));
            consolidatedActions.Columns.Add(Enums.Column.VoiceAttackProfile.ToString(), typeof(string));
            consolidatedActions.Columns.Add(Enums.Column.EliteDangerousBinds.ToString(), typeof(string));

            return consolidatedActions;
        }
    }
}
