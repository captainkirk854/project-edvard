namespace Helpers
{
    using System;
    using System.Linq;
    using System.Xml.Linq;

    public static class GameConfig
    {
        static KeyboardMap KeyMap = new KeyboardMap();

        public static void EliteDangerous(string cfgFilePath)
        {
            var EDCfg = ReadConfig(cfgFilePath);
            ExtractKeyBindingsFromEliteDangerous(EDCfg, "Primary");
            ExtractKeyBindingsFromEliteDangerous(EDCfg, "Secondary");
        }

        public static void VoiceAttack(string cfgFilePath)
        {
            var VACfg = ReadConfig(cfgFilePath);
            ExtractKeyBindingsFromVoiceAttack(VACfg);
        }

        private static XDocument ReadConfig(string cfgFilePath)
        {
            return XDocument.Load(cfgFilePath);
        }

        /// <summary>
        /// Process Voice Attack Config File
        /// Format: XML
        ///             o <Profile/>
        ///               |_ <Commands/>
        ///                  |_<ActionSequences/>
        ///                     !_[some] <CommandActions/>
        ///                              |_<ActionType/>
        /// </summary>
        /// <param name="xdoc"></param>
        private static void ExtractKeyBindingsFromVoiceAttack (XDocument xdoc)
        {
            const string VAKeyBoardInteraction = "PressKey";

            var keyBindingData = from item in xdoc.Descendants("Command")
                                where item.Element("ActionSequence").Element("CommandAction") != null &&
                                      item.Element("ActionSequence").Element("CommandAction").Element("ActionType").Value == VAKeyBoardInteraction
                               select 
                                  new
                                    {
                                        Commandstring = item.Element("CommandString").SafeElementValue(),
                                        Id = item.Element("ActionSequence").Element("CommandAction").Element("Id").SafeElementValue(),
                                        KeyCode = item.Element("ActionSequence").Element("CommandAction").Element("KeyCodes").Element("unsignedShort").SafeElementValue()
                                    };

            foreach (var keyBinding in keyBindingData)
            {
                Console.WriteLine("Binding for [{0}] [{1}]", keyBinding.Commandstring, keyBinding.Id);
                Console.WriteLine("{0} = {1}", keyBinding.KeyCode, KeyMap.GetKeyValue(Int32.Parse(keyBinding.KeyCode)));
                Console.WriteLine();

             ///   Console.ReadKey();
            }        
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
        /// </summary>
        /// <param name="xdoc"></param>
        private static void ExtractKeyBindingsFromEliteDangerous(XDocument xdoc, string DevicePriority)
        {
            const string EDKeyBoardInteraction = "Keyboard";

            // Scan all child nodes from top-level node ..
            foreach (var childNode in xdoc.Element("Root").Elements())
            {
                // if child node itself has children ..
                if (childNode.DescendantNodes().Any())
                {
                   // Console.WriteLine("Scanning {0}", childNode.Name);

                    var keyBindingData = from item in xdoc.Descendants(childNode.Name)
                                        where
                                                item.Element(DevicePriority).SafeAttributeValue("Device") == EDKeyBoardInteraction &&
                                                item.Element(DevicePriority).Attribute("Key").Value.Contains("Key_") == true
                                        select
                                            new
                                            {
                                                DevicePriority = item.Element(DevicePriority).Attribute("Key").Parent.Name,
                                                DeviceType = item.Element(DevicePriority).SafeAttributeValue("Device"),
                                                Key = item.Element(DevicePriority).Attribute("Key").Name,
                                                KeyValue = (item.Element(DevicePriority).SafeAttributeValue("Key")).Substring(4)
                                            };

                    foreach (var keyBinding in keyBindingData)
                    {
                        Console.WriteLine("Binding for [{0}]", childNode.Name);
                        Console.WriteLine("{0} {1} {2} ({3}) =======> {4}", keyBinding.DevicePriority, keyBinding.DeviceType, keyBinding.Key, keyBinding.KeyValue, KeyMap.GetKeyCode((keyBinding.KeyValue)));
                        Console.WriteLine();
                      //  Console.ReadKey();
                    }
                }
            }
        }     
    }
}
