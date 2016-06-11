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
    public sealed class Mapper
    {
        // Initialise class-wide scope variables ..
        private Dictionary<string, int> activeKeyEnum = new Dictionary<string, int>();
        private GameKeyExchanger keyExchanger = new GameKeyExchanger();
        private MapperDictionary keyEnum = new MapperDictionary();

        /// <summary>
        /// Initializes a new instance of the <see cref="Mapper"/> class
        /// </summary>
        /// <param name="keytype"></param>
        public Mapper(Enums.InputKeyEnumType keytype)
        {
            this.activeKeyEnum = this.keyEnum.Get(keytype);
            this.keyExchanger.Initialise(Helper.Enums.Game.EliteDangerous);
            this.KeyType = keytype;
        }

        /// <summary>
        /// Gets Key Type
        /// </summary>
        public Enums.InputKeyEnumType KeyType
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
        public string GetValue(int keyCode)
        {
            string keyValue = string.Empty;

            try
            {
                // Attempt to pull out value from dictionary for KeyCode index ..
                keyValue = this.activeKeyEnum.FirstOrDefault(x => x.Value == keyCode).Key;

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
        /// Get Elite Dangerous Binding Value
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        public string GetEDBindingValue(int keyCode)
        {
            // Initialise ..
            string keyValue = string.Empty;
            string keyValueED = string.Empty;

            // Attempt to pull out key value from dictionary using key code index ..
            try
            {
                keyValue = this.activeKeyEnum.FirstOrDefault(x => x.Value == keyCode).Key;

                // Force a throw null reference exception for unknown key-code ..
                if (keyValue.Trim() == string.Empty || keyValue.Trim() == null) { }

                // Compare keyValue with virtual key value ..                
                string virtualKeyValue = VirtualMapper.GetUnicodeValueFromWindowsInputKeyEnumValueWithOptionalModifiers(keyValue, false, true, false);
                
                // If they match, then current value is not a special character ..
                if (keyValue == virtualKeyValue)
                {
                    keyValueED = keyValue;
                }
                else
                {
                    // Test to see if KeyValue can be found in Exchange dictionary ...
                    try
                    {
                        string exchangekeyValue = this.keyExchanger.GetKey(keyValue);
                        if (exchangekeyValue != null)
                        {
                            keyValueED = exchangekeyValue;
                        }
                        else
                        {
                            keyValueED = keyValue;
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
            return keyValueED;
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
                return this.activeKeyEnum[keyValue];
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
                        string exchangekeyValue = this.keyExchanger.GetValue(keyValue);

                        // If value from Exchange is different, something must have been found ..
                        if (exchangekeyValue != keyValue) 
                        {
                            // Perform recursive check and see if a code can be found using Exchange key value ..
                            return this.GetKey(exchangekeyValue);
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
            foreach (KeyValuePair<string, int> kvp in this.activeKeyEnum)
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
            foreach (KeyValuePair<string, int> kvp in this.activeKeyEnum)
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