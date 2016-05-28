namespace Helpers
{
    using System;
    using System.Data;
    using System.IO;
    using System.Diagnostics;
    using System.Threading;

    public static class Data
    {
        /// <summary>
        /// Sort DataTable contents using sort criteria ..
        /// </summary>
        /// <param name="table"</param>
        /// <param name="sortInstruction">Comma separated field names and sort option. E.g. "field1 ASC, field2 DESC"</param>
        /// <returns>Sorted DataTable</returns>
        /// <remarks>
        ///     o default sort option: asc
        /// </remarks>
        public static DataTable Sort(this DataTable table, string sortInstruction)
        {
            try
            {
                DataView dtView = new DataView(table);
                dtView.Sort = sortInstruction;
                return dtView.ToTable();
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
        /// <param name="Set"></param>
        /// <param name="Where"></param>
        /// <example>Update (iTable, "Column1=A, Column2=B", "Column1=A</example>
        /*
         Have a gut feel that this would be more elegant as a LINQ statement.
         Not sure how to implement column existence and value validity checks that
         this method currently has.
        */
        public static void Update(this DataTable table, string Set, string Where)
        {
            // Initialise ..
            const char comma = ',';
            const char equals = '=';

            // Get column(s) to be updated by set clause ...
            Set = CheckStatement(table, Set, StatementType.Set);

            // Get row(s) affected by where clause ..
            DataRow[] rowsToUpdate = GetRowsInWhere(table, Where);

            // Extract Column(s) and Value(s) from Set clause ...
            string[] setAssignments = Set.Split(comma);
            for (int assignmentIndex = 0; assignmentIndex < setAssignments.Length; assignmentIndex++)
            {
                string setAssignment = setAssignments[assignmentIndex].Trim();
                string[] columnAndvalue = setAssignment.Split(equals);

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
        /// <param name="WhereClause"></param>
        /// <returns></returns>
        public static DataRow[] GetRowsInWhere(this DataTable table, string WhereClause)
        {
            return table.Select(CheckStatement(table, WhereClause, StatementType.Where));
        }

        /// <summary>
        /// Check Column Existence
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static bool IsColumn(this DataTable table, string ColumnName)
        {
            return table.Columns.Contains(ColumnName.Trim());
        }

        /// <summary>
        /// Check Column Existence
        /// </summary>
        /// <param name="row"></param>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public static bool IsColumn(this DataRow row, string ColumnName)
        {
            return Convert.IsDBNull(row[ColumnName.Trim()]);
        }

        /// <summary>
        /// Get Column Data Type
        /// </summary>
        public static Type GetType(this DataTable table, string ColumnName)
        {
            try
            {
                return table.Columns[ColumnName.Trim()].DataType;
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
        /// <param name="ColumnName"></param>
        /// <param name="ColumnDefaultValue"></param>
        public static void AddDefaultColumn(this DataTable table, string ColumnName, string ColumnDefaultValue)
        {
            // Define column ..
            DataColumn newColumn = new DataColumn(ColumnName.Trim(), typeof(string));
            newColumn.DefaultValue = ColumnDefaultValue;
            
            // Stick it ..
            if(!table.Columns.Contains(ColumnName.Trim()))
            {
                table.Columns.Add(newColumn);
            }
        }

        /// <summary>
        /// Display contents of a DataTable type
        /// </summary>
        /// <param name="table"></param>
        /// <param name="ColumnSpacing"></param>
        /// <param name="ColumnSpecific"></param>
        public static void Display(this DataTable table, int ColumnSpacing, string ColumnSpecific)
        {
            // Initialise ..
            DataTable localTable = new DataTable();

            string pipe = " | ";
            char borderElement = '-';
            string border = new string(borderElement, ColumnSpacing);
            string join = borderElement + pipe.Trim() + borderElement;
            string outline = border + join;
            string ellipsis = "...";

            // If only looking to display a specific column in DataTable ..
            if (ColumnSpecific.Length > 0)
            {
                // Query specific field from incoming DataTable with results as a list of anonymous data types ..
                var query = from row in table.AsEnumerable()
                          select
                             new
                               {
                                SpecificField = row.Field<string>(ColumnSpecific)
                               };

                // Add a column to DataTable to receive Custom Field row data ..
                localTable.TableName = "Specific Field Display";
                localTable.Columns.Add(ColumnSpecific, typeof(string));

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
                Console.Write(String.Format("{0, " + borderElement + ColumnSpacing + "}" + pipe, columnName));
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
                    if (columnData.Length > ColumnSpacing)
                    {
                        columnData = columnData.Substring(0, (ColumnSpacing - ellipsis.Length)) + ellipsis;
                    }
                    Console.Write(String.Format("{0, " + borderElement + ColumnSpacing + "}" + pipe, columnData));
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
            const string comma = ",";
            string Column = string.Empty;

            // Get DataTable metrics ..
            int columnTotal = table.Columns.Count;
            int rowTotal = table.Rows.Count;

            if (File.Exists(csvFilepath)) { File.Delete(csvFilepath); }
            StreamWriter csv = File.CreateText(csvFilepath);

            // Create delimited list of column names .. 
            for (int columnIndex = 0; columnIndex < columnTotal; columnIndex++)
            {
                Column += table.Columns[columnIndex].ToString() + comma;
            }
            csv.WriteLine(Column);

            // Create delimited list of row values ..
            for (int rowIndex = 0; rowIndex < rowTotal; rowIndex++)
            {
                string DataRow = string.Empty;
                for (int ColumnIndex = 0; ColumnIndex < columnTotal; ColumnIndex++)
                {
                    DataRow += table.Rows[rowIndex][ColumnIndex].ToString() + comma;
                }
                csv.WriteLine(DataRow);
            }
            csv.Close();

            //Info ..
            Console.WriteLine("CREATED {0}", csvFilepath);
        }

        /// <summary>
        /// Simple File Printing facility
        /// </summary>
        /// <param name="filename"></param>
        private static void SendToPrinter(string filename)
        {
            // Define the print job ..
            ProcessStartInfo printjob = new ProcessStartInfo();
            printjob.Verb = "PRINT";
            printjob.FileName = @filename;
            printjob.CreateNoWindow = true;
            printjob.WindowStyle = ProcessWindowStyle.Hidden;

            // Start ..
            Process printProcess = new Process();
            printProcess.StartInfo = printjob;
            printProcess.Start();

            long ticks = -1;
            while (ticks != printProcess.TotalProcessorTime.Ticks)
            {
                ticks = printProcess.TotalProcessorTime.Ticks;
                Thread.Sleep(1000);
            }

            if (!printProcess.CloseMainWindow())
            {
                printProcess.Kill();
            }
        }

        /// <summary>
        /// Enumeration of supported statement types
        /// </summary>
        private enum StatementType
        {
            Set,
            Where
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
            const char quote = '\'';
            const char comma = ',';
            const char equals = '=';
            const string and = " and ";
            string statementFinal = string.Empty;

            // Set appropriate statement connector ..
            string connector = string.Empty;
            switch (checktype)
            {
                case StatementType.Set:
                    connector = comma.ToString();
                    break;

                case StatementType.Where:
                    connector = and;
                    break;

                default:
                    connector = comma.ToString();
                    break;
            }

            // Decompose multiple assignments into column(s) and value(s) ..
            string[] assignments = statement.Split(comma);
            for (int assignmentIndex = 0; assignmentIndex < assignments.Length; assignmentIndex++)
            {
                string assignment = assignments[assignmentIndex].Trim();
                string[] columnAndvalue = assignment.Split(equals);

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
                            columnValue = quote + columnValue + quote;
                        }

                        // Construct final set statement ...
                        statementFinal += columnName + equals + columnValue + connector;
                    }
                }
            }

            // Remove extra connector ..
            return statementFinal.Substring(0, statementFinal.Length - connector.ToString().Length);
        }
    }
}
