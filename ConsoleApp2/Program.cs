using System;
using System.Collections.Generic;
using System.Reflection;
using System.Configuration;
using System.Threading;
using System.Diagnostics;
using System.Text;
using System.Linq;

namespace ConsoleApp1
{
    interface ILogger
    {
        void WriteLog(string message);
    }
    class ConsoleLogger : ILogger
    {
        public void WriteLog(string message)
        {
            Console.WriteLine(message);
        }
    }
    public enum ResponsePriority
    {
        High = 1,
        Medium,
        Low
    }
    abstract class Employee
    {
        object locker = new object();
        private bool status;
        public string Name { get; set; }
        public bool getStatus() { lock (locker) { return status; } }
        public void setStatus(bool status) { lock (locker) { this.status = status; } }
        public int Priority { get; private set; }
        public Employee() { }
        public Employee(int priority, bool status, string name)
        {
            Priority = priority;
            this.status = status;
            Name = name;
        }
    }
    class Operator : Employee
    {
        public Operator() : base((int)ResponsePriority.High, true, "Оператор ") { }
    }
    class Manager : Employee
    {
        public Manager() : base((int)ResponsePriority.Medium, true, "Менеджер ") { }
    }
    class Director : Employee
    {
        public Director() : base((int)ResponsePriority.Low, true, "Директор ") { }
    }
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
            if(callProcessing(employees.FindAll(emp => emp.Priority == (int)ResponsePriority.High), partialMessage))
                if(callProcessing(employees.FindAll(emp => emp.Priority == (int)ResponsePriority.Medium), partialMessage))
                    callProcessing(employees.FindAll(emp => emp.Priority == (int)ResponsePriority.Low), partialMessage);

            semaphore.Release();
        }
    }
    class EmploeesFactory
    {
        int numberOfOperators;
        int numberOfManagers;
        int numberOfDirectors;

        public EmploeesFactory(int numberOfOperators, int numberOfManagers, int numberOfDirectors)
        {
            this.numberOfOperators = numberOfOperators;
            this.numberOfManagers = numberOfManagers;
            this.numberOfDirectors = numberOfDirectors;
        }

        void initializeGroup(int size, Type type, List<Employee> list)
        {
            for (int i = 0; i < size; i++)
            {
                ConstructorInfo info = type.GetConstructor(new Type[] { });
                Employee employee = (Employee)info.Invoke(new object[] { });
                StringBuilder sb = new StringBuilder(employee.Name);
                employee.Name = sb.Append((i + 1).ToString()).ToString();
                list.Add(employee);
            }
        }
        public List<Employee> getEmployees()
        {
            List<Employee> listEmployee = new List<Employee>();
            Type baseType = typeof(Employee);
            IEnumerable<Type> types = Assembly.GetAssembly(baseType).GetTypes().Where(type => type.IsSubclassOf(baseType));

            foreach (Type type in types)
            {
                switch(type.Name)
                {
                    case "Operator":
                        initializeGroup(numberOfOperators, type, listEmployee);
                        break;
                    case "Manager":
                        initializeGroup(numberOfManagers, type, listEmployee);
                        break;
                    case "Director":
                        initializeGroup(numberOfDirectors, type, listEmployee);
                        break;
                    default:
                        throw new InvalidOperationException("Непонятный тип какой-то");
                }
            }
            return listEmployee;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            int numberOfOperators = int.Parse(ConfigurationManager.AppSettings["operator"]);
            int numberOfManagers = int.Parse(ConfigurationManager.AppSettings["manager"]);
            int numberOfDirectors = 1;
            int poolSize = numberOfOperators + numberOfManagers + numberOfDirectors;

            EmploeesFactory ef = new EmploeesFactory(numberOfOperators, numberOfManagers, numberOfDirectors);

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
    }
}
