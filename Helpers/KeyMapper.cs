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
        private Dictionary<string, int> keyMap = new Dictionary<string, int>();
        private KeyExchange exchange = new KeyExchange();

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyMapper"/> class
        /// </summary>
        /// <param name="keytype"></param>
        public KeyMapper(Enums.KeyEnumType keytype)
        {
            switch (keytype)
            {
                case Enums.KeyEnumType.WindowsForms:
                    this.InitialiseKeyMap_WindowsForms();
                    break;
                case Enums.KeyEnumType.Console:
                    this.InitialiseKeyMap_Console();
                    break;
                default:
                    this.InitialiseKeyMap_Console();
                    break;
            }

            this.exchange.Initialise(keytype);
            this.KeyType = keytype;
        }

        public Enums.KeyEnumType KeyType { get; set; }

        /// <summary>
        /// Get Key Value from Key Code ..
        /// </summary>
        /// {Dictionary Key}
        /// <param name="keyCode"></param>
        /// <returns></returns>
        public string GetValue(int keyCode)
        {
            string keyValue = string.Empty;

            try
            {
                // Attempt to pull out value from dictionary for KeyCode index ..
                keyValue = this.keyMap.FirstOrDefault(x => x.Value == keyCode).Key;

                // Force a throw null reference exception for unknown key-code ..
                if (keyValue.Trim() == string.Empty || keyValue.Trim() == null) { }
            }
            catch
            {
                Console.WriteLine("*** No VALUE for key CODE:[{0}] ***", keyCode.ToString());
                keyValue = "*** " + keyCode.ToString() + ":UNKNOWN ***"; 
            }

            // Return ..
            return keyValue;
        }

        /// <summary>
        /// Get Key Code from Key Value
        /// </summary>
        /// {Dictionary Value}
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public int GetKey(string keyValue)
        {
            try
            {
                // Handle numerics which require prefixing with either 'D' or 'NumPad' ..
                int junk;
                if (int.TryParse(keyValue, out junk))
                {
                   // KeyValue = "NumPad" + KeyValue;
                    keyValue = "D" + keyValue;
                }

                // Handle misc ..
                if (this.KeyType == Enums.KeyEnumType.WindowsForms)
                {
                    if (keyValue == "Enter") { keyValue = "Return"; }
                    if (keyValue == "Backspace") { keyValue = "Back"; }
                }

                return this.keyMap[keyValue];
            }
            catch
            {
                if (keyValue == string.Empty)
                {
                    return -1;
                }
                else
                {
                    // Test to see if KeyValue can be found in Exchange dictionary ...
                    try
                    {
                        // Examine key at Exchange ..
                        string xkeyValue = this.exchange.GetValue(keyValue);

                        // If value from Exchange is different, something must have been found ..
                        if (xkeyValue != keyValue) 
                        {
                            // Perform recursive check and see if a code can be found using Exchange key value ..
                            return this.GetKey(xkeyValue);
                        }
                        else
                        {
                            Console.WriteLine("*** No CODE for key VALUE [{0}] at the Exchange ***", keyValue);
                            return -998;
                        }
                    }
                    catch
                    {
                        // Nothing found to map to using Exchange value ..
                        Console.WriteLine("*** No CODE for key VALUE:[{0}] ***", keyValue);
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
            foreach (KeyValuePair<string, int> kvp in this.keyMap)
            {
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Use Keys Enum to create a lookup dictionary for windows form keys and their codes ..
        /// </summary>
        /// <remarks>
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
        /// </remarks>
        private void InitialiseKeyMap_WindowsForms()
        {
            // Initialise lists of key names and codes ..
            var keyEnums = Enum.GetValues(typeof(Keys)).Cast<Keys>().Distinct();
            var keyNames = keyEnums.ToList();
            var keyCodes = keyEnums.Cast<int>().ToList();

            // Add key names and codes to dictionary ..
            for (int i = 0; i < keyNames.Count; i++)
            {
                this.keyMap.Add(keyNames[i].ToString(), keyCodes[i]);
            }
        }

        /// <summary>
        /// Use Keys Enum to create a lookup dictionary for console keys and their codes ..
        /// ref: https://msdn.microsoft.com/en-us/library/system.consolekey(v=vs.100).aspx
        /// ref: http://www.theasciicode.com.ar/
        /// </summary>
        private void InitialiseKeyMap_Console()
        {
            // Initialise lists of key names and codes ..
            var keyEnums = Enum.GetValues(typeof(ConsoleKey)).Cast<ConsoleKey>().Distinct();
            var keyNames = keyEnums.ToList();
            var keyCodes = keyEnums.Cast<int>().ToList();

            // Add key names and codes to dictionary ..
            for (int i = 0; i < keyNames.Count; i++)
            {
                this.keyMap.Add(keyNames[i].ToString(), keyCodes[i]);
            }
        }
    }
}
