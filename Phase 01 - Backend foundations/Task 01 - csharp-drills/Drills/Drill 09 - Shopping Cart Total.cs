namespace Task_01_csharp_drills.Drills
{
    internal class Drill_09_Shopping_Cart_Total
    {
        public static void Run()
        {
            Console.WriteLine("=======================================");
            Console.WriteLine("||     Shopping Cart Total           ||");
            Console.WriteLine("=======================================\n");

            List<(decimal price, int quantity)> cart = new List<(decimal, int)>();
            int count = 0;
            decimal total=0m, discount;
            do
            {
                Console.Write("How many items?: ");
                var userInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(userInput))
                {
                    Console.WriteLine("Error: Please provide the count.");
                }
                else
                {
                    if (!int.TryParse(userInput, out count))
                    {
                        Console.WriteLine($"Error: Invalid input\" {userInput} \" Please provide a number.");
                        continue;
                    }
                    if (count <= 0)
                    {
                        Console.WriteLine("Error: Please provide a positive number greater than zero.");
                        continue;
                    }

                    break;
                }

                

            } while (true);

            for (int i = 0; i < count; i++)
            {
                decimal price;
                int quantity;
                Console.WriteLine($"---------- Item {i+1} ----------");
                do
                {
                    Console.Write("Enter price: ");
                    if (!decimal.TryParse(Console.ReadLine(), out price) || price <= 0)
                    {
                        Console.WriteLine("Error: Invalid price! Please provide a valid price.\n");
                        continue;
                    }
                    break;
                } while (true);

                do
                {
                    Console.Write("Enter quantity: ");
                    if (!int.TryParse(Console.ReadLine(), out quantity) || quantity <= 0)
                    {
                        Console.WriteLine("Error: Please provide a valid quantity.\n");
                        continue;
                    }
                    break;
                } while (true);

                total += price * quantity;
            }
                 
            if(total > 1000)
            {
                discount = total * 0.10m;
                Console.WriteLine($"Total: {total:F2}$ - discount:{discount}$ - Final: {total-discount:F2}$.");
            }
            else Console.WriteLine($"Total:{total:F2}$ - No discount - Final{total:F2}$.");
            
        }
    }
}