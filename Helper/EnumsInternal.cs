namespace Helper
{
    /// <summary>
    /// Application related Enumerations
    /// </summary>
    public partial class EnumsInternal
    {
        /// <summary>
        /// Enumeration of Table Columns
        /// </summary>
        public enum Column
        {
            BindingSynchronisationStatus,
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
            Internal,
            DeviceType,
            KeyUpdateRequired,
            Rationale,
            EliteDangerousBinds,
            EliteDangerousAction,
            EliteDangerousDevicePriority,
            EliteDangerousInternal,
            EliteDangerousKeyCode,
            EliteDangerousKeyId,
            EliteDangerousKeyValue,
            EliteDangerousModifierKeyCode,
            EliteDangerousModifierKeyValue,
            EliteDangerousModifierKeyId,
            VoiceAttackCommand,
            VoiceAttackProfile,
            VoiceAttackAction,
            VoiceAttackInternal,
            VoiceAttackKeyCode,
            VoiceAttackKeyId,
            VoiceAttackKeyValue,
            VoiceAttackModifierKeyCode,
            VoiceAttackModifierKeyValue,
            VoiceAttackModifierKeyId
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
