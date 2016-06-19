namespace Helper
{
    using System;
    using System.Collections.Specialized;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Generic Command Line Parser
    /// </summary>
    /// <remarks>
    /// Author: Richard Lopes (GriffonRL)
    /// Almost a straight copy (adjusted to StyleCop rules for this solution) from:
    ///   ref: http://www.codeproject.com/Articles/3111/C-NET-Command-Line-Arguments-Parser
    ///   modified: Regex splitter = new Regex(@"^-{1,2}|^/|=|:", RegexOptions.IgnoreCase | RegexOptions.Compiled);  - removed '|:' to allow file path drive letter prefix
    /// </remarks>
    public class CommandLine
    {
        // Variables
        private StringDictionary argParameters;
        private string[] args = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine" /> class.
        /// </summary>
        /// <param name="args"></param>
        public CommandLine(string[] args)
        {
            this.args = args;
        }

        /// <summary>
        /// Parse Argument List and extract Parameter Value
        /// </summary>
        /// <remarks>
        /// Valid parameter forms:
        ///        {-,/,--}param{ ,=,:}((",')value(",'))
        /// Examples: 
        ///        -param1 value1 --param2 /param3:"Test-:-work" 
        ///        /param4=happy -param5 '--=nice=--'
        /// </remarks>
        /// <param name="param"></param>
        /// <param name="isFilepath"></param>
        /// <returns></returns>
        public string Parse(string param, bool isFilepath = false)
        {
            // Initialise ..
            Regex splitter = new Regex(@"^-{1,2}|^/|=|:", RegexOptions.IgnoreCase | RegexOptions.Compiled);     
            if (isFilepath) { splitter = new Regex(@"^-{1,2}|^/|=", RegexOptions.IgnoreCase | RegexOptions.Compiled); }
            Regex remover = new Regex(@"^['""]?(.*?)['""]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            this.argParameters = new StringDictionary();
            string argParameter = null;
            string[] parts;

            // Process each argument ..
            foreach (string arg in this.args)
            {
                // Look for new parameters (-,/ or --) and a possible enclosed value (=,:)
                parts = splitter.Split(arg, 3);

                // Found a value (for the last parameter 
                switch (parts.Length)
                {     
                    // found (space separator))
                    case 1:
                        if (argParameter != null)
                        {
                            if (!this.argParameters.ContainsKey(argParameter)) 
                            {
                                parts[0] = remover.Replace(parts[0], "$1");

                                this.argParameters.Add(argParameter, parts[0]);
                            }

                            argParameter = null;
                        }

                        // else Error: no parameter waiting for a value (skipped)
                        break;

                    // Found just a parameter
                    case 2:
                        // The last parameter is still waiting. 
                        // With no value, set it to true.
                        if (argParameter != null)
                        {
                            if (!this.argParameters.ContainsKey(argParameter))
                            {
                                this.argParameters.Add(argParameter, "true");
                            }
                        }

                        argParameter = parts[1];
                        break;

                    // Parameter with enclosed value
                    case 3:
                        // The last parameter is still waiting. With no value, set it to true.
                        if (argParameter != null)
                        {
                            if (!this.argParameters.ContainsKey(argParameter))
                            {
                                this.argParameters.Add(argParameter, "true");
                            }
                        }

                        argParameter = parts[1];

                        // Remove possible enclosing characters (",')
                        if (!this.argParameters.ContainsKey(argParameter))
                        {
                            parts[2] = remover.Replace(parts[2], "$1");
                            this.argParameters.Add(argParameter, parts[2]);
                        }

                        argParameter = null;
                        break;
                }
            }

            // In case a parameter is still waiting
            if (argParameter != null)
            {
                if (!this.argParameters.ContainsKey(argParameter))
                {
                    this.argParameters.Add(argParameter, "true");
                }
            }

            // Return desired parameter from arguments dictionary ..
            return this.argParameters[param];
        }
    }
}