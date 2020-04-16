namespace ConsoleApp2.Classes
{
    class Manager : Employee
    {
        public Manager() : base((int)ResponsePriority.Medium, true, "Менеджер ") { }
    }
}
