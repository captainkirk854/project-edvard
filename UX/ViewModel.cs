namespace UX
{
    using System.ComponentModel;
    using System.IO;
    using Prism.Commands;
    using UX.BoilerPlate;

    public class ViewModel : ObservableObject
    {
        /// <summary>
        /// The File Path for Elite Dangerous Binds
        /// </summary>
        private string filePathBinds;

        /// <summary>
        /// The File Path for VoiceAttack Profile
        /// </summary>
        private string filePathVAP;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel"/> class
        /// </summary>
        public ViewModel()
        {
            // Define DelegateCommand components ...
            this.SynchronisationCommand = new DelegateCommand(this.Execute, this.CanExecute);
        }

        /// <summary>
        /// Gets or sets selected Binds File Path
        /// </summary>
        public string BindsFile
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
                    this.RaisePropertyChangedEvent();

                    // Evaluate if Execute delegate can run ...
                    this.SynchronisationCommand.RaiseCanExecuteChanged();
                }
            } 
        }

        /// <summary>
        /// Gets or sets selected VAP File Path
        /// </summary>
        public string VAPFile
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
                    this.RaisePropertyChangedEvent();

                    // Evaluate if Execute delegate can run ...
                    this.SynchronisationCommand.RaiseCanExecuteChanged();
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
            var synchro = new Model(this.BindsFile, this.VAPFile);
            var result = synchro.Processed;
        }

        /// <summary>
        /// CanExecute Method of DelegateCommand
        /// </summary>
        /// <returns></returns>
        private bool CanExecute()
        {
            if (!File.Exists(this.BindsFile) || !File.Exists(this.VAPFile))
            {
                return false;
            }

            return true;
        }
    }
}