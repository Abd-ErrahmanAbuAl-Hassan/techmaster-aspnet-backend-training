namespace Task_01_csharp_drills.Drills
{
    internal class Drill_20_Method_Refactoring_Challenge
    {
        public static void Run()
        {
            Console.WriteLine("================================================");
            Console.WriteLine("||  Method Refactoring Challenge             ||");
            Console.WriteLine("================================================\n");

            DisplayMainMenu();
        }

        // ============================================================
        // MAIN MENU - Orchestrates drill selection and execution
        // ============================================================
        private static void DisplayMainMenu()
        {
            do
            {
                Console.WriteLine("Choose a refactored drill to run:");
                Console.WriteLine("1. Duplicate Number Detector (Drill 11)");
                Console.WriteLine("2. Array Rotation (Drill 15)");
                Console.WriteLine("3. Frequency Counter (Drill 16)");
                Console.WriteLine("4. Exit");
                Console.Write("Choose option: ");

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ExecuteDuplicateDetectorDrill();
                        break;
                    case "2":
                        ExecuteArrayRotationDrill();
                        break;
                    case "3":
                        ExecuteFrequencyCounterDrill();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.\n");
                        break;
                }
            } while (true);
        }

        // Gets a list of space-separated numbers from the user.
        private static string GetNumberListInput()
        {
            while (true) 
            {
                Console.Write("Enter a list of numbers separated by spaces: ");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Error: Please provide numbers.\n");
                    continue;
                }
                return input;
            } 
        }

        // Parses and validates a space-separated string of numbers.
        private static List<int> ParseAndValidateNumbers(string input)
        {
            
            List<int> numbers = new List<int>();
            var parts = input.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                if (!int.TryParse(part, out int num))
                {
                    Console.Write($"Error: '{part}' is not a valid number. Enter 1 to replace any other value to discard: ");
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

            return numbers;
        }

        // DRILL 11: DUPLICATE NUMBER DETECTOR - Refactored Structure
        private static void ExecuteDuplicateDetectorDrill()
        {
            Console.WriteLine("\n--- Duplicate Number Detector (Refactored) ---\n");

            string userInput = GetNumberListInput();

            var numbers = ParseAndValidateNumbers(userInput);
            while (numbers == null)
            {
                return;
            }

            var duplicates = IdentifyDuplicates(numbers);

            DisplayDuplicateResults(duplicates);
        }

        private static HashSet<int> IdentifyDuplicates(List<int> numbers)
        {
            HashSet<int> seen = new HashSet<int>();
            HashSet<int> duplicates = new HashSet<int>();

            foreach (var num in numbers)
            {
                if (!seen.Add(num))
                {
                    duplicates.Add(num);
                }
            }

            return duplicates;
        }

        private static void DisplayDuplicateResults(HashSet<int> duplicates)
        {
            if (duplicates.Count == 0)
            {
                Console.WriteLine("No duplicates found.\n");
            }
            else
            {
                Console.WriteLine($"Duplicates found: {string.Join(",",duplicates)}\n");   
            }
        }

        // DRILL 15: ARRAY ROTATION - Refactored Structure
        private static void ExecuteArrayRotationDrill()
        {
            Console.WriteLine("\n--- Array Rotation (Refactored) ---\n");

            string userInput = GetNumberListInput();

            var numbers = ParseAndValidateNumbers(userInput);
            if (numbers == null)
            {
                return;
            }

            if (numbers.Count == 0)
            {
                Console.WriteLine("Error: Array cannot be empty.\n");
                return;
            }

            RotateArrayRight(numbers);

            DisplayRotatedArray(numbers);
        }

        private static void RotateArrayRight(List<int> arr)
        {
            if (arr.Count > 0)
            {
                int last = arr[arr.Count - 1];
                for (int i = arr.Count - 1; i > 0; i--)
                {
                    arr[i] = arr[i - 1];
                }
                arr[0] = last;
            }
        }

        private static void DisplayRotatedArray(List<int> arr)
        {
            Console.WriteLine($"Rotated array: [{string.Join(", ", arr)}]\n");
        }

        // DRILL 16: FREQUENCY COUNTER - Refactored Structure
        private static void ExecuteFrequencyCounterDrill()
        {
            Console.WriteLine("\n--- Frequency Counter (Refactored) ---\n");

            string userInput = GetNumberListInput();

            var numbers = ParseAndValidateNumbers(userInput);
            if (numbers == null)
            {
                return;
            }

            var frequency = CountFrequency(numbers);

            DisplayFrequencyResults(frequency);
        }

        private static Dictionary<int, int> CountFrequency(List<int> numbers)
        {
            Dictionary<int, int> frequency = new Dictionary<int, int>();

            foreach (var num in numbers)
            {
                if (frequency.ContainsKey(num))
                    frequency[num]++;
                else
                    frequency[num] = 1;
            }

            return frequency;
        }

        private static void DisplayFrequencyResults(Dictionary<int, int> frequency)
        {
            Console.Write("\nFrequency Count: ");
            foreach (var x in frequency.OrderByDescending(x => x.Value))
            {
                Console.Write($"{x.Key}>>>{x.Value}, ");
            }
            Console.CursorLeft -= 2;
            Console.WriteLine("  \n");
        }
    }
}