namespace Task_05___Debug___Refactor_Pack.Models;

public record OrderSummary(
    decimal Subtotal,
    decimal Discount,
    decimal AfterDiscount,
    decimal Tax,
    decimal Shipping,
    decimal FinalTotal);
