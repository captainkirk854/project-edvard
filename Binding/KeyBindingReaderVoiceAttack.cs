namespace Binding
{
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
        /// Get CommandStrings related to action CommandStrings
        /// </summary>
        /// <param name="consolidatedBoundCommands"></param>
        /// <returns></returns>
        public DataTable GetRelatedCommandStrings(DataTable consolidatedBoundCommands)
        {
            // Initialise ..
            DataTable associatedCommands = TableShape.AssociatedCommands();
            string prevCommandString = string.Empty;

            // Find associated commandStrings using commandString ActionId ...
            foreach (DataRow consolidatedBoundCommand in consolidatedBoundCommands.Select().OrderBy(orderingColumn => orderingColumn[Edvard.Column.VoiceAttackAction.ToString()]))
            {
                // Get required field information ..
                string voiceAttackCommandString = consolidatedBoundCommand[Edvard.Column.VoiceAttackAction.ToString()].ToString();
                string voiceAttackActionId = consolidatedBoundCommand[Edvard.Column.VoiceAttackKeyId.ToString()].ToString();
                string eliteDangerousAction = consolidatedBoundCommand[Edvard.Column.EliteDangerousAction.ToString()].ToString();
                string bindingSyncStatus = consolidatedBoundCommand[Edvard.Column.KeyUpdateRequired.ToString()].ToString() == Edvard.KeyUpdateRequired.NO.ToString() ? "synchronised" : "*attention required*";
                string voiceAttackFile = Path.GetFileName(consolidatedBoundCommand[Edvard.Column.VoiceAttackProfile.ToString()].ToString());
                string eliteDangerousFile = Path.GetFileName(consolidatedBoundCommand[Edvard.Column.EliteDangerousBinds.ToString()].ToString());

                // Ignore duplicate commandStrings from those with multiple Action Ids ..
                if (voiceAttackCommandString != prevCommandString)
                {
                    var associatedCommandStrings = this.GetCommandStringsFromCommandActionContext(ref this.xCfg, this.GetCommandIdFromCommandActionIdWithKeyPressAction(ref this.xCfg, voiceAttackActionId));
                    
                    string prevAssociatedCommandString = string.Empty;
                    foreach (var associatedCommandString in associatedCommandStrings)
                    {
                        // Ignore any duplicate Associated commandStrings ..
                        if (associatedCommandString != prevAssociatedCommandString)
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

                        prevAssociatedCommandString = associatedCommandString;
                    }
                }

                prevCommandString = voiceAttackCommandString;
            }

            return associatedCommands;
        }

        /// <summary>
        /// Get commandStrings for all Categories and load into DataTable ..
        /// </summary>
        /// <returns></returns>
        public DataTable GetCommandStringsForAllCategories()
        {
            // Datatable to hold tabulated XML contents ..
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
            // Datatable to hold tabulated XML contents ..
            DataTable bindableActions = TableShape.BindableActions();

            // traverse config XML, find all valuated <unsignedShort> nodes, work from inside out to gather pertinent Element data and arrange in row(s) of anonymous types ..
            var xmlExtracts = from item in xdoc.Descendants(XMLUnsignedShort)
                              where
                                    item.Parent.Parent.Parent.Parent.Element(XMLCategory).Value == XMLCategoryKeybindings &&
                                    (item.Parent.Parent.Parent.Parent.Element(XMLActionSequence).Element(XMLCommandAction).Element(XMLActionType).Value == Application.Interaction.PressKey.ToString() ||
                                     item.Parent.Parent.Parent.Parent.Element(XMLActionSequence).Element(XMLCommandAction).Element(XMLActionType).Value == Application.Interaction.KeyUp.ToString() ||
                                     item.Parent.Parent.Parent.Parent.Element(XMLActionSequence).Element(XMLCommandAction).Element(XMLActionType).Value == Application.Interaction.KeyDown.ToString()) &&
                                     item.SafeElementValue() != string.Empty
                              select
                                 new
                                 {
                                     CommandString = item.Parent.Parent.Parent.Parent.Element(XMLCommandString).SafeElementValue(),
                                     ActionType = item.Parent.Parent.Parent.Parent.Element(XMLActionSequence).Element(XMLCommandAction).Element(XMLActionType).Value
                                 };

            // insert anonymous type row data (with some additional values) into DataTable (.Distinct() required as some Commands have multiple (modifier) key codes)
            foreach (var xmlExtract in xmlExtracts.Distinct()) 
            {
                bindableActions.LoadDataRow(new object[] 
                                                {
                                                    Application.Name.VoiceAttack.ToString(), //Context
                                                    xmlExtract.CommandString, //KeyAction
                                                    xmlExtract.ActionType, //KeyActionType
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
        ///                      !_<commandString/>* = Spoken Command  <--------¬
        ///                      !_<Category/>* = <value/>
        ///                      |_<ActionSequence/> 
        ///                         !_[some] <CommandAction/>
        ///                                   |_<ActionType/>* = <value/>
        ///                  
        /// </remarks>
        /// <param name="xdoc"></param>
        /// <param name="commandCategories"></param>
        /// <returns></returns>
        private System.Collections.Generic.IEnumerable<object> GetCommandStringsForCommandCategory(ref XDocument xdoc, System.Collections.Generic.IEnumerable<string> commandCategories)
        {
            // Create list of required anonymous type .. 
            var xmlExtracts = new[] { new { CommandString = string.Empty, CommandCategory = string.Empty, ActionType = string.Empty } }.ToList();
            xmlExtracts.Clear(); // empty first row created whilst keeping anonymous type definition

            // Get Command String(s) for each Command Category ..
            foreach (var commandCategory in commandCategories)
            {
                // traverse config XML, return Command String(s) associated to each Category ..
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
        private System.Collections.Generic.IEnumerable<string> GetAllCommandCategories(ref XDocument xdoc)
        {
            var xmlExtracts = from item in xdoc.Descendants(XMLCategory)
                              select
                                    item.Parent.Element(XMLCategory).SafeElementValue();

            return xmlExtracts.Distinct();
        }

        /// <summary>
        /// Parse Voice Attack Config File to get Command Id related to Command Actions with KeyPress Action Types
        /// </summary>
        /// <remarks>
        ///   Format: XML
        ///             o <Profile/>
        ///               |_ <Commands/>
        ///                  |_ <Command/>
        ///                      |_<Id/>* <--------------------------------¬
        ///                      !_<commandString/> = ((<action name/>))   |
        ///                      |_<ActionSequence/>                       |
        ///                        !_[some] <CommandAction/>               |
        ///                                 !_<Id/> ------------------------
        ///                                 |_<ActionType/> = PressKey/KeyUp/KeyDown
        ///                                 |_<KeyCodes/>
        ///                                   (|_<unsignedShort/> = when modifier present)
        ///                                    |_<unsignedShort/>
        ///                      !_<Category/> = Keybindings
        /// </remarks>
        /// <param name="xdoc"></param>
        /// <param name="commandActionId"></param>
        /// <returns></returns>
        private string GetCommandIdFromCommandActionIdWithKeyPressAction(ref XDocument xdoc, string commandActionId)
        {
            // traverse config XML to <unsignedShort> nodes, return first (and only) ancestral Command Id where Action Id is ancestor of <unsignedShort> ..
            var xmlExtracts = from item in xdoc.Descendants(XMLUnsignedShort)
                              where
                                    item.Parent.Parent.Parent.Parent.Element(XMLCategory).Value == XMLCategoryKeybindings &&
                                    item.Parent.Parent.Element(XMLCommandActionId).SafeElementValue() == commandActionId &&
                                    (item.Parent.Parent.Element(XMLActionType).SafeElementValue() == Application.Interaction.PressKey.ToString() ||
                                     item.Parent.Parent.Element(XMLActionType).SafeElementValue() == Application.Interaction.KeyUp.ToString() ||
                                     item.Parent.Parent.Element(XMLActionType).SafeElementValue() == Application.Interaction.KeyDown.ToString())
                              select
                                    item.Parent.Parent.Parent.Parent.Element(XMLCommandId).SafeElementValue();

            return xmlExtracts.FirstOrDefault();       
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
        ///                      !_<commandString/>* = Spoken Command  <--------¬           
        ///                      |_<ActionSequence/>                            |
        ///                        !_[some] <CommandAction/>                    |
        ///                                 |_<KeyCodes/>                       |
        ///                                 |_<Context/> ------------------------
        ///                      !_<Description/> = Command Description
        ///                      !_<Category/> != Keybindings
        /// </remarks>
        /// <param name="xdoc"></param>
        /// <param name="commandActionId"></param>
        /// <returns></returns>
        private System.Collections.Generic.IEnumerable<string> GetCommandStringsFromCommandActionContext(ref XDocument xdoc, string commandActionId)
        {
            // traverse config XML, return first (and only) Command String where descendant Context = Action Id ..
            var xmlExtracts = from item in xdoc.Descendants(XMLContext)
                              where
                                    item.Parent.Parent.Parent.Element(XMLCategory).Value != XMLCategoryKeybindings &&
                                    item.SafeElementValue() == commandActionId
                              select
                                     item.Parent.Parent.Parent.Element(XMLCommandString).SafeElementValue();

            return xmlExtracts;
        }

        /// <summary>
        /// Parse Voice Attack Config File to get details of Commands and Command Actions with KeyPress Action Types already boundCommands to a non-blank key code
        /// </summary>
        /// <remarks>
        ///   Format: XML
        ///             o <Profile/>
        ///               |_ <Commands/>
        ///                  |_ <Command/>
        ///                      |_<Id/>
        ///                      !_<commandString/>* = ((<action name/>))
        ///                      |_<ActionSequence/>
        ///                        !_[some] <CommandAction/>
        ///                                 !_<Id/>*
        ///                                 |_<ActionType/> = PressKey/KeyUp/KeyDown
        ///                                 |_<KeyCodes/>
        ///                                   (|_<unsignedShort/> = when modifier present)
        ///                                    |_<unsignedShort/>*
        ///                      !_<Category/> = Keybindings
        ///                             
        /// Key Binding Arrangement: 
        ///                VoiceAttack uses system key codes (as opposed to key value). 
        ///                Actions directly mappable to Elite Dangerous have been defined by HCSVoicePacks using: 
        ///                 o Command.Category = Keybindings
        ///                 o Predominant ActionType = PressKey
        ///                 o Command.commandString values which are pre- and post-fixed using '((' and '))'
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
            DataTable keyActionDefinitions = TableShape.KeyActionDefinition();

            // traverse config XML, find all valuated <unsignedShort> nodes, work from inside out  ..
            var xmlExtracts = from item in xdoc.Descendants(XMLUnsignedShort)
                              where
                                    item.Parent.Parent.Parent.Parent.Element(XMLCategory).Value == XMLCategoryKeybindings &&
                                   (item.Parent.Parent.Parent.Parent.Element(XMLActionSequence).Element(XMLCommandAction).Element(XMLActionType).Value == Application.Interaction.PressKey.ToString()) &&
                                    item.SafeElementValue() != string.Empty
                              select
                                 new // create anonymous type for every XMLunsignedShort matching criteria ..
                                 {
                                     Commandstring = item.Parent.Parent.Parent.Parent.Element(XMLCommandString).SafeElementValue(),
                                     CommandActionId = item.Parent.Parent.Element(XMLCommandActionId).SafeElementValue(),
                                     KeyActionType = item.Parent.Parent.Parent.Parent.Element(XMLActionSequence).Element(XMLCommandAction).Element(XMLActionType).Value,
                                     KeyCode = item.SafeElementValue()
                                 };

            // insert anonymous type row data (with some additional values) into DataTable ..
            foreach (var xmlExtract in xmlExtracts)
            {
                // Initialise ..
                string modifierKeyEnumerationValue = StatusCode.EmptyString;
                int regularKeyCode = int.Parse(xmlExtract.KeyCode);

                // Check if modifier key already present in VoiceAttack Profile for current Action Id ..
                int modifierKeyCode = this.GetModifierKey(ref xdoc, xmlExtract.CommandActionId);

                // Ignore if current regular key code is actually a modifier key code ..
                if (regularKeyCode != modifierKeyCode)
                {
                    // If we do have a modifier key code ..
                    if (modifierKeyCode >= 0)
                    {
                        // Get its enumerated key value ..
                        modifierKeyEnumerationValue = Keys.GetKeyValue(modifierKeyCode);

                        // If modifier key found, and it's a regular KeyPress action, do some additional probing of that segment of the XML tree required ..
                        if (xmlExtract.KeyActionType == Application.Interaction.PressKey.ToString())
                        {
                            regularKeyCode = this.GetRegularKey(ref xdoc, xmlExtract.CommandActionId);
                        }
                    }

                    // Load final values into DataTable ..
                    keyActionDefinitions.LoadDataRow(new object[] 
                                                    {
                                                        Application.Name.VoiceAttack.ToString(), //Context
                                                        Keys.KeyType.ToString(), //KeyEnumerationType
                                                        xmlExtract.Commandstring, //BindingAction
                                                        StatusCode.NotApplicable, //Priority
                                                        StatusCode.NotApplicable, //KeyGameValue
                                                        Keys.GetKeyValue(regularKeyCode), //KeyEnumerationValue
                                                        regularKeyCode.ToString(), //KeyEnumerationCode
                                                        xmlExtract.KeyActionType, //KeyActionType
                                                        xmlExtract.CommandActionId, //KeyId
                                                        StatusCode.NotApplicable, //ModifierKeyGameValue
                                                        modifierKeyEnumerationValue, //ModifierKeyEnumerationValue
                                                        modifierKeyCode, //ModifierKeyEnumerationCode
                                                        xmlExtract.CommandActionId //ModifierKeyId
                                                    },
                                                    false);
                }
            }

            // Fetch and append key bindings for KeyUp/KeyDown interactions ..
            keyActionDefinitions.Merge(this.GetKeyBindingsForKeyUpKeyDownInteractions(ref xdoc));

            // return Datatable ..
            return keyActionDefinitions;
        }

        /// <summary>
        /// Parse Voice Attack Config File to get details of Commands and Command Actions with KeyUp/KeyDown Action Types already boundCommands to a non-blank key code
        /// </summary>
        /// <remarks>
        ///   Format: XML
        ///             o <Profile/>
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
        /// Key Binding Arrangement:
        ///     For bindings revolving around ActionType = KeyUp/KeyDown, the arrangement of key codes is different.
        ///     The related ActionSequence is arranged as a series of CommandActions each with its KeyCode value.
        ///     In all cases, the keycode values are just two numbers - the lower one being the actual key and the
        ///     higher being the modifier (this is the one that can appear multiple times). 
        ///     The following code scans through all the keycode values for a particular CommandString and assigns 
        ///     the lowest value found as the regular key code, and the highest as the modifier.
        /// </remarks>
        /// <param name="xdoc"></param>
        /// <returns></returns>
        private DataTable GetKeyBindingsForKeyUpKeyDownInteractions(ref XDocument xdoc)
        {
            // Datatable to hold tabulated XML contents ..
            DataTable keyActionDefinitionsForNonKeyPress = TableShape.KeyActionDefinition();

            // traverse config XML, find all valuated <unsignedShort> nodes, work from inside out  ..
            var xmlExtracts = from item in xdoc.Descendants(XMLUnsignedShort)
                              where
                                    item.Parent.Parent.Parent.Parent.Element(XMLCategory).Value == XMLCategoryKeybindings &&
                                   (item.Parent.Parent.Parent.Parent.Element(XMLActionSequence).Element(XMLCommandAction).Element(XMLActionType).Value == Application.Interaction.KeyUp.ToString() ||
                                    item.Parent.Parent.Parent.Parent.Element(XMLActionSequence).Element(XMLCommandAction).Element(XMLActionType).Value == Application.Interaction.KeyDown.ToString()) &&
                                    item.SafeElementValue() != string.Empty
                              select
                                 new // create anonymous type for every XMLunsignedShort matching criteria ..
                                 {
                                     Commandstring = item.Parent.Parent.Parent.Parent.Element(XMLCommandString).SafeElementValue(),
                                     CommandActionId = item.Parent.Parent.Element(XMLCommandActionId).SafeElementValue(),
                                     KeyActionType = item.Parent.Parent.Parent.Parent.Element(XMLActionSequence).Element(XMLCommandAction).Element(XMLActionType).Value,
                                     KeyCode = System.Convert.ToInt32(item.SafeElementValue())
                                 };

            // Perform a grouping LINQ, to identify max and min KeyCode values for every CommandString as these are the regular and modifier key codes ..
            var nonKeyPressCommands = xmlExtracts.GroupBy(groupingField => new { groupingField.Commandstring })
                                                 .Select(info => new
                                                                   {
                                                                       groupKey = info.Key,
                                                                       commandString = info.Max(t => t.Commandstring),
                                                                       actionId = info.Max(t => t.CommandActionId),
                                                                       actionType = info.Min(t => t.KeyActionType),
                                                                       regular = info.Min(t => t.KeyCode), 
                                                                       modifier = info.Max(t => t.KeyCode)                      
                                                                   });
            
            // Extract condensed aggregation of KeyCodes ..
            foreach (var nonKeyPressCommand in nonKeyPressCommands)
            {
                string modifierKeyEnumerationValue = StatusCode.EmptyString;

                var bindingAction = nonKeyPressCommand.commandString;
                var commandActionId = nonKeyPressCommand.actionId;
                var keyActionType = nonKeyPressCommand.actionType;
                var regularKeyCode = nonKeyPressCommand.regular;
                var modifierKeyCode = nonKeyPressCommand.regular == nonKeyPressCommand.modifier ? StatusCode.EmptyStringInt : nonKeyPressCommand.modifier;

                // If we do have a modifier key code ..
                if (modifierKeyCode >= 0)
                {
                    // Get its enumerated key value ..
                    modifierKeyEnumerationValue = Keys.GetKeyValue(modifierKeyCode);
                }

                // Load final values into DataTable ..
                keyActionDefinitionsForNonKeyPress.LoadDataRow(new object[] 
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
            return keyActionDefinitionsForNonKeyPress;
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
                                    .Where(item => item.Parent.Parent.Parent.Parent.Element(XMLCategory).Value == XMLCategoryKeybindings &&
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
                                    .Where(item => item.Parent.Parent.Parent.Parent.Element(XMLCategory).Value == XMLCategoryKeybindings &&
                                                   item.Parent.Parent.Element(XMLCommandActionId).Value == commandActionId)
                                    .DescendantsAndSelf();

            var countOfKeyCode = keyCodes.Count();

            // Last value is always Regular Key Code ..
            return int.Parse(keyCodes.LastOrDefault().Value);
        }
    }
}