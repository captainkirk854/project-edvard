namespace Items
{
    /// <summary>
    /// Edvard-related Enumerations for Elite Dangerous and Voice Attack
    /// </summary>
    public class Application
    {
        /// <summary>
        /// Enumeration of Applications Names 
        /// </summary>
        public enum Name
        {
            EliteDangerous,
            VoiceAttack
        }

        /// <summary>
        /// Enumeration of Name-interaction Indicator
        /// </summary>
        public enum Interaction
        {
            Keyboard, //Elite Dangerous
            PressKey,  //Voice Attack
            KeyUp, // Voice Attack
            KeyDown, // Voice  Attack
            ExecuteCommand // Voice Attack
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
        /// Enumeration of Elite Dangerous Binding Prefixes
        /// </summary>
        public enum EliteDangerousBindingPrefix
        {
            Key_,
            Joy_
        }
    }
}