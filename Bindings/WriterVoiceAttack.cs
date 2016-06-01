namespace Bindings
{
    using Helpers;
    using System.Data;
    using System.Xml.XPath;

    /// <summary>
    /// Voice Attack Profile update
    /// </summary>
    public static partial class Writer
    {
        // Initialise ..
        private const string XMLCommand = "Command";
        private const string XMLActionSequence = "ActionSequence";
        private const string XMLCommandAction = "CommandAction";
        private const string XMLActionId = "Id";
        private const string XMLKeyCodes = "KeyCodes";
        private const string XMLunsignedShort = "unsignedShort";
        
        /// <summary>
        /// Update Voice Attack Profile
        /// </summary>
        /// <param name="consolidatedkeybindings"></param>
        public static void UpdateVoiceAttackProfile(DataTable consolidatedkeybindings)
        {
            var consolidatedBindings = from cb in consolidatedkeybindings.AsEnumerable()
                                      where cb.Field<string>(Enums.Column.ReMapRequired.ToString()) == Enums.ReMapRequired.YES.ToString()
                                     select
                                        new
                                            {
                                                VAAction = cb.Field<string>(Enums.Column.VoiceAttackAction.ToString()),
                                                VAKeyCode = cb.Field<string>(Enums.Column.VoiceAttackKeyCode.ToString()),
                                                EDKeyCode = cb.Field<string>(Enums.Column.EliteDangerousKeyCode.ToString()),
                                                VAKeyId = cb.Field<string>(Enums.Column.VoiceAttackKeyId.ToString()),
                                                VAP = cb.Field<string>(Enums.Column.VoiceAttackProfile.ToString())
                                            };

            foreach (var consolidatedBinding in consolidatedBindings)
            {
                UpdateVoiceAttackKeyCode(consolidatedBinding.VAP, consolidatedBinding.VAKeyId.Trim(), consolidatedBinding.EDKeyCode);
            }
        }

        /// <summary>
        /// Update Key Code associated to specific Id in Voice Attack
        /// </summary>
        /// <param name="vaprofile"></param>
        /// <param name="vakeyId"></param>
        /// <param name="keyCode"></param>
        private static void UpdateVoiceAttackKeyCode(string vaprofile, string vakeyId, string keyCode)
        {
            // Read Voice Attack Profile ...
            var vap = Xml.ReadXDoc(vaprofile);

            // Construct XPathSelect to locate the element for update ..
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
