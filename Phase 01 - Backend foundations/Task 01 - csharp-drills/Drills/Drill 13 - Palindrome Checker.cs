using System.Text.RegularExpressions;

namespace Task_01_csharp_drills.Drills
{
    internal class Drill_13_Palindrome_Checker
    {
        public static void Run()
        {
            Console.WriteLine("=======================================");
            Console.WriteLine("||      Palindrome Checker          ||");
            Console.WriteLine("=======================================\n");

            do
            {
                Console.Write("Enter a word or sentence: ");
                var userInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(userInput))
                {
                    Console.WriteLine("Error: Please provide input.");
                    continue;
                }

                string cleaned = Regex.Replace(userInput.ToLower(), "[^a-z]^/s", "");

                if (string.IsNullOrWhiteSpace(cleaned))
                {
                    Console.WriteLine("Error: No valid characters to check.");
                    continue;
                }

                string reversed = new string(cleaned.Reverse().ToArray());
                bool isPalindrome = cleaned == reversed;

                if (isPalindrome)
                {
                    Console.WriteLine("Palindrome!");
                }
                else
                {
                    string spaceCleaned = Regex.Replace(userInput.ToLower(), "[^a-z0-9]", "");
                    reversed = new string(spaceCleaned.Reverse().ToArray());
                    if(spaceCleaned != reversed) 
                        Console.WriteLine("Not Palindrome.");  
                    else
                        Console.WriteLine("Palindrome if ignore space.");
                }
                break;
            } while (true);
        }
    }
}