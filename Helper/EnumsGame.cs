namespace Helper
{
    /// <summary>
    /// Application-related Enumerations for Elite Dangerous and Voice Attack
    /// </summary>
    public partial class EnumsInternal
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
        /// Enumeration of Game-interaction Indicator
        /// </summary>
        public enum Interaction
        {
            Keyboard, //Elite Dangerous
            PressKey,  //Voice Attack
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