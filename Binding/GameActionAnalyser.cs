namespace Binding
{
    using System;
    using System.Data;
    using Helper;
    using KeyHelper;
    using System.Linq;

    /// <summary>
    /// Analyse and Consolidate Game Action Bindings between VoiceAttack and Elite Dangerous ..
    /// </summary>
    public static class GameActionAnalyser
    {
        /// <summary>
        /// Match Command Key Codes in VoiceAttack based on Elite Dangerous Command Binds as Master ..
        /// </summary>
        /// <param name="eliteDangerousBinds"></param>
        /// <param name="voiceAttackProfile"></param>
        /// <param name="actionExchange"></param>
        /// <returns></returns>
        public static DataTable VoiceAttack(string eliteDangerousBinds, string voiceAttackProfile, GameActionExchanger actionExchange)
        {
            // Datatable to hold tabulated contents ..
            DataTable consolidatedaction = TableShape.ConsolidatedActions();
            string globalEliteDangerousBindsInternal = string.Empty;

            // Read bindings ..
            var eliteDangerous = new KeyReaderEliteDangerous(eliteDangerousBinds).GetBoundCommands();
            var voiceAttack = new KeyReaderVoiceAttack(voiceAttackProfile).GetBoundCommands();

            // Search through all defined Voice Attack bindings ..
            var voiceattackBindings = from va in voiceAttack.AsEnumerable()
                                    select
                                       new
                                         {
                                            KeyEnumeration = va.Field<string>(Helper.Enums.Column.KeyEnumeration.ToString()),
                                            EliteDangerousAction = actionExchange.GetED(va.Field<string>(Helper.Enums.Column.KeyAction.ToString())),
                                            Action = va.Field<string>(Helper.Enums.Column.KeyAction.ToString()),
                                            KeyValue = va.Field<string>(Helper.Enums.Column.KeyEnumerationValue.ToString()),
                                            KeyCode = va.Field<int>(Helper.Enums.Column.KeyEnumerationCode.ToString()),
                                            KeyID = va.Field<string>(Helper.Enums.Column.KeyId.ToString()),
                                            ModifierKeyGameValue = va.Field<string>(Helper.Enums.Column.ModifierKeyGameValue.ToString()),
                                            ModifierKeyEnumerationValue = va.Field<string>(Helper.Enums.Column.ModifierKeyEnumerationValue.ToString()),
                                            ModifierKeyEnumerationCode = va.Field<int>(Helper.Enums.Column.ModifierKeyEnumerationCode.ToString()),
                                            ModifierKeyID = va.Field<string>(Helper.Enums.Column.ModifierKeyId.ToString()),
                                            FilePath = va.Field<string>(Helper.Enums.Column.FilePath.ToString()),
                                            Internal = va.Field<string>(Helper.Enums.Column.Internal.ToString())
                                         };

            // .. and compare with what has been defined in the Elite Dangerous bindings ..
            foreach (var voiceattackBinding in voiceattackBindings)
            {
                bool commandDefinedInEliteDangerousBindsFile = false;
                string remapRequired = "unknown";
                string rationale = "unknown";

                var elitedangerousBindings = from ed in eliteDangerous.AsEnumerable()
                                             where ed.Field<string>(Helper.Enums.Column.KeyAction.ToString()) == voiceattackBinding.EliteDangerousAction
                                           select
                                              new
                                                {
                                                    Action = ed.Field<string>(Helper.Enums.Column.KeyAction.ToString()),
                                                    KeyPriority = ed.Field<string>(Helper.Enums.Column.DevicePriority.ToString()),
                                                    KeyGameValue = ed.Field<string>(Helper.Enums.Column.KeyGameValue.ToString()),
                                                    KeyEnumerationValue = ed.Field<string>(Helper.Enums.Column.KeyEnumerationValue.ToString()),
                                                    KeyEnumerationCode = ed.Field<int>(Helper.Enums.Column.KeyEnumerationCode.ToString()),
                                                    KeyID = ed.Field<string>(Helper.Enums.Column.KeyId.ToString()),
                                                    ModifierKeyGameValue = ed.Field<string>(Helper.Enums.Column.ModifierKeyGameValue.ToString()),
                                                    ModifierKeyEnumerationValue = ed.Field<string>(Helper.Enums.Column.ModifierKeyEnumerationValue.ToString()),
                                                    ModifierKeyEnumerationCode = ed.Field<int>(Helper.Enums.Column.ModifierKeyEnumerationCode.ToString()),
                                                    ModifierKeyID = ed.Field<string>(Helper.Enums.Column.ModifierKeyId.ToString()),
                                                    FilePath = ed.Field<string>(Helper.Enums.Column.FilePath.ToString()),
                                                    Internal = ed.Field<string>(Helper.Enums.Column.Internal.ToString())
                                                };

                // Compare matching action bindings with their assigned key value/code to evaluate which key code(s) require remapping ..
                foreach (var elitedangerousBinding in elitedangerousBindings)
                {
                    commandDefinedInEliteDangerousBindsFile = true;

                    // Assign for later use ..
                    globalEliteDangerousBindsInternal = elitedangerousBinding.Internal;

                    // Check for: satisfactory alignment of regular and modifier key codes ..
                    if (
                        //// Matching Regular Key Codes with no Modifier Key(s) present ..
                        ((elitedangerousBinding.KeyEnumerationCode == voiceattackBinding.KeyCode) &&
                        (elitedangerousBinding.ModifierKeyEnumerationCode == StatusCode.NotApplicableInt))     ||
                        
                        //// Matching Regular Key Codes with matching Modifier Key(s) present ..
                        ((elitedangerousBinding.KeyEnumerationCode == voiceattackBinding.KeyCode) &&
                        (elitedangerousBinding.ModifierKeyEnumerationCode == voiceattackBinding.ModifierKeyEnumerationCode)))
                    {
                        remapRequired = Helper.Enums.KeyUpdateRequired.NO.ToString();
                        rationale = "ED o--o VA Key Codes are aligned";
                    }
                    else
                    {
                        rationale = string.Empty;

                        // Check for misaligned codes ..
                        if (elitedangerousBinding.KeyEnumerationCode > StatusCode.EmptyStringInt)
                        {
                            remapRequired = Helper.Enums.KeyUpdateRequired.YES_Elite_TO_VoiceAttack.ToString();

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
                            remapRequired = Helper.Enums.KeyUpdateRequired.NO.ToString();
                            rationale += string.Format("Unresolvable key code for: ED[{0}];", elitedangerousBinding.KeyGameValue);
                        }

                        // Check for: unresolvable modifier key codes ..
                        if (elitedangerousBinding.ModifierKeyEnumerationCode == StatusCode.NoEquivalentKeyFoundAtExchange || elitedangerousBinding.ModifierKeyEnumerationCode == StatusCode.NoCodeFoundAfterExchange)
                        {
                            remapRequired = Helper.Enums.KeyUpdateRequired.NO.ToString();
                            rationale += string.Format("Unresolvable modifier key code for: ED[{0}];", elitedangerousBinding.ModifierKeyGameValue);
                        }
                    }

                    // Append evaluated results to DataTable ..
                    consolidatedaction.LoadDataRow(new object[] 
                                                {
                                                 ////--------------------------------------------------------------------------
                                                 voiceattackBinding.KeyEnumeration, //KeyEnumeration
                                                 ////--------------------------------------------------------------------------
                                                 remapRequired, //ReMapRequired
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
                    remapRequired = Helper.Enums.KeyUpdateRequired.YES_VoiceAttack_TO_Elite.ToString();
                    rationale = string.Format("ED[{0}] not bound/bindable to a Key", voiceattackBinding.EliteDangerousAction);
                    consolidatedaction.LoadDataRow(new object[] 
                                                {
                                                 ////--------------------------------------------------------------------------
                                                 voiceattackBinding.KeyEnumeration, //KeyEnumeration
                                                 ////--------------------------------------------------------------------------
                                                 remapRequired, //ReMapRequired
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

            return consolidatedaction;
        }

        /// <summary>
        /// Analyse vacant Game Action Bindings between Elite Dangerous and VoiceAttack ..
        /// </summary>
        /// <param name="eliteDangerousBinds"></param>
        /// <param name="voiceAttackProfile"></param>
        /// <param name="actionExchange"></param>
        /// <returns></returns>
        public static DataTable EliteDangerous(string eliteDangerousBinds, string voiceAttackProfile, GameActionExchanger actionExchange)
        {
            // Datatable to hold tabulated contents ..
            DataTable reversebindableaction = TableShape.ReverseBindableVacantEDActions();

            // Analyse binding differences ..
            var consolidatedaction = GameActionAnalyser.VoiceAttack(eliteDangerousBinds, voiceAttackProfile, actionExchange);

            // Initialise lookup dictionary for inter-game action references ..
            Mapper keyMapper = new Mapper(KeyHelper.Enums.InputKeyEnumType.WindowsForms);

            // Find defined Voice Attack commands where there is potential for update in EliteDangerous binds ..
            var vacantEliteDangerousBindings = from vac in consolidatedaction.AsEnumerable()
                                               where vac.Field<string>(Helper.Enums.Column.KeyUpdateRequired.ToString()) == Helper.Enums.KeyUpdateRequired.YES_VoiceAttack_TO_Elite.ToString()
                                              select
                                                 new
                                                 {
                                                     KeyEnumeration = vac.Field<string>(Helper.Enums.Column.KeyEnumeration.ToString()),
                                                     EliteDangerousAction = vac.Field<string>(Helper.Enums.Column.EliteDangerousAction.ToString()),
                                                     VoiceAttackAction = vac.Field<string>(Helper.Enums.Column.VoiceAttackAction.ToString()),
                                                     VoiceAttackKeyValue = vac.Field<string>(Helper.Enums.Column.VoiceAttackKeyValue.ToString()),
                                                     VoiceAttackKeyCode = vac.Field<string>(Helper.Enums.Column.VoiceAttackKeyCode.ToString()),
                                                     VoiceAttackModifierKeyValue = vac.Field<string>(Helper.Enums.Column.VoiceAttackModifierKeyValue.ToString()),
                                                     VoiceAttackModifierKeyCode = vac.Field<string>(Helper.Enums.Column.VoiceAttackModifierKeyCode.ToString()),
                                                     VoiceAttackInternal = vac.Field<string>(Helper.Enums.Column.VoiceAttackInternal.ToString()),
                                                     VoiceAttackProfile = vac.Field<string>(Helper.Enums.Column.VoiceAttackProfile.ToString()),
                                                     EliteDangerousInternal = vac.Field<string>(Helper.Enums.Column.EliteDangerousInternal.ToString()),
                                                     EliteDangerousBinds = vac.Field<string>(Helper.Enums.Column.EliteDangerousBinds.ToString())
                                                 };

            foreach (var veb in vacantEliteDangerousBindings)
            {
                reversebindableaction.LoadDataRow(new object[] 
                                                {
                                                 ////--------------------------------------------------------------------------
                                                 veb.KeyEnumeration, //KeyEnumeration
                                                 veb.EliteDangerousAction, //EliteDangerousAction
                                                 veb.VoiceAttackAction, //VoiceAttackAction
                                                 ////--------------------------------------------------------------------------
                                                 veb.VoiceAttackKeyValue, //VoiceAttackKeyValue
                                                 veb.VoiceAttackKeyCode, //VoiceAttackKeyCode
                                                 keyMapper.GetEDBindingValue(int.Parse(veb.VoiceAttackKeyCode)), //EliteDangerousKeyValue
                                                 veb.VoiceAttackModifierKeyValue, //VoiceAttackModifierKeyValue
                                                 veb.VoiceAttackModifierKeyCode, //VoiceAttackModifierKeyCode
                                                 keyMapper.GetEDBindingValue(int.Parse(veb.VoiceAttackModifierKeyCode)), //EliteDangerousModifierKeyValue
                                                 ////--------------------------------------------------------------------------
                                                 veb.VoiceAttackInternal, //VoiceAttackInternal
                                                 veb.VoiceAttackProfile, //VoiceAttackProfile
                                                 veb.EliteDangerousInternal, //EliteDangerousInternal
                                                 veb.EliteDangerousBinds //EliteDangerousBinds
                                                },
                                                false);
            }

            return reversebindableaction;
        }
    }
}