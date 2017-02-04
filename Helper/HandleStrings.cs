namespace Helper
{
    using System;

    /// <summary>
    /// Miscellaneous Helper Methods
    /// </summary>
    public static class HandleStrings
    {
        /// <summary>
        /// Get right-most part of string
        /// </summary>
        /// <param name="source"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Right(this string source, int length)
        {
            if (length >= source.Length)
            {
                return source;
            }
            else
            {
                return source.Substring(source.Length - length);
            }
        }

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
