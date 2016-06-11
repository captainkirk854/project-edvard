namespace Binding
{
    using System.Data;
    using Helper;

    public class KeyWriterEliteDangerous : IKeyWriter
    {
        public bool Update(DataTable reverseBindableVacantEDActions)
        {
            bool bindsUpdated = false;

            // Find Elite Dangerous commands which are vacant and available for remapping ..
            var vacantBindings = from vb in reverseBindableVacantEDActions.AsEnumerable()
                                       select
                                          new
                                          {
                                              KeyEnumeration = vb.Field<string>(Enums.Column.KeyEnumeration.ToString()),
                                              EliteDangerousAction = vb.Field<string>(Enums.Column.EliteDangerousAction.ToString()),
                                              VoiceAttackAction = vb.Field<string>(Enums.Column.VoiceAttackAction.ToString()),
                                              VoiceAttackKeyValue = vb.Field<string>(Enums.Column.VoiceAttackKeyValue.ToString()),
                                              VoiceAttackKeyCode = vb.Field<string>(Enums.Column.VoiceAttackKeyCode.ToString()),
                                              VoiceAttackModifierKeyValue = vb.Field<string>(Enums.Column.VoiceAttackModifierKeyValue.ToString()),
                                              VoiceAttackModifierKeyCode = vb.Field<string>(Enums.Column.VoiceAttackModifierKeyCode.ToString()),
                                              VoiceAttackInternal = vb.Field<string>(Enums.Column.VoiceAttackInternal.ToString()),
                                              VoiceAttackProfile = vb.Field<string>(Enums.Column.VoiceAttackProfile.ToString()),
                                              EliteDangerousBinds = vb.Field<string>(Enums.Column.EliteDangerousInternal.ToString()),
                                              EliteDangerousInternal = vb.Field<string>(Enums.Column.EliteDangerousBinds.ToString()),
                                          };

            return bindsUpdated;
        }
    }
}