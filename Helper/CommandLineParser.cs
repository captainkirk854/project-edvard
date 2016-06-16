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
    ///   modified: Regex splitter = new Regex(@"^-{1,2}|^/|=|:", RegexOptions.IgnoreCase | RegexOptions.Compiled);  - removed '|:' to allow file paths
    /// </remarks>
    public class CommandLineParser
    {
        // Variables
        private StringDictionary parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineParser" /> class.
        /// </summary>
        /// <remarks>
        /// Valid parameter forms:
        ///        {-,/,--}param{ ,=,:}((",')value(",'))
        /// Examples: 
        ///        -param1 value1 --param2 /param3:"Test-:-work" 
        ///        /param4=happy -param5 '--=nice=--'
        /// </remarks>
        /// <param name="args"></param>
        public CommandLineParser(string[] args)
        {
            // Initialise ..
            this.parameters = new StringDictionary();
            Regex splitter = new Regex(@"^-{1,2}|^/|=", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Regex remover = new Regex(@"^['""]?(.*?)['""]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            string parameter = null;
            string[] parts;

            // Process each argument ..
            foreach (string text in args)
            {
                // Look for new parameters (-,/ or --) and a possible enclosed value (=,:)
                parts = splitter.Split(text, 3);

                // Found a value (for the last parameter 
                switch (parts.Length)
                {     
                    // found (space separator))
                    case 1:
                        if (parameter != null)
                        {
                            if (!this.parameters.ContainsKey(parameter)) 
                            {
                                parts[0] = 
                                    remover.Replace(parts[0], "$1");

                                this.parameters.Add(parameter, parts[0]);
                            }

                            parameter = null;
                        }

                        // else Error: no parameter waiting for a value (skipped)
                        break;

                    // Found just a parameter
                    case 2:
                        // The last parameter is still waiting. 
                        // With no value, set it to true.
                        if (parameter != null)
                        {
                            if (!this.parameters.ContainsKey(parameter))
                            {
                                this.parameters.Add(parameter, "true");
                            }
                        }

                        parameter = parts[1];
                        break;

                    // Parameter with enclosed value
                    case 3:
                        // The last parameter is still waiting. With no value, set it to true.
                        if (parameter != null)
                        {
                            if (!this.parameters.ContainsKey(parameter))
                            {
                                this.parameters.Add(parameter, "true");
                            }
                        }

                        parameter = parts[1];

                        // Remove possible enclosing characters (",')
                        if (!this.parameters.ContainsKey(parameter))
                        {
                            parts[2] = remover.Replace(parts[2], "$1");
                            this.parameters.Add(parameter, parts[2]);
                        }

                        parameter = null;
                        break;
                }
            }

            // In case a parameter is still waiting
            if (parameter != null)
            {
                if (!this.parameters.ContainsKey(parameter))
                {
                    this.parameters.Add(parameter, "true");
                }
            }
        }

        /// <summary>
        /// Retrieve a parameter value if it exists (overriding C# indexer property)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public string this [string param]
        {
            get
            {
                return this.parameters[param];
            }
        }
    }
}