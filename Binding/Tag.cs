namespace Binding
{
    using System;
    using Helper;

    /// <summary>
    /// Tag Class
    /// </summary>
    public static class Tag
    {
        /// <summary>
        /// Make time-based marker using input string
        /// </summary>
        /// <param name="internalReference"></param>
        /// <returns></returns>
        public static string Make(string internalReference)
        {
            return internalReference + string.Format("[{0}.{1:yyyyMMddHHmm}]", Enums.FileUpdated.EdVard.ToString(), DateTime.Now);
        }
    }
}