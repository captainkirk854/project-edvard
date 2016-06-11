namespace Helper
{
    /// <summary>
    /// Application related Enumerations for Elite Dangerous
    /// </summary>
    public partial class Enums
    {
        /// <summary>
        /// Enumeration of Game Names 
        /// </summary>
        public enum Game
        {
            EliteDangerous,
            VoiceAttack
        }

        /// <summary>
        /// Enumeration of Keyboard-interaction Indicator
        /// </summary>
        public enum KeyboardInteraction
        {
            Keyboard, //EliteDangerous
            PressKey  //VoiceAttack
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
