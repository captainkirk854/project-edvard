namespace Helpers
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Dictionary of Key Bindings
    /// </summary>
    public class KeyMapperExchange
    {
        private Dictionary<string, string> relationship = new Dictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyMapperExchange"/> class
        /// </summary>
        public KeyMapperExchange()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyMapperExchange"/> class
        /// </summary>
        /// <param name="keyType"></param>
        /// <param name="game"></param>
        public KeyMapperExchange(Enums.KeyboardEnumType keyType, Enums.Game game)
        {
            this.Initialise(keyType, game);
        }

        /// <summary>
        /// Initialise Mapping Dictionary to map Keys: [Game -> WindowsForm]
        /// </summary>
        /// <param name="keyType"></param>
        /// <param name="game"></param>
        public void Initialise(Enums.KeyboardEnumType keyType, Enums.Game game)
        {
            if (keyType == Enums.KeyboardEnumType.WindowsForms)
            {
                if (game == Enums.Game.EliteDangerous)
                {
                    this.relationship.Add("Grave", "Grave");
                    this.relationship.Add("LeftBracket", "LeftBracket");
                    this.relationship.Add("RightBracket", "RightBracket");
                    this.relationship.Add("LeftShift", "LShiftKey");
                    this.relationship.Add("RightShift", "RShiftKey");
                    this.relationship.Add("LeftControl", "LControlKey");
                    this.relationship.Add("RightControl", "RControlKey");
                    this.relationship.Add("Minus", "Minus");
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
                    this.relationship.Add("Numpad_Enter", "NumPad_Enter");
                    this.relationship.Add("Numpad_Multiply", "Multiply");
                    this.relationship.Add("Numpad_Plus", "Add");
                    this.relationship.Add("Numpad_Minus", "Subtract");
                    this.relationship.Add("Numpad_Divide", "Divide");
                    this.relationship.Add("PageUp", "PageUp");
                    this.relationship.Add("PageDown", "PageDown");
                    this.relationship.Add("LeftArrow", "Left");
                    this.relationship.Add("UpArrow", "Up");
                    this.relationship.Add("RightArrow", "Right");
                    this.relationship.Add("DownArrow", "Down");
                    this.relationship.Add("Enter", "Return");
                    this.relationship.Add("Backspace", "Back");
                }
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
        /// Get Game Key Name
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