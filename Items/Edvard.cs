namespace Items
{
    /// <summary>
    /// Edvard related Enumerations
    /// </summary>
    public partial class Edvard
    {
        /// <summary>
        /// Enumeration of Table Columns
        /// </summary>
        public enum Column
        {
            BindingSynchronisationStatus,
            Context,
            DevicePriority,
            DeviceType,
            FilePath,
            Internal,
            KeyEnumeration,
            KeyAction,
            KeyGameValue,
            KeyEnumerationValue,
            KeyEnumerationCode,
            KeyId,
            KeyUpdateRequired,
            ModifierKeyGameValue,
            ModifierKeyEnumerationValue,
            ModifierKeyEnumerationCode,
            ModifierKeyId,
            Rationale,
            EliteDangerousAction,
            EliteDangerousBinds,
            EliteDangerousDevicePriority,
            EliteDangerousInternal,
            EliteDangerousKeyCode,
            EliteDangerousKeyId,
            EliteDangerousKeyValue,
            EliteDangerousModifierKeyCode,
            EliteDangerousModifierKeyId,
            EliteDangerousModifierKeyValue,
            VoiceAttackAction,
            VoiceAttackActionType,
            VoiceAttackCategory,
            VoiceAttackCommand,
            VoiceAttackInternal,
            VoiceAttackKeyCode,
            VoiceAttackKeyId,
            VoiceAttackKeyValue,
            VoiceAttackModifierKeyCode,
            VoiceAttackModifierKeyId,
            VoiceAttackModifierKeyValue,
            VoiceAttackProfile
        }

        /// <summary>
        /// Enumeration of ReMapRequired flags
        /// </summary>
        public enum KeyUpdateRequired
        {
            YES_Elite_TO_VoiceAttack,
            YES_VoiceAttack_TO_Elite,
            NO
        }

        /// <summary>
        /// Enumeration of File Updated Indicator
        /// </summary>
        public enum FileUpdated
        {
            EdVard
        }
    }
}
