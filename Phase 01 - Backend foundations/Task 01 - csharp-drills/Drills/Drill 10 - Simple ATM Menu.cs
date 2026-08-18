namespace Task_01_csharp_drills.Drills
{
    internal class Drill_10_Simple_ATM_Menu
    {
        public static void Run()
        {
            Console.WriteLine("=======================================");
            Console.WriteLine("||        Simple ATM Menu            ||");
            Console.WriteLine("=======================================\n");

            decimal balance = 1000m;

            do
            {
                Console.WriteLine("\n--- ATM Menu ---");
                Console.WriteLine("1. Check Balance");
                Console.WriteLine("2. Withdraw");
                Console.WriteLine("3. Deposit");
                Console.WriteLine("4. Exit");
                Console.Write("Choose option: ");

                var option = Console.ReadLine();

                if (option == "1")
                {
                    Console.WriteLine($"Your balance: ${balance:F2}\n");
                }
                else if (option == "2")
                {
                    Console.Write("Enter withdrawal amount: $");
                    if (decimal.TryParse(Console.ReadLine(), out decimal amount) && amount > 0)
                    {
                        if (amount <= balance)
                        {
                            balance -= amount;
                            Console.WriteLine($"Withdrawn: ${amount:F2}. New balance: ${balance:F2}\n");
                        }
                        else
                        {
                            Console.WriteLine("Error: Insufficient balance.\n");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: Please enter a valid amount.\n");
                    }
                }
                else if (option == "3")
                {
                    Console.Write("Enter deposit amount: $");
                    if (decimal.TryParse(Console.ReadLine(), out decimal amount) && amount > 0)
                    {
                        balance += amount;
                        Console.WriteLine($"Deposited: ${amount:F2}. New balance: ${balance:F2}\n");
                    }
                    else
                    {
                        Console.WriteLine("Error: Please enter a valid amount.\n");
                    }
                }
                else if (option == "4")
                {
                    Console.WriteLine("Thank you for using ATM. Goodbye!\n");
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid option. Please try again.\n");
                }
            } while (true);
        }
    }
}