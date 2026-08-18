namespace Task_01_csharp_drills.Drills
{
    internal class Drill_11_Duplicate_Number_Detector
    {
        public static void Run()
        {
            Console.WriteLine("=======================================");
            Console.WriteLine("||  Duplicate Number Detector        ||");
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

                bool valid = true;
                foreach (var part in parts)
                {
                    if (!int.TryParse(part, out int num))
                    {
                        Console.WriteLine($"Error: '{part}' is not a valid number.");
                        valid = false;
                        break;
                    }
                    numbers.Add(num);
                }

                if (!valid)
                    continue;

                HashSet<int> seen = new HashSet<int>();
                HashSet<int> duplicates = new HashSet<int>();

                foreach (var num in numbers)
                {
                    if (!seen.Add(num))
                    {
                        duplicates.Add(num);
                    }
                }

                if (duplicates.Count == 0)
                {
                    Console.WriteLine("No duplicates found.\n");
                }
                else
                {
                    Console.WriteLine($"Duplicates: {string.Join(",", duplicates)}");
                }
                break;
            } while (true);
        }
    }
}