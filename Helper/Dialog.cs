namespace Helper
{
    using System.Windows.Forms; // if a WPF project, System.Windows.Forms reference has be manually added

    public static class Dialog
    {
        public static string OpenSingleFile(string fileType)
        {
            // Initialise ..
            string selectedFile = string.Empty;

            // Select file ..
            OpenFileDialog fileDialog = new OpenFileDialog { Multiselect = false, Filter = "A " + fileType + " File (*." + fileType + ")|*." + fileType };
            switch (fileDialog.ShowDialog())
            {
                case DialogResult.OK:
                    selectedFile = fileDialog.FileName;
                    break;
                case DialogResult.Cancel:
                default:
                    break;
            }

            return selectedFile;
        }
    }
}