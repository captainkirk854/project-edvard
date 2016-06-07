namespace Helper
{
    public static class StatusCode
    {
        // Initialise private internals ..
        private static int emptyString = -1;
        private static int intNA = -2;
        private static string stringNA = "na";
        private static int noEquivalentKeyFoundAtExchange = -998;
        private static int noCodeFoundAfterExchange = -999;

        /// <summary>
        /// Gets Nothing
        /// </summary>
        public static string EmptyString
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Gets (int) code for: Not Applicable
        /// </summary>
        public static int NotApplicableInt
        {
            get { return intNA; }
        }

        /// <summary>
        /// Gets (string) code for: Not Applicable
        /// </summary>
        public static string NotApplicable
        {
            get { return stringNA; }
        }

        /// <summary>
        /// Gets code for: Empty String
        /// </summary>
        public static int EmptyStringInt
        {
            get { return emptyString; }
        }

        /// <summary>
        /// Gets code for: No equivalent Key Value found at Exchange for current Key Value
        /// </summary>
        public static int NoEquivalentKeyFoundAtExchange
        {
            get { return noEquivalentKeyFoundAtExchange; }
        }

        /// <summary>
        /// Gets code for: No Key Code found after Key Value Exchange
        /// </summary>
        public static int NoCodeFoundAfterExchange
        {
            get { return noCodeFoundAfterExchange; }
        }
    }
}