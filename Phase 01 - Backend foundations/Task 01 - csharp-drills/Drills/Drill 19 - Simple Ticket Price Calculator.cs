using System.Threading.Channels;

namespace Task_01_csharp_drills.Drills
{
    internal class Drill_19_Simple_Ticket_Price_Calculator
    {
        public static void Run()
        {
            Console.WriteLine("================================================");
            Console.WriteLine("||  Simple Ticket Price Calculator           ||");
            Console.WriteLine("================================================\n");

            do
            {
                decimal basePrice = 100m;
                decimal discount = 0;
                string discountType = "None";
                do
                {
                    Console.Write("Enter your age: ");
                    if (!int.TryParse(Console.ReadLine(), out int age) || age <= 0)
                    {
                        Console.WriteLine("Error: Invalid age, The age must be greater than 0.");
                        continue;
                    }
                    if (age < 12) { discount = Math.Max(discount, 0.50m); discountType = "Child"; }
                    else if (age > 60) { discount = Math.Max(discount, 0.30m); discountType = "Old"; }
                    do
                    {
                        Console.Write("Are you student? (y/n): ");
                        var op = Console.ReadLine()?.ToLower();
                        if (op == "y" && discount < 0.20m) { discount = Math.Max(discount, 0.20m); discountType = "Student";}
                        else if ( op != "y" && op != "n") { Console.WriteLine("Invalid option."); continue; }

                        break;

                    } while (true);
                    break;
                } while (true);


                decimal finalPrice = basePrice - (basePrice * discount);

                Console.WriteLine("\n--- Ticket Price ---");
                Console.WriteLine($"Base Price: ${basePrice:F2}");
                Console.WriteLine($"Discount ({discountType}): {(int)(discount*100)}%");
                Console.WriteLine($"Final Price: ${finalPrice:F2}\n");
                break;
            } while (true);
        }
    }
}