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

        /// <summary>
        /// Define Binding Actions DataTable Structure
        /// </summary>
        /// <returns></returns>
        private static DataTable DefineBindableActions()
        {
            // New DataTable ..
            DataTable bindableActions = new DataTable();
            bindableActions.TableName = "BindableActions";

            // Define its structure ..
            bindableActions.Columns.Add(Enums.Column.Context.ToString(), typeof(string));
            bindableActions.Columns.Add(Enums.Column.KeyAction.ToString(), typeof(string));
            bindableActions.Columns.Add(Enums.Column.DevicePriority.ToString(), typeof(string));
            bindableActions.Columns.Add(Enums.Column.DeviceType.ToString(), typeof(string));

            return bindableActions;
        }

        /// <summary>
        /// Define Key Actions Bindings DataTable Structure
        /// </summary>
        /// <returns></returns>
        private static DataTable DefineKeyActionBinder()
        {
            // New DataTable ..
            DataTable keyActionBindings = new DataTable();
            keyActionBindings.TableName = "ActionKeyBindings";

            // Define its structure ..
            keyActionBindings.Columns.Add(Enums.Column.Context.ToString(), typeof(string));
            keyActionBindings.Columns.Add(Enums.Column.KeyEnumeration.ToString(), typeof(string));
            keyActionBindings.Columns.Add(Enums.Column.KeyAction.ToString(), typeof(string));
            keyActionBindings.Columns.Add(Enums.Column.DevicePriority.ToString(), typeof(string));
            keyActionBindings.Columns.Add(Enums.Column.KeyGameValue.ToString(), typeof(string));
            keyActionBindings.Columns.Add(Enums.Column.KeyEnumerationValue.ToString(), typeof(string));
            keyActionBindings.Columns.Add(Enums.Column.KeyEnumerationCode.ToString(), typeof(int));
            keyActionBindings.Columns.Add(Enums.Column.KeyId.ToString(), typeof(string));
            keyActionBindings.Columns.Add(Enums.Column.ModifierKeyGameValue.ToString(), typeof(string));
            keyActionBindings.Columns.Add(Enums.Column.ModifierKeyEnumerationValue.ToString(), typeof(string));
            keyActionBindings.Columns.Add(Enums.Column.ModifierKeyEnumerationCode.ToString(), typeof(int));
            keyActionBindings.Columns.Add(Enums.Column.ModifierKeyId.ToString(), typeof(string));

            return keyActionBindings;
        }
    }
}
