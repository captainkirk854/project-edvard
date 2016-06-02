namespace Helpers
{
    /// <summary>
    /// Application related Enumerations
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
        /// Enumeration of Keyboard Enumerations
        /// </summary>
        public enum InputKeyEnumType
        {
            Console,
            SharpDX,
            WindowsForms
        }

        /// <summary>
        /// Enumeration of Table Columns
        /// </summary>
        public enum Column
        {
            Context,
            KeyEnumeration,
            KeyAction,
            DevicePriority,
            KeyGameValue,
            KeyEnumerationValue,
            KeyEnumerationCode,
            KeyId,
            ModifierKeyGameValue,
            ModifierKeyEnumerationValue,
            ModifierKeyEnumerationCode,
            ModifierKeyId,
            FilePath,
            VoiceAttackProfile,
            EliteDangerousBinds,
            DeviceType,
            VoiceAttackAction,
            VoiceAttackKeyValue,
            VoiceAttackKeyCode,
            VoiceAttackKeyId,
            EliteDangerousAction,
            EliteDangerousKeyValue,
            EliteDangerousKeyCode,
            ReMapRequired,
            Rationale
        }

        /// <summary>
        /// Enumeration of ReMapRequired flags
        /// </summary>
        public enum ReMapRequired
        {
            YES,
            NO
        }
    }
}
