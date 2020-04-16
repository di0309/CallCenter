namespace ConsoleApp2
{
    class Director : Employee
    {
        public Director() : base((int)ResponsePriority.Low, true, "Директор ") { }
    }
}
