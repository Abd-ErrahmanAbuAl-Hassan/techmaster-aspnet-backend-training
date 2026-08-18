namespace Task_01_csharp_drills.Drills
{
    internal class Drill_14_Simple_Expense_Tracker
    {
        public static void Run()
        {
            Console.WriteLine("=======================================");
            Console.WriteLine("||    Simple Expense Tracker        ||");
            Console.WriteLine("=======================================\n");

            List<(string name, decimal amount)> expenses = new List<(string, decimal)>();

            do
            {
                Console.WriteLine("Menu:");
                Console.WriteLine("1. Add expense");
                Console.WriteLine("2. View summary");
                Console.WriteLine("3. Exit");
                Console.Write("Choose option: ");

                var option = Console.ReadLine();

                if (option == "1")
                {
                    Console.Write("Enter expense name: ");
                    var name = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        Console.WriteLine("Error: Please provide an expense name.\n");
                        continue;
                    }
                    decimal amount;
                    do
                    {
                        Console.Write("Enter amount: $");
                        if (!decimal.TryParse(Console.ReadLine(), out amount) || amount < 0)
                        {
                            Console.WriteLine("Error: Please provide a valid amount.\n");
                            continue;
                        }
                        break;
                    } while (true);

                    expenses.Add((name, amount));
                    Console.WriteLine("Expense added.\n");
                }
                else if (option == "2")
                {
                    if (expenses.Count == 0)
                    {
                        Console.WriteLine("No expenses recorded.\n");
                        continue;
                    }

                    decimal total = expenses.Sum(e => e.amount);
                    decimal average = total / expenses.Count;
                    var maxExpenses = expenses
                                  .Where(e => e.amount == expenses.Max(x => x.amount))
                                  .Select(e => e.name)
                                  .ToList();
                    Console.WriteLine("\n--- Expense Summary ---");
                    Console.WriteLine($"\nTotal: ${total:F2}");
                    Console.WriteLine($"Average: ${average:F2}");
                    Console.WriteLine($"Highest: {string.Join(",", maxExpenses)}\n");
                }
                else if (option == "3")
                {
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