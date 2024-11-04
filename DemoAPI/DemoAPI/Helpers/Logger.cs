using System;
using System.IO;
using System.Threading.Tasks;

namespace DemoAPI.Helpers
{
    public class Logger
    {
        private readonly string _logFilePath;

        //Constructor that sets the file path where logs will be saved
        public Logger()
        {
            string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            _logFilePath = Path.Combine("C:\\Provnet\\Log", $"Demographics_log_{timeStamp}");
        }

        //Method to log a message asynchronously
        public async Task LogAsync(string message)
        {
            try
            {
                //Ensure the directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath));

                //Open the file to append text asynchronously
                using (StreamWriter writer = new StreamWriter(_logFilePath, append: true))
                {
                    //Format the log entry with a timestamp
                    string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
                    await writer.WriteLineAsync(logEntry);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logging failed: {ex.Message}");
            }
        }
    }
}
