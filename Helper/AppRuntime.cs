namespace Helper
{
    using System;
    using System.IO;

    /// <summary>
    /// Runtime Properties
    /// </summary>
    public static class AppRuntime
    {
        private static readonly string ActiveProjectDirectory = AppDomain.CurrentDomain.BaseDirectory.Replace("Debug", string.Empty).Replace("Release", string.Empty).Replace("bin", string.Empty).Replace("\\\\\\", string.Empty);
        private static readonly string ActiveSolutionDirectory = Path.GetDirectoryName(ActiveProjectDirectory);

        /// <summary>
        /// Gets currently active project directory
        /// </summary>
        /// <returns></returns>
        public static string ProjectDirectory
        {
            get { return ActiveProjectDirectory; }
        }

        /// <summary>
        /// Gets solution directory
        /// </summary>
        /// <returns></returns>
        public static string SolutionDirectory
        {
            get { return ActiveSolutionDirectory; }
        }
    }
}
