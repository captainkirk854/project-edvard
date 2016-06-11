namespace Helper
{
    using System;

    /// <summary>
    /// Method Collections
    /// </summary>
    public static class Stockpile
    {
        /// <summary>
        /// Convert string to enumerated type
        /// </summary>
        /// <typeparam name="T">target: Enum Type</typeparam>
        /// <param name="enumTypeName"></param>
        /// <remarks>
        /// ref: http://stackoverflow.com/questions/16100/how-do-i-convert-a-string-to-an-enum-in-c
        /// </remarks>
        /// <returns></returns>
        public static T ParseStringToEnum<T>(string enumTypeName)
        {
            return (T)Enum.Parse(typeof(T), enumTypeName, true);
        }
    }
}
