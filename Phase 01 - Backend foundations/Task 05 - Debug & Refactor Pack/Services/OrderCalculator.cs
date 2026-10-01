using Task_05___Debug___Refactor_Pack.Models;

namespace Task_05___Debug___Refactor_Pack.Services;

public class OrderCalculator
{
    public const decimal TaxRate = 0.14m;
    public const decimal ShippingFee = 50m;
    public const decimal FreeShippingThreshold = 1000m;

    private const decimal SilverDiscountRate = 0.05m;
    private const decimal GoldDiscountRate = 0.10m;
    private const decimal VipDiscountRate = 0.15m;

    public OrderSummary Calculate(Order order)
    {
        decimal subtotal = order.Subtotal;
        decimal discount = subtotal * GetDiscountRate(order.Customer.Type);   
        decimal afterDiscount = subtotal - discount;
        decimal tax = afterDiscount * TaxRate;                                
        decimal shipping = GetShipping(afterDiscount);                        
        decimal finalTotal = afterDiscount + tax + shipping;

        return new OrderSummary(subtotal, discount, afterDiscount, tax, shipping, finalTotal);
    }

    public static decimal GetDiscountRate(CustomerType type) => type switch
    {
        CustomerType.Regular => 0m,
        CustomerType.Silver => SilverDiscountRate,
        CustomerType.Gold => GoldDiscountRate,
        CustomerType.VIP => VipDiscountRate,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown customer type.")
    };

    // Same behaviour as the original code: threshold is checked on the amount after discount.
    private static decimal GetShipping(decimal afterDiscount) =>
        afterDiscount >= FreeShippingThreshold ? 0m : ShippingFee;
}
