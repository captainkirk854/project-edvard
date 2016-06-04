namespace Bindings
{
    using Helpers;
    using System.Data;
    using System.Xml.XPath;

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
                                                VAAction = cb.Field<string>(Enums.Column.VoiceAttackAction.ToString()),
                                                VAKeyCode = cb.Field<string>(Enums.Column.VoiceAttackKeyCode.ToString()),
                                                EDKeyCode = cb.Field<string>(Enums.Column.EliteDangerousKeyCode.ToString()),
                                                VAKeyId = cb.Field<string>(Enums.Column.VoiceAttackKeyId.ToString()),
                                                VAP = cb.Field<string>(Enums.Column.VoiceAttackProfile.ToString())
                                            };

            // Perform Update(s) for those commands that require it ..
            foreach (var consolidatedBinding in consolidatedBindings)
            {
                this.UpdateVoiceAttackKeyCode(consolidatedBinding.VAP, consolidatedBinding.VAKeyId.Trim(), consolidatedBinding.EDKeyCode);
                profileUpdated = true;
            }

            return profileUpdated;
        }

        /// <summary>
        /// Update Key Code associated to specific [Id] in Voice Attack
        /// </summary>
        /// <param name="vaprofile"></param>
        /// <param name="vakeyId"></param>
        /// <param name="keyCode"></param>
        private void UpdateVoiceAttackKeyCode(string vaprofile, string vakeyId, string keyCode)
        {
            // Read Voice Attack Profile ...
            var vap = Xml.ReadXDoc(vaprofile);

            // Construct XPathSelect to locate element (XMLunsignedShort) for update using XMLActionId ..
            string xPathSelect = "//" + XMLCommand +
                                  "/" + XMLActionSequence +
                                  "/" + XMLCommandAction +
                                  "/" + XMLActionId + "[text() = '" + vakeyId + "']" +
                                  "/.." +
                                  "/" + XMLKeyCodes +
                                  "/" + XMLunsignedShort;

            // Update element value ..
            vap.XPathSelectElement(xPathSelect).Value = keyCode;

            // Save file ..
            vap.Save(vaprofile);
        }
    }
}
