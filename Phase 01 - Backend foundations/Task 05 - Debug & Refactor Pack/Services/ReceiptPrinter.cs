using System.Globalization;
using Task_05___Debug___Refactor_Pack.Models;

namespace Task_05___Debug___Refactor_Pack.Services;

public class ReceiptPrinter
{
    public void Print(Order order, OrderSummary summary)
    {
        Console.WriteLine();
        Console.WriteLine("========== RECEIPT ==========");
        Console.WriteLine($"Customer : {order.Customer.Name} ({order.Customer.Type})");
        Console.WriteLine($"Product  : {order.ProductName}");
        Console.WriteLine($"Price    : {Money(order.UnitPrice)}");
        Console.WriteLine($"Quantity : {order.Quantity}");
        Console.WriteLine("-----------------------------");
        Console.WriteLine($"Subtotal : {Money(summary.Subtotal)}");
        Console.WriteLine($"Discount : -{Money(summary.Discount)}");
        Console.WriteLine($"Tax (14%): {Money(summary.Tax)}");
        Console.WriteLine($"Shipping : {Money(summary.Shipping)}");
        Console.WriteLine("-----------------------------");
        Console.WriteLine($"TOTAL    : {Money(summary.FinalTotal)}");
        Console.WriteLine("=============================");
    }

    private static string Money(decimal value) =>
        value.ToString("N2", CultureInfo.InvariantCulture);
}
