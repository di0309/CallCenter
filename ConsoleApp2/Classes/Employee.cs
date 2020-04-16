namespace ConsoleApp2
{
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
}
