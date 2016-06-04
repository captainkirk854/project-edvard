namespace Bindings
{
    using Helpers;
    using System.Data;
    
    /// <summary>
    /// Base Key-Bindings Reader Class
    /// </summary>
    public class KeyBindingReader
    {
        // Initialise ..
        protected const string NA = "n/a";
        protected const int INA = -2;
        protected readonly KeyMapper KeyMapper = new KeyMapper(KeyType);
        protected string cfgFilePath = string.Empty;
 
        // Startup default for KeyType Property ..
        private static Enums.InputKeyEnumType keyType = Enums.InputKeyEnumType.WindowsForms;
        
        // KeyType Property
        public static Enums.InputKeyEnumType KeyType
        {
            get { return keyType; }

            set { keyType = value; }
        }

        /// <summary>
        /// Write currently enumerated Key Type to CSV File
        /// </summary>
        /// <param name="directoryPath"></param>
        public void WriteKeyMap(string directoryPath)
        {
            directoryPath += "\\" + "KeyCodes" + KeyType.ToString() + ".csv";
            this.KeyMapper.WriteKeyMap(directoryPath);  
        }
    }
}
