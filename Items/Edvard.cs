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

        /// <summary>
        /// Enumeration of Console Arguments
        /// </summary>
        public enum ArgOption
        {
            binds,
            vap,
            sync,
            import,
            export,
            tag,
            analysis,
            format,
            backup,
            help,
            test
        }

        /// <summary>
        /// Enumeration of Console Argument Sub Options
        /// </summary>
        public enum ArgSubOption
        {
            twoway,
            oneway_to_binds,
            oneway_to_vap,
            csv,
            htm,
            none
        }

        public enum FileType
        {
            htm,
            csv
        }

        public enum AnalysisFile
        {
            BindableActions,
            KeyBoundActions,
            ConsolidatedKeyBoundActions,
            RelatedCommands,
            AllCommands
        }

        public enum BackupStatus
        {
            NotSelected,
            HardFailure,
            SoftFailure,
            Success
        }
    }
}
