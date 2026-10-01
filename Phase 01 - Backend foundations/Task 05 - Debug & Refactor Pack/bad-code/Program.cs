//class Program
//{
//    static void Main(string[] args)
//    {
//        Console.WriteLine("Enter customer name:");
//        string c = Console.ReadLine();

//        Console.WriteLine("Enter product name:");
//        string p = Console.ReadLine();

//        Console.WriteLine("Enter product price:");
//        double pr = double.Parse(Console.ReadLine());

//        Console.WriteLine("Enter quantity:");
//        int q = int.Parse(Console.ReadLine());

//        Console.WriteLine("Enter customer type Regular/Silver/Gold/VIP:");
//        string t = Console.ReadLine();

//        double total = pr * q;
//        double discount = 0;

//        if (t == "Regular")
//        {
//            discount = 0;
//        }
//        else if (t == "Silver")
//        {
//            discount = total * 0.05;
//        }
//        else if (t == "Gold")
//        {
//            discount = total * 0.10;
//        }
//        else if (t == "VIP")
//        {
//            discount = total * 0.15;
//        }

//        double afterDiscount = total - discount;
//        double tax = afterDiscount * 0.14;
//        double shipping = 50;

//        if (afterDiscount >= 1000)
//        {
//            shipping = 0;
//        }

//        double finalTotal = afterDiscount + tax + shipping;

//        Console.WriteLine("Customer: " + c);
//        Console.WriteLine("Product: " + p);
//        Console.WriteLine("Price: " + pr);
//        Console.WriteLine("Quantity: " + q);
//        Console.WriteLine("Subtotal: " + total);
//        Console.WriteLine("Discount: " + discount);
//        Console.WriteLine("Tax: " + tax);
//        Console.WriteLine("Shipping: " + shipping);
//        Console.WriteLine("Final Total: " + finalTotal);
//    }
//}
