namespace Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;

    /// <summary>
    /// Implement Key Value/Code lookup dictionary ..
    /// </summary>
    public sealed class KeyMapper
    {
        // Initialise class-wide scope variables ..
        public Enums.KeyEnumType KeyType;
        private Dictionary<string, int> KeyMap = new Dictionary<string, int>();
        private KeyExchange EDKeyXChg = new KeyExchange();

        /// <summary>
        /// Constructor
        /// </summary>
        public KeyMapper(Enums.KeyEnumType keytype)
        {
            switch (keytype)
            {
                case Enums.KeyEnumType.WindowsForms:
                    InitialiseKeyMap_WindowsForms();
                    break;
                case Enums.KeyEnumType.Console:
                    InitialiseKeyMap_Console();
                    break;
                default:
                    InitialiseKeyMap_Console();
                    break;
            }

            EDKeyXChg.Initialise(keytype);
            this.KeyType = keytype;
        }

        /// <summary>
        /// Get Key Value from Key Code ..
        /// </summary>
        /// {Dictionary Key}
        /// <param name="KeyCode"></param>
        /// <returns></returns>
        public string GetValue(int KeyCode)
        {
            string KeyValue = string.Empty;

            try
            {
                // Attempt to pull out value from dictionary for KeyCode index ..
                KeyValue = KeyMap.FirstOrDefault(x => x.Value == KeyCode).Key;

                // Force a throw null reference exception for unknown key-code ..
                if (KeyValue.Trim() == string.Empty || KeyValue.Trim() == null){}
            }
            catch
            {
                Console.WriteLine("*** No VALUE for key CODE:[{0}] ***", KeyCode.ToString());
                KeyValue = "*** " + KeyCode.ToString() + ":UNKNOWN ***"; 
            }

            // Return ..
            return KeyValue;
        }

        /// <summary>
        /// Get Key Code from Key Value
        /// </summary>
        /// {Dictionary Value}
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public int GetKey(string KeyValue)
        {
            try
            {
                // Handle numerics which require prefixing with either 'D' or 'NumPad' ..
                int iJunk;
                if (Int32.TryParse(KeyValue, out iJunk))
                {
                   // KeyValue = "NumPad" + KeyValue;
                    KeyValue = "D" + KeyValue;
                }

                // Handle misc ..
                if (this.KeyType == Enums.KeyEnumType.WindowsForms)
                {
                    if (KeyValue == "Enter") KeyValue = "Return";
                    if (KeyValue == "Backspace") KeyValue = "Back";
                }

                return this.KeyMap[KeyValue];
            }
            catch
            {
                if (KeyValue == string.Empty)
                {
                    return -1;
                }
                else
                {
                    // Test to see if KeyValue can be found in Exchange dictionary ...
                    try
                    {
                        // Examine key at Exchange ..
                        string XKeyValue = EDKeyXChg.GetValue(KeyValue);

                        // If value from Exchange is different, something must have been found ..
                        if (XKeyValue != KeyValue) 
                        {
                            // Perform recursive check and see if a code can be found using Exchange key value ..
                            return GetKey(XKeyValue);
                        }
                        else
                        {
                            return -999;
                        }
                    }
                    catch
                    {
                        // Nothing found to map to using Exchange value ..
                        Console.WriteLine("*** No CODE for key VALUE:[{0}] ***", KeyValue);
                        return -999;
                    }
                }
            }
        }

        /// <summary>
        /// Display derived KeyMap dictionary
        /// </summary>
        public void DisplayKeyMap()
        {
            foreach (KeyValuePair<string, int> kvp in KeyMap)
            {
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Use Keys Enum to create a lookup dictionary for windows form keys and their codes ..
        /// ref: https://msdn.microsoft.com/en-us/library/system.windows.forms.keys(v=vs.110).aspx
        /// ref: http://www.theasciicode.com.ar/
        ///
        /// This enumeration has a FlagsAttribute attribute that allows a bitwise combination of its member values.
        /// e.g.
        ///     Keys keyData;
        ///     if (keyData == Keys.Alt) -- doesn't necessarily detect if ALT key was pressed
        ///     if((keyData & Keys.Alt) == Keys.Alt) {...} -- using a bitwise test will
        ///     if (IsBitSet(keyData, Keys.Alt)) {...}
        ///
        /// ref: http://stackoverflow.com/questions/4850/c-sharp-and-arrow-keys/2033811#2033811
        /// </summary>
        /// <returns></returns>
        private void InitialiseKeyMap_WindowsForms()
        {
            // Initialise lists of key names and codes ..
            var keyEnums = Enum.GetValues(typeof(Keys)).Cast<Keys>().Distinct();
            var keyNames = keyEnums.ToList();
            var keyCodes = keyEnums.Cast<int>().ToList();

            // Add key names and codes to dictionary ..
            for (int i = 0; i < keyNames.Count; i++)
            {
                KeyMap.Add(keyNames[i].ToString(), keyCodes[i]);
            }
        }

        /// <summary>
        /// Use Keys Enum to create a lookup dictionary for console keys and their codes ..
        /// ref: https://msdn.microsoft.com/en-us/library/system.consolekey(v=vs.100).aspx
        /// ref: http://www.theasciicode.com.ar/
        /// </summary>
        /// <returns></returns>
        private void InitialiseKeyMap_Console()
        {
            // Initialise lists of key names and codes ..
            var keyEnums = Enum.GetValues(typeof(ConsoleKey)).Cast<ConsoleKey>().Distinct();
            var keyNames = keyEnums.ToList();
            var keyCodes = keyEnums.Cast<int>().ToList();

            // Add key names and codes to dictionary ..
            for (int i = 0; i < keyNames.Count; i++)
            {
                KeyMap.Add(keyNames[i].ToString(), keyCodes[i]);
            }
        }
    }
}
