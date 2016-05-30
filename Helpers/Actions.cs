namespace Helpers
{
    using System;
    using System.Data;

    public static class Actions
    {
        private const string NA = "n/a";

        /// <summary>
        /// Consolidate Action bindings between VoiceAttack and Elite Dangerous ..
        /// </summary>
        /// <param name="keyBindingsVA"></param>
        /// <param name="keyBindingsED"></param>
        /// <returns></returns>
        public static DataTable Consolidate(DataTable keyBindingsVA, DataTable keyBindingsED)
        {
            // Initialise lookup dictionary ..
            ActionExchange actions = new ActionExchange();
            actions.Initialise();

            // Datatable to hold tabulated contents ..
            DataTable consolidatedaction = new DataTable();
            consolidatedaction.DefineConsolidatedActions();

            // Search through all defined Voice Attack bindings ..
            var voiceattackBindings = from va in keyBindingsVA.AsEnumerable()
                             select
                                new
                                {
                                    VAAction = va.Field<string>(Enums.Column.KeyAction.ToString()),
                                    VAKeyValue = va.Field<string>(Enums.Column.KeyEnumerationValue.ToString()),
                                    VAKeyCode = va.Field<int>(Enums.Column.KeyEnumerationCode.ToString()),
                                    VAKeyID = va.Field<string>(Enums.Column.KeyId.ToString()),
                                    EDAction = actions.GetED(va.Field<string>(Enums.Column.KeyAction.ToString()))
                                };

            // .. and compare with what has been defined in the Elite Dangerous bindings ..
            foreach (var voiceattackBinding in voiceattackBindings)
            {
                bool definedInED = false;
                string operationConclusion = "No Action possible";

                var elitedangerousBindings = from ed in keyBindingsED.AsEnumerable()
                                 where ed.Field<string>(Enums.Column.KeyAction.ToString()) == voiceattackBinding.EDAction
                                 select
                                    new
                                    {
                                        EDAction = ed.Field<string>(Enums.Column.KeyAction.ToString()),
                                        EDKeyValue = ed.Field<string>(Enums.Column.KeyEnumerationValue.ToString()),
                                        EDKeyCode = ed.Field<int>(Enums.Column.KeyEnumerationCode.ToString())
                                    };

                // Compare matching action bindings with their assigned key value/code ..
                foreach (var elitedangerousBinding in elitedangerousBindings)
                {
                    definedInED = true;
                    if (elitedangerousBinding.EDKeyCode == voiceattackBinding.VAKeyCode)
                    {
                        operationConclusion = "No Action required - Key Code matches in both systems";
                    }
                    else
                    {
                        operationConclusion = string.Format("Voice Attack profile requires change in Key Code from {0} to {1}", voiceattackBinding.VAKeyCode, elitedangerousBinding.EDKeyCode);
                    }

                    // Append to DataTable ..
                    consolidatedaction.LoadDataRow(new object[] 
                                                {
                                                 voiceattackBinding.VAAction, //VoiceAttackAction
                                                 voiceattackBinding.EDAction, //EliteDangerousAction
                                                 voiceattackBinding.VAKeyValue, //VoiceAttackKeyValue
                                                 elitedangerousBinding.EDKeyValue, //EliteDangerousKeyValue
                                                 voiceattackBinding.VAKeyCode, //VoiceAttackKeyCode
                                                 elitedangerousBinding.EDKeyCode, //EliteDangerousKeyCode
                                                 voiceattackBinding.VAKeyID, //VoiceAttackKeyId
                                                 operationConclusion //OperationRequired
                                                },
                                                false);
                }

                // If not defined in Elite Dangerous binding file ..
                if (!definedInED)
                {
                    // Append to DataTable 
                    consolidatedaction.LoadDataRow(new object[] 
                                                {
                                                 voiceattackBinding.VAAction, //VoiceAttackAction
                                                 voiceattackBinding.EDAction, //EliteDangerousAction
                                                 voiceattackBinding.VAKeyValue, //VoiceAttackKeyValue
                                                 NA, //EliteDangerousKeyValue
                                                 voiceattackBinding.VAKeyCode, //VoiceAttackKeyCode
                                                 NA, //EliteDangerousKeyCode
                                                 voiceattackBinding.VAKeyID, //VoiceAttackKeyId
                                                 operationConclusion //OperationRequired
                                                },
                                                false);
                }
            }

            return consolidatedaction;
        }

        /// <summary>
        /// Define Binding Actions DataTable Structure
        /// </summary>
        /// <param name="consolidatedActions"></param>
        private static void DefineConsolidatedActions(this DataTable consolidatedActions)
        {
            consolidatedActions.TableName = "ConsolidatedActions";

            // Define table structure ..
            consolidatedActions.Columns.Add(Enums.Column.VoiceAttackAction.ToString(), typeof(string));
            consolidatedActions.Columns.Add(Enums.Column.EliteDangerousAction.ToString(), typeof(string));
            consolidatedActions.Columns.Add(Enums.Column.VoiceAttackKeyValue.ToString(), typeof(string));
            consolidatedActions.Columns.Add(Enums.Column.EliteDangerousKeyValue.ToString(), typeof(string));
            consolidatedActions.Columns.Add(Enums.Column.VoiceAttackKeyCode.ToString(), typeof(string));
            consolidatedActions.Columns.Add(Enums.Column.EliteDangerousKeyCode.ToString(), typeof(string));
            consolidatedActions.Columns.Add(Enums.Column.VoiceAttackKeyId.ToString(), typeof(string));
            consolidatedActions.Columns.Add(Enums.Column.OperationRequired.ToString(), typeof(string));
        }
    }
}
