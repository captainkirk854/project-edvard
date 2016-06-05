namespace Bindings
{
    using System.Data;
    using System.Linq;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using Helpers;

    /// <summary>
    /// Update Voice Attack Profile Command(s) with new Key Codes
    /// </summary>
    public class KeyBindingUpdaterVoiceAttack : IKeyBindingUpdater
    {
        // Initialise ..
        private const string XMLCommand = "Command";
        private const string XMLActionSequence = "ActionSequence";
        private const string XMLCommandAction = "CommandAction";
        private const string XMLActionId = "Id";
        private const string XMLKeyCodes = "KeyCodes";
        private const string XMLunsignedShort = "unsignedShort";
        
        /// <summary>
        /// Update Voice Attack Profile with adjusted KeyCode(s) from Elite Dangerous Key Bindings
        /// </summary>
        /// <param name="consolidatedkeybindings"></param>
        /// <returns></returns>
        public bool Write(DataTable consolidatedkeybindings)
        {
            bool profileUpdated = false;

            // Find VoiceAttack commands which require remapping ..
            var consolidatedBindings = from cb in consolidatedkeybindings.AsEnumerable()
                                      where cb.Field<string>(Enums.Column.KeyUpdateRequired.ToString()) == Enums.KeyUpdateRequired.YES.ToString()
                                     select
                                        new
                                            {
                                                VAP = cb.Field<string>(Enums.Column.VoiceAttackProfile.ToString()),
                                                VAAction = cb.Field<string>(Enums.Column.VoiceAttackAction.ToString()),
                                                VAKeyId = cb.Field<string>(Enums.Column.VoiceAttackKeyId.ToString()),
                                                VAKeyCode = cb.Field<string>(Enums.Column.VoiceAttackKeyCode.ToString()),
                                                VAModifierKeyCode = cb.Field<string>(Enums.Column.VoiceAttackModifierKeyCode.ToString()),
                                                EDKeyCode = cb.Field<string>(Enums.Column.EliteDangerousKeyCode.ToString()),
                                                EDModifierKeyCode = cb.Field<string>(Enums.Column.EliteDangerousModifierKeyCode.ToString())
                                            };

            // Perform Update(s) for those commands that require it ..
            foreach (var consolidatedBinding in consolidatedBindings)
            {
                // Align key code with that used in Elite Dangerous ..
                this.UpdateVoiceAttackKeyCode(consolidatedBinding.VAP, consolidatedBinding.VAKeyId.Trim(), consolidatedBinding.EDKeyCode);

                // Align modifier key codes ..
                if (int.Parse(consolidatedBinding.EDModifierKeyCode) != int.Parse(consolidatedBinding.VAModifierKeyCode))
                {
                    // .. by creating additional element for modifier ..
                    if (int.Parse(consolidatedBinding.EDModifierKeyCode) > 0 && int.Parse(consolidatedBinding.VAModifierKeyCode) < 0)
                    {
                        this.InsertVoiceAttackModifierKeyCode(consolidatedBinding.VAP, consolidatedBinding.VAKeyId.Trim(), consolidatedBinding.EDModifierKeyCode);
                    }

                    // .. or updating the modifier key code that already exists ..
                    if (int.Parse(consolidatedBinding.EDModifierKeyCode) > 0 && int.Parse(consolidatedBinding.VAModifierKeyCode) > 0)
                    {
                        this.UpdateVoiceAttackKeyCode(consolidatedBinding.VAP, consolidatedBinding.VAKeyId.Trim(), consolidatedBinding.EDModifierKeyCode);
                    }
                }

                profileUpdated = true;
            }

            return profileUpdated;
        }

        /// <summary>
        /// Add KeyCode as <unsignedShort> for modifier associated to specific [Id] in Voice Attack
        /// </summary>
        /// <param name="vaprofile"></param>
        /// <param name="vakeyId"></param>
        /// <param name="keyCode"></param>
        private void InsertVoiceAttackModifierKeyCode(string vaprofile, string vakeyId, string keyCode)
        {
            var vap = Xml.ReadXDoc(vaprofile);

            // Search through all <unsignedShort> elements whose grandparent (Parent.Parent) <id> element equals vakeyId
            // and add a new <unsignedShort> element with keyCode value physically before existing one ..
            vap.Descendants(XMLunsignedShort)
               .Where(item => item.Parent.Parent.Element(XMLActionId).Value == vakeyId).FirstOrDefault()
               .AddBeforeSelf(new XElement(XMLunsignedShort, keyCode));

            vap.Save(vaprofile);
        }

        /// <summary>
        /// Update Key Code associated to specific [Id] in Voice Attack
        /// </summary>
        /// <remarks>
        ///   Format: XML
        ///             o <Profile/>
        ///               |_ <Commands/>
        ///                  |_ <Command/>
        ///                      |_<Id/>
        ///                      !_<CommandString/>
        ///                      |_<ActionSequence/>
        ///                        !_[some] <CommandAction/>
        ///                                 !_<Id/>
        ///                                 |_<ActionType/> = PressKey
        ///                                 |_<KeyCodes/>
        ///                                   (|_<unsignedShort/> = when modifier present)
        ///                                    |_<unsignedShort/>
        ///                      !_<Category/> = Keybindings
        /// </remarks>
        /// <param name="vaprofile"></param>
        /// <param name="vakeyId"></param>
        /// <param name="keyCode"></param>
        private void UpdateVoiceAttackKeyCode(string vaprofile, string vakeyId, string keyCode)
        {
            // Read Voice Attack Profile ...
            var vap = Xml.ReadXDoc(vaprofile);

            vap.Descendants(XMLunsignedShort)
               .Where(item => item.Parent.Parent.Element(XMLActionId).Value == vakeyId).FirstOrDefault()
               .SetValue(keyCode);

            // Save file ..
            vap.Save(vaprofile);
        }
    }
}
