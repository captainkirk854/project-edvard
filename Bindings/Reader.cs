namespace Bindings
{
    using Helpers;
    using System.Data;
    
    /// <summary>
    /// Read and process Elite Dangerous and Voice Attack Binding Configuration Files
    /// </summary>
    public static partial class Reader
    {
        // Initialise ..
        private const string D = "+";
        private const string NA = "n/a";
        private const int INA = -2;
        private static readonly KeyMapper KeyMapper = new KeyMapper(KeyType);
        
        /// <summary>
        /// Initializes static members of the <see cref="Reader"/> class
        /// </summary>
        static Reader()
        {
            // Write informational CSV of selected Key Map dictionary ..
            string outputDirectory = System.Environment.ExpandEnvironmentVariables("%UserProfile%") + "\\Desktop";
            KeyMapper.WriteKeyMap(outputDirectory + "\\" + "KeyCodes" + KeyType.ToString() + ".csv");  
        }

        // KeyType Property currently hard-coded ...
        public static Enums.InputKeyEnumType KeyType
        {
            get { return Enums.InputKeyEnumType.WindowsForms; }
        }
    }
}
