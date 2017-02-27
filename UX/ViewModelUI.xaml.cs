namespace UX
{
    using System.Windows;
    using Items;

    /// <summary>
    /// MainView UI
    /// </summary>
    public partial class ViewModelUI : System.Windows.Controls.UserControl
    {
        public ViewModelUI()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Button Open Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBindsOpen_Click(object sender, RoutedEventArgs e)
        {
            TxtBoxSelectedBinds.Text = UX.LocalHelp.Dialog.OpenSingleFile(Edvard.ArgOption.binds.ToString());
        }

        /// <summary>
        /// Button Open Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnVapOpen_Click(object sender, RoutedEventArgs e)
        {
            TxtBoxSelectedVap.Text = UX.LocalHelp.Dialog.OpenSingleFile(Edvard.ArgOption.vap.ToString());
        }
    }
}
