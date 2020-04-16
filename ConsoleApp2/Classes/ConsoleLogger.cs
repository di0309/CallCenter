using System;

namespace ConsoleApp2
{
    class ConsoleLogger : ILogger
    {
        public void WriteLog(string message)
        {
            Console.WriteLine(message);
        }
    }
}
