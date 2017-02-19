namespace Binding
{
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Helper;
    using Items;

    /// <summary>
    /// Parse HCSVoicePacks Voice Attack Profile file
    /// </summary>
    public class KeyBindingReaderVoiceAttack : KeyBindingReader, IKeyBindingReader
    {
        // Initialise ..
        private const string XMLRoot = "Profile";
        private const string XMLName = "Name";
        private const string XMLCommands = "Commands";
        private const string XMLCommand = "Command";
        private const string XMLCommandId = "Id";
        private const string XMLCommandString = "CommandString";
        private const string XMLCategory = "Category";
        private const string XMLActionSequence = "ActionSequence";
        private const string XMLCommandAction = "CommandAction";
        private const string XMLActionType = "ActionType";
        private const string XMLCommandActionId = "Id";
        private const string XMLKeyCodes = "KeyCodes";
        private const string XMLContext = "Context";
        private const string XMLContext2 = "Context2";
        private const string XMLUnsignedShort = "unsignedShort";
        private const string XMLCategoryKeybindings = "Keybindings";
        
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyBindingReaderVoiceAttack" /> class.
        /// Base class constructor loads config.file as XDocument (this.xCfg)
        /// </summary>
        /// <param name="cfgFilePath"></param>
        public KeyBindingReaderVoiceAttack(string cfgFilePath) : base(cfgFilePath)
        {
        }
   
        /// <summary>
        /// Load Voice Attack Commands mapped to Elite Dangerous Key-Bindable Actions into DataTable
        /// </summary>
        /// <returns></returns>
        public DataTable GetBindableCommands()
        {
            // Read bindings and tabulate ..
            DataTable bindableCommands = this.GetCommandStringsWithKeyPressAction(ref this.xCfg);

            // modify table ..
            bindableCommands.AddDefaultColumn(Edvard.Column.Internal.ToString(), this.GetInternalReference(ref this.xCfg));
            bindableCommands.AddDefaultColumn(Edvard.Column.FilePath.ToString(), this.cfgFilePath);

            // return Datatable ..
            return bindableCommands;
        }

        /// <summary>
        /// Load Voice Attack Key Bindings into DataTable
        /// </summary>
        /// <returns></returns>
        public DataTable GetBoundCommands()
        {
            // Read bindings and tabulate ..
            DataTable boundCommands = this.GetKeyBindings(ref this.xCfg);

            // modify table ..
            boundCommands.AddDefaultColumn(Edvard.Column.Internal.ToString(), this.GetInternalReference(ref this.xCfg));
            boundCommands.AddDefaultColumn(Edvard.Column.FilePath.ToString(), this.cfgFilePath);

            // return Datatable ..
            return boundCommands;
        }

        /// <summary>
        /// Get CommandStrings related to key-bound Action Commands
        /// </summary>
        /// <param name="consolidatedBoundCommands"></param>
        /// <returns></returns>
        public DataTable GetRelatedCommandStrings(DataTable consolidatedBoundCommands)
        {
            // Initialise ..
            DataTable associatedCommands = TableShape.AssociatedCommands();

            // Loop through each key-bound Command String ..
            foreach (DataRow consolidatedBoundCommand in consolidatedBoundCommands.Select().Distinct().OrderBy(orderingColumn => orderingColumn[Edvard.Column.VoiceAttackAction.ToString()]))
            {
                // Get required field information ..
                string voiceAttackCommandString = consolidatedBoundCommand[Edvard.Column.VoiceAttackAction.ToString()].ToString();
                string voiceAttackActionId = consolidatedBoundCommand[Edvard.Column.VoiceAttackKeyId.ToString()].ToString();
                string eliteDangerousAction = consolidatedBoundCommand[Edvard.Column.EliteDangerousAction.ToString()].ToString();
                string bindingSyncStatus = consolidatedBoundCommand[Edvard.Column.KeyUpdateRequired.ToString()].ToString() == Edvard.KeyUpdateRequired.NO.ToString() ? "synchronised" : "*attention required*";
                string voiceAttackFile = Path.GetFileName(consolidatedBoundCommand[Edvard.Column.VoiceAttackProfile.ToString()].ToString());
                string eliteDangerousFile = Path.GetFileName(consolidatedBoundCommand[Edvard.Column.EliteDangerousBinds.ToString()].ToString());

                // Find associated Command String(s) using key-bound CommandString ActionId ...
                var associatedCommandStrings = this.GetCommandStringsFromCommandActionContext(ref this.xCfg, this.ParseVoiceAttackProfileForKeyBoundCommandIdsFromCommandActionId(ref this.xCfg, voiceAttackActionId).Distinct().FirstOrDefault().ToString());

                // Find associated Command String(s) using key-bound CommandString Value ...
                var associatedCommandStringsFromContext2 = this.GetCommandStringsFromCommandActionContext2(ref this.xCfg, voiceAttackCommandString);
                if (associatedCommandStrings.Count() > 0) { associatedCommandStrings.Concat(associatedCommandStringsFromContext2); }
                else { associatedCommandStrings = associatedCommandStringsFromContext2; }
                
                // Add to DataTable ..
                foreach (var associatedCommandString in associatedCommandStrings.Distinct())
                {
                    associatedCommands.LoadDataRow(new object[] 
                                                                {
                                                                    voiceAttackCommandString,
                                                                    eliteDangerousAction,
                                                                    associatedCommandString,
                                                                    bindingSyncStatus,
                                                                    voiceAttackFile,
                                                                    eliteDangerousFile
                                                                },
                                                    false);
                }
            }

            return associatedCommands;
        }

        /// <summary>
        /// Get commandStrings for all Categories and load into DataTable ..
        /// </summary>
        /// <returns></returns>
        public DataTable GetCommandStringsForAllCategories()
        {
            // Initialise ..
            DataTable allVoiceCommands = TableShape.AllVoiceCommands();

            // Get anonymous type row data (as object types) ..
            var commandStrings = this.GetCommandStringsForCommandCategory(ref this.xCfg, this.GetAllCommandCategories(ref this.xCfg));

            // insert object's row data into DataTable being able to access its two fields as the anonymous type commandString is declared as a 'dynamic' type ..
            foreach (dynamic commandString in commandStrings)
            {
                allVoiceCommands.LoadDataRow(new object[] 
                                                {
                                                    commandString.CommandCategory,
                                                    commandString.CommandString,
                                                    commandString.ActionType
                                                },
                                            false);
            }

            return allVoiceCommands;
        }

        /// <summary>
        /// Parse Voice Attack Config File to get all Command Strings with valid KeyPress-related Action Types
        /// </summary>
        /// <param name="xdoc"></param>
        /// <returns></returns>
        private DataTable GetCommandStringsWithKeyPressAction(ref XDocument xdoc)
        {
            // Initialise ..
            DataTable bindableActions = TableShape.BindableActions();

            // Use dynamic-magic to create a raw list of anonymous bindable-key object(s) ..
            IEnumerable<dynamic> xmlExtracts = this.ParseVoiceAttackProfileForKeyBindableCommands(ref xdoc);

            // insert anonymous type row data (with some additional values) into DataTable (.Distinct() required as some Commands have multiple (modifier) key codes)
            foreach (var xmlExtract in xmlExtracts.Distinct()) 
            {
                bindableActions.LoadDataRow(new object[] 
                                                {
                                                    Application.Name.VoiceAttack.ToString(), //Context
                                                    xmlExtract.CommandString, //KeyAction
                                                    xmlExtract.KeyActionType, //KeyActionType
                                                    StatusCode.NotApplicable, //Device Priority
                                                    StatusCode.NotApplicable //Device Type binding is applied to
                                                }, 
                                                false);
            }

            // return Datatable ..
            return bindableActions;
        }

        /// <summary>
        /// Parse Voice Attack Config File to get Command String(s) associated to Command Categories
        /// </summary>
        /// <remarks>
        ///   Format: XML
        ///             o <Profile/>
        ///               |_ <Commands/>
        ///                  |_ <Command/>
        ///                      !_<CommandString/>* = Spoken Command  <--------¬
        ///                      !_<Category/>* = <value/>
        ///                      |_<ActionSequence/> 
        ///                         !_[some] <CommandAction/>
        ///                                   |_<ActionType/>* = <value/>
        ///                  
        /// </remarks>
        /// <param name="xdoc"></param>
        /// <param name="commandCategories"></param>
        /// <returns></returns>
        private IEnumerable<object> GetCommandStringsForCommandCategory(ref XDocument xdoc, System.Collections.Generic.IEnumerable<string> commandCategories)
        {
            // Create list of required anonymous type .. 
            var xmlExtracts = new[] { new { CommandString = string.Empty, CommandCategory = string.Empty, ActionType = string.Empty } }.ToList();
            xmlExtracts.Clear(); // empty first row created whilst keeping anonymous type definition

            // Get Command String(s) for each Command Category ..
            foreach (var commandCategory in commandCategories)
            {
                // traverse config XML, return Command String(s) associated to each Category ..
                try
                {
                    var commandStrings = from item in xdoc.Descendants(XMLCategory)
                                         where
                                               item.Parent.Element(XMLCategory).SafeElementValue() == commandCategory
                                         select
                                            new // create anonymous type ..
                                            {
                                                CommandString = item.Parent.Element(XMLCommandString).SafeElementValue(),
                                                CommandCategory = item.Parent.Element(XMLCategory).SafeElementValue(),
                                                ActionType = item.Parent.Element(XMLActionSequence).Element(XMLCommandAction).Element(XMLActionType).SafeElementValue()
                                            };

                    // Add all resultant command strings to list ..
                    foreach (var commandString in commandStrings)
                    {
                        xmlExtracts.Add(commandString);
                    }
                }
                catch
                {
                }
            }

            return xmlExtracts;
        }

        /// <summary>
        /// Parse Voice Attack Config File to get a distinct list of Command Categories
        /// </summary>
        /// <remarks>
        ///   Format: XML
        ///             o <Profile/>
        ///               |_ <Commands/>
        ///                  |_ <Command/>
        ///                      !_<Category/>
        /// </remarks>
        /// <param name="xdoc"></param>
        /// <returns></returns>
        private IEnumerable<string> GetAllCommandCategories(ref XDocument xdoc)
        {
            var xmlExtracts = from item in xdoc.Descendants(XMLCategory)
                            select
                                   item.Parent.Element(XMLCategory).SafeElementValue();

            return xmlExtracts.Distinct();
        }

        /// <summary>
        /// Parse Voice Attack Config File to get Command String associated to an ActionId of a different Command
        /// </summary>
        /// <remarks>
        ///   Format: XML
        ///             o <Profile/>
        ///               |_ <Commands/>
        ///                  |_ <Command/>
        ///                      |_<Id/>
        ///                      !_<CommandString/>* = Associated Spoken Command <¬           
        ///                      |_<ActionSequence/>                              |
        ///                        !_[some] <CommandAction/>                      |
        ///                                 |_<Context/> --------------------------
        ///                      !_<Description/> = Command Description
        ///                      !_<Category/> != Keybindings
        /// </remarks>
        /// <param name="xdoc"></param>
        /// <param name="commandActionId"></param>
        /// <returns></returns>
        private IEnumerable<string> GetCommandStringsFromCommandActionContext(ref XDocument xdoc, string commandActionId)
        {
            // Find Command String(s) where descendant Context = Action Id ..
            var xmlExtracts = from item in xdoc.Descendants(XMLContext)
                             where
                                   !item.Parent.Parent.Parent.Element(XMLCategory).Value.Contains(XMLCategoryKeybindings) &&
                                   item.SafeElementValue() == commandActionId
                            select
                                   item.Parent.Parent.Parent.Element(XMLCommandString).SafeElementValue();

            return xmlExtracts;
        }

        /// <summary>
        /// Parse Voice Attack Config File to get Command String associated to key-bound Command String
        /// </summary>
        /// <remarks>
        ///   Format: XML
        ///             o <Profile/>
        ///               |_ <Commands/>
        ///                  |_ <Command/>
        ///                      |_<Id/>
        ///                      !_<CommandString/>* = Associated Spoken Command <¬           
        ///                      |_<ActionSequence/>                              |
        ///                        !_[some] <CommandAction/>                      |
        ///                                 |_<Context2/ xml:space=preserve>> -----
        ///                      !_<Description/> = Command Description
        ///                      !_<Category/> != Keybindings
        /// </remarks>
        /// <param name="xdoc"></param>
        /// <param name="keyBoundCommandString"></param>
        /// <returns></returns>
        private IEnumerable<string> GetCommandStringsFromCommandActionContext2(ref XDocument xdoc, string keyBoundCommandString)
        {
            // Find Command String(s) where descendant Context = Action Id ..
            var xmlExtracts = from item in xdoc.Descendants(XMLContext2)
                              where
                                    !item.Parent.Parent.Parent.Element(XMLCategory).Value.Contains(XMLCategoryKeybindings) &&
                                    item.SafeElementValue() == keyBoundCommandString
                              select
                                    item.Parent.Parent.Parent.Element(XMLCommandString).SafeElementValue();

            return xmlExtracts;
        }

        /// <summary>
        /// Parse Voice Attack Config File to get details of Commands and Command Actions with KeyPress Action Types already bound to a non-blank key code
        /// </summary>
        /// <remarks>
        /// Key Bindings are identified by max and min KeyCode values for every CommandString. 
        ///  > Max values are considered to be [modifier] key codes
        ///  > Min values are considered to be [regular] key codes
        /// </remarks>
        /// <param name="xdoc"></param>
        /// <returns></returns>
        private DataTable GetKeyBindings(ref XDocument xdoc)
        {
            // Use dynamic-magic to create a list of raw (with potential duplicates) anonymous bound-key object(s) ..
            IEnumerable<dynamic> rawKeyBoundCommands = this.ParseVoiceAttackProfileForKeyBoundCommands(ref xdoc);

            // Use Linq grouping-magic to remove duplicates and condense raw bound-key object list ..
            IEnumerable<dynamic> summarisedKeyBoundCommands = rawKeyBoundCommands.GroupBy(groupingField => new { groupingField.CommandString })
                                                                                 .Select(info => new
                                                                                            {
                                                                                                groupKey = info.Key,
                                                                                                commandString = info.Max(t => t.CommandString),
                                                                                                actionId = info.Max(t => t.CommandActionId),
                                                                                                actionType = info.Min(t => t.KeyActionType),
                                                                                                regular = info.Min(t => t.KeyCode),
                                                                                                modifier = info.Max(t => t.KeyCode)
                                                                                            });

          // Finalise information ...
          return this.Finalise(summarisedKeyBoundCommands);
        }

        /// <summary>
        /// Prepare KeyCodes and their conversions.
        /// </summary>
        /// <param name="keyBoundCommands"></param>
        /// <returns></returns>
        private DataTable Finalise(IEnumerable<dynamic> keyBoundCommands)
        {
            // Initialise ..
            DataTable boundKeyActionDefinitions = TableShape.KeyActionDefinition();

            // Extract condensed aggregation of KeyCodes ..
            foreach (var keyBoundCommand in keyBoundCommands)
            {
                string modifierKeyEnumerationValue = StatusCode.EmptyString;

                var bindingAction = keyBoundCommand.commandString;
                var commandActionId = keyBoundCommand.actionId;
                var keyActionType = keyBoundCommand.actionType;
                var regularKeyCode = keyBoundCommand.regular;
                var modifierKeyCode = keyBoundCommand.regular == keyBoundCommand.modifier ? StatusCode.EmptyStringInt : keyBoundCommand.modifier;

                // If we do have a modifier key code ..
                if (modifierKeyCode >= 0)
                {
                    // Get its enumerated key value ..
                    modifierKeyEnumerationValue = Keys.GetKeyValue(modifierKeyCode);
                }

                // Load final values into DataTable ..
                boundKeyActionDefinitions.LoadDataRow(new object[] 
                                                                    {
                                                                        Application.Name.VoiceAttack.ToString(), //Context
                                                                        Keys.KeyType.ToString(), //KeyEnumerationType
                                                                        bindingAction, //BindingAction
                                                                        StatusCode.NotApplicable, //Priority
                                                                        StatusCode.NotApplicable, //KeyGameValue
                                                                        Keys.GetKeyValue(regularKeyCode), //KeyEnumerationValue
                                                                        regularKeyCode.ToString(), //KeyEnumerationCode
                                                                        keyActionType, //KeyActionType
                                                                        commandActionId, //KeyId
                                                                        StatusCode.NotApplicable, //ModifierKeyGameValue
                                                                        modifierKeyEnumerationValue, //ModifierKeyEnumerationValue
                                                                        modifierKeyCode, //ModifierKeyEnumerationCode
                                                                        commandActionId //ModifierKeyId
                                                                    },
                                                                    false);
            }

            // return Datatable ..
            return boundKeyActionDefinitions;
        }

        /// <summary>
        /// Parse Voice Attack Config File to find internal name reference
        /// </summary>
        /// <remarks>
        ///   Format: XML
        ///             o <Profile/>
        ///               |_ <Name>       
        /// </remarks>
        /// <param name="xdoc"></param>
        /// <returns></returns>
        private string GetInternalReference(ref XDocument xdoc)
        {
            try
            {
                return xdoc.Element(XMLRoot).Element(XMLName).SafeElementValue().Trim();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Get (any) Modifier Key Code associated to a Command Action Id
        /// </summary>
        /// <param name="xdoc"></param>
        /// <param name="commandActionId"></param>
        /// <returns></returns>
        private int GetModifierKey(ref XDocument xdoc, string commandActionId)
        {
            // Count number of unsigned short elements (KeyCode) exist per ActionId ...
            var keyCodes = xdoc.Descendants(XMLUnsignedShort)
                                    .Where(item => item.Parent.Parent.Parent.Parent.Element(XMLCategory).Value.Contains(XMLCategoryKeybindings) &&
                                                   item.Parent.Parent.Element(XMLCommandActionId).Value == commandActionId)
                                    .DescendantsAndSelf();

            var countOfKeyCode = keyCodes.Count();

            // Check to see if modifier already exists in VoiceAttack Profile ..
            if (countOfKeyCode > 1)
            {
                // First value is always Modifier Key Code ..
                return int.Parse(keyCodes.FirstOrDefault().Value);
            }
            else
            {
                return StatusCode.EmptyStringInt;
            }
        }

        /// <summary>
        /// Get regular (non-Modifier) Key Code associated to a Command Action Id
        /// </summary>
        /// <param name="xdoc"></param>
        /// <param name="commandActionId"></param>
        /// <returns></returns>
        private int GetRegularKey(ref XDocument xdoc, string commandActionId)
        {
            // Count number of unsigned short elements (KeyCode) exist per ActionId ...
            var keyCodes = xdoc.Descendants(XMLUnsignedShort)
                                    .Where(item => item.Parent.Parent.Parent.Parent.Element(XMLCategory).Value.Contains(XMLCategoryKeybindings) &&
                                                   item.Parent.Parent.Element(XMLCommandActionId).Value == commandActionId)
                                    .DescendantsAndSelf();

            var countOfKeyCode = keyCodes.Count();

            // Last value is always Regular Key Code ..
            return int.Parse(keyCodes.LastOrDefault().Value);
        }

        /// <summary>
        /// Parse Voice Attack Config File to extract Commands and Command Actions for Key Up/Down/Press Action Types already bound to a non-blank key code
        /// </summary>
        /// <remarks>
        ///   Format: XML
        ///             o <Profile/>
        ///               |_ <Commands/>
        ///                  |_ <Command/>
        ///                      |_<Id/>
        ///                      !_<CommandString/>* = ((<action name/>))
        ///                      |_<ActionSequence/>
        ///                        !_[some] <CommandAction/>
        ///                                 !_<Id/>*
        ///                                 |_<ActionType/> = PressKey/KeyUp/KeyDown
        ///                                 |_<KeyCodes/>
        ///                                   (|_<unsignedShort/> = when modifier present)
        ///                                    |_<unsignedShort/>*
        ///                      !_<Category/> = Keybindings
        ///               |_ <Commands/>
        ///                  |_ <Command/>
        ///                      |_<Id/>
        ///                      !_<commandString/>* = ((<action name/>))
        ///                      |_<ActionSequence/>
        ///                        !_[some] <CommandAction/>
        ///                                 !_<Id/>*
        ///                                 |_<ActionType/> = KeyUp
        ///                                 |_<KeyCodes/>
        ///                                    |_<unsignedShort/>*
        ///                        !_[some] <CommandAction/>
        ///                                 !_<Id/>*
        ///                                 |_<ActionType/> = PressKey
        ///                                 |_<KeyCodes/>
        ///                                    |_<unsignedShort/>*
        ///                        !_[some] <CommandAction/>
        ///                                 !_<Id/>*
        ///                                 |_<ActionType/> = KeyDown
        ///                                 |_<KeyCodes/>
        ///                                    |_<unsignedShort/>*
        ///                      !_<Category/> = Keybindings
        ///                             
        ///    Key Binding Arrangement: 
        ///                VoiceAttack uses system key codes (as opposed to key value). 
        ///                Actions directly mappable to Elite Dangerous have been defined by HCSVoicePacks using: 
        ///                 o Command.Category = Keybindings or variation (e.g. Keybindings - SRV)
        ///                 o Predominant ActionType = PressKey
        ///                 o Command.commandString values which are usually pre- and post-fixed using '((' and '))'
        ///                   e.g. 
        ///                    ((Shield Cell)) : 222 (= Oem7 Numpad?7)
        ///                    ((Power To Weapons)) : 39  (= Right arrow)
        ///                    ((Select Target Ahead)) : 84 (= T)
        ///                    ((Flight Assist)) : 90 (= Z)                
        /// </remarks>
        /// <param name="xdoc"></param>
        /// <returns></returns>
        /// 
        private System.Collections.Generic.IEnumerable<object> ParseVoiceAttackProfileForKeyBoundCommands(ref XDocument xdoc)
        {
            // Find properties linked to any Key-related CommandString node with a valuated KeyCode ..
            return
                from item in xdoc.Descendants(XMLUnsignedShort)
                orderby (item.Parent.Parent.Parent.Parent.SafeAttributeName(XMLCommandString) +
                         item.SafeAttributeName(XMLUnsignedShort))
                where
                    ////---------------------------------XML Hierarchy Integrity-Check Section---------------------------------
                        item.Parent.Parent.Parent.Parent.Parent.Parent.SafeAttributeName(XMLCommands) == XMLCommands &&
                        item.Parent.Parent.Parent.Parent.Parent.SafeAttributeName(XMLCommand) == XMLCommand &&
                        item.Parent.Parent.Parent.Parent.SafeAttributeName(XMLCommandString) == XMLCommandString &&
                        item.Parent.Parent.Parent.SafeAttributeName(XMLActionSequence) == XMLActionSequence &&
                        item.Parent.Parent.SafeAttributeName(XMLCommandAction) == XMLCommandAction &&
                        item.Parent.SafeAttributeName(XMLCommandActionId) == XMLCommandActionId &&
                        item.Parent.SafeAttributeName(XMLActionType) == XMLActionType &&
                        item.Parent.SafeAttributeName(XMLKeyCodes) == XMLKeyCodes &&
                        item.SafeAttributeName(XMLUnsignedShort) == XMLUnsignedShort &&
                        item.Parent.Parent.Parent.Parent.SafeAttributeName(XMLCategory) == XMLCategory &&
                        item.Parent.Parent.Parent.Parent.Element(XMLCategory).SafeElementValue().Contains(XMLCategoryKeybindings)
                    ////---------------------------------XML Hierarchy Integrity-Check Section---------------------------------
                        &&
                        (item.Parent.Parent.Element(XMLActionType).SafeElementValue() == Application.Interaction.PressKey.ToString() ||
                         item.Parent.Parent.Element(XMLActionType).SafeElementValue() == Application.Interaction.KeyDown.ToString() ||
                         item.Parent.Parent.Element(XMLActionType).SafeElementValue() == Application.Interaction.KeyUp.ToString())
                        &&
                        item.SafeElementValue() != string.Empty
                select
                    new
                    {
                        CommandString = item.Parent.Parent.Parent.Parent.Element(XMLCommandString).SafeElementValue(),
                        CommandActionId = item.Parent.Parent.Element(XMLCommandActionId).SafeElementValue(),
                        KeyActionType = item.Parent.Parent.Element(XMLActionType).SafeElementValue(),
                        KeyCode = System.Convert.ToInt32(item.SafeElementValue())
                    };
        }

        /// <summary>
        /// Parse Voice Attack Config File to extract Commands and Command Actions for any Key Up/Down/Press Action Type
        /// </summary>
        /// <param name="xdoc"></param>
        /// <returns></returns>
        /// 
        private System.Collections.Generic.IEnumerable<object> ParseVoiceAttackProfileForKeyBindableCommands(ref XDocument xdoc)
        {
            // // Find properties linked to any Key-related CommandString node (whether keyCode present or not) ...
            return
                from item in xdoc.Descendants(XMLUnsignedShort)
                orderby (item.Parent.Parent.Parent.Parent.SafeAttributeName(XMLCommandString) +
                         item.SafeAttributeName(XMLUnsignedShort))
                where
                    ////---------------------------------XML Hierarchy Integrity-Check Section---------------------------------
                        item.Parent.Parent.Parent.Parent.Parent.Parent.SafeAttributeName(XMLCommands) == XMLCommands &&
                        item.Parent.Parent.Parent.Parent.Parent.SafeAttributeName(XMLCommand) == XMLCommand &&
                        item.Parent.Parent.Parent.Parent.SafeAttributeName(XMLCommandString) == XMLCommandString &&
                        item.Parent.Parent.Parent.SafeAttributeName(XMLActionSequence) == XMLActionSequence &&
                        item.Parent.Parent.SafeAttributeName(XMLCommandAction) == XMLCommandAction &&
                        item.Parent.SafeAttributeName(XMLCommandActionId) == XMLCommandActionId &&
                        item.Parent.SafeAttributeName(XMLActionType) == XMLActionType &&
                        item.Parent.SafeAttributeName(XMLKeyCodes) == XMLKeyCodes &&
                        item.SafeAttributeName(XMLUnsignedShort) == XMLUnsignedShort &&
                        item.Parent.Parent.Parent.Parent.SafeAttributeName(XMLCategory) == XMLCategory &&
                        item.Parent.Parent.Parent.Parent.Element(XMLCategory).SafeElementValue().Contains(XMLCategoryKeybindings)
                    ////---------------------------------XML Hierarchy Integrity-Check Section---------------------------------
                        &&
                        (item.Parent.Parent.Element(XMLActionType).SafeElementValue() == Application.Interaction.PressKey.ToString() ||
                         item.Parent.Parent.Element(XMLActionType).SafeElementValue() == Application.Interaction.KeyDown.ToString() ||
                         item.Parent.Parent.Element(XMLActionType).SafeElementValue() == Application.Interaction.KeyUp.ToString())
                select
                    new
                    {
                        CommandString = item.Parent.Parent.Parent.Parent.Element(XMLCommandString).SafeElementValue(),
                        KeyActionType = item.Parent.Parent.Element(XMLActionType).SafeElementValue()
                    };
        }

        /// <summary>
        /// Parse Voice Attack Config File to extract Command Ids linked with a keyboard-related ActionId
        /// </summary>
        /// <param name="xdoc"></param>
        /// <param name="commandActionId"></param>
        /// <remarks>
        ///   Format: XML
        ///             o <Profile/>
        ///               |_ <Commands/>
        ///                  |_ <Command/>
        ///                      |_<Id/> *  <-----------------------------------¬
        ///                      !_<CommandString/>                             |
        ///                      |_<ActionSequence/>                            |
        ///                        !_[some] <CommandAction/>                    |
        ///                                 |_<Id> -----------------------------
        ///                                 |_<KeyCodes/>
        ///                                 |_<Context/> 
        ///                      !_<Description/> = Command Description
        ///                      !_<Category/> != Keybindings
        /// </remarks>
        /// <returns></returns>
        private System.Collections.Generic.IEnumerable<object> ParseVoiceAttackProfileForKeyBoundCommandIdsFromCommandActionId(ref XDocument xdoc, string commandActionId)
        {
            return
                from item in xdoc.Descendants(XMLUnsignedShort)
                orderby (item.Parent.Parent.Parent.Parent.SafeAttributeName(XMLCommandString) +
                         item.SafeAttributeName(XMLUnsignedShort))
                where
                    ////---------------------------------XML Hierarchy Integrity-Check Section---------------------------------
                        item.Parent.Parent.Parent.Parent.Parent.Parent.SafeAttributeName(XMLCommands) == XMLCommands &&
                        item.Parent.Parent.Parent.Parent.Parent.SafeAttributeName(XMLCommand) == XMLCommand &&
                        item.Parent.Parent.Parent.Parent.SafeAttributeName(XMLCommandString) == XMLCommandString &&
                        item.Parent.Parent.Parent.SafeAttributeName(XMLActionSequence) == XMLActionSequence &&
                        item.Parent.Parent.SafeAttributeName(XMLCommandAction) == XMLCommandAction &&
                        item.Parent.SafeAttributeName(XMLCommandActionId) == XMLCommandActionId &&
                        item.Parent.SafeAttributeName(XMLActionType) == XMLActionType &&
                        item.Parent.SafeAttributeName(XMLKeyCodes) == XMLKeyCodes &&
                        item.SafeAttributeName(XMLUnsignedShort) == XMLUnsignedShort &&
                        item.Parent.Parent.Parent.Parent.SafeAttributeName(XMLCategory) == XMLCategory &&
                        item.Parent.Parent.Parent.Parent.Element(XMLCategory).SafeElementValue().Contains(XMLCategoryKeybindings)
                    ////---------------------------------XML Hierarchy Integrity-Check Section---------------------------------
                        &&
                        (item.Parent.Parent.Element(XMLActionType).SafeElementValue() == Application.Interaction.PressKey.ToString() ||
                         item.Parent.Parent.Element(XMLActionType).SafeElementValue() == Application.Interaction.KeyDown.ToString() ||
                         item.Parent.Parent.Element(XMLActionType).SafeElementValue() == Application.Interaction.KeyUp.ToString())
                        &&
                         item.Parent.Parent.Element(XMLCommandActionId).SafeElementValue() == commandActionId
                select
                         item.Parent.Parent.Parent.Parent.Element(XMLCommandId).SafeElementValue();
        }
    }
}