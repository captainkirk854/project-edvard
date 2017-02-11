namespace Binding
{
    using Helper;
    using KeyHelper;
    using System.Data;
    using System.Xml.Linq;
    using Items;
    
    /// <summary>
    /// Base Key-Bindings Reader Class
    /// </summary>
    public class KeyBindingReader
    {
        // Initialise ..
        protected readonly GameKeyAndSystemKeyConnector Keys = new GameKeyAndSystemKeyConnector(KeyType);
        protected string cfgFilePath = string.Empty;
        protected XDocument xCfg = new XDocument();
        private static KeyEnum.Type keyType = KeyEnum.Type.WindowsForms; // Default startup value for KeyType Property ..
        
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyBindingReader" /> class.
        /// </summary>
        /// <param name="cfgFilePath"></param>
        public KeyBindingReader(string cfgFilePath)
        {
            this.cfgFilePath = cfgFilePath;

            // Load XDocument into memory for availability in any derived classes ..
            this.xCfg = HandleXml.ReadXDoc(this.cfgFilePath);
        }

        // KeyType Property
        public static KeyEnum.Type KeyType
        {
            get { return keyType; }

            set { keyType = value; }
        }

        /// <summary>
        /// Write currently enumerated Key Type to CSV File
        /// </summary>
        /// <param name="directoryPath"></param>
        public void WriteKeyTypes(string keyMapDirectoryPath)
        {
            string keyMapFileName = "KeyCodes" + KeyType.ToString();

            this.Keys.WriteKeyMap(keyMapDirectoryPath, keyMapFileName);  
        }
    }
}
