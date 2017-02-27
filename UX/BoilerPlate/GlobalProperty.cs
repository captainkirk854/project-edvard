namespace UX.BoilerPlate
{
    using System.Runtime.CompilerServices; // For [CallerMemberName] attribute ...
    using System.Windows;

    public static class GlobalProperty
    {
        /// <summary>
        /// Stores property and its value in application dictionary
        /// </summary>
        /// <param name="propertyValue"></param>
        /// <param name="propertyName"></param>
        public static void Set(object propertyValue, [CallerMemberName] string propertyName = null)
        {
            Application.Current.Properties[propertyName] = propertyValue;
        }

        /// <summary>
        /// Gets property value from application dictionary
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object Get(string propertyName)
        {
            return Application.Current.Properties[propertyName];
        }
    }
}
