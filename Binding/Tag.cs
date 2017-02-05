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
            // Test to see if internal reference has been previously tagged  ..
            int pos = internalReference.IndexOf(string.Format("[{0}", EnumsInternal.FileUpdated.EdVard.ToString()));
            if (pos > -1)
            {
                // Remove old tag and recreate original internal name ..
                internalReference = internalReference.Substring(0, pos);
            }
            
            // Create and return tagged internal reference ..
            return internalReference + string.Format("[{0}.{1:yyyyMMddHHmm}]", EnumsInternal.FileUpdated.EdVard.ToString(), DateTime.Now);
        }
    }
}