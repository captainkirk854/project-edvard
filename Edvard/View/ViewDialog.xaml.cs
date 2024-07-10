namespace EDVArd
{
    using System.Windows;
    using Helper;
    using Items;

    /// <summary>
    /// MainView UI
    /// </summary>
    public partial class ViewDialog : System.Windows.Controls.UserControl
    {
        public ViewDialog()
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
            TxtBoxSelectedBinds.Text = Dialog.OpenSingleFile(EDVArd.ArgOption.binds.ToString());
        }

        /// <summary>
        /// Button Open Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnVapOpen_Click(object sender, RoutedEventArgs e)
        {
            TxtBoxSelectedVap.Text = Dialog.OpenSingleFile(EDVArd.ArgOption.vap.ToString());
        }
    }
}
