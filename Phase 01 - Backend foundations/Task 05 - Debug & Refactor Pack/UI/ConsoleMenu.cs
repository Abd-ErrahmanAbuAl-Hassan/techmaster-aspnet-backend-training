using System.Globalization;
using Task_05___Debug___Refactor_Pack.Models;
using Task_05___Debug___Refactor_Pack.Services;

namespace Task_05___Debug___Refactor_Pack.UI;

public class ConsoleMenu
{
    private readonly OrderCalculator _calculator = new();
    private readonly ReceiptPrinter _printer = new();

    public void Run()
    {
        string name = Ask("Enter customer name:", v => ValidationHelper.RequireNotEmpty(v, "Customer name"));
        string product = Ask("Enter product name:", v => ValidationHelper.RequireNotEmpty(v, "Product name"));
        decimal price = Ask("Enter product price:", ParsePrice);
        int quantity = Ask("Enter quantity:", ParseQuantity);
        CustomerType type = Ask("Enter customer type Regular/Silver/Gold/VIP:", ParseCustomerType);

        var order = new Order(new Customer(name, type), product, price, quantity);
        OrderSummary summary = _calculator.Calculate(order);
        _printer.Print(order, summary);
    }

    private static T Ask<T>(string prompt, Func<string?, T> parser)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            try
            {
                return parser(Console.ReadLine());
            }
            catch (Exception ex) when (ex is ArgumentException or FormatException)
            {
                Console.WriteLine($"Invalid input: {ex.Message} Please try again.");
            }
        }
    }

    private static decimal ParsePrice(string? input)
    {
        if (!decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal price))
            throw new FormatException("Price must be a number.");
        return ValidationHelper.RequirePositive(price, "Price");
    }

    private static int ParseQuantity(string? input)
    {
        if (!int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out int quantity))
            throw new FormatException("Quantity must be a whole number.");
        return ValidationHelper.RequirePositive(quantity, "Quantity");
    }

    private static CustomerType ParseCustomerType(string? input)
    {
        if (!Enum.TryParse(input?.Trim(), ignoreCase: true, out CustomerType type)
            || !Enum.IsDefined(type))
            throw new FormatException("Customer type must be Regular, Silver, Gold or VIP.");
        return type;
    }
}
