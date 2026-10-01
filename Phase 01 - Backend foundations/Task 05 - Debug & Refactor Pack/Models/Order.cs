using Task_05___Debug___Refactor_Pack.Services;

namespace Task_05___Debug___Refactor_Pack.Models;

public class Order
{
    public Customer Customer { get; }
    public string ProductName { get; }
    public decimal UnitPrice { get; }
    public int Quantity { get; }

    public decimal Subtotal => UnitPrice * Quantity;

    public Order(Customer customer, string productName, decimal unitPrice, int quantity)
    {
        Customer = customer ?? throw new ArgumentNullException(nameof(customer));
        ProductName = ValidationHelper.RequireNotEmpty(productName, "Product name");
        UnitPrice = ValidationHelper.RequirePositive(unitPrice, "Price");
        Quantity = ValidationHelper.RequirePositive(quantity, "Quantity");
    }
}
