namespace Helper
{
    using System;
    using System.Diagnostics;
    using System.Threading;

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

        /// <summary>
        /// Simple File Printing facility
        /// </summary>
        /// <param name="filename"></param>
        public static void SendToPrinter(string filename)
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