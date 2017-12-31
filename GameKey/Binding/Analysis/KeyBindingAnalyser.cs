namespace GameKey.Binding.Analysis
{
    using GameKey.Adapter;
    using GameKey.Binding.Readers;
    using Helper;
    using Items;
    using System.Data;
    using Utility;

    /// <summary>
    /// Analyse and consolidate game actions between VoiceAttack Commands and Elite Dangerous Bindings ..
    /// </summary>
    public static class KeyBindingAnalyser
    {
        /// <summary>
        /// Adjust VoiceAttack Command key codes to match those in Elite Dangerous using Elite Dangerous command bindings as master ..
        /// </summary>
        /// <param name="eliteDangerousBinds"></param>
        /// <param name="voiceAttackProfile"></param>
        /// <param name="keyLookup"></param>
        /// <returns></returns>
        public static DataTable VoiceAttack(string eliteDangerousBinds, string voiceAttackProfile, GameKeyAndCommandBindingsAdapter keyLookup)
        {
            // Datatable to hold tabulated contents ..
            DataTable adjustedVoiceAttackCommands = TableShape.ConsolidatedActions();
            string globalEliteDangerousBindsInternal = string.Empty;

            // Read bindings ..
            var eliteDangerous = new KeyBindingReaderEliteDangerous(eliteDangerousBinds).GetBoundCommands();
            var voiceAttack = new KeyBindingReaderVoiceAttack(voiceAttackProfile).GetBoundCommands();

            // Search through all defined Voice Attack bindings ..
            var voiceattackBindings = from va in voiceAttack.AsEnumerable()
                                    select
                                       new
                                         {
                                            KeyEnumeration = va.Field<string>(EDVArd.Column.KeyEnumeration.ToString()),
                                            EliteDangerousAction = keyLookup.GetEliteDangerousBinding(va.Field<string>(EDVArd.Column.KeyAction.ToString())),
                                            Action = va.Field<string>(EDVArd.Column.KeyAction.ToString()),
                                            KeyValue = va.Field<string>(EDVArd.Column.KeyEnumerationValue.ToString()),
                                            KeyCode = va.Field<int>(EDVArd.Column.KeyEnumerationCode.ToString()),
                                            KeyActionType = va.Field<string>(EDVArd.Column.KeyActionType.ToString()),
                                            KeyID = va.Field<string>(EDVArd.Column.KeyId.ToString()),
                                            ModifierKeyGameValue = va.Field<string>(EDVArd.Column.ModifierKeyGameValue.ToString()),
                                            ModifierKeyEnumerationValue = va.Field<string>(EDVArd.Column.ModifierKeyEnumerationValue.ToString()),
                                            ModifierKeyEnumerationCode = va.Field<int>(EDVArd.Column.ModifierKeyEnumerationCode.ToString()),
                                            ModifierKeyID = va.Field<string>(EDVArd.Column.ModifierKeyId.ToString()),
                                            FilePath = va.Field<string>(EDVArd.Column.FilePath.ToString()),
                                            Internal = va.Field<string>(EDVArd.Column.Internal.ToString())
                                         };

            // .. and compare to Elite Dangerous bindings ..
            foreach (var voiceattackBinding in voiceattackBindings)
            {
                bool commandDefinedInEliteDangerousBindsFile = false;
                string updateRequirementStatus = "unknown";
                string rationale = "unknown";

                var elitedangerousBindings = from ed in eliteDangerous.AsEnumerable()
                                             where ed.Field<string>(EDVArd.Column.KeyAction.ToString()) == voiceattackBinding.EliteDangerousAction
                                           select
                                              new
                                                {
                                                    Action = ed.Field<string>(EDVArd.Column.KeyAction.ToString()),
                                                    KeyPriority = ed.Field<string>(EDVArd.Column.DevicePriority.ToString()),
                                                    KeyGameValue = ed.Field<string>(EDVArd.Column.KeyGameValue.ToString()),
                                                    KeyEnumerationValue = ed.Field<string>(EDVArd.Column.KeyEnumerationValue.ToString()),
                                                    KeyEnumerationCode = ed.Field<int>(EDVArd.Column.KeyEnumerationCode.ToString()),
                                                    KeyID = ed.Field<string>(EDVArd.Column.KeyId.ToString()),
                                                    ModifierKeyGameValue = ed.Field<string>(EDVArd.Column.ModifierKeyGameValue.ToString()),
                                                    ModifierKeyEnumerationValue = ed.Field<string>(EDVArd.Column.ModifierKeyEnumerationValue.ToString()),
                                                    ModifierKeyEnumerationCode = ed.Field<int>(EDVArd.Column.ModifierKeyEnumerationCode.ToString()),
                                                    ModifierKeyID = ed.Field<string>(EDVArd.Column.ModifierKeyId.ToString()),
                                                    FilePath = ed.Field<string>(EDVArd.Column.FilePath.ToString()),
                                                    Internal = ed.Field<string>(EDVArd.Column.Internal.ToString())
                                                };

                // Compare matching action bindings with their assigned key value/code to evaluate which key code(s) require re-mapping ..
                foreach (var elitedangerousBinding in elitedangerousBindings)
                {
                    commandDefinedInEliteDangerousBindsFile = true;

                    // Assign for later use ..
                    globalEliteDangerousBindsInternal = elitedangerousBinding.Internal;

                    // Check for: satisfactory alignment of regular and modifier key codes ..
                    if (
                        // Matching Regular Key Codes with no Modifier Key(s) present ..
                        ((elitedangerousBinding.KeyEnumerationCode == voiceattackBinding.KeyCode) &&
                        (elitedangerousBinding.ModifierKeyEnumerationCode == StatusCode.NotApplicableInt))     ||
                        
                        // Matching Regular Key Codes with matching Modifier Key(s) present ..
                        ((elitedangerousBinding.KeyEnumerationCode == voiceattackBinding.KeyCode) &&
                        (elitedangerousBinding.ModifierKeyEnumerationCode == voiceattackBinding.ModifierKeyEnumerationCode)))
                    {
                        updateRequirementStatus = EDVArd.KeyUpdateRequired.NO.ToString();
                        rationale = "ED o--o VA Key Codes are aligned";
                    }
                    else
                    {
                        rationale = string.Empty;

                        // Check for misaligned codes ..
                        if (elitedangerousBinding.KeyEnumerationCode > StatusCode.EmptyStringInt)
                        {
                            updateRequirementStatus = EDVArd.KeyUpdateRequired.YES_Elite_TO_VoiceAttack.ToString();

                            // Check for: misaligned key codes ..
                            if (elitedangerousBinding.KeyEnumerationCode != voiceattackBinding.KeyCode)
                            {
                                rationale += string.Format("Misaligned key codes: ED[{0}] o--O VA[{1}];", elitedangerousBinding.KeyEnumerationCode, voiceattackBinding.KeyCode);
                            }

                            // Check for: misaligned modifier key codes ..
                            if (elitedangerousBinding.ModifierKeyEnumerationCode != voiceattackBinding.ModifierKeyEnumerationCode)
                            {
                                if (voiceattackBinding.ModifierKeyEnumerationCode == StatusCode.EmptyStringInt)
                                {
                                    rationale += string.Format("Misaligned modifier key codes: ED[{0}] o--O VA[{1}];", elitedangerousBinding.ModifierKeyEnumerationCode, "no-modifier defined");
                                }
                                else
                                {
                                    rationale += string.Format("Misaligned modifier key codes: ED[{0}] o--O VA[{1}];", elitedangerousBinding.ModifierKeyEnumerationCode, voiceattackBinding.ModifierKeyEnumerationCode);
                                }
                            }
                        }

                        // Check for unresolvable key codes ..
                        if (elitedangerousBinding.KeyEnumerationCode == StatusCode.NoEquivalentKeyFoundAtExchange || elitedangerousBinding.KeyEnumerationCode == StatusCode.NoCodeFoundAfterExchange)
                        {
                            updateRequirementStatus = EDVArd.KeyUpdateRequired.NO.ToString();
                            rationale += string.Format("Unresolvable key code for: ED[{0}];", elitedangerousBinding.KeyGameValue);
                        }

                        // Check for: unresolvable modifier key codes ..
                        if (elitedangerousBinding.ModifierKeyEnumerationCode == StatusCode.NoEquivalentKeyFoundAtExchange || elitedangerousBinding.ModifierKeyEnumerationCode == StatusCode.NoCodeFoundAfterExchange)
                        {
                            updateRequirementStatus = EDVArd.KeyUpdateRequired.NO.ToString();
                            rationale += string.Format("Unresolvable modifier key code for: ED[{0}];", elitedangerousBinding.ModifierKeyGameValue);
                        }
                    }

                    // Append evaluated results to DataTable ..
                    adjustedVoiceAttackCommands.LoadDataRow(new object[] 
                                                {
                                                 ////--------------------------------------------------------------------------
                                                 voiceattackBinding.KeyEnumeration, //KeyEnumeration
                                                 ////--------------------------------------------------------------------------
                                                 updateRequirementStatus, //ReMapRequired
                                                 rationale, //Rationale
                                                 ////--------------------------------------------------------------------------
                                                 voiceattackBinding.Action, //VoiceAttackAction
                                                 voiceattackBinding.EliteDangerousAction, //EliteDangerousAction
                                                 ////--------------------------------------------------------------------------
                                                 voiceattackBinding.KeyValue, //VoiceAttackKeyValue
                                                 voiceattackBinding.KeyCode, //VoiceAttackKeyCode
                                                 voiceattackBinding.KeyID, //VoiceAttackKeyId
                                                 voiceattackBinding.ModifierKeyEnumerationValue, //VoiceAttackKeyEnumerationValue
                                                 voiceattackBinding.ModifierKeyEnumerationCode, //VoiceAttackModifierKeyCode
                                                 voiceattackBinding.ModifierKeyID, //VoiceAttackModifierKeyId
                                                 ////--------------------------------------------------------------------------                                                 
                                                 elitedangerousBinding.KeyPriority, //EliteDangerousDevicePriority
                                                 elitedangerousBinding.KeyGameValue, //EliteDangerousKeyValue
                                                 elitedangerousBinding.KeyEnumerationCode, //EliteDangerousKeyCode
                                                 elitedangerousBinding.KeyID, //EliteDangerousKeyId
                                                 elitedangerousBinding.ModifierKeyGameValue, //EliteDangerousModifierKeyValue
                                                 elitedangerousBinding.ModifierKeyEnumerationCode, //EliteDangerousModifierKeyCode
                                                 elitedangerousBinding.ModifierKeyID, //EliteDangerousModifierKeyId
                                                 ////--------------------------------------------------------------------------
                                                 voiceattackBinding.Internal, //VoiceAttackInternal
                                                 voiceattackBinding.FilePath, //VoiceAttackProfile
                                                 elitedangerousBinding.Internal, //EliteDangerousInternal
                                                 elitedangerousBinding.FilePath //EliteDangerousFilePath
                                                 ////--------------------------------------------------------------------------
                                                }, 
                                                false);
                }

                // If not defined or binding space unavailable in Elite Dangerous binding file ..
                if (!commandDefinedInEliteDangerousBindsFile)
                {
                    // Append to DataTable
                    updateRequirementStatus = EDVArd.KeyUpdateRequired.YES_VoiceAttack_TO_Elite.ToString();
                    rationale = string.Format("ED[{0}] not bound/bindable to a key", voiceattackBinding.EliteDangerousAction);
                    adjustedVoiceAttackCommands.LoadDataRow(new object[] 
                                                {
                                                 ////--------------------------------------------------------------------------
                                                 voiceattackBinding.KeyEnumeration, //KeyEnumeration
                                                 ////--------------------------------------------------------------------------
                                                 updateRequirementStatus, //ReMapRequired
                                                 rationale, //Rationale
                                                 ////--------------------------------------------------------------------------
                                                 voiceattackBinding.Action, //VoiceAttackAction
                                                 voiceattackBinding.EliteDangerousAction, //EliteDangerousAction
                                                 ////--------------------------------------------------------------------------
                                                 voiceattackBinding.KeyValue, //VoiceAttackKeyValue
                                                 voiceattackBinding.KeyCode, //VoiceAttackKeyCode
                                                 voiceattackBinding.KeyID, //VoiceAttackKeyId
                                                 voiceattackBinding.ModifierKeyGameValue, //VoiceAttackModifierKeyValue
                                                 voiceattackBinding.ModifierKeyEnumerationCode, //VoiceAttackModifierKeyCode
                                                 voiceattackBinding.ModifierKeyID, //VoiceAttackModifierKeyId
                                                 ////--------------------------------------------------------------------------
                                                 StatusCode.NotApplicable, //EliteDangerousDevicePriority                 
                                                 StatusCode.NotApplicable, //EliteDangerousKeyValue
                                                 StatusCode.NotApplicableInt.ToString(), //EliteDangerousKeyCode
                                                 StatusCode.NotApplicable, //EliteDangerousKeyId
                                                 StatusCode.NotApplicable, //EliteDangerousModifierKeyValue
                                                 StatusCode.NotApplicableInt.ToString(), //EliteDangerousModifierKeyCode
                                                 StatusCode.NotApplicable, //EliteDangerousModifierKeyId
                                                 ////--------------------------------------------------------------------------                                               
                                                 voiceattackBinding.Internal, //VoiceAttackInternal
                                                 voiceattackBinding.FilePath, //VoiceAttackProfile
                                                 globalEliteDangerousBindsInternal, //EliteDangerousInternal
                                                 eliteDangerousBinds //EliteDangerousFilePath
                                                 ////--------------------------------------------------------------------------
                                                },
                                                false);
                }
            }

            return adjustedVoiceAttackCommands;
        }

        /// <summary>
        /// Search for undefined actions in Elite Dangerous Bindings that have been defined in VoiceAttack ..
        /// </summary>
        /// <param name="eliteDangerousBinds"></param>
        /// <param name="voiceAttackProfile"></param>
        /// <param name="keyLookup"></param>
        /// <returns></returns>
        public static DataTable EliteDangerous(string eliteDangerousBinds, string voiceAttackProfile, GameKeyAndCommandBindingsAdapter keyLookup)
        {
            // Datatable to hold tabulated contents ..
            DataTable reverseBindableActions = TableShape.ReverseBindableVacantEliteDangerousActions();

            // Analyse binding differences ..
            var analysedGameActions = KeyBindingAnalyser.VoiceAttack(eliteDangerousBinds, voiceAttackProfile, keyLookup);

            // Initialise lookup dictionary for game binding to action references ..
            var keys = new GameAndSystemKeyAdapter(KeyEnum.Type.WindowsForms);

            // Find defined Voice Attack commands with potential for update in Elite Dangerous binds ..
            var vacantEliteDangerousBindings = from vac in analysedGameActions.AsEnumerable()
                                              where vac.Field<string>(EDVArd.Column.KeyUpdateRequired.ToString()) == EDVArd.KeyUpdateRequired.YES_VoiceAttack_TO_Elite.ToString()
                                             select
                                                new
                                                 {
                                                     KeyEnumeration = vac.Field<string>(EDVArd.Column.KeyEnumeration.ToString()),
                                                     EliteDangerousAction = vac.Field<string>(EDVArd.Column.EliteDangerousAction.ToString()),
                                                     VoiceAttackAction = vac.Field<string>(EDVArd.Column.VoiceAttackAction.ToString()),
                                                     VoiceAttackKeyValue = vac.Field<string>(EDVArd.Column.VoiceAttackKeyValue.ToString()),
                                                     VoiceAttackKeyCode = vac.Field<string>(EDVArd.Column.VoiceAttackKeyCode.ToString()),
                                                     VoiceAttackModifierKeyValue = vac.Field<string>(EDVArd.Column.VoiceAttackModifierKeyValue.ToString()),
                                                     VoiceAttackModifierKeyCode = vac.Field<string>(EDVArd.Column.VoiceAttackModifierKeyCode.ToString()),
                                                     VoiceAttackInternal = vac.Field<string>(EDVArd.Column.VoiceAttackInternal.ToString()),
                                                     VoiceAttackProfile = vac.Field<string>(EDVArd.Column.VoiceAttackProfile.ToString()),
                                                     EliteDangerousInternal = vac.Field<string>(EDVArd.Column.EliteDangerousInternal.ToString()),
                                                     EliteDangerousBinds = vac.Field<string>(EDVArd.Column.EliteDangerousBinds.ToString())
                                                 };

            foreach (var vacantEliteDangerousBinding in vacantEliteDangerousBindings)
            {
                reverseBindableActions.LoadDataRow(new object[] 
                                                {
                                                 ////--------------------------------------------------------------------------
                                                 vacantEliteDangerousBinding.KeyEnumeration, //KeyEnumeration
                                                 vacantEliteDangerousBinding.EliteDangerousAction, //EliteDangerousAction
                                                 vacantEliteDangerousBinding.VoiceAttackAction, //VoiceAttackAction
                                                 ////--------------------------------------------------------------------------
                                                 vacantEliteDangerousBinding.VoiceAttackKeyValue, //VoiceAttackKeyValue
                                                 vacantEliteDangerousBinding.VoiceAttackKeyCode, //VoiceAttackKeyCode
                                                 keys.GetEliteDangerousKeyBinding(int.Parse(vacantEliteDangerousBinding.VoiceAttackKeyCode)), //EliteDangerousKeyValue
                                                 vacantEliteDangerousBinding.VoiceAttackModifierKeyValue, //VoiceAttackModifierKeyValue
                                                 vacantEliteDangerousBinding.VoiceAttackModifierKeyCode, //VoiceAttackModifierKeyCode
                                                 keys.GetEliteDangerousKeyBinding(int.Parse(vacantEliteDangerousBinding.VoiceAttackModifierKeyCode)), //EliteDangerousModifierKeyValue
                                                 ////--------------------------------------------------------------------------
                                                 vacantEliteDangerousBinding.VoiceAttackInternal, //VoiceAttackInternal
                                                 vacantEliteDangerousBinding.VoiceAttackProfile, //VoiceAttackProfile
                                                 vacantEliteDangerousBinding.EliteDangerousInternal, //EliteDangerousInternal
                                                 vacantEliteDangerousBinding.EliteDangerousBinds //EliteDangerousBinds
                                                },
                                                false);
            }

            return reverseBindableActions;
        }
    }
}