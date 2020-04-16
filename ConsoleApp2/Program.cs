using System;
using System.Configuration;
using System.Threading;
using ConsoleApp2;
using ConsoleApp2.Classes;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            int numberOfOperators = int.Parse(ConfigurationManager.AppSettings["operator"]);
            int numberOfManagers = int.Parse(ConfigurationManager.AppSettings["manager"]);
            int numberOfDirectors = 1;
            int poolSize = numberOfOperators + numberOfManagers + numberOfDirectors;
            ILogger logger = new ConsoleLogger();

            EmploeesFactory ef = new EmploeesFactory(numberOfOperators, numberOfManagers, numberOfDirectors);
            try
            {
                CallProcessor callProcessor = new CallProcessor(poolSize, new ConsoleLogger(), ef.getEmployees());

                int i = 0;
                Thread thread;

                while (true)
                {
                    i++;
                    string operation = Console.ReadKey(true).KeyChar.ToString().ToUpper();
                    thread = new Thread(callProcessor.CallTraffic);
                    thread.Name = i.ToString();
                    thread.Start();
                    if (operation == "Q") break;
                }
            }
            catch(CtorException ex)
            {
                logger.WriteLog(ex.Message);
            }
            catch(InvalidOperationException ex)
            {
                logger.WriteLog(ex.Message);
            }
            catch(Exception ex)
            {
                logger.WriteLog(ex.Message);
            }
        }
    }
}
