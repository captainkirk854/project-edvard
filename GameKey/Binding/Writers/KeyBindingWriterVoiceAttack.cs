namespace GameKey.Binding.Writers
{
    using Items;
    using System.Data;
    using System.Linq;
    using System.Xml.Linq;
    using Utility;

    /// <summary>
    /// Update Voice Attack Profile Command(s) with new Key Codes
    /// </summary>
    public class KeyBindingWriterVoiceAttack : IKeyBindingWriter
    {
        // Initialise ..
        private const string XMLName = "Name";
        private const string XMLCommand = "Command";
        private const string XMLActionSequence = "ActionSequence";
        private const string XMLCommandAction = "CommandAction";
        private const string XMLActionId = "Id";
        private const string XMLKeyCodes = "KeyCodes";
        private const string XMLunsignedShort = "unsignedShort";
        
        /// <summary>
        /// Update Voice Attack Profile with adjusted KeyCode(s) from Elite Dangerous Key Bindings
        /// </summary>
        /// <param name="consolidatedActions"></param>
        /// <param name="updateChangeTag"></param>
        /// <returns></returns>
        public bool Update(DataTable consolidatedActions, bool updateChangeTag)
        {
            bool profileUpdated = false;
            string globalVoiceAttackProfileInternal = string.Empty;
            string globalVoiceAttackProfileFilePath = string.Empty;

            // Find VoiceAttack commands which require remapping ..
            var consolidatedBindings = from cb in consolidatedActions.AsEnumerable()
                                      where cb.Field<string>(EDVArd.Column.KeyUpdateRequired.ToString()) == EDVArd.KeyUpdateRequired.YES_Elite_TO_VoiceAttack.ToString()
                                     select
                                        new
                                            {
                                                VoiceAttackInternal = cb.Field<string>(EDVArd.Column.VoiceAttackInternal.ToString()),
                                                VoiceAttackProfile = cb.Field<string>(EDVArd.Column.VoiceAttackProfile.ToString()),
                                                VoiceAttackAction = cb.Field<string>(EDVArd.Column.VoiceAttackAction.ToString()),
                                                VoiceAttackKeyId = cb.Field<string>(EDVArd.Column.VoiceAttackKeyId.ToString()),
                                                VoiceAttackKeyCode = cb.Field<string>(EDVArd.Column.VoiceAttackKeyCode.ToString()),
                                                VoiceAttackModifierKeyCode = cb.Field<string>(EDVArd.Column.VoiceAttackModifierKeyCode.ToString()),
                                                EliteDangerousKeyCode = cb.Field<string>(EDVArd.Column.EliteDangerousKeyCode.ToString()),
                                                EliteDangerousModifierKeyCode = cb.Field<string>(EDVArd.Column.EliteDangerousModifierKeyCode.ToString())
                                            };

            // Perform key code value update(s) for those commands that require it ..
            foreach (var consolidatedBinding in consolidatedBindings)
            {
                // Align key code in Voice Attack with that used in Elite Dangerous ..
                this.UpdateVoiceAttackKeyCode(consolidatedBinding.VoiceAttackProfile,
                                              consolidatedBinding.VoiceAttackKeyId.Trim(), 
                                              consolidatedBinding.EliteDangerousKeyCode);

                // Remove any other (modifier) key code(s) associated to the VA Key Id ..
                this.RemoveAnyOtherVoiceAttackKeyCode(consolidatedBinding.VoiceAttackProfile, 
                                                      consolidatedBinding.VoiceAttackKeyId.Trim(), 
                                                      consolidatedBinding.EliteDangerousKeyCode);

                // Align modifier key code in VoiceAttack if there is a valid modifier key code from Elite Dangerous ..                  
                if (int.Parse(consolidatedBinding.EliteDangerousModifierKeyCode) > 0)
                {
                    // .. by creating additional XElement to house modifier key code ..
                    if (int.Parse(consolidatedBinding.VoiceAttackModifierKeyCode) < 0)
                    {
                        this.InsertVoiceAttackModifierKeyCode(consolidatedBinding.VoiceAttackProfile, 
                                                              consolidatedBinding.VoiceAttackKeyId.Trim(), 
                                                              consolidatedBinding.EliteDangerousModifierKeyCode);
                    }
                }

                globalVoiceAttackProfileInternal = consolidatedBinding.VoiceAttackInternal;
                globalVoiceAttackProfileFilePath = consolidatedBinding.VoiceAttackProfile;
                profileUpdated = true;
            }

            // Update internal reference ..
            if (profileUpdated && updateChangeTag)
            {
                this.UpdateVoiceAttackProfileName(globalVoiceAttackProfileFilePath, globalVoiceAttackProfileInternal, Tag.Make(globalVoiceAttackProfileInternal));
            }

            return profileUpdated;
        }

        /// <summary>
        /// Add KeyCode for modifier associated to specific [Id] in Voice Attack
        /// </summary>
        /// <remarks>
        /// Search for any <unsignedShort/> elements whose grandparent (Parent.Parent) <id> element equals vakeyId
        /// and add a new <unsignedShort/> XElement with keyCode value physically before existing one ..
        ///   Format: XML
        ///             o <Profile/>
        ///               |_ <Commands/>
        ///                  |_ <Command/>
        ///                      |_<Id/>
        ///                      !_<commandString/>
        ///                      |_<ActionSequence/>
        ///                        !_[some] <CommandAction/>
        ///                                 !_<Id/>
        ///                                 |_<ActionType/> = PressKey
        ///                                 |_<KeyCodes/>
        ///                                   (|_<unsignedShort/> = when modifier present)[*]
        ///                                    |_<unsignedShort/>
        ///                      !_<Category/> = Keybindings
        /// </remarks>
        /// <param name="vaprofile"></param>
        /// <param name="vakeyId"></param>
        /// <param name="keyCode"></param>
        private void InsertVoiceAttackModifierKeyCode(string profileFilepath, string vakeyId, string keyCode)
        {
            var vap = HandleXml.ReadXDoc(profileFilepath);

            // Insert XMLunsignedShort XElement before existing one ..
            vap.Descendants(XMLunsignedShort)
               .Where(item => item.Parent.Parent.Element(XMLActionId).Value == vakeyId).FirstOrDefault()
               .AddBeforeSelf(new XElement(XMLunsignedShort, keyCode));

            vap.Save(profileFilepath);
        }

        /// <summary>
        /// Remove KeyCode(s) that do not match KeyCode value where Id = specific [Id] in Voice Attack
        /// </summary>
        /// <remarks>
        /// Search for any <unsignedShort/> elements whose grandparent (Parent.Parent) <id> element equals vakeyId
        /// and remove any <unsignedShort/> XElement(s) that do not match keyCode value ..
        ///   Format: XML
        ///             o <Profile/>
        ///               |_ <Commands/>
        ///                  |_ <Command/>
        ///                      |_<Id/>
        ///                      !_<commandString/>
        ///                      |_<ActionSequence/>
        ///                        !_[some] <CommandAction/>
        ///                                 !_<Id/>
        ///                                 |_<ActionType/> = PressKey
        ///                                 |_<KeyCodes/>
        ///                                   (|_<unsignedShort/> = when modifier present)[*]
        ///                                    |_<unsignedShort/>[*]
        ///                      !_<Category/> = Keybindings
        /// </remarks>
        /// <param name="vaprofile"></param>
        /// <param name="vakeyId"></param>
        /// <param name="keyCode"></param>
        private void RemoveAnyOtherVoiceAttackKeyCode(string profileFilepath, string vakeyId, string keyCode)
        {
            var vap = HandleXml.ReadXDoc(profileFilepath);

            // Remove all XMLunsignedShort XElements ...
            vap.Descendants(XMLunsignedShort)
               .Where(item => item.Parent.Parent.Element(XMLActionId).Value == vakeyId && item.Value != keyCode)
               .Remove();

            vap.Save(profileFilepath);
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
        ///                      !_<commandString/>
        ///                      |_<ActionSequence/>
        ///                        !_[some] <CommandAction/>
        ///                                 !_<Id/>
        ///                                 |_<ActionType/> = PressKey
        ///                                 |_<KeyCodes/>
        ///                                   (|_<unsignedShort/> = when modifier present)[*]
        ///                                    |_<unsignedShort/>[*]
        ///                      !_<Category/> = Keybindings
        /// </remarks>
        /// <param name="profileFilepath"></param>
        /// <param name="vakeyId"></param>
        /// <param name="keyCode"></param>
        private void UpdateVoiceAttackKeyCode(string profileFilepath, string vakeyId, string keyCode)
        {
            var vap = HandleXml.ReadXDoc(profileFilepath);

            // Update XMLunsignedShort XElement ..
            vap.Descendants(XMLunsignedShort)
               .Where(item => item.Parent.Parent.Element(XMLActionId).Value == vakeyId).FirstOrDefault()
               .SetValue(keyCode);

            vap.Save(profileFilepath);
        }

        /// <summary>
        /// Update Voice Attack Profile Name
        /// </summary>
        /// <remarks>
        ///   Format: XML
        ///             o <Profile/>[*]
        ///               |_ <Name/>
        ///               |_ <Commands/>
        ///                  |_ <Command/>
        /// </remarks>
        /// <param name="profileFilepath"></param>
        /// <param name="profileName"></param>
        /// <param name="updatedProfileName"></param>
        private void UpdateVoiceAttackProfileName(string profileFilepath, string profileName, string updatedProfileName)
        {
            var vap = HandleXml.ReadXDoc(profileFilepath);

            // Update XMLunsignedShort XMLName ..
            vap.Descendants(XMLName)
               .Where(item => item.SafeElementValue() == profileName).FirstOrDefault()
               .SetValue(updatedProfileName);

            vap.Save(profileFilepath);
        }
    }
}
