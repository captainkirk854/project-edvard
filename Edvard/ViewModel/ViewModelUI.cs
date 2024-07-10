namespace EDVArd
{
    using System.ComponentModel;
    using System.IO;
    using System.Threading.Tasks;
    using Utility.Mvvm;

    public class ViewModelUI : ObservableObject
    {
        /// <summary>
        /// File Path for Elite Dangerous Binds
        /// </summary>
        private string filePathBinds;

        /// <summary>
        /// File Path for VoiceAttack Profile
        /// </summary>
        private string filePathVAP;

        /// <summary>
        /// Voice Attack Profile Sync Status
        /// </summary>
        private bool syncVoiceAttackProfile = false;

        /// <summary>
        /// Elite Dangerous Binds Sync Status
        /// </summary>
        private bool syncEliteDangerousBinds = false;

        /// <summary>
        /// Initialise Model instance ..
        /// </summary>
        private Model files = new Model();

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelUI"/> class
        /// </summary>
        public ViewModelUI()
        {
            // Define DelegateCommand components ...
            this.SynchronisationCommand = new DelegateCommand(this.Execute, this.CanExecute);
        }

        /// <summary>
        /// Gets or sets selected Binds File Path
        /// </summary>
        public string SelectedBindsFile
        {
            get
            {
                return this.filePathBinds;
            }

            set
            {
                if (this.filePathBinds != value)
                {
                    this.filePathBinds = value;
                    this.OnPropertyChangedAuto();

                    // Evaluate if Execute delegate can run ...
                    this.SynchronisationCommand.RaiseCanExecuteChanged();
                }
            } 
        }

        /// <summary>
        /// Gets or sets selected VAP File Path
        /// </summary>
        public string SelectedVAPFile
        {
            get
            {
                return this.filePathVAP;
            }

            set
            {
                if (this.filePathVAP != value)
                {
                    this.filePathVAP = value;
                    this.OnPropertyChangedAuto();

                    // Evaluate if Execute delegate can run ...
                    this.SynchronisationCommand.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Voice Attack Profile has been synchronised
        /// </summary>
        public bool VoiceAttackProfileSync
        {
            get
            {
                return this.syncVoiceAttackProfile;
            }

            private set 
            {
                if (this.syncVoiceAttackProfile != value)
                {
                    this.syncVoiceAttackProfile = value;
                    this.OnPropertyChangedAuto();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Elite Dangerous Binds file has been synchronised
        /// </summary>
        public bool EliteDangerousBindsSync
        {
            get
            {
                return this.syncEliteDangerousBinds;
            }

            private set
            {
                if (this.syncEliteDangerousBinds != value)
                {
                    this.syncEliteDangerousBinds = value;
                    this.OnPropertyChangedAuto();
                }
            }
        }

        /// <summary>
        /// Gets or sets Synchronisation Command Controller
        /// </summary>
        public DelegateCommand SynchronisationCommand 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Action on change of related Command Property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.SynchronisationCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Execute Method of DelegateCommand
        /// </summary>
        private void Execute()
        {
            // Update selected file properties ..
            this.files.EliteDangerousBinds = this.SelectedBindsFile;
            this.files.VoiceAttackProfile = this.SelectedVAPFile;

            // Process file(s) asynchronously ..
            Task x = Task.Factory.StartNew(() =>
            {
                this.VoiceAttackProfileSync = files.VoiceAttackProfileSyncStatus;
            }).ContinueWith((y) =>
            {
                this.EliteDangerousBindsSync = files.EliteDangerousBindSyncStatus; 
            });
        }

        /// <summary>
        /// CanExecute Method of DelegateCommand
        /// </summary>
        /// <returns></returns>
        private bool CanExecute()
        {
            if (!File.Exists(this.SelectedBindsFile) || !File.Exists(this.SelectedVAPFile))
            {
                return false;
            }

            return true;
        }
    }
}