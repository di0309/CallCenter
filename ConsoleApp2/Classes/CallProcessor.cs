using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace ConsoleApp2.Classes
{
    class CallProcessor
    {
        SemaphoreSlim semaphore;
        int queueCounter = 0;
        object countLocker = new object();
        ILogger logger;
        List<Employee> employees;

        public CallProcessor(int poolSize, ILogger logger, List<Employee> employees)
        {
            semaphore = new SemaphoreSlim(poolSize, poolSize);
            this.logger = logger;
            this.employees = employees;
        }
        bool callProcessing(List<Employee> employees, string partialMessage)
        {
            Random rnd = new Random();
            for (int i = 0; i < employees.Count; i++)
            {
                if (employees[i].getStatus())
                {
                    employees[i].setStatus(false);
                    logger.WriteLog(string.Format(partialMessage + "Звонок {0} - {1}", Thread.CurrentThread.Name, employees[i].Name));
                    Thread.Sleep(rnd.Next(1000, 2000));
                    logger.WriteLog(string.Format("Звонок {0} -----> завершен.", Thread.CurrentThread.Name));
                    employees[i].setStatus(true);
                    return false;
                }
            }
            return true;
        }
        public void CallTraffic()
        {
            Stopwatch sw = new Stopwatch();
            int count = 0;
            string partialMessage = string.Empty;

            if (semaphore.CurrentCount == 0)
            {
                sw.Start();
                lock (countLocker)
                {
                    count = ++queueCounter;
                }
            }
            else
            {
                lock (countLocker)
                {
                    queueCounter = 0;
                }
            }

            semaphore.Wait();
            if (count > 0)
            {
                sw.Stop();
                partialMessage = string.Format("В ожидании ({0} секунда, {1}й в очереди) ", sw.Elapsed.TotalSeconds, count);
            }
            if (callProcessing(employees.FindAll(emp => emp.Priority == (int)ResponsePriority.High), partialMessage))
                if (callProcessing(employees.FindAll(emp => emp.Priority == (int)ResponsePriority.Medium), partialMessage))
                    callProcessing(employees.FindAll(emp => emp.Priority == (int)ResponsePriority.Low), partialMessage);

            semaphore.Release();
        }
    }
}
