namespace EDVArd
{
    using GameKey.Adapter;
    using GameKey.Binding.Analysis;
    using GameKey.Binding.Writers;
    using Utility.Mvvm;

    public class Model : ObservableObject
    {
        /// <summary>
        /// File Path for Elite Dangerous Binds
        /// </summary>
        private string filepathEliteDangerousBinds;

        /// <summary>
        /// File Path for VoiceAttack Profile
        /// </summary>
        private string filepathVoiceAttackProfile;

        /// <summary>
        /// Key Bindings Lookup
        /// </summary>
        private GameKeyAndCommandBindingsAdapter bindingsAdapter = new GameKeyAndCommandBindingsAdapter();
        
        /// <summary>
        /// Voice Attack Writer
        /// </summary>
        private KeyBindingWriterVoiceAttack keyWriterVoiceAttack = new KeyBindingWriterVoiceAttack();
        
        /// <summary>
        /// Elite Dangerous Writer
        /// </summary>
        private KeyBindingWriterEliteDangerous keyWriterEliteDangerous = new KeyBindingWriterEliteDangerous();

        /// <summary>
        /// Gets or sets selected Binds File Path
        /// </summary>
        public string EliteDangerousBinds
        {
            get
            {
                return this.filepathEliteDangerousBinds;
            }

            set
            {
                if (this.filepathEliteDangerousBinds != value)
                {
                    this.filepathEliteDangerousBinds = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets selected VAP File Path
        /// </summary>
        public string VoiceAttackProfile
        {
            get
            {
                return this.filepathVoiceAttackProfile;
            }

            set
            {
                if (this.filepathVoiceAttackProfile != value)
                {
                    this.filepathVoiceAttackProfile = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Voice Attack Profile has been synchronised
        /// </summary>
        public bool VoiceAttackProfileSyncStatus
        {
            get { return this.SynchroniseVoiceAttack(); }
        }

        /// <summary>
        /// Gets a value indicating whether the Elite Dangerous Binds file has been synchronised
        /// </summary>
        public bool EliteDangerousBindSyncStatus
        {
            get { return this.SynchroniseEliteDangerous(); }
        }

        /// <summary>
        /// Synchronise Voice Attack Profile
        /// </summary>
        /// <returns></returns>
        private bool SynchroniseVoiceAttack()
        {
            return this.keyWriterVoiceAttack.Update(KeyBindingAnalyser.VoiceAttack(this.EliteDangerousBinds, this.VoiceAttackProfile, this.bindingsAdapter), true);
        }

        /// <summary>
        /// Synchronise Elite Dangerous Binds
        /// </summary>
        /// <returns></returns>
        private bool SynchroniseEliteDangerous()
        {
            return this.keyWriterEliteDangerous.Update(KeyBindingAnalyser.EliteDangerous(this.EliteDangerousBinds, this.VoiceAttackProfile, this.bindingsAdapter), true);
        }
    }
}