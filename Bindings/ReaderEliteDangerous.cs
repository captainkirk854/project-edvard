namespace Bindings
{
    using Helpers;
    using System.Data;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// BindingsReader content for parsing Elite Dangerous Bind file(s)
    /// </summary>
    public static partial class Reader
    {
        //Initialise ..
        private const string XMLRoot = "Root";
        private const string XMLKey = "Key";
        private const string XMLDevice = "Device";
        private const string XMLModifier = "Modifier";
        private static KeyMapperExchange exchange = new KeyMapperExchange(KeyType, Enums.Game.EliteDangerous);
        private static string[] keybindingIndicatorED = { "Key_" };

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
            /*
            const string XMLRoot = "Root";
            const string XMLKey = "Key";
            const string XMLDevice = "Device";
            const string XMLModifier = "Modifier";
            string[] keybindingIndicator = { "Key_" };
            */
            string devicePriority = devicepriority.ToString();

            // Datatable to hold tabulated XML contents ..
            DataTable keyactionbinder = TableType.KeyActionBinder();

            // traverse config XML and gather pertinent element data arranged in row(s) of anonymous types ..
            // Scan all child nodes from top-level node ..
            foreach (var childNode in xdoc.Element(XMLRoot).Elements())
            {
                // can only process if child node itself has children ..
                if (childNode.DescendantNodes().Any())
                {
                    var xmlExtracts = from item in xdoc.Descendants(childNode.Name)
                                      where
                                            item.Element(devicePriority).SafeAttributeValue(XMLDevice) == Enums.KeyboardInteraction.Keyboard.ToString() &&
                                            item.Element(devicePriority).Attribute(XMLKey).Value.Contains(keybindingIndicatorED[0]) == true
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

                        keyactionbinder.LoadDataRow(new object[] 
                                                        {
                                                         Enums.Game.EliteDangerous.ToString(), //Context
                                                         KeyMapper.KeyType.ToString(), //KeyEnumerationType
                                                         childNode.Name, //BindingAction
                                                         xmlExtract.xmlNode_DevicePriority, //Priority 
                                                         xmlExtract.KeyValue, //KeyGameValue
                                                         exchange.GetValue(xmlExtract.KeyValue), //KeyEnumerationValue
                                                         KeyMapper.GetKey(xmlExtract.KeyValue), //KeyEnumerationCode
                                                         customKeyId, //KeyId
                                                         xmlExtract.ModifierKeyValue, //ModifierKeyGameValue
                                                         exchange.GetValue(xmlExtract.ModifierKeyValue), //ModifierKeyEnumerationValue
                                                         KeyMapper.GetKey(xmlExtract.ModifierKeyValue), //ModifierKeyEnumerationCode
                                                         customModifierKeyId //ModifierId
                                                        },
                                               false);
                    }
                }
            }

            // return Datatable ..
            return keyactionbinder;
        }

        /// <summary>
        /// Process Elite Dangerous Config File to return all possible bindable actions
        /// </summary>
        /// <param name="xdoc"></param>
        /// <returns></returns>
        private static DataTable GetEDBindingActions(XDocument xdoc)
        {
            // Initialise ..
            string[] devicePriority = { Enums.EliteDangerousDevicePriority.Primary.ToString(), Enums.EliteDangerousDevicePriority.Secondary.ToString() };

            // Datatable to hold tabulated XML contents ..
            DataTable bindableactions = TableType.BindableActions();

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
                    var secondaryDevices = from item in xdoc.Descendants(childNode.Name)
                                           where item.Element(devicePriority[1]).SafeElementName() == devicePriority[1]
                                           select
                                              new
                                              {
                                                BindingAction = item.SafeElementName(),
                                                Priority = item.Element(devicePriority[1]).SafeElementName(),
                                                DeviceType = item.Element(devicePriority[1]).SafeAttributeValue(XMLDevice)
                                              };

                    // Perform a 'union all' ...
                    var xmlExtracts = primaryDevices.Concat(secondaryDevices);

                    // insert anonymous type row data (with some additional values) into DataTable ..
                    foreach (var xmlExtract in xmlExtracts)
                    {
                        bindableactions.LoadDataRow(new object[] 
                                                        {
                                                         Enums.Game.EliteDangerous.ToString(), //Context
                                                         xmlExtract.BindingAction, //BindingAction
                                                         xmlExtract.Priority, // Device priority
                                                         xmlExtract.DeviceType // Device binding applied to
                                                        },
                                               false);
                    }
                }
            }

            // return Datatable ..
            return bindableactions;
        }
    }
}
