namespace Binding
{
    using Helper;
    using KeyHelper;
    using System.Data;
    using System.Xml.Linq;
    
    /// <summary>
    /// Base Key-Bindings Reader Class
    /// </summary>
    public class KeyReader
    {
        // Initialise ..
        protected readonly Mapper KeyMapper = new Mapper(KeyType);
        protected string cfgFilePath = string.Empty;
        protected XDocument xCfg = new XDocument();
        private static KeyHelper.Enums.InputKeyEnumType keyType = KeyHelper.Enums.InputKeyEnumType.WindowsForms; // Default startup value for KeyType Property ..
        
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyReader" /> class.
        /// </summary>
        /// <param name="cfgFilePath"></param>
        public KeyReader(string cfgFilePath)
        {
            this.cfgFilePath = cfgFilePath;

            // Load XDocument into memory for availability in any derived classes ..
            this.xCfg = Xml.ReadXDoc(this.cfgFilePath);
        }

        // KeyType Property
        public static KeyHelper.Enums.InputKeyEnumType KeyType
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
