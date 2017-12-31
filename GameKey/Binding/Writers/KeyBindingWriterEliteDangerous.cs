namespace GameKey.Binding.Writers
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Xml.Linq;
    using Utility;
    using Items;

    public class KeyBindingWriterEliteDangerous : IKeyBindingWriter
    {
        //Initialise ..
        private const string XMLRoot = "Root";
        private const string XMLPresetName = "PresetName";
        private const string XMLKey = "Key";
        private const string XMLDevice = "Device";
        private const string XMLModifier = "Modifier";
        private const string VacantDeviceIndicator = "{NoDevice}";

        /// <summary>
        /// Update vacant Elite Dangerous Name Action binding with Key derived from Voice Attack Profile ..
        /// </summary>
        /// <param name="reverseBindableVacantEDActions"></param>
        /// <param name="updateChangeTag"></param>
        /// <returns></returns>
        public bool Update(DataTable reverseBindableVacantEDActions, bool updateChangeTag)
        {
            // Initialise ..
            bool bindsUpdated = false;
            string globalEliteDangerousInternal = string.Empty;
            string globalVoiceAttackProfileFilePath = string.Empty;

            // Find Elite Dangerous commands which are vacant and available for remapping ..
            var vacantBindings = from vb in reverseBindableVacantEDActions.AsEnumerable()
                                       select
                                          new
                                          {
                                              KeyEnumeration = vb.Field<string>(EDVArd.Column.KeyEnumeration.ToString()),
                                              EliteDangerousAction = vb.Field<string>(EDVArd.Column.EliteDangerousAction.ToString()),
                                              VoiceAttackAction = vb.Field<string>(EDVArd.Column.VoiceAttackAction.ToString()),
                                              VoiceAttackKeyValue = vb.Field<string>(EDVArd.Column.VoiceAttackKeyValue.ToString()),
                                              VoiceAttackKeyCode = vb.Field<string>(EDVArd.Column.VoiceAttackKeyCode.ToString()),
                                              EliteDangerousKeyValue = vb.Field<string>(EDVArd.Column.EliteDangerousKeyValue.ToString()),
                                              VoiceAttackModifierKeyValue = vb.Field<string>(EDVArd.Column.VoiceAttackModifierKeyValue.ToString()),
                                              VoiceAttackModifierKeyCode = vb.Field<string>(EDVArd.Column.VoiceAttackModifierKeyCode.ToString()),
                                              EliteDangerousModifierKeyValue = vb.Field<string>(EDVArd.Column.EliteDangerousModifierKeyValue.ToString()),
                                              VoiceAttackInternal = vb.Field<string>(EDVArd.Column.VoiceAttackInternal.ToString()),
                                              VoiceAttackProfile = vb.Field<string>(EDVArd.Column.VoiceAttackProfile.ToString()),
                                              EliteDangerousInternal = vb.Field<string>(EDVArd.Column.EliteDangerousInternal.ToString()),
                                              EliteDangerousBinds = vb.Field<string>(EDVArd.Column.EliteDangerousBinds.ToString()),
                                          };

            // Process each potentially vacant binding ..
            foreach (var vacantBinding in vacantBindings)
            {
                bool updateStatus = false;

                // Try to update Primary bind ..
                updateStatus = this.UpdateVacantEliteDangerousBinding(vacantBinding.EliteDangerousBinds, 
                                                                      Application.EliteDangerousDevicePriority.Primary.ToString(), 
                                                                      vacantBinding.EliteDangerousAction, 
                                                                      vacantBinding.EliteDangerousKeyValue,
                                                                      vacantBinding.EliteDangerousModifierKeyValue);

                // If Primary bind attempt fails, try to update Secondary bind ..
                if (!updateStatus)
                {
                    updateStatus = this.UpdateVacantEliteDangerousBinding(vacantBinding.EliteDangerousBinds,
                                                                          Application.EliteDangerousDevicePriority.Secondary.ToString(),
                                                                          vacantBinding.EliteDangerousAction,
                                                                          vacantBinding.EliteDangerousKeyValue,
                                                                          vacantBinding.EliteDangerousModifierKeyValue);
                }
                
                if (updateStatus)
                {
                    globalEliteDangerousInternal = vacantBinding.EliteDangerousInternal;
                    globalVoiceAttackProfileFilePath = vacantBinding.EliteDangerousBinds;
                    bindsUpdated = true;
                }
            }

            // Update internal reference ..
            if (bindsUpdated && updateChangeTag)
            {
                this.UpdateBindsPresetName(globalVoiceAttackProfileFilePath, globalEliteDangerousInternal, Tag.Make(globalEliteDangerousInternal));
            }

            return bindsUpdated;
        }

        /// <summary>
        /// Update unbound Elite Dangerous Actions with Key Codes from Voice Attack
        /// </summary>
        /// </remarks>
        /// <param name="eliteDangerousBinds"></param>
        /// <param name="devicePriority"></param>
        /// <param name="actionName"></param>
        /// <param name="modifierKeyValue"></param>
        /// <returns></returns>
        private bool UpdateVacantEliteDangerousBinding(string eliteDangerousBinds, string devicePriority, string actionName, string regularKeyValue, string modifierKeyValue)
        {
            // Initialise ..
            var binds = HandleXml.ReadXDoc(eliteDangerousBinds);
            bool success = false;

            // Attempt to update regular key codes ..
            success = this.UpdateVacantEliteDangerousRegularKeyBinding(binds, devicePriority, actionName, regularKeyValue);

            // Attempt to update modifier key codes ..
            if (modifierKeyValue.Length > 0)
            {
                success = this.UpdateVacantEliteDangerousModifierKeyBinding(binds, devicePriority, actionName, regularKeyValue, modifierKeyValue);
            }

            if (success)
            {
                binds.Save(eliteDangerousBinds);
            }

            return success;
        }

        /// <summary>
        /// Update Key Code of a Regular Key Action associated to specific Voice Attack Id when that Action has not been bound ..
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
        ///                     |_<Device = {NoDevice}/>[*]
        ///                     |_<Key/ = empty>[*]
        ///                  |_<Secondary/>
        ///                     |_<Device/>
        ///                     |_<Key/>
        /// </remarks>
        /// <param name="eliteDangerousBindsXML"></param>
        /// <param name="devicePriority"></param>
        /// <param name="actionName"></param>
        /// <param name="regularKeyValue"></param>
        /// <returns></returns>
        private bool UpdateVacantEliteDangerousRegularKeyBinding(XDocument eliteDangerousBindsXML, string devicePriority, string actionName, string regularKeyValue)
        {
            // Initialise ..
            XElement primaryKeyBindingIsSet = null;
            bool success = false;

            // Check if Key_Value already set on primary binding for Action (no need to set same binding on secondary) ..
            try
            {
                primaryKeyBindingIsSet = eliteDangerousBindsXML.Descendants(Application.EliteDangerousDevicePriority.Primary.ToString())
                                            .Where(item => item.Parent.SafeElementName() == actionName &&
                                                   item.SafeElementName() == Application.EliteDangerousDevicePriority.Primary.ToString() &&
                                                   item.SafeAttributeValue(XMLDevice) == Application.Interaction.Keyboard.ToString() &&
                                                   item.SafeAttributeValue(XMLKey) == Application.EliteDangerousBindingPrefix.Key_.ToString() + regularKeyValue).FirstOrDefault();
            }
            catch
            {
            }

            // If not, attempt binding update ..
            if (primaryKeyBindingIsSet == null)
            {
                try
                {
                    // Update [Key Binding] for Elite Dangerous Action using Key Value  ..
                    eliteDangerousBindsXML.Descendants(devicePriority)
                       .Where(item => item.Parent.SafeElementName() == actionName &&
                              item.SafeElementName() == devicePriority &&
                              item.SafeAttributeValue(XMLDevice) == VacantDeviceIndicator &&
                              item.SafeAttributeValue(XMLKey) == string.Empty).FirstOrDefault()
                       .SetAttributeValue(XMLKey, Application.EliteDangerousBindingPrefix.Key_.ToString() + regularKeyValue);

                    // Update [Device Type] for Elite Dangerous Action (must always follow key-binding update) ..
                    eliteDangerousBindsXML.Descendants(devicePriority)
                       .Where(item => item.Parent.SafeElementName() == actionName &&
                              item.SafeElementName() == devicePriority &&
                              item.SafeAttributeValue(XMLDevice) == VacantDeviceIndicator &&
                              item.SafeAttributeValue(XMLKey) == Application.EliteDangerousBindingPrefix.Key_.ToString() + regularKeyValue).FirstOrDefault()
                       .SetAttributeValue(XMLDevice, Application.Interaction.Keyboard.ToString());

                    success = true;
                }
                catch (Exception)
                {
                    success = false;
                }
            }

            return success;
        }

        /// <summary>
        /// Update Key Code of a Regular Key Action associated to specific Voice Attack Id when that Action has not been bound ..
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
        ///                       |_<Modifier/> [*]
        ///                          |_<Device = {NoDevice}/>[*]
        ///                          |_<Key/ = empty>[*]
        ///                  |_<Secondary/>
        ///                     |_<Device/>
        ///                     |_<Key/>
        ///                       |_<Modifier/> [*]
        ///                          |_<Device = {NoDevice}/>[*]
        ///                          |_<Key/ = empty>[*]
        /// </remarks>
        /// <param name="eliteDangerousBindsXML"></param>
        /// <param name="devicePriority"></param>
        /// <param name="actionName"></param>
        /// <param name="regularKeyValue"></param>
        /// <param name="modifierKeyValue"></param>
        /// <returns></returns>
        private bool UpdateVacantEliteDangerousModifierKeyBinding(XDocument eliteDangerousBindsXML, string devicePriority, string actionName, string regularKeyValue, string modifierKeyValue)
        {
            // Initialise ..
            bool success = false;

            // Create a Modifier Key entry if regular Key does not have a modifier with modifier key value ..
            if (!this.CheckExistenceOfEliteDangerousModifierKey(eliteDangerousBindsXML, devicePriority, actionName, regularKeyValue, modifierKeyValue))
            {
                success = this.CreateEliteDangerousModifier(eliteDangerousBindsXML, devicePriority, actionName, regularKeyValue, Application.Interaction.Keyboard.ToString(), modifierKeyValue);
            } 

            return success;
        }

        /// <summary>
        /// Checks to see if Regular Key has a child Modifier Key node
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
        ///                  |_<PRIORITY/>
        ///                     |_<Device = VALUE/>
        ///                     |_<Key/ = VALUE>
        ///                       |_<Modifier/> [*]
        ///                          |_<Device = VALUE/>[*]
        ///                          |_<Key/ = VALUE>[*]
        /// </remarks>
        /// <param name="eliteDangerousBindsXML"></param>
        /// <param name="devicePriority"></param>
        /// <param name="actionName"></param>
        /// <param name="regularKeyValue"></param>
        /// <returns></returns>
        private bool CheckExistenceOfEliteDangerousModifierKey(XDocument eliteDangerousBindsXML, string devicePriority, string actionName, string regularKeyValue, string modifierKeyValue)
        {
            // Initialise ..
            XElement primaryKeyBindingIsSet = null;
            bool exists = true;

            // Check if Modifier Key_Value already set
            try
            {
              primaryKeyBindingIsSet = eliteDangerousBindsXML.Descendants(devicePriority)
                                               .Where(item => item.Parent.SafeElementName() == actionName &&
                                                      item.SafeElementName() == devicePriority &&
                                                      item.SafeAttributeValue(XMLDevice) == Application.Interaction.Keyboard.ToString() &&
                                                      item.SafeAttributeValue(XMLKey) == Application.EliteDangerousBindingPrefix.Key_.ToString() + regularKeyValue &&
                                                      item.Descendants(XMLModifier).First().Attribute(XMLDevice).Value == Application.Interaction.Keyboard.ToString() &&
                                                      item.Descendants(XMLModifier).First().Attribute(XMLKey).Value == Application.EliteDangerousBindingPrefix.Key_.ToString() + modifierKeyValue).FirstOrDefault();
            }
            catch
            {
                exists = false;
            }

            return exists;
        }

        /// <summary>
        /// Create default Modifier Key Child Node for Regular Key
        /// </summary>
        /// <param name="eliteDangerousBindsXML"></param>
        /// <param name="devicePriority"></param>
        /// <param name="actionName"></param>
        /// <param name="regularKeyValue"></param>
        /// <returns></returns>
        private bool CreateDefaultEliteDangerousModifier(XDocument eliteDangerousBindsXML, string devicePriority, string actionName, string regularKeyValue)
        {
            return this.CreateEliteDangerousModifier(eliteDangerousBindsXML, devicePriority, actionName, regularKeyValue, VacantDeviceIndicator, string.Empty);
        }

        /// <summary>
        /// Create Modifier Key Child Node for Regular Key
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
        ///                  |_<PRIORITY/>
        ///                     |_<Device = <>/>
        ///                     |_<Key/ = <>>
        ///                       |_<Modifier/> [*]
        ///                          |_<Device = VALUE/>[*]
        ///                          |_<Key/ = VALUE>[*]
        /// </remarks>
        /// <param name="eliteDangerousBindsXML"></param>
        /// <param name="devicePriority"></param>
        /// <param name="actionName"></param>
        /// <param name="regularKeyValue"></param>
        /// <param name="modifierDevice"></param>
        /// <param name="modifierKeyValue"></param>
        /// <returns></returns>
        private bool CreateEliteDangerousModifier(XDocument eliteDangerousBindsXML, string devicePriority, string actionName, string regularKeyValue, string modifierDevice, string modifierKeyValue)
        {
            // Initialise ..
            bool done = true;

            // Attempt to add a new child XMLModifier XElement with two valuated XAttributes: XMLDevice and XMLKey ..
            try
            {
                eliteDangerousBindsXML.Descendants(devicePriority)
                                               .Where(item => item.Parent.SafeElementName() == actionName &&
                                                      item.SafeElementName() == devicePriority &&
                                                      item.SafeAttributeValue(XMLDevice) == Application.Interaction.Keyboard.ToString() &&
                                                      item.SafeAttributeValue(XMLKey) == Application.EliteDangerousBindingPrefix.Key_.ToString() + regularKeyValue).FirstOrDefault()
                                               .Add(new XElement(XMLModifier,
                                                                    new XAttribute(XMLDevice, modifierDevice),
                                                                    new XAttribute(XMLKey, Application.EliteDangerousBindingPrefix.Key_.ToString() + modifierKeyValue)));
            }
            catch
            {
                done = false;
            }

            return done;
        }

        /// <summary>
        /// Update Elite Dangerous Binds Preset Name
        /// </summary>
        /// <remarks>
        ///   Format: XML
        ///             o <Root PresetName=[*]/>
        ///               |_ <KeyboardLayout/>
        ///               |_ <things/>
        ///                  |_<Binding/>
        ///                  |_<Inverted/>
        ///                  |_<Deadzone/>
        ///               |_ <things/>
        ///                  |_<Primary/>
        ///                     |_<Device = {NoDevice}/>
        ///                     |_<Key/ = empty>
        /// </remarks>
        /// <param name="eliteDangerousBinds"></param>
        /// <param name="presetName"></param>
        /// <param name="updatedPresetName"></param>
        private void UpdateBindsPresetName(string eliteDangerousBinds, string presetName, string updatedPresetName)
        {
            var binds = HandleXml.ReadXDoc(eliteDangerousBinds);
 
            // Update attribute of root node ..
            binds.Root
               .Attributes(XMLPresetName)
               .Where(item => item.Value == presetName).FirstOrDefault()
               .SetValue(updatedPresetName);

            binds.Save(eliteDangerousBinds);
        }
    }
}