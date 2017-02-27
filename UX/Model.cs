namespace UX
{
    using System.IO;
    using Binding;
    using UX.BoilerPlate;

    public class Model : ObservableObject
    {
    //    private bool processed;
        private string bindsFile;
        private string vapFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="Model"/> class
        /// </summary>
        /// <param name="binds"></param>
        /// <param name="vap"></param>
        public Model(string binds, string vap)
        {
            this.bindsFile = binds;
            this.vapFile = vap;
        }

        public bool Processed 
        {
            get { return this.Synchronise(this.bindsFile, this.vapFile); }
        }

        private bool Synchronise(string bindsFile, string vapFile)
        {
            if (File.Exists(bindsFile) && File.Exists(vapFile))
            {
                KeyBindingAndCommandConnector keyLookup = new KeyBindingAndCommandConnector();

                // Voice Attack Profile synchronise ..
                KeyBindingWriterVoiceAttack newVoiceAttack = new KeyBindingWriterVoiceAttack();
                newVoiceAttack.Update(KeyBindingAnalyser.VoiceAttack(bindsFile, vapFile, keyLookup), true);

                // Elite Dangerous Binds synchronise ..
                KeyBindingWriterEliteDangerous newEliteDangerous = new KeyBindingWriterEliteDangerous();
                newEliteDangerous.Update(KeyBindingAnalyser.EliteDangerous(bindsFile, vapFile, keyLookup), true);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}