﻿namespace Bindings
{
    using System;
    using System.Data;
    using Helpers;

    /// <summary>
    /// Synchronise Binding Codes between VoiceAttack and Elite Dangerous ..
    /// </summary>
    public static class GameBindingsSynchroniser
    {
        private const string NA = "n/a";

        /// <summary>
        /// Update Command Codes in VoiceAttack based on Elite Dangerous Binds as Master ..
        /// </summary>
        /// <param name="voiceAttack"></param>
        /// <param name="eliteDangerous"></param>
        /// <returns></returns>
        public static DataTable ForUpdateInVoiceAttack(DataTable voiceAttack, DataTable eliteDangerous)
        {
            // Initialise lookup dictionary for inter-game action references ..
            CommandExchange actions = new CommandExchange();
            actions.Initialise();

            // Datatable to hold tabulated contents ..
            DataTable consolidatedaction = TableType.ConsolidatedActions();

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
                                            FilePath = va.Field<string>(Enums.Column.FilePath.ToString()),
                                            Internal = va.Field<string>(Enums.Column.Internal.ToString())
                                         };

            // .. and compare with what has been defined in the Elite Dangerous bindings ..
            foreach (var voiceattackBinding in voiceattackBindings)
            {
                bool commandDefinedInEliteDangerousBindsFile = false;
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
                                                    FilePath = ed.Field<string>(Enums.Column.FilePath.ToString()),
                                                    Internal = ed.Field<string>(Enums.Column.Internal.ToString())
                                                };

                // Compare matching action bindings with their assigned key value/code ..
                foreach (var elitedangerousBinding in elitedangerousBindings)
                {
                    commandDefinedInEliteDangerousBindsFile = true;
                    if (elitedangerousBinding.KeyEnumerationCode == voiceattackBinding.KeyCode)
                    {
                        remapRequired = Enums.KeyUpdateRequired.NO.ToString();
                        rationale = "Key codes are aligned";
                    }
                    else
                    {
                        if (elitedangerousBinding.KeyEnumerationCode > 0)
                        {
                            remapRequired = Enums.KeyUpdateRequired.YES.ToString();
                            rationale = string.Format("Misaligned key codes: Voice Attack Profile requires change in key code from [{0}] to [{1}]", voiceattackBinding.KeyCode, elitedangerousBinding.KeyEnumerationCode);
                        }
                        else
                        {
                            remapRequired = Enums.KeyUpdateRequired.NO.ToString();
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
                                                 voiceattackBinding.Internal, //VoiceAttackInternal
                                                 elitedangerousBinding.Internal, //EliteDangerousInternal
                                                 voiceattackBinding.FilePath, //VoiceAttackProfile
                                                 elitedangerousBinding.FilePath//EliteDangerousBinds
                                                }, false);
                }

                // If not defined in Elite Dangerous binding file ..
                if (!commandDefinedInEliteDangerousBindsFile)
                {
                    // Append to DataTable
                    remapRequired = Enums.KeyUpdateRequired.NO.ToString();
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
                                                 voiceattackBinding.Internal, //VoiceAttackInternal
                                                 NA, //EliteDangerousInternal
                                                 voiceattackBinding.FilePath, //VoiceAttackProfile
                                                 NA //EliteDangerousBinds
                                                },
                                                false);
                }
            }

            return consolidatedaction;
        }
    }
}