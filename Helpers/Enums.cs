namespace Helpers
{
    /// <summary>
    /// Application related Enumerations
    /// </summary>
    public class Enums
    {
        /// <summary>
        /// Enumeration of System Keyboard Types
        /// </summary>
        public enum KeyEnumType
        {
            WindowsForms,
            Console
        }

        /// <summary>
        /// Enumeration of Game Names 
        /// </summary>
        public enum Game
        {
            EliteDangerous,
            VoiceAttack
        }

        /// <summary>
        /// Enumeration of Key-binding Priorities in Elite Dangerous
        /// </summary>
        public enum EliteDangerousDevicePriority
        {   
            Primary,
            Secondary
        }

        /// <summary>
        /// Enumeration of Elite Dangerous Keys
        /// </summary>
        public enum EliteDangerousKey
        {
            Grave,
            LeftBracket,
            RightBracket,
            LeftShift,
            RightShift,
            LeftControl,
            RightControl,
            Minus,
            Slash,
            Numpad_0,
            Numpad_1,
            Numpad_2,
            Numpad_3,
            Numpad_4,
            Numpad_5,
            Numpad_6,
            Numpad_7,
            Numpad_8,
            Numpad_9,
            Numpad_Enter,
            Numpad_Multiply,
            Numpad_Plus,
            Numpad_Minus,
            Numpad_Divide,
            PageUp,
            PageDown,
            LeftArrow,
            UpArrow,
            RightArrow,
            DownArrow,
            Equals //?
        }

        public enum WindowsFormKey
        {
            Grave,
            LeftBracket, //?
            RightBracket, //?
            LShiftKey,
            RShiftKey,
            LControlKey,
            RControlKey,
            Minus, //?
            Slash, //?
            NumPad0,
            NumPad1,
            NumPad2,
            NumPad3,
            NumPad4,
            NumPad5,
            NumPad6,
            NumPad7,
            NumPad8,
            NumPad9,
            NumPad_Enter, //?
            Multiply,
            Add,
            Subtract,
            Divide,
            PageUp, //present = 33
            PageDown,
            Left,
            Up,
            Right,
            Down
        }
    }
}
