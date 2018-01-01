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
        /// <param name="filepathEliteDangerousBinds"></param>
        /// <param name="filepathVoiceAttackProfile"></param>
        /// <param name="bindingsAdapter"></param>
        /// <returns></returns>
        public static DataTable VoiceAttack(string filepathEliteDangerousBinds, string filepathVoiceAttackProfile, GameKeyAndCommandBindingsAdapter bindingsAdapter)
        {
            // Datatable to hold tabulated contents ..
            DataTable adjustedVoiceAttackCommands = TableShape.ConsolidatedActions();
            string globalEliteDangerousBindsInternal = string.Empty;

            // Read bindings for each ..
            var boundCommandsEliteDangerous = new KeyBindingReaderEliteDangerous(filepathEliteDangerousBinds).GetBoundCommands();
            var boundCommandsVoiceAttack = new KeyBindingReaderVoiceAttack(filepathVoiceAttackProfile).GetBoundCommands();

            // Search through all defined Voice Attack bindings ..
            var bindingsVoiceAttack = from va in boundCommandsVoiceAttack.AsEnumerable()
                                    select
                                       new
                                         {
                                            KeyEnumeration = va.Field<string>(EDVArd.Column.KeyEnumeration.ToString()),
                                            EliteDangerousAction = bindingsAdapter.GetEliteDangerousBinding(va.Field<string>(EDVArd.Column.KeyAction.ToString())),
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
            foreach (var bindingVoiceAttack in bindingsVoiceAttack)
            {
                bool commandDefinedInEliteDangerousBindsFile = false;
                string updateRequirementStatus = "unknown";
                string rationale = "unknown";

                var bindingsEliteDangerous = from ed in boundCommandsEliteDangerous.AsEnumerable()
                                            where ed.Field<string>(EDVArd.Column.KeyAction.ToString()) == bindingVoiceAttack.EliteDangerousAction
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
                foreach (var bindingEliteDangerous in bindingsEliteDangerous)
                {
                    commandDefinedInEliteDangerousBindsFile = true;

                    // Assign for later use ..
                    globalEliteDangerousBindsInternal = bindingEliteDangerous.Internal;

                    // Check for: satisfactory alignment of regular and modifier key codes ..
                    if (
                        // Matching Regular Key Codes with no Modifier Key(s) present ..
                        ((bindingEliteDangerous.KeyEnumerationCode == bindingVoiceAttack.KeyCode) &&
                        (bindingEliteDangerous.ModifierKeyEnumerationCode == StatusCode.NotApplicableInt))     ||
                        
                        // Matching Regular Key Codes with matching Modifier Key(s) present ..
                        ((bindingEliteDangerous.KeyEnumerationCode == bindingVoiceAttack.KeyCode) &&
                        (bindingEliteDangerous.ModifierKeyEnumerationCode == bindingVoiceAttack.ModifierKeyEnumerationCode)))
                    {
                        updateRequirementStatus = EDVArd.KeyUpdateRequired.NO.ToString();
                        rationale = "ED o--o VA Key Codes are aligned";
                    }
                    else
                    {
                        rationale = string.Empty;

                        // Check for misaligned codes ..
                        if (bindingEliteDangerous.KeyEnumerationCode > StatusCode.EmptyStringInt)
                        {
                            updateRequirementStatus = EDVArd.KeyUpdateRequired.YES_Elite_TO_VoiceAttack.ToString();

                            // Check for: misaligned key codes ..
                            if (bindingEliteDangerous.KeyEnumerationCode != bindingVoiceAttack.KeyCode)
                            {
                                rationale += string.Format("Misaligned key codes: ED[{0}] o--O VA[{1}];", bindingEliteDangerous.KeyEnumerationCode, bindingVoiceAttack.KeyCode);
                            }

                            // Check for: misaligned modifier key codes ..
                            if (bindingEliteDangerous.ModifierKeyEnumerationCode != bindingVoiceAttack.ModifierKeyEnumerationCode)
                            {
                                if (bindingVoiceAttack.ModifierKeyEnumerationCode == StatusCode.EmptyStringInt)
                                {
                                    rationale += string.Format("Misaligned modifier key codes: ED[{0}] o--O VA[{1}];", bindingEliteDangerous.ModifierKeyEnumerationCode, "no-modifier defined");
                                }
                                else
                                {
                                    rationale += string.Format("Misaligned modifier key codes: ED[{0}] o--O VA[{1}];", bindingEliteDangerous.ModifierKeyEnumerationCode, bindingVoiceAttack.ModifierKeyEnumerationCode);
                                }
                            }
                        }

                        // Check for unresolvable key codes ..
                        if (bindingEliteDangerous.KeyEnumerationCode == StatusCode.NoEquivalentKeyFoundAtExchange || bindingEliteDangerous.KeyEnumerationCode == StatusCode.NoCodeFoundAfterExchange)
                        {
                            updateRequirementStatus = EDVArd.KeyUpdateRequired.NO.ToString();
                            rationale += string.Format("Unresolvable key code for: ED[{0}];", bindingEliteDangerous.KeyGameValue);
                        }

                        // Check for: unresolvable modifier key codes ..
                        if (bindingEliteDangerous.ModifierKeyEnumerationCode == StatusCode.NoEquivalentKeyFoundAtExchange || bindingEliteDangerous.ModifierKeyEnumerationCode == StatusCode.NoCodeFoundAfterExchange)
                        {
                            updateRequirementStatus = EDVArd.KeyUpdateRequired.NO.ToString();
                            rationale += string.Format("Unresolvable modifier key code for: ED[{0}];", bindingEliteDangerous.ModifierKeyGameValue);
                        }
                    }

                    // Append evaluated results to DataTable ..
                    adjustedVoiceAttackCommands.LoadDataRow(new object[] 
                                                {
                                                 ////--------------------------------------------------------------------------
                                                 bindingVoiceAttack.KeyEnumeration, //KeyEnumeration
                                                 ////--------------------------------------------------------------------------
                                                 updateRequirementStatus, //ReMapRequired
                                                 rationale, //Rationale
                                                 ////--------------------------------------------------------------------------
                                                 bindingVoiceAttack.Action, //VoiceAttackAction
                                                 bindingVoiceAttack.EliteDangerousAction, //EliteDangerousAction
                                                 ////--------------------------------------------------------------------------
                                                 bindingVoiceAttack.KeyValue, //VoiceAttackKeyValue
                                                 bindingVoiceAttack.KeyCode, //VoiceAttackKeyCode
                                                 bindingVoiceAttack.KeyID, //VoiceAttackKeyId
                                                 bindingVoiceAttack.ModifierKeyEnumerationValue, //VoiceAttackKeyEnumerationValue
                                                 bindingVoiceAttack.ModifierKeyEnumerationCode, //VoiceAttackModifierKeyCode
                                                 bindingVoiceAttack.ModifierKeyID, //VoiceAttackModifierKeyId
                                                 ////--------------------------------------------------------------------------                                                 
                                                 bindingEliteDangerous.KeyPriority, //EliteDangerousDevicePriority
                                                 bindingEliteDangerous.KeyGameValue, //EliteDangerousKeyValue
                                                 bindingEliteDangerous.KeyEnumerationCode, //EliteDangerousKeyCode
                                                 bindingEliteDangerous.KeyID, //EliteDangerousKeyId
                                                 bindingEliteDangerous.ModifierKeyGameValue, //EliteDangerousModifierKeyValue
                                                 bindingEliteDangerous.ModifierKeyEnumerationCode, //EliteDangerousModifierKeyCode
                                                 bindingEliteDangerous.ModifierKeyID, //EliteDangerousModifierKeyId
                                                 ////--------------------------------------------------------------------------
                                                 bindingVoiceAttack.Internal, //VoiceAttackInternal
                                                 bindingVoiceAttack.FilePath, //VoiceAttackProfile
                                                 bindingEliteDangerous.Internal, //EliteDangerousInternal
                                                 bindingEliteDangerous.FilePath //EliteDangerousFilePath
                                                 ////--------------------------------------------------------------------------
                                                }, 
                                                false);
                }

                // If not defined or binding space unavailable in Elite Dangerous binding file ..
                if (!commandDefinedInEliteDangerousBindsFile)
                {
                    // Append to DataTable
                    updateRequirementStatus = EDVArd.KeyUpdateRequired.YES_VoiceAttack_TO_Elite.ToString();
                    rationale = string.Format("ED[{0}] not bound/bindable to a key", bindingVoiceAttack.EliteDangerousAction);
                    adjustedVoiceAttackCommands.LoadDataRow(new object[] 
                                                {
                                                 ////--------------------------------------------------------------------------
                                                 bindingVoiceAttack.KeyEnumeration, //KeyEnumeration
                                                 ////--------------------------------------------------------------------------
                                                 updateRequirementStatus, //ReMapRequired
                                                 rationale, //Rationale
                                                 ////--------------------------------------------------------------------------
                                                 bindingVoiceAttack.Action, //VoiceAttackAction
                                                 bindingVoiceAttack.EliteDangerousAction, //EliteDangerousAction
                                                 ////--------------------------------------------------------------------------
                                                 bindingVoiceAttack.KeyValue, //VoiceAttackKeyValue
                                                 bindingVoiceAttack.KeyCode, //VoiceAttackKeyCode
                                                 bindingVoiceAttack.KeyID, //VoiceAttackKeyId
                                                 bindingVoiceAttack.ModifierKeyGameValue, //VoiceAttackModifierKeyValue
                                                 bindingVoiceAttack.ModifierKeyEnumerationCode, //VoiceAttackModifierKeyCode
                                                 bindingVoiceAttack.ModifierKeyID, //VoiceAttackModifierKeyId
                                                 ////--------------------------------------------------------------------------
                                                 StatusCode.NotApplicable, //EliteDangerousDevicePriority                 
                                                 StatusCode.NotApplicable, //EliteDangerousKeyValue
                                                 StatusCode.NotApplicableInt.ToString(), //EliteDangerousKeyCode
                                                 StatusCode.NotApplicable, //EliteDangerousKeyId
                                                 StatusCode.NotApplicable, //EliteDangerousModifierKeyValue
                                                 StatusCode.NotApplicableInt.ToString(), //EliteDangerousModifierKeyCode
                                                 StatusCode.NotApplicable, //EliteDangerousModifierKeyId
                                                 ////--------------------------------------------------------------------------                                               
                                                 bindingVoiceAttack.Internal, //VoiceAttackInternal
                                                 bindingVoiceAttack.FilePath, //VoiceAttackProfile
                                                 globalEliteDangerousBindsInternal, //EliteDangerousInternal
                                                 filepathEliteDangerousBinds //EliteDangerousFilePath
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
        /// <param name="filepathEliteDangerousBinds"></param>
        /// <param name="filepathVoiceAttackProfile"></param>
        /// <param name="bindingsAdapter"></param>
        /// <returns></returns>
        public static DataTable EliteDangerous(string filepathEliteDangerousBinds, string filepathVoiceAttackProfile, GameKeyAndCommandBindingsAdapter bindingsAdapter)
        {
            // Datatable to hold tabulated contents ..
            DataTable reverseBindableVacantActions = TableShape.ReverseBindableVacantEliteDangerousActions();

            // Analyse binding differences ..
            var analysedGameActions = KeyBindingAnalyser.VoiceAttack(filepathEliteDangerousBinds, filepathVoiceAttackProfile, bindingsAdapter);

            // Initialise lookup dictionary for game binding to action references ..
            var keys = new GameAndSystemKeyAdapter(KeyEnum.Type.WindowsForms);

            // Find defined Voice Attack commands with potential for update in Elite Dangerous binds ..
            var vacantBindingsEliteDangerous = from vac in analysedGameActions.AsEnumerable()
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

            foreach (var vacantBindingEliteDangerous in vacantBindingsEliteDangerous)
            {
                reverseBindableVacantActions.LoadDataRow(new object[] 
                                                {
                                                 ////--------------------------------------------------------------------------
                                                 vacantBindingEliteDangerous.KeyEnumeration, //KeyEnumeration
                                                 vacantBindingEliteDangerous.EliteDangerousAction, //EliteDangerousAction
                                                 vacantBindingEliteDangerous.VoiceAttackAction, //VoiceAttackAction
                                                 ////--------------------------------------------------------------------------
                                                 vacantBindingEliteDangerous.VoiceAttackKeyValue, //VoiceAttackKeyValue
                                                 vacantBindingEliteDangerous.VoiceAttackKeyCode, //VoiceAttackKeyCode
                                                 keys.GetEliteDangerousKeyBinding(int.Parse(vacantBindingEliteDangerous.VoiceAttackKeyCode)), //EliteDangerousKeyValue
                                                 vacantBindingEliteDangerous.VoiceAttackModifierKeyValue, //VoiceAttackModifierKeyValue
                                                 vacantBindingEliteDangerous.VoiceAttackModifierKeyCode, //VoiceAttackModifierKeyCode
                                                 keys.GetEliteDangerousKeyBinding(int.Parse(vacantBindingEliteDangerous.VoiceAttackModifierKeyCode)), //EliteDangerousModifierKeyValue
                                                 ////--------------------------------------------------------------------------
                                                 vacantBindingEliteDangerous.VoiceAttackInternal, //VoiceAttackInternal
                                                 vacantBindingEliteDangerous.VoiceAttackProfile, //VoiceAttackProfile
                                                 vacantBindingEliteDangerous.EliteDangerousInternal, //EliteDangerousInternal
                                                 vacantBindingEliteDangerous.EliteDangerousBinds //EliteDangerousBinds
                                                },
                                                false);
            }

            return reverseBindableVacantActions;
        }
    }
}