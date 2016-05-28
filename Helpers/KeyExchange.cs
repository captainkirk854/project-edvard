namespace Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;

    /// <summary>
    /// Key Exchange for Elite Dangerous Key Bindings
    /// </summary>
    public class KeyExchange
    {
        private Dictionary<string, string> map = new Dictionary<string, string>();

        /// <summary>
        /// Initialise Mapping Dictionary to map Game Keys to Form Keys
        /// </summary>
        /// <param name="keyType"></param>
        public void Initialise(Enums.KeyEnumType keyType)
        {
            if (keyType == Enums.KeyEnumType.WindowsForms)
            {
                this.map.Add(Enums.EliteDangerousKey.Grave.ToString(), Enums.WindowsFormKey.Grave.ToString()); //?
                this.map.Add(Enums.EliteDangerousKey.LeftBracket.ToString(), Enums.WindowsFormKey.LeftBracket.ToString()); //?
                this.map.Add(Enums.EliteDangerousKey.RightBracket.ToString(), Enums.WindowsFormKey.RightBracket.ToString()); //?
                this.map.Add(Enums.EliteDangerousKey.LeftShift.ToString(), Enums.WindowsFormKey.LShiftKey.ToString());
                this.map.Add(Enums.EliteDangerousKey.RightShift.ToString(), Enums.WindowsFormKey.RShiftKey.ToString());
                this.map.Add(Enums.EliteDangerousKey.LeftControl.ToString(), Enums.WindowsFormKey.LControlKey.ToString());
                this.map.Add(Enums.EliteDangerousKey.RightControl.ToString(), Enums.WindowsFormKey.RControlKey.ToString());
                this.map.Add(Enums.EliteDangerousKey.Minus.ToString(), Enums.WindowsFormKey.Minus.ToString()); //?
                this.map.Add(Enums.EliteDangerousKey.Slash.ToString(), Enums.WindowsFormKey.Slash.ToString()); //?
                this.map.Add(Enums.EliteDangerousKey.Numpad_0.ToString(), Enums.WindowsFormKey.NumPad0.ToString());
                this.map.Add(Enums.EliteDangerousKey.Numpad_1.ToString(), Enums.WindowsFormKey.NumPad1.ToString());
                this.map.Add(Enums.EliteDangerousKey.Numpad_2.ToString(), Enums.WindowsFormKey.NumPad2.ToString());
                this.map.Add(Enums.EliteDangerousKey.Numpad_3.ToString(), Enums.WindowsFormKey.NumPad3.ToString());
                this.map.Add(Enums.EliteDangerousKey.Numpad_4.ToString(), Enums.WindowsFormKey.NumPad4.ToString());
                this.map.Add(Enums.EliteDangerousKey.Numpad_5.ToString(), Enums.WindowsFormKey.NumPad5.ToString());
                this.map.Add(Enums.EliteDangerousKey.Numpad_6.ToString(), Enums.WindowsFormKey.NumPad6.ToString());
                this.map.Add(Enums.EliteDangerousKey.Numpad_7.ToString(), Enums.WindowsFormKey.NumPad7.ToString());
                this.map.Add(Enums.EliteDangerousKey.Numpad_8.ToString(), Enums.WindowsFormKey.NumPad8.ToString());
                this.map.Add(Enums.EliteDangerousKey.Numpad_9.ToString(), Enums.WindowsFormKey.NumPad9.ToString());
                this.map.Add(Enums.EliteDangerousKey.Numpad_Enter.ToString(), Enums.WindowsFormKey.NumPad_Enter.ToString()); //?
                this.map.Add(Enums.EliteDangerousKey.Numpad_Multiply.ToString(), Enums.WindowsFormKey.Multiply.ToString());
                this.map.Add(Enums.EliteDangerousKey.Numpad_Plus.ToString(), Enums.WindowsFormKey.Add.ToString());
                this.map.Add(Enums.EliteDangerousKey.Numpad_Minus.ToString(), Enums.WindowsFormKey.Subtract.ToString());
                this.map.Add(Enums.EliteDangerousKey.Numpad_Divide.ToString(), Enums.WindowsFormKey.Divide.ToString());
                this.map.Add(Enums.EliteDangerousKey.PageUp.ToString(), Enums.WindowsFormKey.PageUp.ToString());
                this.map.Add(Enums.EliteDangerousKey.PageDown.ToString(), Enums.WindowsFormKey.PageDown.ToString());
                this.map.Add(Enums.EliteDangerousKey.LeftArrow.ToString(), Enums.WindowsFormKey.Left.ToString());
                this.map.Add(Enums.EliteDangerousKey.UpArrow.ToString(), Enums.WindowsFormKey.Up.ToString());
                this.map.Add(Enums.EliteDangerousKey.RightArrow.ToString(), Enums.WindowsFormKey.Right.ToString());
                this.map.Add(Enums.EliteDangerousKey.DownArrow.ToString(), Enums.WindowsFormKey.Down.ToString());
            }
        }

        /// <summary>
        /// Get Windows Key Name
        /// </summary>
        /// {Dictionary Value}
        /// <param name="keyName"></param>
        /// <returns></returns>
        public string GetValue(string keyName)
        {
            return this.map[keyName];
        }

        /// <summary>
        /// Get Game Key Name
        /// </summary>
        /// {Dictionary Key}
        /// <param name="keyName"></param>
        /// <returns></returns>
        public string GetKey(string keyName)
        {
            return this.map.FirstOrDefault(x => x.Value == keyName).Key;
        }
    }
}