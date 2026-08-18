namespace Task_01_csharp_drills.Drills
{
    internal class Drill_18_Number_Statistics
    {
        public static void Run()
        {
            Console.WriteLine("=======================================");
            Console.WriteLine("||     Number Statistics            ||");
            Console.WriteLine("=======================================\n");

            do
            {
                Console.Write("Enter a list of numbers separated by spaces: ");
                var userInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(userInput))
                {
                    Console.WriteLine("Error: Please provide numbers.");
                    continue;
                }

                List<int> numbers = new List<int>();
                var parts = userInput.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var part in parts)
                {
                    if (!int.TryParse(part, out int num))
                    {
                        Console.Write($"Error: '{part}' is not a valid number. Enter 1 to replace, any other value to discard: ");
                        var op = Console.ReadLine();
                        if (op == "1")
                        {
                            do
                            {
                                Console.Write("Enter the new value: ");
                                var _userInput = Console.ReadLine();

                                if (string.IsNullOrWhiteSpace(_userInput))
                                {
                                    Console.WriteLine("Error: Please provide value.");
                                }
                                else
                                {
                                    if (!int.TryParse(_userInput, out num))
                                    {
                                        Console.WriteLine($"Error: Invalid input\" {_userInput} \" Please provide a number.");
                                        continue;
                                    }
                                    break;
                                }
                            }
                            while (true);
                        }
                        else continue;

                    }
                    numbers.Add(num);
                }

                int negative=0, positive=0 , zero=0;
                foreach (var num in numbers)
                {
                    if (num < 0) negative++;
                    else if(num>0)positive++;
                    else zero++;
                }
                Console.WriteLine("\n--- Statistics ---");
                Console.WriteLine($"Count: {numbers.Count}");
                Console.WriteLine($"Sum: {numbers.Sum()}");
                Console.WriteLine($"Average: {numbers.Average():F2}");
                Console.WriteLine($"Maximum: {numbers.Max()}");
                Console.WriteLine($"Minimum: {numbers.Min()}");
                Console.WriteLine($"Positives: {positive}");
                Console.WriteLine($"Negatives: {negative}");
                Console.WriteLine($"Zeros: {zero}\n");
                break;
            } while (true);
        }
    }
}