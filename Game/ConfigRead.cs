namespace Game
{
    using System;
    using System.Linq;
    using System.Xml.Linq;
    using System.Data;
    using Helpers;

    public static class ConfigRead
    {
        // Determine what kind of Key mapping to use ..
        static Enums.KeyType KeyType = Enums.KeyType.WindowsForms
            ;
        static readonly KeyboardMap KeyMap = new KeyboardMap(KeyType);

        // Initialise Dictionary ..
        static DataTable KeyBindingsTable = new DataTable();

        /// <summary>
        /// Constructor initialises DataTable structure ..
        /// </summary>
        static ConfigRead()
        {
            DefineKeyBindingsTableStructure(KeyBindingsTable);
        }

        /// <summary>
        /// Parse Elite Dangerous Key Bindings into DataTable
        /// </summary>
        /// <param name="cfgFilePath"></param>
        /// <returns></returns>
        public static DataTable EliteDangerous(string cfgFilePath)
        {
            // Load configuration file as xml object ..
            var EDCfg = Xml.ReadXDoc(cfgFilePath);

            // Read configuration xml and convert to DataTable ..
            DataTable primary = ExtractKeyBindings_EliteDangerous(EDCfg, Enums.EliteDangerousDevicePriority.Primary);
            DataTable secondary = ExtractKeyBindings_EliteDangerous(EDCfg, Enums.EliteDangerousDevicePriority.Secondary);

            // Merge ..
            primary.Merge(secondary);

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
            // Load configuration file as xml object .. 
            var VACfg = Xml.ReadXDoc(cfgFilePath);

            // Return as DataTable  ..
            return ExtractKeyBindings_VoiceAttack(VACfg);
        }

        /// <summary>
        /// Define KeyBindings DataTable Structure
        /// </summary>
        /// <param name="KeyBindings"></param>
        private static void DefineKeyBindingsTableStructure(DataTable KeyBindings)
        {
            KeyBindings.TableName = "KeyBindings";
            KeyBindings.Columns.Add("Context", typeof(string));
            KeyBindings.Columns.Add("KeyMappingType", typeof(string));
            KeyBindings.Columns.Add("KeyFunction", typeof(string));
            KeyBindings.Columns.Add("Priority", typeof(string));
            KeyBindings.Columns.Add("KeyValue", typeof(string));
            KeyBindings.Columns.Add("KeyModifier", typeof(string));
            KeyBindings.Columns.Add("KeyCode", typeof(string));
            KeyBindings.Columns.Add("Id", typeof(string));
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
        private static DataTable ExtractKeyBindings_VoiceAttack(XDocument xdoc)
        {
            // Initialise ..
            const string VAKeyBoardInteraction = "PressKey";
            const string KeyBindingContext = "VoiceAttack";

            // traverse config XML and gather pertinent element data arranged in row(s) of anonymous types ..
            var keyBindings = from item in xdoc.Descendants("Command")
                             where item.Element("ActionSequence").Element("CommandAction") != null &&
                                   item.Element("ActionSequence").Element("CommandAction").Element("ActionType").Value == VAKeyBoardInteraction
                            select 
                               new // create anonymous type for every key code ..
                                 {
                                    Commandstring = item.Element("CommandString").SafeElementValue(),
                                    Id = item.Element("ActionSequence").Element("CommandAction").Element("Id").SafeElementValue(),
                                    KeyCode = item.Element("ActionSequence").Element("CommandAction").Element("KeyCodes").Element("unsignedShort").SafeElementValue()
                                 };

            // insert anonymous type row data (with some additional values) into DataTable ..
            foreach (var keyBinding in keyBindings)
            {
                KeyBindingsTable.LoadDataRow(new object[] 
                                                {KeyBindingContext,
                                                 KeyMap.KeyType.ToString(),
                                                 keyBinding.Commandstring,
                                                 "N/A",
                                                 KeyMap.GetKeyValue(Int32.Parse(keyBinding.KeyCode)),
                                                 "Modifier:UNKNOWN",
                                                 keyBinding.KeyCode,
                                                 keyBinding.Id}
                                             , false);

            }

            // return Datatable ..
            return KeyBindingsTable;
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
        ///                  |_<Secondary/>
        ///                  |_<ToggleOn/>
        ///                  
        /// Selected Keys: Use actual Key value (as opposed to key code)
        ///                e.g. 
        ///                     SystemMapOpen : B
        ///                     GalaxyMapOpen : M
        ///                   FocusRightPanel : 4
        ///                
        /// </summary>
        /// <param name="xdoc"></param>
        private static DataTable ExtractKeyBindings_EliteDangerous(XDocument xdoc, Enums.EliteDangerousDevicePriority devicepriority)
        {
            // Initialise ..
            const string KeyBindingContext = "EliteDangerous";

            const string EDKeyBoardInteraction = "Keyboard";
            string DevicePriority = devicepriority.ToString();

            // traverse config XML and gather pertinent element data arranged in row(s) of anonymous types ..
            // Scan all child nodes from top-level node ..
            foreach (var childNode in xdoc.Element("Root").Elements())
            {
                // can only process if child node itself has children ..
                if (childNode.DescendantNodes().Any())
                {
                    var keyBindings = from item in xdoc.Descendants(childNode.Name)
                                     where
                                           item.Element(DevicePriority).SafeAttributeValue("Device") == EDKeyBoardInteraction &&
                                           item.Element(DevicePriority).Attribute("Key").Value.Contains("Key_") == true
                                     select
                                        new // create anonymous type for every key code ..
                                          {
                                            DevicePriority = item.Element(DevicePriority).Attribute("Key").Parent.Name,
                                            Device = item.Element(DevicePriority).Attribute("Device").Name,
                                            DeviceType = item.Element(DevicePriority).SafeAttributeValue("Device"),
                                            Key = item.Element(DevicePriority).Attribute("Key").Name,
                                            KeyValueFull = (item.Element(DevicePriority).SafeAttributeValue("Key")),
                                            KeyValue = (item.Element(DevicePriority).SafeAttributeValue("Key")).Substring(4)
                                          };

                    foreach (var keyBinding in keyBindings)
                    {
                        string CustomId = childNode.Name + "." +
                                          keyBinding.DevicePriority + "." +
                                          keyBinding.Device + "." +
                                          keyBinding.DeviceType + "." +
                                          keyBinding.Key + "." +
                                          keyBinding.KeyValueFull;

                        KeyBindingsTable.LoadDataRow(new object[] 
                                                        {KeyBindingContext,
                                                         KeyMap.KeyType.ToString(),
                                                         childNode.Name,
                                                         keyBinding.DevicePriority,
                                                         keyBinding.KeyValue,
                                                         "Modifier:UNKNOWN",
                                                         KeyMap.GetKeyCode((keyBinding.KeyValue)),
                                                         CustomId}
                                                     , false);
                    }
                }
            }

            // return Datatable ..
            return KeyBindingsTable;
        }     
    }
}
