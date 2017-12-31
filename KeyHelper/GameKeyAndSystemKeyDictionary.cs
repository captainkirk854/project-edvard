namespace KeyHelper
{
    using Utility;
    using Items;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Dictionary of Key Bindings
    /// </summary>
    public class GameKeyAndSystemKeyDictionary
    {
        private Dictionary<string, string> relationship = new Dictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GameKeyAndSystemKeyDictionary"/> class
        /// </summary>
        public GameKeyAndSystemKeyDictionary()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameKeyAndSystemKeyDictionary"/> class
        /// </summary>
        /// <param name="game"></param>
        public GameKeyAndSystemKeyDictionary(Items.Application.Name game)
        {
            this.Initialise(game);
        }

        /// <summary>
        /// Initialise Mapping Dictionary for certain named key codes: [Name o-o WindowsForm]
        /// </summary>
        /// <param name="game"></param>
        public void Initialise(Items.Application.Name game)
        {
            if (game == Items.Application.Name.EliteDangerous)
            {
                this.relationship.Add("0", "D0");
                this.relationship.Add("1", "D1");
                this.relationship.Add("2", "D2");
                this.relationship.Add("3", "D3");
                this.relationship.Add("4", "D4");
                this.relationship.Add("5", "D5");
                this.relationship.Add("6", "D6");
                this.relationship.Add("7", "D7");
                this.relationship.Add("8", "D8");
                this.relationship.Add("9", "D9");
                this.relationship.Add("Numpad_0", "NumPad0");
                this.relationship.Add("Numpad_1", "NumPad1");
                this.relationship.Add("Numpad_2", "NumPad2");
                this.relationship.Add("Numpad_3", "NumPad3");
                this.relationship.Add("Numpad_4", "NumPad4");
                this.relationship.Add("Numpad_5", "NumPad5");
                this.relationship.Add("Numpad_6", "NumPad6");
                this.relationship.Add("Numpad_7", "NumPad7");
                this.relationship.Add("Numpad_8", "NumPad8");
                this.relationship.Add("Numpad_9", "NumPad9");
                this.relationship.Add("Numpad_Enter", "Return");
                this.relationship.Add("Numpad_Multiply", "Multiply");
                this.relationship.Add("Numpad_Add", "Add");
                this.relationship.Add("Numpad_Minus", "Subtract");
                this.relationship.Add("Numpad_Subtract", "Subtract");
                this.relationship.Add("Numpad_Divide", "Divide");
                this.relationship.Add("Numpad_Decimal", "Decimal");
                this.relationship.Add("PageDown", "Next");
                this.relationship.Add("LeftArrow", "Left");
                this.relationship.Add("UpArrow", "Up");
                this.relationship.Add("RightArrow", "Right");
                this.relationship.Add("DownArrow", "Down");
                this.relationship.Add("Enter", "Return");
                this.relationship.Add("LeftShift", "LShiftKey");
                this.relationship.Add("RightShift", "RShiftKey");
                this.relationship.Add("LeftControl", "LControlKey");
                this.relationship.Add("RightControl", "RControlKey");
                this.relationship.Add("LeftAlt", "LMenu");
                this.relationship.Add("RightAlt", "RMenu");
                this.relationship.Add("LeftBracket", "OemOpenBrackets");
                this.relationship.Add("RightBracket", "Oem6");
                this.relationship.Add("Semicolon", "Oem1");
                this.relationship.Add("SemiColon", "Oem1");
                this.relationship.Add("ForwardSlash", "OemQuestion");
                this.relationship.Add("BackSlash", "OemBackSlash");
                this.relationship.Add("Slash", "Oem5");
                this.relationship.Add("Grave", "Oem8");
                this.relationship.Add("ScrollLock", "Scroll");
                this.relationship.Add("Dash", "Separator");
                this.relationship.Add("Minus", "OemMinus");
                this.relationship.Add("Backspace", "Back");
                this.relationship.Add("Period", "OemPeriod");
                this.relationship.Add("Comma", "Oemcomma");
                this.relationship.Add("Equals", "Oemplus");
                this.relationship.Add("Hash", "Oem7");
                this.relationship.Add("Tilde", "OemTilde");
            }
        }

        /// <summary>
        /// Get Windows Key Name
        /// </summary>
        /// {Dictionary Type}
        /// <param name="keyName"></param>
        /// <returns></returns>
        public string GetValue(string keyName)
        {
            try
            {
                return this.relationship[keyName];
            }
            catch
            {
                return keyName;
            }
        }

        /// <summary>
        /// Get Name Key Name
        /// </summary>
        /// {Dictionary Key}
        /// <param name="keyName"></param>
        /// <returns></returns>
        public string GetKey(string keyName)
        {
            return this.relationship.FirstOrDefault(x => x.Value == keyName).Key;
        }
    }
}