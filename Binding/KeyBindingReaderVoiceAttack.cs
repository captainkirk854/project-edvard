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
        private const string XMLCategoryEducation = "education";
        
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
            DataTable bindable = this.GetCommandStringsWithAssociatedKeys(ref this.xCfg);

            // modify table ..
            bindable.AddDefaultColumn(Edvard.Column.Internal.ToString(), this.GetInternalReference(ref this.xCfg));
            bindable.AddDefaultColumn(Edvard.Column.FilePath.ToString(), this.cfgFilePath);

            // return Datatable ..
            return bindable;
        }

        /// <summary>
        /// Load Voice Attack Key Bindings into DataTable
        /// </summary>
        /// <returns></returns>
        public DataTable GetBoundCommands()
        {
            // Read bindings and tabulate ..
            DataTable bound = this.GetKeyBindings(ref this.xCfg);

            // modify table ..
            bound.AddDefaultColumn(Edvard.Column.Internal.ToString(), this.GetInternalReference(ref this.xCfg));
            bound.AddDefaultColumn(Edvard.Column.FilePath.ToString(), this.cfgFilePath);

            // return Datatable ..
            return bound;
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
                    var associatedCommandStrings = this.GetCommandStringsFromCommandActionContext(ref this.xCfg, this.GetCommandIdFromCommandActionIdWithBoundKeys(ref this.xCfg, voiceAttackActionId));
                    
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
        /// Parse Voice Attack Config File to get all Command Strings with valid Actions Types
        /// </summary>
        /// <param name="xdoc"></param>
        /// <returns></returns>
        private DataTable GetCommandStringsWithAssociatedKeys(ref XDocument xdoc)
        {
            // Datatable to hold tabulated XML contents ..
            DataTable bindableactions = TableShape.BindableActions();

            // traverse config XML, find all valuated <unsignedShort> nodes, work from inside out to gather pertinent Element data and arrange in row(s) of anonymous types ..
            var xmlExtracts = from item in xdoc.Descendants(XMLUnsignedShort)
                              where
                                    item.Parent.Parent.Parent.Parent.Element(XMLCategory).Value == XMLCategoryKeybindings &&
                                    (item.Parent.Parent.Parent.Parent.Element(XMLActionSequence).Element(XMLCommandAction).Element(XMLActionType).Value == Application.Interaction.PressKey.ToString() ||
                                     item.Parent.Parent.Parent.Parent.Element(XMLActionSequence).Element(XMLCommandAction).Element(XMLActionType).Value == Application.Interaction.KeyUp.ToString() ||
                                     item.Parent.Parent.Parent.Parent.Element(XMLActionSequence).Element(XMLCommandAction).Element(XMLActionType).Value == Application.Interaction.KeyDown.ToString() ||
                                     item.Parent.Parent.Parent.Parent.Element(XMLActionSequence).Element(XMLCommandAction).Element(XMLActionType).Value == Application.Interaction.ExecuteCommand.ToString()) &&
                                     item.SafeElementValue() != string.Empty
                              select
                                 new
                                 {
                                     CommandString = item.Parent.Parent.Parent.Parent.Element(XMLCommandString).SafeElementValue()
                                 };

            // insert anonymous type row data (with some additional values) into DataTable (.Distinct() required as some Commands have multiple (modifier) key codes)
            foreach (var xmlExtract in xmlExtracts.Distinct()) 
            {
                bindableactions.LoadDataRow(new object[] 
                                                {
                                                    Application.Name.VoiceAttack.ToString(), //Context
                                                    xmlExtract.CommandString, //BindingAction
                                                    StatusCode.NotApplicable, // Device priority
                                                    Application.Interaction.Keyboard.ToString() // Device binding is applied to
                                                }, 
                                                false);
            }

            // return Datatable ..
            return bindableactions;
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
        /// Parse Voice Attack Config File to get Command Id
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
        private string GetCommandIdFromCommandActionIdWithBoundKeys(ref XDocument xdoc, string commandActionId)
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
        /// Parse Voice Attack Config File to get key details of all actions already bound to a key code
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
        /// Keys Bindings: 
        ///                VoiceAttack uses system key codes (as opposed to key value). 
        ///                Actions directly mappable to Elite Dangerous have been defined by HCSVoicePacks using: 
        ///                 o Command.Category = Keybindings
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
            DataTable binderKeyAction = TableShape.KeyActionBinder();

            // traverse config XML, find all valuated <unsignedShort> nodes, work from inside out to gather pertinent Element data and arrange in row(s) of anonymous types ..
            var xmlExtracts = from item in xdoc.Descendants(XMLUnsignedShort)
                              where
                                    item.Parent.Parent.Parent.Parent.Element(XMLCategory).Value == XMLCategoryKeybindings &&
                                   (item.Parent.Parent.Parent.Parent.Element(XMLActionSequence).Element(XMLCommandAction).Element(XMLActionType).Value == Application.Interaction.PressKey.ToString() ||
                                    item.Parent.Parent.Parent.Parent.Element(XMLActionSequence).Element(XMLCommandAction).Element(XMLActionType).Value == Application.Interaction.KeyUp.ToString() ||
                                    item.Parent.Parent.Parent.Parent.Element(XMLActionSequence).Element(XMLCommandAction).Element(XMLActionType).Value == Application.Interaction.KeyDown.ToString() ||
                                    item.Parent.Parent.Parent.Parent.Element(XMLActionSequence).Element(XMLCommandAction).Element(XMLActionType).Value == Application.Interaction.ExecuteCommand.ToString()) &&
                                    item.SafeElementValue() != string.Empty
                              select
                                 new // create anonymous type for every XMLunsignedShort matching criteria ..
                                 {
                                     Commandstring = item.Parent.Parent.Parent.Parent.Element(XMLCommandString).SafeElementValue(),
                                     CommandActionId = item.Parent.Parent.Element(XMLCommandActionId).SafeElementValue(),
                                     KeyCode = item.SafeElementValue()
                                 };

            // insert anonymous type row data (with some additional values) into DataTable ..
            foreach (var xmlExtract in xmlExtracts)
            {
                // Initialise ..
                string modifierKeyEnumerationValue = StatusCode.EmptyString;
                int regularKeyCode = int.Parse(xmlExtract.KeyCode);

                // Check for modifier key already present in VoiceAttack Profile for current Action Id ..
                int modifierKeyCode = this.GetModifierKey(ref xdoc, xmlExtract.CommandActionId);

                // Ignore if current regular key code is actually a modifier key code ..
                if (regularKeyCode != modifierKeyCode)
                {
                    if (modifierKeyCode >= 0)
                    {
                        // If modifier found, some additional probing of that segment of the XML tree required ..
                        modifierKeyEnumerationValue = Keys.GetKeyValue(modifierKeyCode);
                        regularKeyCode = this.GetRegularKey(ref xdoc, xmlExtract.CommandActionId);
                    }

                    // Load final values into datatable ..
                    binderKeyAction.LoadDataRow(new object[] 
                                                    {
                                                        Application.Name.VoiceAttack.ToString(), //Context
                                                        Keys.KeyType.ToString(), //KeyEnumerationType
                                                        xmlExtract.Commandstring, //BindingAction
                                                        StatusCode.NotApplicable, //Priority
                                                        Keys.GetKeyValue(regularKeyCode), //KeyGameValue
                                                        Keys.GetKeyValue(regularKeyCode), //KeyEnumerationValue
                                                        regularKeyCode.ToString(), //KeyEnumerationCode
                                                        xmlExtract.CommandActionId, //KeyId
                                                        modifierKeyEnumerationValue, //ModifierKeyGameValue
                                                        modifierKeyEnumerationValue, //ModifierKeyEnumerationValue
                                                        modifierKeyCode, //ModifierKeyEnumerationCode
                                                        xmlExtract.CommandActionId //ModifierKeyId
                                                    },
                                                    false);
                }
            }

            // return Datatable ..
            return binderKeyAction;
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