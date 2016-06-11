namespace KeyHelper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class MapperDictionary
    {
        private Dictionary<string, int> keyEnum = new Dictionary<string, int>();

        /// <summary>
        /// Get Key Enumeration Dictionary for KeyType 
        /// </summary>
        /// <param name="keytype"></param>
        /// <returns></returns>
        public Dictionary<string, int> Get(Enums.InputKeyEnumType keytype)
        {
            switch (keytype)
            {
                case Enums.InputKeyEnumType.Console:
                    this.keyEnum = this.Console();
                    break;

                case Enums.InputKeyEnumType.WindowsForms:
                    this.keyEnum = this.WindowsForms();
                    break;

                case Enums.InputKeyEnumType.WindowsInput:
                    this.keyEnum = this.WindowsInput();
                    break;

                case Enums.InputKeyEnumType.SharpDX:
                    this.keyEnum = this.SharpDX();
                    break;

                default:
                    this.keyEnum = this.WindowsForms();
                    break;
            }

            return this.keyEnum;
        }

        /// <summary>
        /// Use Windows Forms Keys Enum to create a lookup dictionary for key values and codes ..
        /// </summary>
        /// <remarks>
        /// ref: https://msdn.microsoft.com/en-us/library/system.windows.forms.keys(v=vs.110).aspx
        /// ref: http://stackoverflow.com/questions/4850/c-sharp-and-arrow-keys/2033811#2033811
        ///
        /// This enumeration has a FlagsAttribute attribute that allows a bitwise combination of its member values.
        /// e.g.
        ///     Keys keyData;
        ///     if (keyData == Keys.Alt) -- doesn't necessarily detect if ALT key was pressed
        ///     if((keyData & Keys.Alt) == Keys.Alt) {...} -- using a bitwise test will
        ///     if (IsBitSet(keyData, Keys.Alt)) {...}
        /// </remarks>
        /// <returns></returns>
        private Dictionary<string, int> WindowsForms()
        {
            return this.MakeDictionary(Enum.GetValues(typeof(System.Windows.Forms.Keys)).Cast<System.Windows.Forms.Keys>().Distinct());
        }

        /// <summary>
        /// Use System.Console Keys Enum to create a lookup dictionary for key values and codes ..
        /// </summary>
        /// <remarks>
        /// ref: https://msdn.microsoft.com/en-us/library/system.consolekey(v=vs.100).aspx
        /// </remarks>
        /// <returns></returns>
        private Dictionary<string, int> Console()
        {
            return this.MakeDictionary(Enum.GetValues(typeof(System.ConsoleKey)).Cast<System.ConsoleKey>().Distinct());
        }

        /// <summary>
        /// Use System.Windows Input Keys Enum to create a lookup dictionary for key values and codes ..
        /// </summary>
        /// <remarks>
        /// ref: https://msdn.microsoft.com/en-us/library/system.windows.input.key(v=vs.110).aspx
        /// </remarks>
        /// <returns></returns>
        private Dictionary<string, int> WindowsInput()
        {
            return this.MakeDictionary(Enum.GetValues(typeof(System.Windows.Input.Key)).Cast<System.Windows.Input.Key>().Distinct());
        }

        /// <summary>
        /// Use SharpDX DirectInput Keys Enum to create a lookup dictionary for key values and codes ..
        /// </summary>
        /// <remarks>
        /// ref: https://msdn.microsoft.com/en-us/library/windows/desktop/bb321074(v=vs.85).aspx
        /// </remarks>
        /// <returns></returns>
        private Dictionary<string, int> SharpDX()
        {
            return this.MakeDictionary(Enum.GetValues(typeof(SharpDX.DirectInput.Key)).Cast<SharpDX.DirectInput.Key>().Distinct());
        }

        /// <summary>
        /// Make a Dictionary<string, int> Dictionary 
        /// </summary>
        /// <param name="keyEnums"></param>
        /// <returns></returns>
        private Dictionary<string, int> MakeDictionary<T>(System.Collections.Generic.IEnumerable<T> keyEnums)
        {
            var keyNames = keyEnums.ToList();
            var keyCodes = keyEnums.Cast<int>().ToList();

            // Add key names and codes to dictionary ..
            for (int i = 0; i < keyNames.Count; i++)
            {
                this.keyEnum.Add(keyNames[i].ToString(), keyCodes[i]);
            }

            return this.keyEnum;
        }
    }
}
