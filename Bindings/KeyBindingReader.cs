namespace Bindings
{
    using Helpers;
    using System.Data;
    using System.Xml.Linq;
    
    /// <summary>
    /// Base Key-Bindings Reader Class
    /// </summary>
    public class KeyBindingReader
    {
        // Initialise ..
        protected readonly KeyMapper KeyMapper = new KeyMapper(KeyType);
        protected string cfgFilePath = string.Empty;
        protected XDocument xCfg = new XDocument();
        private static Enums.InputKeyEnumType keyType = Enums.InputKeyEnumType.WindowsForms; // Default startup value for KeyType Property ..
        
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyBindingReader" /> class.
        /// </summary>
        /// <param name="cfgFilePath"></param>
        public KeyBindingReader(string cfgFilePath)
        {
            this.cfgFilePath = cfgFilePath;

            // Load XDocument into memory for availability in any derived classes ..
            this.xCfg = Xml.ReadXDoc(this.cfgFilePath);
        }

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
