namespace KeyHelper
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Helper;

    /// <summary>
    /// Key Value/Code Mapping ..
    /// </summary>
    public sealed class GameKeyAndSystemKeyConnector
    {
        // Initialise class-wide scope variables ..
        private Dictionary<string, int> currentKeyEnumType = new Dictionary<string, int>();
        private GameKeyAndSystemKeyDictionary gameKeys = new GameKeyAndSystemKeyDictionary();
        private SystemKeyTypeDictionary systemKeys = new SystemKeyTypeDictionary();

        /// <summary>
        /// Initializes a new instance of the <see cref="GameKeyAndSystemKeyConnector"/> class
        /// </summary>
        /// <param name="keytype"></param>
        public GameKeyAndSystemKeyConnector(EnumsKeyEnumType.InputKeyEnumType keytype)
        {
            this.currentKeyEnumType = this.systemKeys.Get(keytype);
            this.gameKeys.Initialise(Helper.EnumsInternal.Game.EliteDangerous);
            this.KeyType = keytype;
        }

        /// <summary>
        /// Gets Key Type
        /// </summary>
        public EnumsKeyEnumType.InputKeyEnumType KeyType
        {
            get;
            private set;
        }

        /// <summary>
        /// Get Key Value from Key Code ..
        /// </summary>
        /// {Dictionary Key}
        /// <param name="keyCode"></param>
        /// <returns></returns>
        public string GetKeyValue(int keyCode)
        {
            string keyValue = string.Empty;

            try
            {
                // Attempt to pull out value from dictionary for KeyCode index ..
                keyValue = this.currentKeyEnumType.FirstOrDefault(x => x.Value == keyCode).Key;

                // Force a throw null reference exception for unknown key-code ..
                if (keyValue.Trim() == string.Empty || keyValue.Trim() == null) { }
            }
            catch
            {
                keyValue = "*** " + keyCode.ToString() + ":UNKNOWN ***"; 
            }

            // Return ..
            return keyValue;
        }

        /// <summary>
        /// Get Elite Dangerous Binding Value from Key code
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        public string GetEliteDangerousKeyBinding(int keyCode)
        {
            // Initialise ..
            string keyValue = string.Empty;
            string keyBinding = string.Empty; // Elite Dangerous key binding value

            // Attempt to pull out key value from dictionary using key code index ..
            try
            {
                keyValue = this.currentKeyEnumType.FirstOrDefault(x => x.Value == keyCode).Key;

                // Force a throw null reference exception for unknown key-code ..
                if (keyValue.Trim() == string.Empty || keyValue.Trim() == null) { }

                // Compare keyValue with virtual key value ..                
                string virtualKeyValue = VirtualKeyCodeDictionary.GetUnicodeValueFromWindowsInputKeyEnumValueWithOptionalModifiers(keyValue, false, true, false);
                
                // If they match, then current value is not a special character ..
                if (keyValue == virtualKeyValue)
                {
                    keyBinding = keyValue;
                }
                else
                {
                    // Test to see if KeyValue can be found in game-keys dictionary ...
                    try
                    {
                        string exchangekeyValue = this.gameKeys.GetKey(keyValue);
                        if (exchangekeyValue != null)
                        {
                            keyBinding = exchangekeyValue;
                        }
                        else
                        {
                            keyBinding = keyValue;
                        }
                    }
                    catch
                    {
                        return StatusCode.NoEquivalentKeyFoundAtExchange.ToString();
                    }
                }
            }
            catch
            {
                keyValue = "*** " + keyCode.ToString() + ":UNKNOWN ***";
            }

            // Return ..
            return keyBinding;
        }

        /// <summary>
        /// Get Key Code from Key Value
        /// </summary>
        /// {Dictionary Value}
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public int GetKeyCode(string keyValue)
        {
            try
            {
                return this.currentKeyEnumType[keyValue];
            }
            catch
            {
                if (keyValue == string.Empty)
                {
                    return StatusCode.EmptyStringInt;
                }
                else
                {
                    // Test to see if KeyValue can be found in Exchange dictionary ...
                    try
                    {
                        // Examine key at Exchange ..
                        string exchangekeyValue = this.gameKeys.GetValue(keyValue);

                        // If value from Exchange is different, something must have been found ..
                        if (exchangekeyValue != keyValue) 
                        {
                            // Perform recursive check and see if a code can be found using Exchange key value ..
                            return this.GetKeyCode(exchangekeyValue);
                        }
                        else
                        {
                            return StatusCode.NoEquivalentKeyFoundAtExchange;
                        }
                    }
                    catch
                    {
                        // Nothing found to map to using Exchange value ..
                        return StatusCode.NoCodeFoundAfterExchange;
                    }
                }
            }
        }

        /// <summary>
        /// Display derived KeyMap dictionary
        /// </summary>
        public void DisplayKeyMap()
        {
            foreach (KeyValuePair<string, int> kvp in this.currentKeyEnumType)
            {
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Write Key Enumeration Dictionary as CSV
        /// </summary>
        /// <param name="csvPath"></param>
        public void WriteKeyMap(string csvPath)
        {
            // Create DataTable with correct structure ..
            DataTable keyMap = TableShape.DefineKeyMap();

            // Loop through dictionary adding information to DataTable ..
            foreach (KeyValuePair<string, int> kvp in this.currentKeyEnumType)
            {
                keyMap.LoadDataRow(new object[] 
                                                {
                                                 this.KeyType.ToString(),
                                                 kvp.Key,
                                                 kvp.Value
                                                },
                                   false); 
            }

            // Write DataTable contents as csv ..
            keyMap.CreateCSV(csvPath);
        }
    }
}