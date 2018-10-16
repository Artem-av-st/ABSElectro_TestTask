using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSElectro_TestTask
{
    public interface ILogger
    {
        void Message(string message);
        void Debug(string message);
        void Warning(string message);
        void Error(string message);
    }
}
