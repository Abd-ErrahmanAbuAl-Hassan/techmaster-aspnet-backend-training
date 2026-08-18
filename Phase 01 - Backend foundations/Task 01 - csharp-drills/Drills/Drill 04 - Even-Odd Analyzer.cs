using System;
using System.Security.Cryptography;

namespace Task_01_csharp_drills.Drills
{
    internal class Drill_04_Even_Odd_Analyzer
    {
        public static void Run()
        {
            Console.WriteLine("=============================");
            Console.WriteLine("||    Even/Odd Analyzer    ||");
            Console.WriteLine("=============================\n");
            int count;
            do
            {
                Console.Write("Enter the Numbers count: ");
                var userInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(userInput))
                {
                    Console.WriteLine("Error: Please provide the count.");
                }
                else
                {
                    if (!int.TryParse(userInput, out count)) { 
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

            List<int> oddList = new List<int>();
            List<int> evenList = new List<int>();
            int num;
            for (int i = 0; i < count; i++)
            {
                do
                {
                    Console.Write($"Enter the Number {i + 1}: ");
                    var userInput = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(userInput))
                    {
                        Console.WriteLine("Error: Please provide a number.");
                    }
                    else
                    {
                        if (!int.TryParse(userInput, out num))
                        {
                            Console.WriteLine($"Error: Invalid input\" {userInput} \" Please provide a number.");
                            continue;
                        }

                        break;
                    }

                } while (true);

                if (num % 2 == 0) evenList.Add(num);
                else oddList.Add(num);

            }

            if (oddList.Count > 0 && evenList.Count > 0)
                Console.WriteLine($"Even: {string.Join(',', evenList)} | Odd: {string.Join(',', oddList)}");
            else if (evenList.Count <= 0)
                Console.WriteLine($"Even: No even Numbers | Odd: {string.Join(',', oddList)}");
            else if (oddList.Count <= 0)
                Console.WriteLine($"Even: {string.Join(',', evenList)} | Odd: No odd Numbers ");

        }
    }
}
