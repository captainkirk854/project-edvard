namespace Application
{
    using Helpers;
    using System.Data;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// BindingsReader content for parsing Voice Attack Profile file(s)
    /// </summary>
    public static partial class BindingsReader
    {
        // Initialise ..
        private const string XMLCommand = "Command";
        private const string XMLCommandString = "CommandString";
        private const string XMLActionSequence = "ActionSequence";
        private const string XMLCommandAction = "CommandAction";
        private const string XMLActionType = "ActionType";
        private const string XMLActionId = "Id";
        private const string XMLKeyCodes = "KeyCodes";
        private const string XMLunsignedShort = "unsignedShort";
        private static string[] keybindingIndicatorVA = { "((", "))" };

        /// <summary>
        /// Read Voice Attack Key Bindings into DataTable
        /// </summary>
        /// <param name="cfgFilePath"></param>
        /// <returns></returns>
        public static DataTable VoiceAttackKeyBindings(string cfgFilePath)
        {
            // Load configuration file as xml document object .. 
            var cfgVA = Xml.ReadXDoc(cfgFilePath);

            // Read bindings and tabulate ..
            DataTable primary = GetVAKeyBindings(cfgVA);

            // Modify ..
            primary.AddDefaultColumn(Enums.Column.FilePath.ToString(), cfgFilePath);

            // return Datatable ..
            return primary;
        }

        /// <summary>
        /// Read Voice Attack Binding Actions into DataTable
        /// </summary>
        /// <param name="cfgFilePath"></param>
        /// <returns></returns>
        public static DataTable VoiceAttackBindings(string cfgFilePath)
        {
            // Load configuration file as xml document object .. 
            var cfgVA = Xml.ReadXDoc(cfgFilePath);

            // Read bindings and tabulate ..
            DataTable primary = GetVABindingActions(cfgVA);

            // Modify ..
            primary.AddDefaultColumn(Enums.Column.FilePath.ToString(), cfgFilePath);

            // return Datatable ..
            return primary;
        }

        /// <summary>
        /// Process Voice Attack Config File looking for keyboard-specific bindings
        ///   Format: XML
        ///             o <Profile/>
        ///               |_ <Commands/>
        ///                  |_ <Command/>
        ///                      |_<Id/>
        ///                      !_<CommandString/>
        ///                      |_<ActionSequences/>
        ///                        !_[some] <CommandActions/>
        ///                                 |_<ActionType/>
        ///                                  |_<KeyCodes/>
        ///                             
        /// Keys Bindings: 
        ///                VA uses actual key codes (as opposed to key value). Actions directly mappable to Elite Dangerous
        ///                are defined by CommandString values which are pre- and post-fixed using '((' and '))'
        ///                e.g. 
        ///                   ((Shield Cell)) : 222 (= Oem7 Numpad?7)
        ///                   ((Power To Weapons)) : 39  (= Right arrow)
        ///                   ((Select Target Ahead)) : 84 (= T)
        ///                   ((Flight Assist)) : 90 (= Z)
        ///                   
        ///                Note 
        ///                There are other commands that also use key codes which are part of the multi-command suite.
        ///                These are currently ignored
        /// </summary>
        /// <param name="xdoc"></param>
        /// <returns></returns>
        private static DataTable GetVAKeyBindings(XDocument xdoc)
        {
            // Datatable to hold tabulated XML contents ..
            DataTable keyactionbinder = new DataTable();
            keyactionbinder.DefineKeyActionBinder();

            // traverse config XML and gather pertinent element data arranged in row(s) of anonymous types ..
            var xmlExtracts = from item in xdoc.Descendants(XMLCommand)
                              where item.Element(XMLCommandString).SafeElementValue().Contains(keybindingIndicatorVA[0]) &&
                                    item.Element(XMLCommandString).SafeElementValue().Contains(keybindingIndicatorVA[1]) &&
                                    item.Element(XMLActionSequence).Element(XMLCommandAction) != null &&
                                    item.Element(XMLActionSequence).Element(XMLCommandAction).Element(XMLActionType).Value == Enums.KeyboardInteraction.PressKey.ToString()
                              select
                                 new // create anonymous type for every key code ..
                                 {
                                     Commandstring = item.Element(XMLCommandString).SafeElementValue(),
                                     Id = item.Element(XMLActionSequence).Element(XMLCommandAction).Element(XMLActionId).SafeElementValue(),
                                     KeyCode = item.Element(XMLActionSequence).Element(XMLCommandAction).Element(XMLKeyCodes).Element(XMLunsignedShort).SafeElementValue()
                                 };

            // insert anonymous type row data (with some additional values) into DataTable ..
            foreach (var xmlExtract in xmlExtracts)
            {
                keyactionbinder.LoadDataRow(new object[] 
                                                {
                                                 Enums.Game.VoiceAttack.ToString(), //Context
                                                 KeyMapper.KeyType.ToString(), //KeyEnumerationType
                                                 xmlExtract.Commandstring, //BindingAction
                                                 NA, //Priority
                                                 NA, //KeyGameValue
                                                 KeyMapper.GetValue(int.Parse(xmlExtract.KeyCode)), //KeyEnumerationValue
                                                 xmlExtract.KeyCode, //KeyEnumerationCode
                                                 xmlExtract.Id, //KeyId
                                                 NA, //ModifierKeyGameValue
                                                 NA, //ModifierKeyEnumerationValue
                                                 INA, //ModifierKeyEnumerationCode
                                                 NA //ModifierId
                                                },
                                       false);
            }

            // return Datatable ..
            return keyactionbinder;
        }

        /// <summary>
        /// Process Voice Attack Config File to return all possible bindable actions
        /// </summary>
        /// <param name="xdoc"></param>
        /// <returns></returns>
        private static DataTable GetVABindingActions(XDocument xdoc)
        {
            // Datatable to hold tabulated XML contents ..
            DataTable bindableactions = new DataTable();
            bindableactions.DefineBindableActions();

            // traverse config XML and gather pertinent element data arranged in row(s) of anonymous types ..
            var xmlExtracts = from item in xdoc.Descendants(XMLCommand)
                              where item.Element(XMLCommandString).SafeElementValue().Contains(keybindingIndicatorVA[0]) &&
                                    item.Element(XMLCommandString).SafeElementValue().Contains(keybindingIndicatorVA[1]) &&
                                    item.Element(XMLActionSequence).Element(XMLCommandAction) != null &&
                                    item.Element(XMLActionSequence).Element(XMLCommandAction).Element(XMLActionType).Value == Enums.KeyboardInteraction.PressKey.ToString()
                              select
                                 new
                                 {
                                     Commandstring = item.Element(XMLCommandString).SafeElementValue()
                                 };

            // insert anonymous type row data (with some additional values) into DataTable ..
            foreach (var xmlExtract in xmlExtracts)
            {
                bindableactions.LoadDataRow(new object[] 
                                                {
                                                 Enums.Game.VoiceAttack.ToString(), //Context
                                                 xmlExtract.Commandstring, //BindingAction
                                                 NA, // Device priority
                                                 Enums.KeyboardInteraction.Keyboard.ToString() // Device binding applied to
                                                },
                                       false);
            }

            // return Datatable ..
            return bindableactions;
        }
    }
}
