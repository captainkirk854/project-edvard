namespace Game
{
    using Helpers;
    using System.Data;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Read and process Elite Dangerous and Voice Attack Binding Configuration Files
    /// </summary>
    public static class BindingsReader
    {
        // Initialise ..
        private const string D = "+";
        private const string NA = "n/a";
        private const int INA = -2;
        private static readonly KeyMapper KeyMap = new KeyMapper(keyType);

        // Preset Key Map Enumeration to use ..
        private static Enums.KeyEnumType keyType = Enums.KeyEnumType.WindowsForms;
        private static KeyExchange exchange = new KeyExchange(keyType);

        /// <summary>
        /// Read Elite Dangerous Key Bindings into DataTable
        /// </summary>
        /// <param name="cfgFilePath"></param>
        /// <returns></returns>
        public static DataTable EliteDangerousKeyBindings(string cfgFilePath)
        {
            // Load configuration file as xml document object ..
            var cfgED = Xml.ReadXDoc(cfgFilePath);

            // Read bindings and tabulate ..
            DataTable primary = GetEDKeyBindings(cfgED, Enums.EliteDangerousDevicePriority.Primary);            
            DataTable secondary = GetEDKeyBindings(cfgED, Enums.EliteDangerousDevicePriority.Secondary);

            // Merge ..
            primary.Merge(secondary);

            // Add column ..
            primary.AddDefaultColumn(Enums.Column.FilePath.ToString(), cfgFilePath);

            // Return merged DataTable contents ..
            return primary;
        }

        /// <summary>
        ///  Read Elite Dangerous Binding Actions into DataTable
        /// </summary>
        /// <param name="cfgFilePath"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static DataTable EliteDangerousBindings(string cfgFilePath)
        {
            // Load configuration file as xml document object ..
            var cfgED = Xml.ReadXDoc(cfgFilePath);

            // Read bindings and tabulate ..
            DataTable primary = GetEDBindingActions(cfgED);

            // Add column ..
            primary.AddDefaultColumn(Enums.Column.FilePath.ToString(), cfgFilePath);

            // Return merged DataTable contents ..
            return primary;
        }

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
        /// Define Binding Actions DataTable Structure
        /// </summary>
        /// <param name="bindingActions"></param>
        private static void DefineBindingActions(this DataTable bindingActions)
        {
            bindingActions.TableName = "BindingActions";

            // Define table structure ..
            bindingActions.Columns.Add(Enums.Column.Context.ToString(), typeof(string));
            bindingActions.Columns.Add(Enums.Column.KeyAction.ToString(), typeof(string));
            bindingActions.Columns.Add(Enums.Column.DevicePriority.ToString(), typeof(string));
            bindingActions.Columns.Add(Enums.Column.DeviceType.ToString(), typeof(string));
        }

        /// <summary>
        /// Define Key Bindings DataTable Structure
        /// </summary>
        /// <param name="keyBindings"></param>
        private static void DefineKeyBinder(this DataTable keyBindings)
        {
            keyBindings.TableName = "KeyBindings";

            // Define table structure ..
            keyBindings.Columns.Add(Enums.Column.Context.ToString(), typeof(string));
            keyBindings.Columns.Add(Enums.Column.KeyEnumeration.ToString(), typeof(string));
            keyBindings.Columns.Add(Enums.Column.KeyAction.ToString(), typeof(string));
            keyBindings.Columns.Add(Enums.Column.DevicePriority.ToString(), typeof(string));
            keyBindings.Columns.Add(Enums.Column.KeyGameValue.ToString(), typeof(string));
            keyBindings.Columns.Add(Enums.Column.KeyEnumerationValue.ToString(), typeof(string));
            keyBindings.Columns.Add(Enums.Column.KeyEnumerationCode.ToString(), typeof(int));
            keyBindings.Columns.Add(Enums.Column.KeyId.ToString(), typeof(string));
            keyBindings.Columns.Add(Enums.Column.ModifierKeyGameValue.ToString(), typeof(string));
            keyBindings.Columns.Add(Enums.Column.ModifierKeyEnumerationValue.ToString(), typeof(string));
            keyBindings.Columns.Add(Enums.Column.ModifierKeyEnumerationCode.ToString(), typeof(int));
            keyBindings.Columns.Add(Enums.Column.ModifierKeyId.ToString(), typeof(string));
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
            // Initialise ..
            const string XMLCommand = "Command";
            const string XMLCommandString = "CommandString";
            const string XMLActionSequence = "ActionSequence";
            const string XMLCommandAction = "CommandAction";
            const string XMLActionType = "ActionType";
            const string XMLActionId = "Id";
            const string XMLKeyCodes = "KeyCodes";
            const string XMLunsignedShort = "unsignedShort";
            string[] keybindingIndicator = { "((", "))" };
 
            // Datatable to hold tabulated XML contents ..
            DataTable keybinder = new DataTable();
            keybinder.DefineKeyBinder();

            // traverse config XML and gather pertinent element data arranged in row(s) of anonymous types ..
            var xmlExtracts = from item in xdoc.Descendants(XMLCommand)
                             where item.Element(XMLCommandString).SafeElementValue().Contains(keybindingIndicator[0]) &&
                                   item.Element(XMLCommandString).SafeElementValue().Contains(keybindingIndicator[1]) &&
                                   item.Element(XMLActionSequence).Element(XMLCommandAction) != null &&
                                   item.Element(XMLActionSequence).Element(XMLCommandAction).Element(XMLActionType).Value == Enums.GameInteraction.PressKey.ToString()
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
                keybinder.LoadDataRow(new object[] 
                                                {
                                                 Enums.GameName.VoiceAttack.ToString(), //Context
                                                 KeyMap.KeyType.ToString(), //KeyEnumerationType
                                                 xmlExtract.Commandstring, //BindingAction
                                                 NA, //Priority
                                                 NA, //KeyGameValue
                                                 KeyMap.GetValue(int.Parse(xmlExtract.KeyCode)), //KeyEnumerationValue
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
            return keybinder;
        }

        /// <summary>
        /// Process Voice Attack Config File to return all possible binding actions
        /// </summary>
        /// <param name="xdoc"></param>
        /// <returns></returns>
        private static DataTable GetVABindingActions(XDocument xdoc)
        {
            // Initialise ..
            const string XMLCommand = "Command";
            const string XMLCommandString = "CommandString";
            const string XMLActionSequence = "ActionSequence";
            const string XMLCommandAction = "CommandAction";
            const string XMLActionType = "ActionType";
            string[] keybindingIndicator = { "((", "))" };

            // Datatable to hold tabulated XML contents ..
            DataTable bindingactions = new DataTable();
            bindingactions.DefineBindingActions();

            // traverse config XML and gather pertinent element data arranged in row(s) of anonymous types ..
            var xmlExtracts = from item in xdoc.Descendants(XMLCommand)
                             where item.Element(XMLCommandString).SafeElementValue().Contains(keybindingIndicator[0]) &&
                                   item.Element(XMLCommandString).SafeElementValue().Contains(keybindingIndicator[1]) &&
                                   item.Element(XMLActionSequence).Element(XMLCommandAction) != null &&
                                   item.Element(XMLActionSequence).Element(XMLCommandAction).Element(XMLActionType).Value == Enums.GameInteraction.PressKey.ToString()
                            select
                               new
                                 {
                                    Commandstring = item.Element(XMLCommandString).SafeElementValue()
                                 };

            // insert anonymous type row data (with some additional values) into DataTable ..
            foreach (var xmlExtract in xmlExtracts)
            {
                bindingactions.LoadDataRow(new object[] 
                                                {
                                                 Enums.GameName.VoiceAttack.ToString(), //Context
                                                 xmlExtract.Commandstring, //BindingAction
                                                 NA, // Device priority
                                                 Enums.GameInteraction.Keyboard.ToString() // Device binding applied to
                                                },
                                       false);
            }

            // return Datatable ..
            return bindingactions;
        }

        /// <summary>
        /// Process Elite Dangerous Config File looking for keyboard-specific bindings
        ///   Keys can be in assigned with Primary or Secondary Priorities
        ///   Format: XML
        ///             o <Root/>
        ///               |_ <KeyboardLayout/>
        ///               |_ <things></things>.[Value] attribute
        ///               |_ <things/>
        ///                  |_<Binding/>
        ///                  |_<Inverted/>
        ///                  |_<Deadzone/>
        ///               |_ <things/>
        ///                  |_<Primary/>
        ///                     |_<Device/>
        ///                     |_<Key/>
        ///                  |_<Secondary/>
        ///                     |_<Device/>
        ///                     |_<Key/>
        ///               |_ <things/>
        ///                  |_<Primary/>
        ///                     |_<Modifier/>
        ///                         |_<Device/>
        ///                         |_<Key/>
        ///                  |_<Secondary/>
        ///                     |_<Modifier/>
        ///                         |_<Device/>
        ///                         |_<Key/>
        ///               |_ <things/>
        ///                  |_<Primary/>
        ///                  |_<Secondary/>
        ///                  |_<ToggleOn/>
        ///                  
        /// Selected Keys: Use an esoteric key value (as opposed to key code)
        ///                e.g. 
        ///                     SystemMapOpen : Key_B
        ///                     GalaxyMapOpen : Key_M
        ///                   FocusRightPanel : Key_4
        ///                       SetSpeedZero: Key_0 + Key_RightShift (modifier)
        ///                
        /// </summary>
        /// <param name="xdoc"></param>
        /// <param name="devicepriority"></param>
        /// <returns></returns>
        private static DataTable GetEDKeyBindings(XDocument xdoc, Enums.EliteDangerousDevicePriority devicepriority)
        {
            // Initialise ..
            const string XMLRoot = "Root";
            const string XMLKey = "Key";
            const string XMLDevice = "Device";
            const string XMLModifier = "Modifier";
            string[] keybindingIndicator = { "Key_" };
            string devicePriority = devicepriority.ToString();

            // Datatable to hold tabulated XML contents ..
            DataTable keybinder = new DataTable();
            keybinder.DefineKeyBinder();

            // traverse config XML and gather pertinent element data arranged in row(s) of anonymous types ..
            // Scan all child nodes from top-level node ..
            foreach (var childNode in xdoc.Element(XMLRoot).Elements())
            {
                // can only process if child node itself has children ..
                if (childNode.DescendantNodes().Any())
                {
                    var xmlExtracts = from item in xdoc.Descendants(childNode.Name)
                                     where
                                           item.Element(devicePriority).SafeAttributeValue(XMLDevice) == Enums.GameInteraction.Keyboard.ToString() &&
                                           item.Element(devicePriority).Attribute(XMLKey).Value.Contains(keybindingIndicator[0]) == true
                                     select
                                        new // create anonymous type for every key code ..
                                          {
                                              //---------------------------------------------------------------------------------
                                              // Priority ..
                                              //---------------------------------------------------------------------------------
                                              xmlNode_DevicePriority = item.Element(devicePriority).Attribute(XMLKey).Parent.Name,
                                            
                                              //---------------------------------------------------------------------------------
                                              // Main Key Binding ..
                                              //---------------------------------------------------------------------------------
                                              xmlNode_Device = item.Element(devicePriority).SafeAttributeName(XMLDevice),
                                              xmlNode_Key = item.Element(devicePriority).SafeAttributeName(XMLKey),
                                              DeviceType = item.Element(devicePriority).SafeAttributeValue(XMLDevice),
                                              KeyValueFull = item.Element(devicePriority).SafeAttributeValue(XMLKey),
                                              KeyValue = item.Element(devicePriority).SafeAttributeValue(XMLKey) != string.Empty ? item.Element(devicePriority).SafeAttributeValue(XMLKey).Substring(4) : string.Empty,

                                              //---------------------------------------------------------------------------------
                                              // Modifier Key Binding (should it exist) ..
                                              //---------------------------------------------------------------------------------
                                              xmlNode_Modifier = item.Element(devicePriority).Element(XMLModifier).SafeElementName(),
                                              xmlNode_ModifierDevice = item.Element(devicePriority).Element(XMLModifier).SafeAttributeName(XMLDevice),
                                              xmlNode_ModifierKey = item.Element(devicePriority).Element(XMLModifier).SafeAttributeName(XMLKey),
                                              ModifierDeviceType = item.Element(devicePriority).Element(XMLModifier).SafeAttributeValue(XMLDevice),
                                              ModifierKeyValueFull = item.Element(devicePriority).Element(XMLModifier).SafeAttributeValue(XMLKey),
                                              ModifierKeyValue = item.Element(devicePriority).Element(XMLModifier).SafeAttributeValue(XMLKey) != string.Empty ? item.Element(devicePriority).Element(XMLModifier).SafeAttributeValue(XMLKey).Substring(4) : string.Empty
                                          };

                    // insert anonymous type row data (with some additional values) into DataTable ..
                    foreach (var xmlExtract in xmlExtracts)
                    {
                        string customKeyId = childNode.Name + D +
                                             xmlExtract.xmlNode_DevicePriority + D +
                                             xmlExtract.xmlNode_Device + D +
                                             xmlExtract.DeviceType + D +
                                             xmlExtract.xmlNode_Key + D +
                                             xmlExtract.KeyValueFull;

                        string customModifierKeyId = string.Empty;
                        if (xmlExtract.xmlNode_Modifier != string.Empty)
                        {
                            customModifierKeyId = childNode.Name + D +
                                                  xmlExtract.xmlNode_DevicePriority + D +
                                                  xmlExtract.xmlNode_Modifier + D +
                                                  xmlExtract.xmlNode_ModifierDevice + D +
                                                  xmlExtract.ModifierDeviceType + D +
                                                  xmlExtract.xmlNode_ModifierKey + D +
                                                  xmlExtract.ModifierKeyValueFull;
                        }

                        keybinder.LoadDataRow(new object[] 
                                                        {
                                                         Enums.GameName.EliteDangerous.ToString(), //Context
                                                         KeyMap.KeyType.ToString(), //KeyEnumerationType
                                                         childNode.Name, //BindingAction
                                                         xmlExtract.xmlNode_DevicePriority, //Priority 
                                                         xmlExtract.KeyValue, //KeyGameValue
                                                         exchange.GetValue(xmlExtract.KeyValue), //KeyEnumerationValue
                                                         KeyMap.GetKey(xmlExtract.KeyValue), //KeyEnumerationCode
                                                         customKeyId, //KeyId
                                                         xmlExtract.ModifierKeyValue, //ModifierKeyGameValue
                                                         exchange.GetValue(xmlExtract.ModifierKeyValue), //ModifierKeyEnumerationValue
                                                         KeyMap.GetKey(xmlExtract.ModifierKeyValue), //ModifierKeyEnumerationCode
                                                         customModifierKeyId //ModifierId
                                                        },
                                               false);
                    }
                }
            }

            // return Datatable ..
            return keybinder;
        }

        /// <summary>
        /// Process Elite Dangerous Config File to return all possible binding actions
        /// </summary>
        /// <param name="xdoc"></param>
        /// <param name="devicepriority"></param>
        /// <returns></returns>
        private static DataTable GetEDBindingActions(XDocument xdoc)
        {
            // Initialise ..
            const string XMLRoot = "Root";
            const string XMLDevice = "Device";
            string[] devicePriority = { Enums.EliteDangerousDevicePriority.Primary.ToString(), Enums.EliteDangerousDevicePriority.Secondary.ToString() };

            // Datatable to hold tabulated XML contents ..
            DataTable bindingactions = new DataTable();
            bindingactions.DefineBindingActions();

            // traverse config XML and gather pertinent element data arranged in row(s) of anonymous types ..
            // Scan all child nodes from top-level node ..
            foreach (var childNode in xdoc.Element(XMLRoot).Elements())
            {
                // can only process if child node itself has children ..
                if (childNode.DescendantNodes().Any())
                {
                    // Get all primary devices ..
                    var primaryDevices = from item in xdoc.Descendants(childNode.Name)
                                      where item.Element(devicePriority[0]).SafeElementName() == devicePriority[0]
                                      select
                                         new
                                           {
                                               BindingAction = item.SafeElementName(),
                                               Priority = item.Element(devicePriority[0]).SafeElementName(),
                                               DeviceType = item.Element(devicePriority[0]).SafeAttributeValue(XMLDevice)
                                           };

                    // Get all secondary devices ..
                    var secondaryDevices =  (from item in xdoc.Descendants(childNode.Name)
                                      where item.Element(devicePriority[1]).SafeElementName() == devicePriority[1]
                                      select
                                         new
                                           {
                                               BindingAction = item.SafeElementName(),
                                               Priority = item.Element(devicePriority[1]).SafeElementName(),
                                               DeviceType = item.Element(devicePriority[1]).SafeAttributeValue(XMLDevice)
                                           });

                    // Perform a 'union all' ...
                    var xmlExtracts = primaryDevices.Concat(secondaryDevices);

                    // insert anonymous type row data (with some additional values) into DataTable ..
                    foreach (var xmlExtract in xmlExtracts)
                    {
                        bindingactions.LoadDataRow(new object[] 
                                                        {
                                                         Enums.GameName.EliteDangerous.ToString(), //Context
                                                         xmlExtract.BindingAction, //BindingAction
                                                         xmlExtract.Priority, // Device priority
                                                         xmlExtract.DeviceType // Device binding applied to
                                                        },
                                               false);
                    }
                }
            }

            // return Datatable ..
            return bindingactions;
        }
    }
}
