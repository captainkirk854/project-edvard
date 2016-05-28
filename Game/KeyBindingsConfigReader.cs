namespace Game
{
    using Helpers;
    using System.Data;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Reads and process Key Binding Configuration Files
    /// </summary>
    public static class KeyBindingsConfigReader
    {
        // Initialise ..
        private const string D = "+";
        private const string NA = "-";
        private const int INA = -2;
        private const string FilePath = "FilePath";
        private static readonly KeyMapper KeyMap = new KeyMapper(keyType);

        // Preset Key Map Enumeration to use ..
        private static Enums.KeyEnumType keyType = Enums.KeyEnumType.WindowsForms;

        /// <summary>
        /// Parse Elite Dangerous Key Bindings into DataTable
        /// </summary>
        /// <param name="cfgFilePath"></param>
        /// <returns></returns>
        public static DataTable EliteDangerous(string cfgFilePath)
        {
            // Load configuration file as xml document object ..
            var cfgED = Xml.ReadXDoc(cfgFilePath);

            // Read bindings and tabulate ..
            DataTable primary = ExtractKeyBindings_EliteDangerous(cfgED, Enums.EliteDangerousDevicePriority.Primary);            
            DataTable secondary = ExtractKeyBindings_EliteDangerous(cfgED, Enums.EliteDangerousDevicePriority.Secondary);

            // Merge ..
            primary.Merge(secondary);

            // Modify ..
            primary.AddDefaultColumn(FilePath, cfgFilePath);

            // Return merged DataTable contents ..
            return primary;
        }

        /// <summary>
        /// Parse Voice Attack Key Bindings into DataTable
        /// </summary>
        /// <param name="cfgFilePath"></param>
        /// <returns></returns>
        public static DataTable VoiceAttack(string cfgFilePath)
        {
            // Load configuration file as xml document object .. 
            var cfgVA = Xml.ReadXDoc(cfgFilePath);

            // Read bindings and tabulate ..
            DataTable primary = ExtractKeyBindings_VoiceAttack(cfgVA);

            // Modify ..
            primary.AddDefaultColumn(FilePath, cfgFilePath);

            // return Datatable ..
            return primary;
        }

        /// <summary>
        /// Define Key Bindings DataTable Structure
        /// </summary>
        /// <param name="keyBindings"></param>
        private static void DefineStructure(this DataTable keyBindings)
        {
            keyBindings.TableName = "KeyBindings";

            // Define table structure ..
            keyBindings.Columns.Add("Context", typeof(string));
            keyBindings.Columns.Add("KeyEnumerationType", typeof(string));
            keyBindings.Columns.Add("KeyFunction", typeof(string));
            keyBindings.Columns.Add("Priority", typeof(string));
            keyBindings.Columns.Add("KeyValue", typeof(string));
            keyBindings.Columns.Add("KeyCode", typeof(int));
            keyBindings.Columns.Add("KeyId", typeof(string));
            keyBindings.Columns.Add("ModifierKeyValue", typeof(string));
            keyBindings.Columns.Add("ModifierKeyCode", typeof(int));
            keyBindings.Columns.Add("ModifierKeyId", typeof(string));
        }

        /// <summary>
        /// Process Voice Attack Config File
        /// Format: XML
        ///             o <Profile/>
        ///               |_ <Commands/>
        ///                  |_<ActionSequences/>
        ///                     !_[some] <CommandActions/>
        ///                              |_<ActionType/>
        ///                             
        /// Selected Keys: Use actual Key code (as opposed to key value)
        ///                e.g. 
        ///                   ((Shield Cell)) : 222 (= Oem7 Numpad?7)
        ///                   ((Power To Weapons)) : 39  (= Right arrow)
        ///                   ((Select Target Ahead)) : 84 (= T)
        ///                   ((Flight Assist)) : 90 (= Z)
        /// </summary>
        /// <param name="xdoc"></param>
        /// <returns></returns>
        private static DataTable ExtractKeyBindings_VoiceAttack(XDocument xdoc)
        {
            // Initialise ..
            const string XMLCommand = "Command";
            const string XMLActionSequence = "ActionSequence";
            const string XMLCommandAction = "CommandAction";
 
            // Datatable to hold tabulated XML contents ..
            DataTable keybinder = new DataTable();
            keybinder.DefineStructure();

            // traverse config XML and gather pertinent element data arranged in row(s) of anonymous types ..
            var keyBindings = from item in xdoc.Descendants(XMLCommand)
                              where item.Element(XMLActionSequence).Element(XMLCommandAction) != null &&
                                   item.Element(XMLActionSequence).Element(XMLCommandAction).Element("ActionType").Value == Enums.Interaction.PressKey.ToString()
                            select 
                               new // create anonymous type for every key code ..
                                 {
                                    Commandstring = item.Element("CommandString").SafeElementValue(),
                                    Id = item.Element(XMLActionSequence).Element(XMLCommandAction).Element("Id").SafeElementValue(),
                                    KeyCode = item.Element(XMLActionSequence).Element(XMLCommandAction).Element("KeyCodes").Element("unsignedShort").SafeElementValue()
                                 };

            // insert anonymous type row data (with some additional values) into DataTable ..
            foreach (var keyBinding in keyBindings)
            {
                keybinder.LoadDataRow(new object[] 
                                                {
                                                 Enums.Game.VoiceAttack.ToString(), //Context
                                                 KeyMap.KeyType.ToString(), //KeyMappingType
                                                 keyBinding.Commandstring, //KeyFunction
                                                 NA, //Priority
                                                 KeyMap.GetValue(int.Parse(keyBinding.KeyCode)), //KeyValue
                                                 keyBinding.KeyCode, //KeyCode
                                                 keyBinding.Id, //KeyId
                                                 NA, //ModifierKeyValue
                                                 INA, //ModifierKeyCode
                                                 NA //ModifierId
                                                },
                                       false);
            }

            // return Datatable ..
            return keybinder;
        }

        /// <summary>
        /// Process Elite Dangerous Config File
        /// Keys can be in assigned with Primary or Secondary priorities
        /// Format: XML
        ///             o <Root/>
        ///               |_ <KeyboardLayout/>
        ///               |_ <things></things>.[Value] attribute
        ///               |_ <things/>
        ///                  |_<Binding/>
        ///                  |_<Inverted/>
        ///                  |_<Deadzone/>
        ///               |_ <things/>
        ///                  |_<Primary/>
        ///                  |_<Secondary/>
        ///               |_ <things/>
        ///                  |_<Primary/>
        ///                     |_<Modifier/>
        ///                  |_<Secondary/>
        ///                     |_<Modifier/>
        ///               |_ <things/>
        ///                  |_<Primary/>
        ///                  |_<Secondary/>
        ///                  |_<ToggleOn/>
        ///                  
        /// Selected Keys: Use actual Key value (as opposed to key code)
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
        private static DataTable ExtractKeyBindings_EliteDangerous(XDocument xdoc, Enums.EliteDangerousDevicePriority devicepriority)
        {
            // Initialise ..
            const string XMLKey = "Key";
            const string XMLDevice = "Device";
            const string XMLModifier = "Modifier";
            string devicePriority = devicepriority.ToString();

            // Datatable to hold tabulated XML contents ..
            DataTable keybinder = new DataTable();
            keybinder.DefineStructure();

            // traverse config XML and gather pertinent element data arranged in row(s) of anonymous types ..
            // Scan all child nodes from top-level node ..
            foreach (var childNode in xdoc.Element("Root").Elements())
            {
                // can only process if child node itself has children ..
                if (childNode.DescendantNodes().Any())
                {
                    var keyBindings = from item in xdoc.Descendants(childNode.Name)
                                     where
                                           item.Element(devicePriority).SafeAttributeValue(XMLDevice) == Enums.Interaction.Keyboard.ToString() &&
                                           item.Element(devicePriority).Attribute(XMLKey).Value.Contains("Key_") == true
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

                    foreach (var keyBinding in keyBindings)
                    {
                        string customKeyId = childNode.Name + D +
                                             keyBinding.xmlNode_DevicePriority + D +
                                             keyBinding.xmlNode_Device + D +
                                             keyBinding.DeviceType + D +
                                             keyBinding.xmlNode_Key + D +
                                             keyBinding.KeyValueFull;

                        string customModifierKeyId = string.Empty;
                        if (keyBinding.xmlNode_Modifier != string.Empty)
                        {
                            customModifierKeyId = childNode.Name + D +
                                                  keyBinding.xmlNode_DevicePriority + D +
                                                  keyBinding.xmlNode_Modifier + D +
                                                  keyBinding.xmlNode_ModifierDevice + D +
                                                  keyBinding.ModifierDeviceType + D +
                                                  keyBinding.xmlNode_ModifierKey + D +
                                                  keyBinding.ModifierKeyValueFull;
                        }

                        keybinder.LoadDataRow(new object[] 
                                                        {
                                                         Enums.Game.EliteDangerous.ToString(), //Context
                                                         KeyMap.KeyType.ToString(), //KeyMappingType
                                                         childNode.Name, //KeyFunction
                                                         keyBinding.xmlNode_DevicePriority, //Priority 
                                                         keyBinding.KeyValue, //KeyValue
                                                         KeyMap.GetKey(keyBinding.KeyValue), //KeyCode
                                                         customKeyId, //KeyId
                                                         keyBinding.ModifierKeyValue, //ModifierKeyValue
                                                         KeyMap.GetKey(keyBinding.ModifierKeyValue), //ModifierKeyCode
                                                         customModifierKeyId //ModifierId
                                                        },
                                               false);
                    }
                }
            }

            // return Datatable ..
            return keybinder;
        }     
    }
}
