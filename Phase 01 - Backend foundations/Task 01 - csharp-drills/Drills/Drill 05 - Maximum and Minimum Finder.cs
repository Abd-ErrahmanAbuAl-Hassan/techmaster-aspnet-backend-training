using System;
using System.Security.Cryptography;

namespace Task_01_csharp_drills.Drills
{
    internal class Drill_05_Maximum_and_Minimum_Finder
    {
        public static void Run()
        {
            Console.WriteLine("===============================");
            Console.WriteLine("|| Maximum and Minimum Finder ||");
            Console.WriteLine("===============================\n");
            List<int> nums = new List<int>();
            do
            {
                Console.Write("Enter List of numbers by white space separation: ");
                var userInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(userInput))
                {
                    Console.WriteLine("Error: Please provide the count.");
                    continue;
                }

                foreach(var i in userInput.Trim().Split(" "))
                {
                    if (!int.TryParse(i, out int num))
                    {
                        Console.Write($"Error: '{i}' is not a valid number., Enter 1 to replace any other value to discard: ");
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
                    nums.Add(num);
                }
                break;
            } while (true);

            int min = nums[0];
            int max = min; 

            for (int i = 1; i < nums.Count; i++)
            {
                if(nums[i] > max)  max = nums[i];
                if(nums[i] < min)  min = nums[i];
            }

            Console.WriteLine($"Min: {min} | Max: {max}");

            // Bonus : Using LINQ
            //Console.WriteLine($"Min: {nums.Min()} | Max: {nums.Max()}");


        }
    }
}
