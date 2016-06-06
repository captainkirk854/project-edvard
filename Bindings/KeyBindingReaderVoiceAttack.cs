namespace Bindings
{
    using Helpers;
    using System.Data;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Parse HCSVoicePacks Voice Attack Profile File
    /// </summary>
    public class KeyBindingReaderVoiceAttack : KeyBindingReader, IKeyBindingReader
    {
        // Initialise ..
        private const string XMLRoot = "Profile";
        private const string XMLName = "Name";
        private const string XMLCommand = "Command";
        private const string XMLCommandString = "CommandString";
        private const string XMLCategory = "Category";
        private const string XMLActionSequence = "ActionSequence";
        private const string XMLCommandAction = "CommandAction";
        private const string XMLActionType = "ActionType";
        private const string XMLActionId = "Id";
        private const string XMLKeyCodes = "KeyCodes";
        private const string XMLunsignedShort = "unsignedShort";
        private const string KeybindingCategoryHCSVoicePack = "Keybindings";
        
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyBindingReaderVoiceAttack" /> class.
        /// Base class constructor loads config.file as XDocument (this.xCfg)
        /// </summary>
        /// <param name="cfgFilePath"></param>
        public KeyBindingReaderVoiceAttack(string cfgFilePath) : base(cfgFilePath)
        {
        }
   
        /// <summary>
        /// Read all Voice Attack Commands mapped to Elite Dangerous Key-Bindable Actions into DataTable
        /// </summary>
        /// <returns></returns>
        public DataTable GetBindableCommands()
        {
            // Read bindings and tabulate ..
            DataTable primary = this.GetBindableActions(ref this.xCfg);

            // Add column ..
            primary.AddDefaultColumn(Enums.Column.Internal.ToString(), this.GetInternalReference(ref this.xCfg));

            // Add column ..
            primary.AddDefaultColumn(Enums.Column.FilePath.ToString(), this.cfgFilePath);

            // return Datatable ..
            return primary;
        }

        /// <summary>
        /// Read Voice Attack Key Bindings into DataTable
        /// </summary>
        /// <returns></returns>
        public DataTable GetBoundCommands()
        {
            // Read bindings and tabulate ..
            DataTable primary = this.GetKeyBindings(ref this.xCfg);

            // Add column ..
            primary.AddDefaultColumn(Enums.Column.Internal.ToString(), this.GetInternalReference(ref this.xCfg));

            // Add column ..
            primary.AddDefaultColumn(Enums.Column.FilePath.ToString(), this.cfgFilePath);

            // return Datatable ..
            return primary;
        }

        /// <summary>
        /// Process Voice Attack Config File to return all possible bindable actions
        /// </summary>
        /// <param name="xdoc"></param>
        /// <returns></returns>
        private DataTable GetBindableActions(ref XDocument xdoc)
        {
            // Datatable to hold tabulated XML contents ..
            DataTable bindableactions = TableShape.BindableActions();

            // traverse config XML and gather pertinent element data arranged in row(s) of anonymous types ..
            var xmlExtracts = from item in xdoc.Descendants(XMLCommand)
                              where
                                    item.Element(XMLCategory).Value == KeybindingCategoryHCSVoicePack &&
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

        /// <summary>
        /// Process Voice Attack Profile looking for Elite Dangerous keyboard-specific bindings as defined by HCSVoicePacks
        /// </summary>
        /// <remarks>
        ///   Format: XML
        ///             o <Profile/>
        ///               |_ <Commands/>
        ///                  |_ <Command/>
        ///                      |_<Id/>
        ///                      !_<CommandString/> = ((<action name/>))
        ///                      |_<ActionSequence/>
        ///                        !_[some] <CommandAction/>
        ///                                 !_<Id/>
        ///                                 |_<ActionType/> = PressKey
        ///                                 |_<KeyCodes/>
        ///                                   (|_<unsignedShort/> = when modifier present)
        ///                                    |_<unsignedShort/>
        ///                      !_<Category/> = Keybindings
        ///                             
        /// Keys Bindings: 
        ///                VA uses actual key codes (as opposed to key value). 
        ///                Actions directly mappable to Elite Dangerous have been defined by HCSVoicePacks using: 
        ///                 o Command.Category = Keybindings
        ///                 o Command.CommandString values which are pre- and post-fixed using '((' and '))'
        ///                   e.g. 
        ///                    ((Shield Cell)) : 222 (= Oem7 Numpad?7)
        ///                    ((Power To Weapons)) : 39  (= Right arrow)
        ///                    ((Select Target Ahead)) : 84 (= T)
        ///                    ((Flight Assist)) : 90 (= Z)
        ///                   
        ///                Note 
        ///                There are other commands that also use key codes which are part of the multi-command suite.
        ///                These are ignored
        /// </remarks>
        /// <param name="xdoc"></param>
        /// <returns></returns>
        private DataTable GetKeyBindings(ref XDocument xdoc)
        {
            // Datatable to hold tabulated XML contents ..
            DataTable keyactionbinder = TableShape.KeyActionBinder();

            // traverse config XML and gather pertinent element data arranged in row(s) of anonymous types ..
            var xmlExtracts = from item in xdoc.Descendants(XMLCommand)
                              where
                                    item.Element(XMLCategory).Value == KeybindingCategoryHCSVoicePack &&
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
                // Initialise ..
                string modifierKeyEnumerationValue = NA;
                int regularKeyCode = int.Parse(xmlExtract.KeyCode);

                // Check for modifier key already present in VoiceAttack Profile for current Key Id ..
                int modifierKeyCode = this.GetModifierKey(ref xdoc, xmlExtract.Id);
                if (modifierKeyCode >= 0)
                {
                    // If modifier found, some additional probing of that segment of the XML tree required ..
                    modifierKeyEnumerationValue = KeyMapper.GetValue(modifierKeyCode);
                    regularKeyCode = this.GetRegularKey(ref xdoc, xmlExtract.Id);
                }

                // Load final values into datatable ..
                keyactionbinder.LoadDataRow(new object[] 
                                                {
                                                 Enums.Game.VoiceAttack.ToString(), //Context
                                                 KeyMapper.KeyType.ToString(), //KeyEnumerationType
                                                 xmlExtract.Commandstring, //BindingAction
                                                 NA, //Priority
                                                 KeyMapper.GetValue(regularKeyCode), //KeyGameValue
                                                 KeyMapper.GetValue(regularKeyCode), //KeyEnumerationValue
                                                 regularKeyCode.ToString(), //KeyEnumerationCode
                                                 xmlExtract.Id, //KeyId
                                                 modifierKeyEnumerationValue, //ModifierKeyGameValue
                                                 modifierKeyEnumerationValue, //ModifierKeyEnumerationValue
                                                 modifierKeyCode, //ModifierKeyEnumerationCode
                                                 NA //ModifierId
                                                }, 
                                                false);
            }

            // return Datatable ..
            return keyactionbinder;
        }

        /// <summary>
        /// Process Voice Attack Config File looking for internal reference
        ///   Format: XML
        ///             o <Profile/>
        ///               |_ <Name>       
        /// </summary>
        /// <param name="xdoc"></param>
        /// <returns></returns>
        private string GetInternalReference(ref XDocument xdoc)
        {
            return xdoc.Element(XMLRoot).Element(XMLName).SafeElementValue().Trim();
        }

        /// <summary>
        /// Check if Modifier Key Code is present and return it if it is ..
        /// </summary>
        /// <param name="xdoc"></param>
        /// <param name="keyId"></param>
        /// <returns></returns>
        private int GetModifierKey(ref XDocument xdoc, string keyId)
        {
            // Count number of unsigned short elements (KeyCode) exist per ActionId ...
            var keyCodes = xdoc.Descendants(XMLunsignedShort)
                                    .Where(item => item.Parent.Parent.Parent.Parent.Element(XMLCategory).Value == KeybindingCategoryHCSVoicePack &&
                                                   item.Parent.Parent.Element(XMLActionId).Value == keyId)
                                    .DescendantsAndSelf();

            var countOfKeyCode = keyCodes.Count();

            // Check to see if modifier already exists in VoiceAttack Profile ..
            if (countOfKeyCode > 1)
            {
                // First value is Modifier Key Code ..
                return int.Parse(keyCodes.FirstOrDefault().Value);
            }
            else
            {
                return KeyBindingReader.INA;
            }
        }

        /// <summary>
        /// Get regular (non-Modifier) Key Code when Modifier Key Code is present and return key code ..
        /// </summary>
        /// <param name="xdoc"></param>
        /// <param name="keyId"></param>
        /// <returns></returns>
        private int GetRegularKey(ref XDocument xdoc, string keyId)
        {
            // Count number of unsigned short elements (KeyCode) exist per ActionId ...
            var keyCodes = xdoc.Descendants(XMLunsignedShort)
                                    .Where(item => item.Parent.Parent.Parent.Parent.Element(XMLCategory).Value == KeybindingCategoryHCSVoicePack &&
                                                   item.Parent.Parent.Element(XMLActionId).Value == keyId)
                                    .DescendantsAndSelf();

            var countOfKeyCode = keyCodes.Count();

            // Check to see if modifier already exists in VoiceAttack Profile ..
            if (countOfKeyCode > 1)
            {
                // Last value is Regular Key Code ..
                return int.Parse(keyCodes.LastOrDefault().Value);
            }
            else
            {
                return KeyBindingReader.INA;
            }
        }
    }
}