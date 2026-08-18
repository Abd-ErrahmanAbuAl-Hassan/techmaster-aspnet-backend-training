namespace Task_01_csharp_drills.Drills
{
    internal class Drill_15_Array_Rotation
    {
        public static void Run()
        {
            Console.WriteLine("=======================================");
            Console.WriteLine("||       Array Rotation             ||");
            Console.WriteLine("=======================================\n");

            do
            {
                Console.Write("Enter array elements separated by spaces: ");
                var userInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(userInput))
                {
                    Console.WriteLine("Error: Please provide elements.");
                    continue;
                }

                List<int> arr = new List<int>();
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
                    arr.Add(num);
                }

                if (arr.Count == 0)
                {
                    Console.WriteLine("Error: Array cannot be empty.\n");
                    continue;
                }

                int last = arr[arr.Count - 1];
                for (int i = arr.Count - 1; i > 0; i--)
                {
                    arr[i] = arr[i - 1];
                }
                arr[0] = last;

                Console.WriteLine($"Rotated array: [{string.Join(", ", arr)}]\n");
                break;
            } while (true);
        }
    }
}