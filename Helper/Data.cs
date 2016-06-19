namespace Helper
{
    using System;
    using System.Data;
    using System.IO;

    public static class Data
    {
        /// <summary>
        /// Enumeration of supported statement types
        /// </summary>
        private enum StatementType
        {
            Set,
            Where
        }

        /// <summary>
        /// Sort DataTable contents using sort criteria ..
        /// </summary>
        /// <param name="table"></param>
        /// <param name="sortInstruction"></param>
        /// <returns>Sorted DataTable</returns>
        /// <remarks>
        /// default sort option: asc
        /// Comma separated field names and sort option. E.g. "field1 ASC, field2 DESC"
        /// </remarks>
        public static DataTable Sort(this DataTable table, string sortInstruction)
        {
            try
            {
                DataView view = new DataView(table);
                view.Sort = sortInstruction;
                return view.ToTable();
            }
            catch
            {
                return table;
            }
        }

        /// <summary>
        /// Update valid Column Field(s) matching valid Where Condition
        /// </summary>
        /// <param name="table"></param>
        /// <param name="set"></param>
        /// <param name="where"></param>
        /// <example>Update (iTable, "Column1=A, Column2=B", "Column1=A</example>
        /*
         Have a gut feel that this would be more elegant as a LINQ statement.
         Not sure how to implement column existence and value validity checks that
         this method currently has.
        */
        public static void Update(this DataTable table, string set, string where)
        {
            // Initialise ..
            const char Comma = ',';
            const char Equals = '=';

            // Get column(s) to be updated by set clause ...
            set = CheckStatement(table, set, StatementType.Set);

            // Get row(s) affected by where clause ..
            DataRow[] rowsToUpdate = GetRowsInWhere(table, where);

            // Extract Column(s) and Value(s) from Set clause ...
            string[] setAssignments = set.Split(Comma);
            for (int assignmentIndex = 0; assignmentIndex < setAssignments.Length; assignmentIndex++)
            {
                string setAssignment = setAssignments[assignmentIndex].Trim();
                string[] columnAndvalue = setAssignment.Split(Equals);

                // Decompose further if assignment statement is valid ..
                if (columnAndvalue.Length == 2)
                {
                    string columnName = columnAndvalue[0].Trim();
                    string columnValue = columnAndvalue[1].Trim();

                    // Loop through and update affected column(s) of affected row(s) ..
                    foreach (DataRow rowToUpdate in rowsToUpdate)
                    {
                        rowToUpdate.SetField(columnName, columnValue);
                    }
                }
            }
        }

        /// <summary>
        /// Find Row(s) matching Where Condition in DataTable
        /// </summary>
        /// <param name="table"></param>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        public static DataRow[] GetRowsInWhere(this DataTable table, string whereClause)
        {
            return table.Select(CheckStatement(table, whereClause, StatementType.Where));
        }

        /// <summary>
        /// Check Column Existence
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static bool IsColumn(this DataTable table, string columnName)
        {
            return table.Columns.Contains(columnName.Trim());
        }

        /// <summary>
        /// Check Column Existence
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static bool IsColumn(this DataRow row, string columnName)
        {
            return Convert.IsDBNull(row[columnName.Trim()]);
        }

        /// <summary>
        /// Get Column Data Type
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static Type GetType(this DataTable table, string columnName)
        {
            try
            {
                return table.Columns[columnName.Trim()].DataType;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Add Column with Default Value
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnName"></param>
        /// <param name="columnDefaultValue"></param>
        public static void AddDefaultColumn(this DataTable table, string columnName, string columnDefaultValue)
        {
            // Define column ..
            DataColumn newColumn = new DataColumn(columnName.Trim(), typeof(string));
            newColumn.DefaultValue = columnDefaultValue;
            
            // Stick it ..
            if (!table.Columns.Contains(columnName.Trim()))
            {
                table.Columns.Add(newColumn);
            }
        }

        /// <summary>
        /// Display contents of a DataTable type
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnSpacing"></param>
        /// <param name="columnSpecific"></param>
        public static void Display(this DataTable table, int columnSpacing, string columnSpecific)
        {
            // Initialise ..
            DataTable localTable = new DataTable();

            string pipe = " | ";
            char borderElement = '-';
            string border = new string(borderElement, columnSpacing);
            string join = borderElement + pipe.Trim() + borderElement;
            string outline = border + join;
            string ellipsis = "...";

            // If only looking to display a specific column in DataTable ..
            if (columnSpecific.Length > 0)
            {
                // Query specific field from incoming DataTable with results as a list of anonymous data types ..
                var query = from row in table.AsEnumerable()
                          select
                             new
                               {
                                SpecificField = row.Field<string>(columnSpecific)
                               };

                // Add a column to DataTable to receive Custom Field row data ..
                localTable.TableName = "Specific Field Display";
                localTable.Columns.Add(columnSpecific, typeof(string));

                // Add the rowdata ..
                foreach (var queryresult in query)
                {
                    localTable.Rows.Add(queryresult.SpecificField);
                }
            }
            else
            {
                localTable = table; // Copy entire DataTable as everything is to be seen ..
            }

            // Get DataTable metrics ..
            int columnTotal = localTable.Columns.Count;
            int rowTotal = localTable.Rows.Count;

            // Display ..
            Console.WriteLine();
            Console.WriteLine("--- DataTable({0}) ---", localTable.TableName);
            Console.WriteLine("--- {0} Column(s) x {1} Row(s) ---", columnTotal, rowTotal);
            Console.WriteLine();
         
            // Construct Column Header(s) ...
            for (int i = 0; i < columnTotal; i++) { Console.Write(outline); } // Column top ..
            Console.Write(Environment.NewLine);

            // Column Name(s) ...
            for (int columnIndex = 0; columnIndex < columnTotal; columnIndex++)
            {
                string columnName = localTable.Columns[columnIndex].ToString();
                Console.Write(string.Format("{0, " + borderElement + columnSpacing + "}" + pipe, columnName));
            }

            Console.Write(Environment.NewLine);

            for (int i = 0; i < columnTotal; i++) { Console.Write(outline); } // Column bottom ..
            Console.Write(Environment.NewLine);

            // Output row data ..
            for (int rowIndex = 0; rowIndex < rowTotal; rowIndex++)
            {
                DataRow row = localTable.Rows[rowIndex];
                for (int columnIndex = 0; columnIndex < columnTotal; columnIndex++)
                {
                    string columnData = row[columnIndex].ToString();
                    if (columnData.Length > columnSpacing)
                    {
                        columnData = columnData.Substring(0, columnSpacing - ellipsis.Length) + ellipsis;
                    }

                    Console.Write(string.Format("{0, " + borderElement + columnSpacing + "}" + pipe, columnData));
                }

                Console.Write(Environment.NewLine);
            }

            // Construct Column Footer(s) ...
            for (int i = 0; i < columnTotal; i++) { Console.Write(outline); }
            Console.Write(Environment.NewLine);
        }

        /// <summary>
        /// Create a CSV file
        /// </summary>
        /// <param name="table"></param>
        /// <param name="csvFilepath"></param>
        public static void CreateCSV(this DataTable table, string csvFilepath)
        {
            // Initialise ..
            const string Comma = ",";
            string column = string.Empty;

            // Get DataTable metrics ..
            int columnTotal = table.Columns.Count;
            int rowTotal = table.Rows.Count;

            if (File.Exists(csvFilepath)) { File.Delete(csvFilepath); }
            StreamWriter csv = File.CreateText(csvFilepath);

            // Create delimited list of column names .. 
            for (int columnIndex = 0; columnIndex < columnTotal; columnIndex++)
            {
                column += table.Columns[columnIndex].ToString() + Comma;
            }

            csv.WriteLine(column);

            // Create delimited list of row values ..
            for (int rowIndex = 0; rowIndex < rowTotal; rowIndex++)
            {
                string dataRow = string.Empty;
                for (int columnIndex = 0; columnIndex < columnTotal; columnIndex++)
                {
                    dataRow += table.Rows[rowIndex][columnIndex].ToString() + Comma;
                }

                csv.WriteLine(dataRow);
            }

            csv.Close();
        }

        /// <summary>
        /// Check existence of referenced Column(s) and convert to 'legal' Statement
        /// </summary>
        /// <example>CheckStatement(DataTable, "Column1Exists = Red, Column2NotExists = £$"$""!!, Column3Exists = 5);</example>
        /// <param name="table"></param>
        /// <param name="statement"></param>
        /// <param name="checktype"></param>
        /// <returns></returns>
        private static string CheckStatement(DataTable table, string statement, StatementType checktype)
        {
            // Initialise ..
            const char Quote = '\'';
            const char Comma = ',';
            const char Equals = '=';
            const string And = " and ";
            string statementFinal = string.Empty;

            // Set appropriate statement connector ..
            string connector = string.Empty;
            switch (checktype)
            {
                case StatementType.Set:
                    connector = Comma.ToString();
                    break;

                case StatementType.Where:
                    connector = And;
                    break;

                default:
                    connector = Comma.ToString();
                    break;
            }

            // Decompose multiple assignments into column(s) and value(s) ..
            string[] assignments = statement.Split(Comma);
            for (int assignmentIndex = 0; assignmentIndex < assignments.Length; assignmentIndex++)
            {
                string assignment = assignments[assignmentIndex].Trim();
                string[] columnAndvalue = assignment.Split(Equals);

                // Decompose further if assignment statement is valid ..
                if (columnAndvalue.Length == 2)
                {
                    string columnName = columnAndvalue[0].Trim();
                    string columnValue = columnAndvalue[1].Trim();

                    //Check column exists in DataTable ..
                    if (IsColumn(table, columnName))
                    {
                        // .. and get its datatype ..
                        if (GetType(table, columnName) == typeof(string))
                        {
                            columnValue = Quote + columnValue + Quote;
                        }

                        // Construct final set statement ...
                        statementFinal += columnName + Equals + columnValue + connector;
                    }
                }
            }

            // Remove extra connector ..
            return statementFinal.Substring(0, statementFinal.Length - connector.ToString().Length);
        }
    }
}