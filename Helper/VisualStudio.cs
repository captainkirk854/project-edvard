namespace Helper
{
    using System;

    public static class VisualStudio
    {
        private static readonly string Information = AppDomain.CurrentDomain.BaseDirectory.Replace("Debug", string.Empty).Replace("Release", string.Empty).Replace("bin", string.Empty).Replace("\\\\\\", string.Empty);
  
        /// <summary>
        /// Gets in a crude way current Visual Studio project base directory
        /// </summary>
        /// <returns></returns>
        public static string ProjectBaseDirectory
        {
            get { return Information; }
        }
    }
}
