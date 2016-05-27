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
        /// Display contents of a DataTable type
        /// </summary>
        /// <param name="iTable"></param>
        /// <param name="ColumnSpacing"></param>
        /// <param name="ColumnSpecific"></param>
        public static void Display(this DataTable iTable, int ColumnSpacing, string ColumnSpecific)
        {
            // Initialise ..
            DataTable table = new DataTable();

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
                var query = from row in iTable.AsEnumerable()
                          select
                             new
                               {
                                SpecificField = row.Field<string>(ColumnSpecific)
                               };

                // Add a column to DataTable to receive Custom Field row data ..
                table.TableName = "Specific Field Display";
                table.Columns.Add(ColumnSpecific, typeof(string));

                // Add the rowdata ..
                foreach (var queryresult in query)
                {
                    table.Rows.Add(queryresult.SpecificField);
                }
            }
            else
            {
                table = iTable; // Copy entire DataTable as everything is to be seen ..
            }

            // Get DataTable metrics ..
            int columnTotal = table.Columns.Count;
            int rowTotal = table.Rows.Count;

            // Display ..
            Console.WriteLine();
            Console.WriteLine("--- DataTable({0}) ---", table.TableName);
            Console.WriteLine("--- {0} Column(s) x {1} Row(s) ---", columnTotal, rowTotal);
            Console.WriteLine();
         
            // Construct Column Header(s) ...
            for (int i = 0; i < columnTotal; i++) { Console.Write(outline); } // Column top ..
            Console.Write(Environment.NewLine);

            // Column Name(s) ...
            for (int columnIndex = 0; columnIndex < columnTotal; columnIndex++)
            {
                string columnName = table.Columns[columnIndex].ToString();
                Console.Write(String.Format("{0, " + borderElement + ColumnSpacing + "}" + pipe, columnName));
            }
            Console.Write(Environment.NewLine);

            for (int i = 0; i < columnTotal; i++) { Console.Write(outline); } // Column bottom ..
            Console.Write(Environment.NewLine);

            // Output row data ..
            for (int rowIndex = 0; rowIndex < rowTotal; rowIndex++)
            {
                DataRow row = table.Rows[rowIndex];
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
        /// <param name="iTable"></param>
        /// <param name="csvFilepath"></param>
        public static void CreateCSV(this DataTable iTable, string csvFilepath)
        {
            // Initialise ..
            const string comma = ",";
            string Column = string.Empty;

            // Get DataTable metrics ..
            int columnTotal = iTable.Columns.Count;
            int rowTotal = iTable.Rows.Count;

            if (File.Exists(csvFilepath)) { File.Delete(csvFilepath); }
            StreamWriter csv = File.CreateText(csvFilepath);

            // Create delimited list of column names .. 
            for (int columnIndex = 0; columnIndex < columnTotal; columnIndex++)
            {
                Column += iTable.Columns[columnIndex].ToString() + comma;
            }
            csv.WriteLine(Column);

            // Create delimited list of row values ..
            for (int rowIndex = 0; rowIndex < rowTotal; rowIndex++)
            {
                string DataRow = string.Empty;
                for (int ColumnIndex = 0; ColumnIndex < columnTotal; ColumnIndex++)
                {
                    DataRow += iTable.Rows[rowIndex][ColumnIndex].ToString() + comma;
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
    }
}
