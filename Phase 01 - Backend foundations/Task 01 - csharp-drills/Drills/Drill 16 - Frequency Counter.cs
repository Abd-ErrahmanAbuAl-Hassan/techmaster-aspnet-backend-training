namespace Task_01_csharp_drills.Drills
{
    internal class Drill_16_Frequency_Counter
    {
        public static void Run()
        {
            Console.WriteLine("=======================================");
            Console.WriteLine("||      Frequency Counter           ||");
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

                Dictionary<int, int> frequency = new Dictionary<int, int>();

                foreach (var num in numbers)
                {
                    if (frequency.ContainsKey(num))
                        frequency[num]++;
                    else
                        frequency[num] = 1;
                }

                Console.Write("\nFrequency Count: ");
                foreach (var x in frequency.OrderByDescending(x => x.Value))
                {
                    Console.Write($"{x.Key}>>>{x.Value}, ");
                }
                Console.CursorLeft -= 2;
                Console.WriteLine("  ");
                break;
            } while (true);
        }
    }
}