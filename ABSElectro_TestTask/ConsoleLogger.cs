using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSElectro_TestTask
{
    public class ConsoleLogger : ILogger
    {
        public void Debug(string message)
        {
            Log("DEBUG: " + message);
        }

        public void Error(string message)
        {
            Log("ERROR: " + message);
        }

        public void Message(string message)
        {
            Log("MESSAGE: " + message);
        }

        public void Warning(string message)
        {
            Log("WARNING: " + message);
        }

        private void Log(string message)
        {
            Console.WriteLine(DateTime.Now+" "+message);
        }
    }
}
