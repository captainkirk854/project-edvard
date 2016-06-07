namespace Binding
{
    using System.Data;
    using System.Linq;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using Helper;

    /// <summary>
    /// Update Voice Attack Profile Command(s) with new Key Codes
    /// </summary>
    public class KeyWriterVoiceAttack : IKeyWriter
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
        public bool Update(DataTable consolidatedkeybindings)
        {
            bool profileUpdated = false;

            // Find VoiceAttack commands which require remapping ..
            var consolidatedBindings = from cb in consolidatedkeybindings.AsEnumerable()
                                      where cb.Field<string>(Enums.Column.KeyUpdateRequired.ToString()) == Enums.KeyUpdateRequired.YES_ed_to_va.ToString()
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

            // Perform key code value update(s) for those commands that require it ..
            foreach (var consolidatedBinding in consolidatedBindings)
            {
                // Align key code in Voice Attack with that used in Elite Dangerous ..
                this.UpdateVoiceAttackKeyCode(consolidatedBinding.VAP, consolidatedBinding.VAKeyId.Trim(), consolidatedBinding.EDKeyCode);

                // Remove any other (modifier) key code(s) associated to the VA Key Id ..
                this.RemoveAnyOtherVoiceAttackKeyCode(consolidatedBinding.VAP, consolidatedBinding.VAKeyId.Trim(), consolidatedBinding.EDKeyCode);

                // Align modifier key code in VoiceAttack if there is a valid modifier key code from Elite Dangerous ..                  
                if (int.Parse(consolidatedBinding.EDModifierKeyCode) > 0)
                {
                    // .. by creating additional XElement to house modifier key code ..
                    if (int.Parse(consolidatedBinding.VAModifierKeyCode) < 0)
                    {
                        this.InsertVoiceAttackModifierKeyCode(consolidatedBinding.VAP, consolidatedBinding.VAKeyId.Trim(), consolidatedBinding.EDModifierKeyCode);
                    }
                }

                profileUpdated = true;
            }

            return profileUpdated;
        }

        /// <summary>
        /// Add KeyCode for modifier associated to specific [Id] in Voice Attack
        /// </summary>
        /// <remarks>
        /// Search for any <unsignedShort/> elements whose grandparent (Parent.Parent) <id> element equals vakeyId
        ///  and add a new <unsignedShort/> XElement with keyCode value physically before existing one ..
        /// </remarks>
        /// <param name="vaprofile"></param>
        /// <param name="vakeyId"></param>
        /// <param name="keyCode"></param>
        private void InsertVoiceAttackModifierKeyCode(string vaprofile, string vakeyId, string keyCode)
        {
            var vap = Xml.ReadXDoc(vaprofile);

            vap.Descendants(XMLunsignedShort)
               .Where(item => item.Parent.Parent.Element(XMLActionId).Value == vakeyId).FirstOrDefault()
               .AddBeforeSelf(new XElement(XMLunsignedShort, keyCode));

            vap.Save(vaprofile);
        }

        /// <summary>
        /// Remove KeyCode(s) that do not match KeyCode value where Id = specific [Id] in Voice Attack
        /// </summary>
        /// <remarks>
        /// Search for any <unsignedShort/> elements whose grandparent (Parent.Parent) <id> element equals vakeyId
        /// and remove any <unsignedShort/> XElement(s) that do not match keyCode value ..
        /// </remarks>
        /// <param name="vaprofile"></param>
        /// <param name="vakeyId"></param>
        /// <param name="keyCode"></param>
        private void RemoveAnyOtherVoiceAttackKeyCode(string vaprofile, string vakeyId, string keyCode)
        {
            var vap = Xml.ReadXDoc(vaprofile);

            vap.Descendants(XMLunsignedShort)
               .Where(item => item.Parent.Parent.Element(XMLActionId).Value == vakeyId && item.Value != keyCode)
               .Remove();

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
