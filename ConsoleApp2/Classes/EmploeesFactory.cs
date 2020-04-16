using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ConsoleApp2.Classes
{
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
                ConstructorInfo info = type.GetConstructor(Type.EmptyTypes);
                Employee employee = (info != null) ? info.Invoke(null) as Employee : null;
                if (employee != null)
                {
                    StringBuilder sb = new StringBuilder(employee.Name);
                    employee.Name = sb.Append((i + 1).ToString()).ToString();
                    list.Add(employee);
                }
                else throw new CtorException("Не найден конструктор");
            }
        }
        public List<Employee> getEmployees()
        {
            List<Employee> listEmployee = new List<Employee>();
            Type baseType = typeof(Employee);
            IEnumerable<Type> types = Assembly.GetAssembly(baseType).GetTypes().Where(type => type.IsSubclassOf(baseType));

            foreach (Type type in types)
            {
                switch (type.Name)
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
}
