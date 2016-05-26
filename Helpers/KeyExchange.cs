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
        Dictionary<string, string> Map = new Dictionary<string, string>();

        /// <summary>
        /// Initialise Mapping Dictionary to map Game Keys to Form Keys
        /// </summary>
        public void Initialise(Enums.KeyEnumType KeyType)
        {
            if (KeyType == Enums.KeyEnumType.WindowsForms)
            {
                Map.Add(Enums.EliteDangerousKey.Grave.ToString(), Enums.WindowsFormKey.Grave.ToString()); //?
                Map.Add(Enums.EliteDangerousKey.LeftBracket.ToString(), Enums.WindowsFormKey.LeftBracket.ToString()); //?
                Map.Add(Enums.EliteDangerousKey.RightBracket.ToString(), Enums.WindowsFormKey.RightBracket.ToString()); //?
                Map.Add(Enums.EliteDangerousKey.LeftShift.ToString(), Enums.WindowsFormKey.LShiftKey.ToString());
                Map.Add(Enums.EliteDangerousKey.RightShift.ToString(), Enums.WindowsFormKey.RShiftKey.ToString());
                Map.Add(Enums.EliteDangerousKey.LeftControl.ToString(), Enums.WindowsFormKey.LControlKey.ToString());
                Map.Add(Enums.EliteDangerousKey.RightControl.ToString(), Enums.WindowsFormKey.RControlKey.ToString());
                Map.Add(Enums.EliteDangerousKey.Minus.ToString(), Enums.WindowsFormKey.Minus.ToString()); //?
                Map.Add(Enums.EliteDangerousKey.Slash.ToString(), Enums.WindowsFormKey.Slash.ToString()); //?
                Map.Add(Enums.EliteDangerousKey.Numpad_0.ToString(), Enums.WindowsFormKey.NumPad0.ToString());
                Map.Add(Enums.EliteDangerousKey.Numpad_1.ToString(), Enums.WindowsFormKey.NumPad1.ToString());
                Map.Add(Enums.EliteDangerousKey.Numpad_2.ToString(), Enums.WindowsFormKey.NumPad2.ToString());
                Map.Add(Enums.EliteDangerousKey.Numpad_3.ToString(), Enums.WindowsFormKey.NumPad3.ToString());
                Map.Add(Enums.EliteDangerousKey.Numpad_4.ToString(), Enums.WindowsFormKey.NumPad4.ToString());
                Map.Add(Enums.EliteDangerousKey.Numpad_5.ToString(), Enums.WindowsFormKey.NumPad5.ToString());
                Map.Add(Enums.EliteDangerousKey.Numpad_6.ToString(), Enums.WindowsFormKey.NumPad6.ToString());
                Map.Add(Enums.EliteDangerousKey.Numpad_7.ToString(), Enums.WindowsFormKey.NumPad7.ToString());
                Map.Add(Enums.EliteDangerousKey.Numpad_8.ToString(), Enums.WindowsFormKey.NumPad8.ToString());
                Map.Add(Enums.EliteDangerousKey.Numpad_9.ToString(), Enums.WindowsFormKey.NumPad9.ToString());
                Map.Add(Enums.EliteDangerousKey.Numpad_Enter.ToString(), Enums.WindowsFormKey.NumPad_Enter.ToString()); //?
                Map.Add(Enums.EliteDangerousKey.Numpad_Multiply.ToString(), Enums.WindowsFormKey.Multiply.ToString());
                Map.Add(Enums.EliteDangerousKey.Numpad_Plus.ToString(), Enums.WindowsFormKey.Add.ToString());
                Map.Add(Enums.EliteDangerousKey.Numpad_Minus.ToString(), Enums.WindowsFormKey.Subtract.ToString());
                Map.Add(Enums.EliteDangerousKey.Numpad_Divide.ToString(), Enums.WindowsFormKey.Divide.ToString());
                Map.Add(Enums.EliteDangerousKey.PageUp.ToString(), Enums.WindowsFormKey.PageUp.ToString());
                Map.Add(Enums.EliteDangerousKey.PageDown.ToString(), Enums.WindowsFormKey.PageDown.ToString());
                Map.Add(Enums.EliteDangerousKey.LeftArrow.ToString(), Enums.WindowsFormKey.Left.ToString());
                Map.Add(Enums.EliteDangerousKey.UpArrow.ToString(), Enums.WindowsFormKey.Up.ToString());
                Map.Add(Enums.EliteDangerousKey.RightArrow.ToString(), Enums.WindowsFormKey.Right.ToString());
                Map.Add(Enums.EliteDangerousKey.DownArrow.ToString(), Enums.WindowsFormKey.Down.ToString());
            }
        }

        /// <summary>
        /// Get Windows Key Name
        /// </summary>
        /// {Dictionary Value}
        /// <param name="KeyName"></param>
        /// <returns></returns>
        public string GetValue(string KeyName)
        {
            return this.Map[KeyName];
        }

        /// <summary>
        /// Get Game Key Name
        /// </summary>
        /// {Dictionary Key}
        /// <param name="KeyName"></param>
        /// <returns></returns>
        public string GetKey(string KeyName)
        {
            return Map.FirstOrDefault(x => x.Value == KeyName).Key;
        }


    }
}
