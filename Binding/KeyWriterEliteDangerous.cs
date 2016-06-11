namespace Binding
{
    using System.Data;
    using System.Linq;
    using Helper;

    public class KeyWriterEliteDangerous : IKeyWriter
    {
        //Initialise ..
        private const string XMLRoot = "Root";
        private const string XMLKey = "Key";
        private const string XMLDevice = "Device";
        private const string XMLModifier = "Modifier";

        /// <summary>
        /// Update vacant Elite Dangerous Game Action binding with Key derived from Voice Attack Profile ..
        /// </summary>
        /// <param name="reverseBindableVacantEDActions"></param>
        /// <returns></returns>
        public bool Update(DataTable reverseBindableVacantEDActions)
        {
            bool bindsUpdated = false;

            // Find Elite Dangerous commands which are vacant and available for remapping ..
            var vacantBindings = from vb in reverseBindableVacantEDActions.AsEnumerable()
                                       select
                                          new
                                          {
                                              KeyEnumeration = vb.Field<string>(Enums.Column.KeyEnumeration.ToString()),
                                              EliteDangerousAction = vb.Field<string>(Enums.Column.EliteDangerousAction.ToString()),
                                              VoiceAttackAction = vb.Field<string>(Enums.Column.VoiceAttackAction.ToString()),
                                              VoiceAttackKeyValue = vb.Field<string>(Enums.Column.VoiceAttackKeyValue.ToString()),
                                              VoiceAttackKeyCode = vb.Field<string>(Enums.Column.VoiceAttackKeyCode.ToString()),
                                              EliteDangerousKeyValue = vb.Field<string>(Enums.Column.EliteDangerousKeyValue.ToString()),
                                              VoiceAttackModifierKeyValue = vb.Field<string>(Enums.Column.VoiceAttackModifierKeyValue.ToString()),
                                              VoiceAttackModifierKeyCode = vb.Field<string>(Enums.Column.VoiceAttackModifierKeyCode.ToString()),
                                              EliteDangerousModifierKeyValue = vb.Field<string>(Enums.Column.EliteDangerousModifierKeyValue.ToString()),
                                              VoiceAttackInternal = vb.Field<string>(Enums.Column.VoiceAttackInternal.ToString()),
                                              VoiceAttackProfile = vb.Field<string>(Enums.Column.VoiceAttackProfile.ToString()),
                                              EliteDangerousInternal = vb.Field<string>(Enums.Column.EliteDangerousInternal.ToString()),
                                              EliteDangerousBinds = vb.Field<string>(Enums.Column.EliteDangerousBinds.ToString()),
                                          };

            // Update each vacant, Primary binding ..
            foreach (var vacantBinding in vacantBindings)
            {
                this.UpdateVacantEliteDangerousBinding(vacantBinding.EliteDangerousBinds, 
                                                       Helper.Enums.EliteDangerousDevicePriority.Primary.ToString(), 
                                                       vacantBinding.EliteDangerousAction, 
                                                       vacantBinding.EliteDangerousKeyValue);
                bindsUpdated = true;
            }

            return bindsUpdated;
        }

        /// <summary>
        /// Update Key Code associated to specific [Id] in Voice Attack when Action has not been bound ..
        /// </summary>
        /// <remarks>
        ///   Format: XML
        ///             o <Root/>
        ///               |_ <KeyboardLayout/>
        ///               |_ <things></things>.[Value] attribute
        ///               |_ <things/>
        ///                  |_<Binding/>
        ///                  |_<Inverted/>
        ///                  |_<Deadzone/>
        ///               |_ <things/>
        ///                  |_<Primary/>
        ///                     |_<Device = {NoDevice}/>
        ///                     |_<Key/ = empty>
        ///                  |_<Secondary/>
        ///                     |_<Device/>
        ///                     |_<Key/>
        /// </remarks>
        /// <param name="edbinds"></param>
        /// <param name="devicePriority"></param>
        /// <param name="actionName"></param>
        /// <param name="keyvalue"></param>
        private void UpdateVacantEliteDangerousBinding(string edbinds, string devicePriority, string actionName, string keyvalue)
        {
            // Initialise ..
            const string VacantDeviceIndicator = "{NoDevice}";

            // Read Voice Attack Profile ...
            var edb = Xml.ReadXDoc(edbinds);

            // Update [Key Binding] for Elite Dangerous Action using Key Value  ..
            edb.Descendants(devicePriority)
               .Where(item => item.Parent.SafeElementName() == actionName &&
                      item.SafeElementName() == devicePriority &&
                      item.SafeAttributeValue(XMLDevice) == VacantDeviceIndicator &&
                      item.SafeAttributeValue(XMLKey) == string.Empty).FirstOrDefault()
               .SetAttributeValue(XMLKey, Enums.EliteDangerousBindingPrefix.Key_.ToString() + keyvalue);

            // Update [Device Type] for Elite Dangerous Action (must always follow key-binding update) ..
            edb.Descendants(devicePriority)
               .Where(item => item.Parent.SafeElementName() == actionName &&
                      item.SafeElementName() == devicePriority &&
                      item.SafeAttributeValue(XMLDevice) == VacantDeviceIndicator &&
                      item.SafeAttributeValue(XMLKey) == Enums.EliteDangerousBindingPrefix.Key_.ToString() + keyvalue).FirstOrDefault()
               .SetAttributeValue(XMLDevice, Enums.HumanGameInteraction.Keyboard.ToString());

            // Save file ..
            edb.Save(edbinds);
        }
    }
}