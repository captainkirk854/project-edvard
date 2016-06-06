namespace Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    /// <summary>
    /// Implement Key Value/Code lookup dictionary ..
    /// </summary>
    public sealed class KeyMapper
    {
        // Initialise class-wide scope variables ..
        private Dictionary<string, int> relationship = new Dictionary<string, int>();
        private KeyMapperExchange exchange = new KeyMapperExchange();

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyMapper"/> class
        /// </summary>
        /// <param name="keytype"></param>
        public KeyMapper(Enums.InputKeyEnumType keytype)
        {
            switch (keytype)
            {
                case Enums.InputKeyEnumType.Console:
                    this.InitialiseKeyMap_Console();
                    break;

                case Enums.InputKeyEnumType.SharpDX:
                    this.InitialiseKeyMap_SharpDX();
                    break;

                case Enums.InputKeyEnumType.WindowsForms:
                    this.InitialiseKeyMap_WindowsForms();
                    break;

                default:
                    this.InitialiseKeyMap_Console();
                    break;
            }

            this.exchange.Initialise(keytype, Enums.Game.EliteDangerous);
            this.KeyType = keytype;
        }

        public Enums.InputKeyEnumType KeyType 
        { 
            get; 
            set; 
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
                keyValue = this.relationship.FirstOrDefault(x => x.Value == keyCode).Key;

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
        /// Get Key Code from Key Value
        /// </summary>
        /// {Dictionary Value}
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public int GetKey(string keyValue)
        {
            try
            {
                // Handle numerics which cannot be enumerated ..
                if (this.KeyType == Enums.InputKeyEnumType.WindowsForms)
                {
                    // Handle numerics which require prefixing with 'D' ..
                    int junk;
                    if (int.TryParse(keyValue, out junk))
                    {
                        keyValue = "D" + keyValue;
                    }
                }

                return this.relationship[keyValue];
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
                        string exchangekeyValue = this.exchange.GetValue(keyValue);

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
            foreach (KeyValuePair<string, int> kvp in this.relationship)
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
            DataTable keyMap = DefineKeyMap();

            // Loop through dictionary adding information to DataTable ..
            foreach (KeyValuePair<string, int> kvp in this.relationship)
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

        /// <summary>
        /// Define Binding Actions DataTable Structure
        /// </summary>
        /// <returns></returns>
        private static DataTable DefineKeyMap()
        {
            // New DataTable ..
            DataTable keyMap = new DataTable();
            keyMap.TableName = "KeyMap";

            // Define its structure ..
            keyMap.Columns.Add(Enums.Column.KeyEnumeration.ToString(), typeof(string));
            keyMap.Columns.Add(Enums.Column.KeyEnumerationValue.ToString(), typeof(string));
            keyMap.Columns.Add(Enums.Column.KeyEnumerationCode.ToString(), typeof(int));

            return keyMap;
        }

        /// <summary>
        /// Use Windows Forms Keys Enum to create a lookup dictionary for keys and their codes ..
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
            var keyEnums = Enum.GetValues(typeof(System.Windows.Forms.Keys)).Cast<System.Windows.Forms.Keys>().Distinct();
            var keyNames = keyEnums.ToList();
            var keyCodes = keyEnums.Cast<int>().ToList();

            // Add key names and codes to dictionary ..
            for (int i = 0; i < keyNames.Count; i++)
            {
                this.relationship.Add(keyNames[i].ToString(), keyCodes[i]);
            }
        }

        /// <summary>
        /// Use System.Console Keys Enum to create a lookup dictionary for keys and their codes ..
        /// ref: https://msdn.microsoft.com/en-us/library/system.consolekey(v=vs.100).aspx
        /// </summary>
        private void InitialiseKeyMap_Console()
        {
            // Initialise lists of key names and codes ..
            var keyEnums = Enum.GetValues(typeof(System.ConsoleKey)).Cast<System.ConsoleKey>().Distinct();
            var keyNames = keyEnums.ToList();
            var keyCodes = keyEnums.Cast<int>().ToList();

            // Add key names and codes to dictionary ..
            for (int i = 0; i < keyNames.Count; i++)
            {
                this.relationship.Add(keyNames[i].ToString(), keyCodes[i]);
            }
        }

        /// <summary>
        /// Use DirectX DirectInput Keys Enum to create a lookup dictionary for keys and their codes ..
        /// ref: https://msdn.microsoft.com/en-us/library/windows/desktop/bb321074(v=vs.85).aspx
        /// </summary>
        private void InitialiseKeyMap_SharpDX()
        {
            // Initialise lists of key names and codes ..
            var keyEnums = Enum.GetValues(typeof(SharpDX.DirectInput.Key)).Cast<SharpDX.DirectInput.Key>().Distinct();
            var keyNames = keyEnums.ToList();
            var keyCodes = keyEnums.Cast<int>().ToList();

            // Add key names and codes to dictionary ..
            for (int i = 0; i < keyNames.Count; i++)
            {
                this.relationship.Add(keyNames[i].ToString(), keyCodes[i]);
            }
        }
    }
}
