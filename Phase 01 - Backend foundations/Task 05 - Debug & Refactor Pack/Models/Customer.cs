using Task_05___Debug___Refactor_Pack.Services;

namespace Task_05___Debug___Refactor_Pack.Models;

public class Customer
{
    public string Name { get; }
    public CustomerType Type { get; }

    public Customer(string name, CustomerType type)
    {
        Name = ValidationHelper.RequireNotEmpty(name, "Customer name");
        Type = type;
    }
}
