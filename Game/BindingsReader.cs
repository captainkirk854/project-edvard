namespace Game
{
    using Helpers;
    using System.Data;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Read and process Elite Dangerous and Voice Attack Binding Configuration Files
    /// </summary>
    public static partial class BindingsReader
    {
        // Initialise ..
        private const string D = "+";
        private const string NA = "n/a";
        private const int INA = -2;
        private static readonly KeyMapper KeyMapper = new KeyMapper(keyType);
        private static Enums.KeyEnumType keyType = Enums.KeyEnumType.WindowsForms;

        /// <summary>
        /// Define Binding Actions DataTable Structure
        /// </summary>
        /// <param name="bindableActions"></param>
        private static void DefineBindableActions(this DataTable bindableActions)
        {
            bindableActions.TableName = "BindableActions";

            // Define table structure ..
            bindableActions.Columns.Add(Enums.Column.Context.ToString(), typeof(string));
            bindableActions.Columns.Add(Enums.Column.KeyAction.ToString(), typeof(string));
            bindableActions.Columns.Add(Enums.Column.DevicePriority.ToString(), typeof(string));
            bindableActions.Columns.Add(Enums.Column.DeviceType.ToString(), typeof(string));
        }

        /// <summary>
        /// Define Key Actions Bindings DataTable Structure
        /// </summary>
        /// <param name="keyActionBindings"></param>
        private static void DefineKeyActionBinder(this DataTable keyActionBindings)
        {
            keyActionBindings.TableName = "ActionKeyBindings";

            // Define table structure ..
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
        }
    }
}
